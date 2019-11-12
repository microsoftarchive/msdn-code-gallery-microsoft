//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
//
// BackgroundTask.cpp
// Implementation of the background task handler.
//

#include "pch.h"
#include "ConfigStore.h"
#include "BackgroundTask.h"

using namespace concurrency;
using namespace HotspotAuthenticationTask;
using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Networking::NetworkOperators;
using namespace Windows::System::Threading;
using namespace Windows::UI::Notifications;
using namespace Windows::Data::Xml::Dom;

// Helper for writing debug output to the connected debugger
void Debug(String^ message)
{
	String^ finalMessage = ref new String(message->Data());
	finalMessage += "\r\n";
	OutputDebugString(finalMessage->Data());
}

// Default constructor
AuthenticationTask::AuthenticationTask()
{
	_foregroundAppId = "HotspotAuthenticationApp.App";
	_cancelRequested = false;
}

// This is the only method of the IBackgroundTask interface.
void AuthenticationTask::Run(IBackgroundTaskInstance^ taskInstance)
{
	Debug("Background " + taskInstance->Task->Name + " starting...");

	// Associate a cancelation handler with the background task for handlers
	// that may take a considerable time to complete.
	taskInstance->Canceled += ref new BackgroundTaskCanceledEventHandler(this, &AuthenticationTask::OnCanceled);

	// Do the background task activity. First, get the authentication context.
	Debug("Getting event details");
	auto details = dynamic_cast<HotspotAuthenticationEventDetails^>(taskInstance->TriggerDetails);

	HotspotAuthenticationContext^ context;
	if (!HotspotAuthenticationContext::TryGetAuthenticationContext(details->EventToken, &context))
	{
		// The event is not targetting this application. There is no further processing to do.
		Debug("Failed to get event context");
		return;
	}

	auto ssidBlob = context->WirelessNetworkId;
	WCHAR ssidWide[32 + 1] = { 0 };
	if (0 != MultiByteToWideChar(CP_UTF8, 0, (LPCCH)ssidBlob->Data, ssidBlob->Length, ssidWide, ARRAYSIZE(ssidWide)))
	{
		String^ ssidStr = ref new String(ssidWide);
		Debug("SSID: " + ssidStr);
	}

	// Get configuration from application storage.
	bool markAsManualConnect = ConfigStore::MarkAsManualConnect;

	#if WINAPI_FAMILY == WINAPI_FAMILY_PC_APP
	
	// Following code can be used if using native WISPr implementation. Please note that 
	// following HotspotAuthenticationContext properties only work on windows and not on windows phone. 
	// On Windows Phone they return un-useful strings
	// Developers are expected to implement their own WISPr implementation on Phone

	Debug("AuthenticationUrl: " + context->AuthenticationUrl->RawUri);
	Debug("RedirectMessageUrl: " + context->RedirectMessageUrl->RawUri);
	Debug("RedirectMessageXml: " + context->RedirectMessageXml->GetXml());

	// In this sample, the AuthenticationUrl is always checked in the background task handler
	// to avoid launching the foreground app in case the authentication host is not trusted.
	if (ConfigStore::AuthenticationHost != context->AuthenticationUrl->Host)
	{
		// Hotspot is not using the trusted authentication server.
		// Abort authentication and disconnect.
		Debug("Authentication server is untrusted");
		context->AbortAuthentication(markAsManualConnect);
		return;
	}

	#endif


	// Check if authentication is handled by foreground app.
	if (!ConfigStore::AuthenticateThroughBackgroundTask)
	{
		Debug("Triggering foreground application");
		// Pass event token to application
		ConfigStore::AuthenticationToken = details->EventToken;

		// Trigger notification  
		// Since TriggerAttentionRequired function throws NotImplementedException on phone we will be using
		// regular Toast Notification to notify user about the authentication, Tapping on the notification will
		// launch the application where user can complete the authentication
		#if WINAPI_FAMILY == WINAPI_FAMILY_PC_APP		
			context->TriggerAttentionRequired(_foregroundAppId, "");
		#else
		
			auto toastXml = ToastNotificationManager::GetTemplateContent(ToastTemplateType::ToastText01);
			toastXml->GetElementsByTagName("text")->GetAt(0)->AppendChild(toastXml->CreateTextNode("Auth by foreground"));
			IXmlNode^ toastNode = toastXml->SelectSingleNode("/toast");
			((Windows::Data::Xml::Dom::XmlElement^)toastNode)->SetAttribute("launch", "AuthByForeground");

			auto toast = ref new ToastNotification(toastXml);

			Type^ typeofToastNotification = toast->GetType();
									
			toast->Tag = "AuthByForeground";
			toast->Group = "HotspotAuthAPI";
			
			auto notification = ToastNotificationManager::CreateToastNotifier();
			notification->Show(toast);
		#endif

		return;
	}

	// Handle authentication in background task.
	// In case this handler performs more complex tasks, it may get canceled at runtime.
	// Check if task was canceled by now.
	if (_cancelRequested)
	{
		// In case the task handler takes too long to generate credentials and gets canceled,
		// the handler should terminate the authentication by aborting it
		Debug("Aborting authentication");
		context->AbortAuthentication(markAsManualConnect);
		return;
	}

	// Before calling an asynchronous API from the background task,
	// get the deferral object from the task instance.
	Deferral = taskInstance->GetDeferral();

	#if WINAPI_FAMILY == WINAPI_FAMILY_PC_APP
	// The most common way of handling an authentication attempt is by providing WISPr credentials
	// through the IssueCredentialsAsync API.
	// If the task doesn't take any actions for authentication failures, it can use the
	// IssueCredentials API to just provide credenstials.
	// Alternatively, an application could run its own business logic to authentication with the
	// hotspot. In this case it should call the SkipAuthentication API. Note that it should call
	// SkipAuthentication after it has authenticated to allow Windows to refresh the network connectivity
	// state instantly.
	// On Windows Phone IssueCredentialsAsync is not supported so , On Phone only available option is to implement
	// custom business logic for authentication and call SkipAuthentication
	Debug("Issuing credentials");
	task<HotspotCredentialsAuthenticationResult^> getAuthenticationResult(
		context->IssueCredentialsAsync(
		ConfigStore::UserName,
		ConfigStore::Password,
		ConfigStore::ExtraParameters,
		ConfigStore::MarkAsManualConnect)
		);
	getAuthenticationResult.then([=](task<HotspotCredentialsAuthenticationResult^> authenticationTask)
	{
		auto result = authenticationTask.get();
		if (HotspotAuthenticationResponseCode::LoginSucceeded == result->ResponseCode)
		{
			Debug("Authentication succeeded");
			auto logoffUrl = result->LogoffUrl;
			if (logoffUrl != nullptr)
			{
				Debug("The logoff URL is: " + logoffUrl->RawUri);
			}
		}
		else
		{
			Debug("Authentication failed");
		}
		Debug("Background " + taskInstance->Task->Name + " completed");


	});

	#else

	// On Windows Phone IssueCredentialsAsync is not supported so , On Phone only available option is to implement
	// custom business logic for authentication and call SkipAuthentication
	// TODO: Do any custom authentication here and call SkipAuthentication
	// This call also serves the purpose of indicating a successful authentication.
	context->SkipAuthentication();

	#endif

	Deferral->Complete();
}

// Handles background task cancellation.
void AuthenticationTask::OnCanceled(IBackgroundTaskInstance^ sender, BackgroundTaskCancellationReason reason)
{
	// Indicate that the background task is canceled.
	_cancelRequested = true;

	Debug("Background " + sender->Task->Name + " cancel requested...");
}
