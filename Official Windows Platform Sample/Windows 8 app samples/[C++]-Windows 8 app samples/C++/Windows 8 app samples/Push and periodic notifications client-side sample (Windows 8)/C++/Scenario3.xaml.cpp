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
#include "Scenario3.xaml.h"

using namespace SDKSample::SDKTemplate;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Networking::PushNotifications;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario3::Scenario3()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::SDKTemplate::Scenario3::AddCallback_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if (UpdateListener(true))
	{
		rootPage->NotifyUser("Now listening for notifications", NotifyType::StatusMessage);
	}
	else
	{
		rootPage->NotifyUser("Channel not open. Open the channel in scenario 1", NotifyType::ErrorMessage);
	}
}

void SDKSample::SDKTemplate::Scenario3::RemoveCallback_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if (UpdateListener(false))
    {
        rootPage->NotifyUser("No longer listening for notifications", NotifyType::StatusMessage);
    }
    else
    {
        rootPage->NotifyUser("Channel not open--open a channel in Scenario 1", NotifyType::ErrorMessage);
    }
}

bool SDKSample::SDKTemplate::Scenario3::UpdateListener(bool add)
{
	if (rootPage->Channel != nullptr)
	{
		if (add && pushReceivedToken.Value == 0)
		{
			pushReceivedToken = rootPage->Channel->PushNotificationReceived += ref new TypedEventHandler<PushNotificationChannel^, PushNotificationReceivedEventArgs^>(this, &SDKSample::SDKTemplate::Scenario3::OnPushNotificationReceived);
		}
		else if (!add && pushReceivedToken.Value != 0)
		{
			rootPage->Channel->PushNotificationReceived -= pushReceivedToken;
			pushReceivedToken.Value = 0;
		}
		return true;
	}
	return false;
}

void Scenario3::OnNavigatedFrom(NavigationEventArgs^ e)
{
    if (rootPage->Channel != nullptr)
	{
		rootPage->Channel->PushNotificationReceived -= pushReceivedToken;
	}
}

void SDKSample::SDKTemplate::Scenario3::OnPushNotificationReceived(PushNotificationChannel^ sender, PushNotificationReceivedEventArgs^ e)
{
    String^ typeString;
    String^ notificationContent;
    switch (e->NotificationType)
    {
	case PushNotificationType::Badge:
		typeString = "Badge";
		notificationContent = e->BadgeNotification->Content->GetXml();
		break;
	case PushNotificationType::Tile:
        notificationContent = e->TileNotification->Content->GetXml();
        typeString = "Tile";
        break;
	case PushNotificationType::Toast:
        notificationContent = e->ToastNotification->Content->GetXml();
        typeString = "Toast";
        break;
	case PushNotificationType::Raw:
		notificationContent = e->RawNotification->Content;
		typeString = "Raw";
		break;
    }

    // Setting the cancel property prevents the notification from being delivered. It's especially important to do this for toasts:
    // if your application is already on the screen, there's no need to display a toast from push notifications.
    e->Cancel = true;

    String^ text = "Received a " + typeString + " notification, containing: " + notificationContent;
    Window::Current->Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, text] ()
    {
		rootPage->NotifyUser(text, NotifyType::StatusMessage);
    }));
}
