//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include <pch.h>

#include <windows.foundation.h>
#include <collection.h>

namespace PushNotificationsHelper
{
	// This class is used internally. Since it doesn't cross the ABI, it's not necessary to expose
	// its public members as properties
	ref class TileChannelData sealed
	{
	public:
		TileChannelData(
			Platform::String^ url, 
			Platform::String^ channelUri, 
			bool isAppId,
			ULONGLONG renewed) : TargetUrl(url), ChannelUri(channelUri), IsAppId(isAppId), Renewed(renewed) { }
	internal:
		Platform::String^ TargetUrl;
		Platform::String^ ChannelUri;
		bool IsAppId;
		ULONGLONG Renewed;
	};

	public ref class ChannelAndWebResponse sealed
    {
	public:
		ChannelAndWebResponse(
			Windows::Networking::PushNotifications::PushNotificationChannel^ channel, 
			Platform::String^ webResponse) : _channel(channel), _webResponse(webResponse) { }

		property Windows::Networking::PushNotifications::PushNotificationChannel^ Channel
		{
			Windows::Networking::PushNotifications::PushNotificationChannel^ get()
			{
				return _channel;
			}
		}
        property Platform::String^ WebResponse
		{
			Platform::String^ get()
			{
				return _webResponse;
			}
		}
	private:
		Windows::Networking::PushNotifications::PushNotificationChannel^ _channel;
		Platform::String^ _webResponse;
    };
	
    public ref class Notifier sealed
    {
    public:
        Notifier();

		Windows::Foundation::IAsyncAction^ RenewAllAsync(bool forceEvenIfFresh);
		Windows::Foundation::IAsyncOperation<ChannelAndWebResponse^>^ OpenChannelAndUploadAsync(Platform::String^ targetUrl);
		Windows::Foundation::IAsyncOperation<ChannelAndWebResponse^>^ OpenChannelAndUploadAsync(
			Platform::String^ targetUrl, 
			Platform::String^ inputTileId, 
			bool isPrimaryTile);

	private:
		~Notifier();
		Platform::Collections::Map<Platform::String^, TileChannelData^>^ tileIdMap;
		CRITICAL_SECTION mapLock;

		TileChannelData^ TryGetTileChannelData(Platform::String^ key);
		void SetTileChannelData(Platform::String^ key, TileChannelData^ dataToSet);
		void UpdateTargetUrl(Platform::String^ newTargetUrl, Platform::String^ channelUri, Platform::String^ inputTileId, bool isPrimaryTile);
		void SaveTileIds();
		Windows::Foundation::IAsyncOperation<ChannelAndWebResponse^>^ ExecuteChannelOperation(
			Windows::Foundation::IAsyncOperation<Windows::Networking::PushNotifications::PushNotificationChannel^>^ channelOperation,
			Platform::String^ targetUrl, 
			Platform::String^ inputTileId, 
			bool isPrimaryTile);

		// Utility functions
		inline ULONGLONG GetCurrentTime();
		bool TryGetNamedString(Windows::Data::Json::JsonObject^ object, Platform::String^ key, Platform::String^* returnValue);
		bool TryGetNamedBoolean(Windows::Data::Json::JsonObject^ object, Platform::String^ key, bool* returnValue);
		std::wstring EncodeURI(Platform::String^ rawInput);
    };
}
