//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "Helper.h"
#include "HttpRequest.h"

PCWSTR APP_TILE_ID_KEY = L"appTileIds";
PCWSTR MAIN_APP_TILE_KEY = L"mainAppTileKey";
const double DAYS_TO_RENEW = 10 * 24 * 60;

using namespace PushNotificationsHelper;
using namespace concurrency;
using namespace Microsoft::WRL;
using namespace Microsoft::WRL::Details;
using namespace Platform;
using namespace Platform::Collections;
using namespace std;
using namespace Web;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Networking::PushNotifications;
using namespace Windows::Data::Json;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;

Notifier::Notifier()
{
	if (!InitializeCriticalSectionEx(&mapLock, 0, 0)) 
	{
		throw ref new Platform::Exception(HRESULT_FROM_WIN32(GetLastError()));
	}

	tileIdMap = ref new Map<String^, TileChannelData^>();
	
	auto currentData = ApplicationData::Current->LocalSettings->Values;
	auto urlString = dynamic_cast<String^>(currentData->Lookup(ref new String(APP_TILE_ID_KEY)));
		
	// Data validation
	JsonArray^ storedTileIds;
	if (JsonArray::TryParse(urlString, &storedTileIds))
	{
		for (auto iter : storedTileIds)
		{
			if (iter->ValueType == JsonValueType::String)
			{
				auto tileId = iter->GetString();
				auto dataString = dynamic_cast<String^>(currentData->Lookup(tileId));
				if (dataString)
				{
					JsonObject^ storedJson;
					if (JsonObject::TryParse(dataString, &storedJson))
					{
						String^ targetUrl;
						String^ channelUri;
						bool isPrimaryTile;
						String^ renewedString;
					
						if (TryGetNamedString(storedJson, "TargetUrl", &targetUrl) && 
							TryGetNamedString(storedJson, "ChannelUri", &channelUri) && 
							TryGetNamedBoolean(storedJson, "IsPrimaryTile", &isPrimaryTile) && 
							TryGetNamedString(storedJson, "Renewed", &renewedString))
						{
							ULONGLONG renewed;
							wstringstream stream(renewedString->Data());
							stream >> renewed;
							
							// Check if the stored renewal string is an actual ULONGLONG
							if (stream)
							{
								auto dataToStore = ref new TileChannelData(
									targetUrl,
									channelUri,
									isPrimaryTile,
									renewed
								);

								// We should expect, based off the way this class works, for the serialized data
								// to have unique keys. 
								tileIdMap->Insert(tileId, dataToStore);
							}
						}
					}
				}
			}
		}
	}
}

Notifier::~Notifier()
{
	DeleteCriticalSection(&mapLock);
}

TileChannelData^ Notifier::TryGetTileChannelData(String^ key)
{
	TileChannelData^ returnedData = nullptr;
	EnterCriticalSection(&mapLock);
	try 
	{
		if (tileIdMap->HasKey(key))
		{
			returnedData = tileIdMap->Lookup(key);
		}
	}
	catch (Exception^ ex)
	{
		LeaveCriticalSection(&mapLock);
		// Propagate the exception
		throw ex;
	}

	LeaveCriticalSection(&mapLock);
	return returnedData;
}

void Notifier::SetTileChannelData(String^ key, TileChannelData^ dataToSet)
{
	EnterCriticalSection(&mapLock);
	try
	{
		tileIdMap->Insert(key, dataToSet);
	}
	catch (Exception^ ex)
	{
		LeaveCriticalSection(&mapLock);
		// Propagate the exception
		throw ex;
	}

	LeaveCriticalSection(&mapLock);
}

