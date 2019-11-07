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
// DeleteMessage.xaml.cpp
// Implementation of the DeleteMessage class
//

#include "pch.h"
#include "DeleteMessage.xaml.h"

using namespace SDKSample::SmsSendReceive;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Devices::Sms;
using namespace Windows::Globalization::NumberFormatting;
using namespace concurrency;

DeleteMessage::DeleteMessage()
{
    InitializeComponent();
}

// Initialize variables and controls for the scenario.
// This method is called just before the scenario page is displayed.
void DeleteMessage::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

// Clean-up when scenario page is left. This is called when the
// user navigates away from the scenario page.
void DeleteMessage::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // Release the device.
    device = nullptr;
}

void SDKSample::SmsSendReceive::DeleteMessage::Delete_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Parse the entered message ID and pass it to the common delete method
    auto parser = ref new DecimalFormatter();
    auto getId = parser->ParseUInt(DeleteIdText->Text);

    if ((getId != nullptr) && (getId->Value < UINT32_MAX))
    {
        DeleteMessages(static_cast<uint32>(getId->Value));
    }
    else
    {
        rootPage->NotifyUser("Invalid message ID entered", NotifyType::ErrorMessage);
    }
}

void SDKSample::SmsSendReceive::DeleteMessage::DeleteAll_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Call the common delete method with MaxValue to indicate delete all.
    DeleteMessages(UINT32_MAX);
}

// Delete one or all messages.
// The ID of the message to delete is passed as a parameter. An ID of MaxValue
// specifies that all messages should be deleted.
void DeleteMessage::DeleteMessages(uint32 messageId)
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
        }).then([this, messageId] ()
        {
            DoDelete(messageId);
        });
    }
    else
    {
        DoDelete(messageId);
    }
}

void DeleteMessage::DoDelete(uint32 messageId)
{
    // Delete one or all messages.
    if (messageId < UINT32_MAX)
    {
        // Verify ID is within range (1 to message store capacity). Note that a SIM
        // can have gaps in its message array, so all valid IDs do not necessarily map
        // to messages.
        if (messageId >= 1 && messageId <= device->MessageStore->MaxMessages)
        {
            // Delete the selected message asynchronously.
            rootPage->NotifyUser("Deleting message ...", NotifyType::StatusMessage);

            create_task(device->MessageStore->DeleteMessageAsync(messageId))
                .then([this, messageId] (task<void> catchErrors)
            {
                try
                {
                    catchErrors.get();
                    rootPage->NotifyUser("Message " + messageId + " deleted", NotifyType::StatusMessage);
                }
                catch (Platform::Exception^ ex)
                {
                    rootPage->NotifyUser("Failed to delete message", NotifyType::ErrorMessage);
                }
            });
        }
        else
        {
            rootPage->NotifyUser("Message ID entered is out of range", NotifyType::ErrorMessage);
        }
    }
    else
    {
        // Delete all messages asynchronously.
        rootPage->NotifyUser("Deleting all messages ...", NotifyType::StatusMessage);
        create_task(device->MessageStore->DeleteMessagesAsync(SmsMessageFilter::All))
            .then([this, messageId] (task<void> catchErrors)
        {
            try
            {
                catchErrors.get();
                rootPage->NotifyUser("All messages deleted", NotifyType::StatusMessage);
            }
            catch (Platform::Exception^ ex)
            {
                rootPage->NotifyUser("Failed to delete messages", NotifyType::ErrorMessage);
            }
        });
    }
}
