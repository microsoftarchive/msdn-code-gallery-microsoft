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
// ReadMessage.xaml.cpp
// Implementation of the ReadMessage class
//

#include "pch.h"
#include "ReadMessage.xaml.h"
#include <string>

using namespace SDKSample::SmsSendReceive;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Devices::Sms;
using namespace concurrency;
using namespace Windows::Globalization::NumberFormatting;

ReadMessage::ReadMessage()
{
    InitializeComponent();
}

// Initialize variables and controls for the scenario.
// This method is called just before the scenario page is displayed.
void ReadMessage::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;

    ReadIdText->Text = "";
    DateText->Text = "";
    ReadFromText->Text = "";
    ReadMessageText->Text = "";
}

// Clean-up when scenario page is left. This is called when the
// user navigates away from the scenario page.
void ReadMessage::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // Release the device.
    device = nullptr;
}

void ReadMessage::Read_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
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
            DoReadMessage();
        });
    }
    else
    {
        DoReadMessage();
    }
}

void ReadMessage::DoReadMessage()
{
    // Clear message display.
    DateText->Text = "";
    ReadFromText->Text = "";
    ReadMessageText->Text = "";

    // Parse the message ID - must be number between 1 and maximum message count.
    auto parser = ref new DecimalFormatter();
    auto getId = parser->ParseUInt(ReadIdText->Text);

    if ((getId!=nullptr) && (getId->Value >= 1) && (getId->Value<=UINT32_MAX) && (getId->Value <= device->MessageStore->MaxMessages))
    {
        rootPage->NotifyUser("Reading message ...", NotifyType::StatusMessage);

        // Get the selected message from message store asynchronously.
        create_task([this, getId] () {
            return device->MessageStore->GetMessageAsync(static_cast<uint32> (getId->Value));
        }).then([this] (ISmsMessage^ msg)
        {
            ISmsBinaryMessage^ binaryMsg = static_cast<ISmsBinaryMessage^> (msg);
            SmsTextMessage^ textMsg = nullptr;

            // See if this is a text message by querying for the text message interface.
            if (binaryMsg != nullptr)
            {
                textMsg = SmsTextMessage::FromBinaryMessage((SmsBinaryMessage^) msg);
            }

            // Display the text message information.
            if (textMsg != nullptr)
            {
                DateText->Text = textMsg->Timestamp.ToString();
                ReadFromText->Text = textMsg->From;
                ReadMessageText->Text = textMsg->Body;

                rootPage->NotifyUser("Message read.", NotifyType::StatusMessage);
            }
        }).then([this] (task<void> catchErrors)
        {
            try
            {
                catchErrors.get();
            }
            catch (Platform::Exception^ ex)
            {
                rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);

                // On failure, release the device. If the user revoked access or the device
                // is removed, a new device object must be obtained.
                device = nullptr;
            }
        });
    }
    else
    {
        rootPage->NotifyUser("Invalid ID number entered.", NotifyType::ErrorMessage);
    }
}

