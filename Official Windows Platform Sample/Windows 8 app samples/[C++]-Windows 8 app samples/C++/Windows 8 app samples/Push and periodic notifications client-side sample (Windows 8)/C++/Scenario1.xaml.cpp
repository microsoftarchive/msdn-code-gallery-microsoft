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
#include "Scenario1.xaml.h"

using namespace SDKSample::SDKTemplate;

using namespace Concurrency;
using namespace PushNotificationsHelper;
using namespace Windows::Networking::PushNotifications;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    InitializeComponent();	
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

	if (rootPage->Notifier == nullptr)
	{
		rootPage->Notifier = ref new Notifier();
	}
}

// The Notifier object allows using the same code in the maintenance task and this foreground application
void SDKSample::SDKTemplate::Scenario1::OpenChannel_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	create_task(rootPage->Notifier->OpenChannelAndUploadAsync(ServerText->Text)).then([this] (ChannelAndWebResponse^ channelAndResponse)
	{
		rootPage->NotifyUser("Channel uploaded! Response:" + channelAndResponse->WebResponse, NotifyType::StatusMessage);
		rootPage->Channel = channelAndResponse->Channel;
	});
}

void SDKSample::SDKTemplate::Scenario1::CloseChannel_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (rootPage->Channel != nullptr)
    {
        // Closing the channel prevents all future cloud notifications from 
        // being delivered to the application or application related UI
        rootPage->Channel->Close();
        rootPage->Channel = nullptr;

		rootPage->NotifyUser("Channel closed", NotifyType::StatusMessage);
    }
    else
    {
		rootPage->NotifyUser("No channel opened", NotifyType::ErrorMessage);
    }
}
