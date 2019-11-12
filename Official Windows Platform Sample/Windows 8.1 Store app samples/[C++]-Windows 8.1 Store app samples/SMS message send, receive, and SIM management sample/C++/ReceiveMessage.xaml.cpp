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
// ReceiveMessage.xaml.cpp
// Implementation of the ReceiveMessage class
//

#include "pch.h"
#include "ReceiveMessage.xaml.h"
#include <ppltasks.h>

using namespace SDKSample::SmsSendReceive;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Devices::Sms;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace concurrency;

ReceiveMessage::ReceiveMessage()
{
    InitializeComponent();

    sampleDispatcher = Window::Current->Dispatcher;
}

// Initialize variables and controls for the scenario.
// This method is called just before the scenario page is displayed.
void ReceiveMessage::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    listening = false;
    msgCount = 0;

    ReceivedFromText->Text = "";
    ReceivedMessageText->Text = "";
}

// Clean-up when scenario page is left. This is called when the
// user navigates away from the scenario page.
void ReceiveMessage::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // Detach event handler
    if (listening)
    {
        device->SmsMessageReceived -= smsMessageReceivedToken;
    }

    // Release the device
    device = nullptr;
}

void ReceiveMessage::Receive_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (device == nullptr)
    {
        rootPage->NotifyUser("Getting default SMS device ...", NotifyType::StatusMessage);

        create_task(SmsDevice::GetDefaultAsync()).then([this] (SmsDevice^ getSmsDevice)
        {
            this->device = getSmsDevice;
            // Attach a message received handler to the device, if not already listening
            if (!listening)
            {
                AddListener();
            }
        }).then([this] (task<void> catchErrors)
        {
            try
            {
                catchErrors.get();
            }
            catch (Platform::Exception^ ex)
            {
                rootPage->NotifyUser("Failed to find SMS device\n" + ex->Message, NotifyType::ErrorMessage);
            }
        });
    }
    else if (!listening)
    {
        AddListener();
    }
}

void ReceiveMessage::AddListener()
{
    try
    {
        msgCount = 0;
        smsMessageReceivedToken = device->SmsMessageReceived += ref new SmsMessageReceivedEventHandler (this, &ReceiveMessage::device_SmsMessageReceived);
        listening = true;
        rootPage->NotifyUser("Waiting for message ...", NotifyType::StatusMessage);
    }
    catch (Platform::Exception^ ex)
    {
        rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);

        // On failure, release the device. If the user revoked access or the device
        // is removed, a new device object must be obtained.
        device = nullptr;
    }
}

// Handle a received message event.
void ReceiveMessage::device_SmsMessageReceived(SmsDevice^ sender, SmsMessageReceivedEventArgs^ args)
{
    // Dispatch anonymous function to UI thread to display message properties or error
    sampleDispatcher->RunAsync(CoreDispatcherPriority::Normal,
        ref new DispatchedHandler ([this, args] ()
        {
            try
            {
                // Get message from the event args.
                SmsTextMessage^ msg = args->TextMessage;
                msgCount += 1;

                ReceivedCountText->Text = msgCount.ToString();
                ReceivedFromText->Text = msg->From;
                ReceivedMessageText->Text = msg->Body;
                rootPage->NotifyUser(msgCount.ToString() + ((msgCount == 1) ? " message" : " messages") + " received", NotifyType::StatusMessage);
            }
            catch (Platform::Exception^ ex)
            {
                rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
            }
        }));
}

