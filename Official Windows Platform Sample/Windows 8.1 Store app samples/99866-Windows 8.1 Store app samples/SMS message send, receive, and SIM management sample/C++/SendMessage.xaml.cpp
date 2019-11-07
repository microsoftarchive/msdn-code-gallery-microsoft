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
// SendMessage.xaml.cpp
// Implementation of the SendMessage class
//

#include "pch.h"
#include "SendMessage.xaml.h"
#include <ppltasks.h>

using namespace SDKSample::SmsSendReceive;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Devices::Sms;
using namespace concurrency;

SendMessage::SendMessage()
{
    InitializeComponent();
}

// Initialize variables and controls for the scenario.
// This method is called just before the scenario page is displayed.
void SendMessage::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    SendToText->Text = "";
    SendMessageText->Text = "";
}

// Clean-up when scenario page is left. This is called when the
// user navigates away from the scenario page.
void SendMessage::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // Release the device.
    device = nullptr;
}

void SendMessage::Send_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // If this is the first request, get the default SMS device. If this
    // is the first SMS device access, the user will be prompted to grant
    // access permission for this application.
    if (device == nullptr)
    {
        rootPage->NotifyUser("Getting default SMS device ...", NotifyType::StatusMessage);
        create_task(SmsDevice::GetDefaultAsync()).then([this] (SmsDevice^ getSmsDevice)
        {
            this->device = getSmsDevice;
        }).then([this] (task<void> catchErrors)
        {
            try
            {
                catchErrors.get();
            }
            catch (Platform::Exception^ ex)
            {
                rootPage->NotifyUser("Failed to find SMS device\n" + ex->Message, NotifyType::ErrorMessage);
                // On failure, release the device. If the user revoked access or the device
                // is removed, a new device object must be obtained.
                device = nullptr;
                cancel_current_task();
            }
        }).then([this] ()
        {
            DoSend();
        });
    }
    else
    {
        DoSend();
    }
}

void SendMessage::DoSend()
{
    // Create a text message - set the entered destination number and message text.
    SmsTextMessage^ msg = ref new SmsTextMessage();
    msg->To = SendToText->Text;
    msg->Body = SendMessageText->Text;

    auto newMsgRef = std::make_shared<SmsTextMessage^>(msg);

    // Send the message asynchronously
    rootPage->NotifyUser("Sending message ...", NotifyType::StatusMessage);

    create_task([this, newMsgRef] ()
    {
        return (device->SendMessageAsync(*newMsgRef));
    }).then([this] (task<void> catchErrors)
    {
        try
        {
            catchErrors.get();
            rootPage->NotifyUser("Text message sent", NotifyType::StatusMessage);
        }
        catch (Platform::Exception^ ex)
        {
            rootPage->NotifyUser("Failed to send SMS message\n" + ex->Message, NotifyType::ErrorMessage);
        }
    });
}
