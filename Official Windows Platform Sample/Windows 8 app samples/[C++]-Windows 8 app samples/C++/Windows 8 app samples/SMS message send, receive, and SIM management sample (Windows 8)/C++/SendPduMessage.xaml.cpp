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
// SendPduMessage.xaml.cpp
// Implementation of the SendPduMessage class
//

#include "pch.h"
#include "SendPduMessage.xaml.h"

using namespace SDKSample::SmsSendReceive;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Devices::Sms;
using namespace concurrency;

SendPduMessage::SendPduMessage()
{
    InitializeComponent();
}

// Initialize variables and controls for the scenario.
// This method is called just before the scenario page is displayed.
void SendPduMessage::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

// Clean-up when scenario page is left. This is called when the
// user navigates away from the scenario page.
void SendPduMessage::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // Release the device.
    device = nullptr;
}

void SendPduMessage::Send_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
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

void SendPduMessage::DoSend()
{
    // Convert the entered message from hex to a byte array.
    Platform::Array<unsigned char, 1U>^ data;

    // Parse the hex string.
    try
    {
        data = ParseHexString(PduMessageText->Text);
    }
    catch (Platform::Exception^ ex)
    {
        rootPage->NotifyUser("Failed to parse message\n" + ex->Message, NotifyType::ErrorMessage);
        return;
    }

    // Create a binary message and set the data.
    SmsBinaryMessage^ msg = ref new SmsBinaryMessage();
    msg->SetData(data);

    // Set format based on the SMS device cellular type (GSM or CDMA)
    msg->Format = (device->CellularClass == CellularClass::Gsm) ? SmsDataFormat::GsmSubmit : SmsDataFormat::CdmaSubmit;

    // Send message asynchronously.
    rootPage->NotifyUser("Sending message ...", NotifyType::StatusMessage);
    create_task(device->SendMessageAsync(msg))
        .then([this] (task<void> catchErrors)
    {
        try
        {
            catchErrors.get();
            rootPage->NotifyUser("Sent message in PDU format", NotifyType::StatusMessage);
        }
        catch(Platform::Exception^ ex)
        {
            rootPage->NotifyUser("Failed to send message\n" + ex->Message, NotifyType::ErrorMessage);
        }
    });
}

// Parse hex string to binary array
Platform::Array<unsigned char, 1U>^ SendPduMessage::ParseHexString(Platform::String^ messageText)
{
    auto messageWcharData = messageText->Data();
    auto messageLength = wcslen(messageWcharData);

    if ((messageLength%2 == 0) && (messageLength > 0))
    {
        auto binaryLength = messageLength/2;

        // Used to initialize the binary array
        auto binaryValues = new unsigned char[binaryLength];

        wchar_t subString[3] = L"";

        // Used to check wcstoul result
        wchar_t* ptr;

        // Convert 2 hex characters to binary value
        for (unsigned int i = 0; i < binaryLength; i++)
        {
            subString[0] = messageWcharData[i*2];
            subString[1] = messageWcharData[i*2+1];

            // reset errno value to check if errors occured in wcstoul
            errno = 0;

            auto binaryNumber = wcstoul(subString, &ptr, 16);
            if (errno != 0 || ptr==nullptr || ptr == subString || *ptr != '\0')
            {
                delete [] binaryValues;
                throw ref new Platform::InvalidArgumentException;
            }

            binaryValues[i] = static_cast<unsigned char> (binaryNumber);
        }

        auto message = ref new Platform::Array<unsigned char, 1U> (binaryValues,binaryLength);
        delete [] binaryValues;
        return message;
    }
    else
    {
        throw ref new Platform::InvalidArgumentException;
    }
}