IAsyncAction^ Notifier::RenewAllAsync(bool forceEvenIfFresh)
{
	ULONGLONG now = GetCurrentTime();
	
	return create_async([this, now, forceEvenIfFresh] ()
	{
		std::vector<task<ChannelAndWebResponse^>> renewalTasks;
		EnterCriticalSection(&mapLock);
		try
		{
			std::vector<IKeyValuePair<String^, TileChannelData^>^> channelsToRenew;
			auto iter = tileIdMap->First();
			while (iter->HasCurrent)
			{
				if (forceEvenIfFresh || (now - iter->Current->Value->Renewed) > DAYS_TO_RENEW)
				{
					channelsToRenew.push_back(iter->Current);
				}
				iter->MoveNext();
			}
		
			
			std::for_each(channelsToRenew.begin(), channelsToRenew.end(), [&renewalTasks, this] (IKeyValuePair<String^, TileChannelData^>^ pair)
			{			
				if (pair->Key == ref new String(MAIN_APP_TILE_KEY))
				{
					renewalTasks.push_back(create_task(OpenChannelAndUploadAsync(pair->Value->TargetUrl)));
				}
				else
				{
					renewalTasks.push_back(create_task(OpenChannelAndUploadAsync(pair->Value->TargetUrl, pair->Key, pair->Value->IsAppId)));
				}	
			});
		}
		catch (Exception^ ex)
		{
			LeaveCriticalSection(&mapLock);
			// Propagate the exception
			throw ex;
		}
		LeaveCriticalSection(&mapLock);

		return when_all(renewalTasks.begin(), renewalTasks.end()).then([] (std::vector<ChannelAndWebResponse^>) {});
	});
}

IAsyncOperation<ChannelAndWebResponse^>^ Notifier::OpenChannelAndUploadAsync(String^ targetUrl)
{
	auto channelOperation = PushNotificationChannelManager::CreatePushNotificationChannelForApplicationAsync();
	return ExecuteChannelOperation(channelOperation, targetUrl, ref new String(MAIN_APP_TILE_KEY), true);
}

IAsyncOperation<ChannelAndWebResponse^>^ Notifier::OpenChannelAndUploadAsync(String^ targetUrl, String^ inputTileId, bool isPrimaryTile)
{
	IAsyncOperation<PushNotificationChannel^>^ channelOperation = nullptr;
	if (isPrimaryTile)
	{
		channelOperation = PushNotificationChannelManager::CreatePushNotificationChannelForApplicationAsync(inputTileId);
	}
	else
	{
		channelOperation = PushNotificationChannelManager::CreatePushNotificationChannelForSecondaryTileAsync(inputTileId);
	}

	return ExecuteChannelOperation(channelOperation, targetUrl, inputTileId, isPrimaryTile);
}

IAsyncOperation<ChannelAndWebResponse^>^ Notifier::ExecuteChannelOperation(IAsyncOperation<PushNotificationChannel^>^ channelOperation, String^ targetUrl, String^ inputTileId, bool isPrimaryTile)
{
	return create_async([this, channelOperation, targetUrl, inputTileId, isPrimaryTile]() -> task<ChannelAndWebResponse^>
	{
		return create_task(channelOperation).then([this, targetUrl, inputTileId, isPrimaryTile] (PushNotificationChannel^ channel) -> ChannelAndWebResponse^
		{
			TileChannelData^ dataForTile = TryGetTileChannelData(inputTileId);

			if (dataForTile != nullptr && channel->Uri == dataForTile->ChannelUri)
			{
				UpdateTargetUrl(targetUrl, channel->Uri, inputTileId, isPrimaryTile);
				return ref new ChannelAndWebResponse(channel, "Channel already uploaded");
			}
			else
			{	
				wstring dataToPost = wstring(L"ChannelUri=") + EncodeURI(channel->Uri) + wstring(L"&ItemId=") + EncodeURI(inputTileId);
				
				HttpRequest httpRequest;
				auto task = create_task(httpRequest.PostAsync(ref new Uri(targetUrl), dataToPost)).then([this, targetUrl, inputTileId, isPrimaryTile, channel] (wstring& response) -> ChannelAndWebResponse^
				{
					// Only update the data on the client if uploading the channel URI succeeds.
					// If it fails, you may considered setting another background task, trying again, etc.
					// OpenChannelAndUploadAsync will throw an exception if upload fails
					UpdateTargetUrl(targetUrl, channel->Uri, inputTileId, isPrimaryTile);
					return ref new ChannelAndWebResponse(channel, ref new String(response.c_str()));
				}, task_continuation_context::use_arbitrary());
				
				task.wait();
				return task.get();
			}
		}, task_continuation_context::use_arbitrary());
	});
}

