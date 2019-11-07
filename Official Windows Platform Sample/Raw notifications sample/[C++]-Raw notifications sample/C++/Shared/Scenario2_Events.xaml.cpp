// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "Scenario2_Events.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;

using namespace Platform;
using namespace Windows::Networking::PushNotifications;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Notifications;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
	InitializeComponent();
	_dispatcher = Window::Current->Dispatcher;
}

void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
	// Get a pointer to our main page.
	rootPage = MainPage::Current;
}

void Scenario2::OnNavigatedFrom(NavigationEventArgs^ e)
{
	UpdateListener(false);
}

void Scenario2::Scenario2AddListener_Click(Object^ sender, RoutedEventArgs^ e)
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

void Scenario2::Scenario2RemoveListener_Click(Object^ sender, RoutedEventArgs^ e)
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

bool Scenario2::UpdateListener(bool add)
{
	if (rootPage->Channel != nullptr)
	{
		if (add && pushToken.Value == 0)
		{
			pushToken = rootPage->Channel->PushNotificationReceived += ref new TypedEventHandler<PushNotificationChannel^, PushNotificationReceivedEventArgs^>(this, &Scenario2::OnPushNotificationReceived);
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

void Scenario2::OnPushNotificationReceived(PushNotificationChannel^ sender, PushNotificationReceivedEventArgs^ e)
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