// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario1Input.xaml.cpp
// Implementation of the Scenario1Input class
//

#include "pch.h"
#include "ScenarioInput2.xaml.h"

using namespace RawNotificationsSampleCPP;

using namespace Platform;
using namespace Windows::Networking::PushNotifications;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

ScenarioInput2::ScenarioInput2()
{
	InitializeComponent();
    _dispatcher = Window::Current->Dispatcher;
}

ScenarioInput2::~ScenarioInput2()
{
}

void ScenarioInput2::OnNavigatedTo(NavigationEventArgs^ e)
{
	// Get a pointer to our main page.
	rootPage = dynamic_cast<MainPage^>(e->Parameter);
}

void ScenarioInput2::OnNavigatedFrom(NavigationEventArgs^ e)
{
	UpdateListener(false);
}

void ScenarioInput2::Scenario2AddListener_Click(Object^ sender, RoutedEventArgs^ e)
{
	if (UpdateListener(true))
    {
        rootPage->NotifyUser("Now listening for raw notifications", NotifyType::StatusMessage);
    }
    else
    {
        rootPage->NotifyUser("Channel not open--open a channel in Scenario 1", NotifyType::ErrorMessage);
    }
}

void ScenarioInput2::Scenario2RemoveListener_Click(Object^ sender, RoutedEventArgs^ e)
{
	if (UpdateListener(false))
    {
        rootPage->NotifyUser("No longer listening for raw notifications", NotifyType::StatusMessage);
    }
    else
    {
        rootPage->NotifyUser("Channel not open--open a channel in Scenario 1", NotifyType::ErrorMessage);
    }
}

bool ScenarioInput2::UpdateListener(bool add)
{
	if (rootPage->Channel != nullptr)
	{
		if (add && pushToken.Value == 0)
		{
			pushToken = rootPage->Channel->PushNotificationReceived += ref new TypedEventHandler<PushNotificationChannel^, PushNotificationReceivedEventArgs^>(this, &ScenarioInput2::OnPushNotificationReceived);
		}
		else if (!add && pushToken.Value != 0)
		{
			rootPage->Channel->PushNotificationReceived -= pushToken;
			pushToken.Value = 0;
		}
		return true;
	}
	return false;
}

void ScenarioInput2::OnPushNotificationReceived(PushNotificationChannel^ sender, PushNotificationReceivedEventArgs^ e)
{
	if (e->NotificationType == PushNotificationType::Raw)
	{
		e->Cancel = true;
		auto content = e->RawNotification->Content;
		_dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, content] ()
		{
			rootPage->NotifyUser("Raw notification received with content: " + content, NotifyType::StatusMessage);
		}, CallbackContext::Any));
	}
}