void Notifier::UpdateTargetUrl(String^ url, String^ channelUri, String^ inputTileId, bool isPrimaryTile)
{
	auto tileId = isPrimaryTile && inputTileId == nullptr ? ref new String(MAIN_APP_TILE_KEY) : inputTileId;

	bool shouldSerialize = TryGetTileChannelData(tileId) == nullptr;
	auto dataToStore = ref new TileChannelData(url, channelUri, isPrimaryTile, GetCurrentTime());
	SetTileChannelData(tileId, dataToStore);

	auto jsonToStore = ref new JsonObject();
	jsonToStore->SetNamedValue("TargetUrl", JsonValue::CreateStringValue(dataToStore->TargetUrl));
	jsonToStore->SetNamedValue("ChannelUri", JsonValue::CreateStringValue(dataToStore->ChannelUri));
	jsonToStore->SetNamedValue("IsPrimaryTile", JsonValue::CreateBooleanValue(dataToStore->IsAppId));

	// Convert the renewal time into a string. The only number type supported by JSON objects
	// is double, which will have a loss of precision
	wstringstream stream;
	stream << dataToStore->Renewed;
	jsonToStore->SetNamedValue("Renewed", JsonValue::CreateStringValue(ref new String(stream.str().c_str())));

	ApplicationData::Current->LocalSettings->Values->Insert(tileId, jsonToStore->Stringify());

	if (shouldSerialize)
	{
		SaveTileIds();
	}
}

void Notifier::SaveTileIds()
{
	auto keysToStore = ref new JsonArray();
	EnterCriticalSection(&mapLock);
	try
	{
		auto iter = tileIdMap->First();
		while (iter->HasCurrent)
		{
			keysToStore->Append(JsonValue::CreateStringValue(iter->Current->Key));
			iter->MoveNext();
		}
	}
	catch (Exception^ ex)
	{
		LeaveCriticalSection(&mapLock);
		throw ex;
	}
	LeaveCriticalSection(&mapLock);
	ApplicationData::Current->LocalSettings->Values->Insert(ref new String(APP_TILE_ID_KEY), keysToStore->Stringify());
}


ULONGLONG Notifier::GetCurrentTime()
{
	FILETIME currentFileTime;
	GetSystemTimeAsFileTime(&currentFileTime);

	ULARGE_INTEGER currentLargeTime;
	currentLargeTime.HighPart = currentFileTime.dwHighDateTime;
	currentLargeTime.LowPart = currentFileTime.dwLowDateTime;
	return currentLargeTime.QuadPart;
}

bool Notifier::TryGetNamedString(JsonObject^ object, String^ key, String^* returnValue)
{
	bool hasValue = object->HasKey(key);
	if (hasValue)
	{
		auto value = object->GetNamedValue(key);
		hasValue = value->ValueType == JsonValueType::String;
		if (hasValue)
		{
			*returnValue = value->GetString();
		}
	}
	return hasValue;
}

bool Notifier::TryGetNamedBoolean(JsonObject^ object, String^ key, bool* returnValue)
{
	bool hasValue = object->HasKey(key);
	if (hasValue)
	{
		auto value = object->GetNamedValue(key);
		hasValue = value->ValueType == JsonValueType::Boolean;
		if (hasValue)
		{
			*returnValue = value->GetBoolean();
		}
	}
	return hasValue;
}

wstring Notifier::EncodeURI(String^ rawInput)
{
	wstringstream outputStream;
	outputStream << setbase(16);
	
	for (const wchar_t* i = rawInput->Begin(); i != rawInput->End(); i++)
	{
		if (isalnum(*i))
		{
			outputStream << *i;
		}
		else
		{
			outputStream << L"%" << setw(2) << setfill<wchar_t>(L'0') << ((unsigned int)*i);
		}
	}
	return outputStream.str();
}

