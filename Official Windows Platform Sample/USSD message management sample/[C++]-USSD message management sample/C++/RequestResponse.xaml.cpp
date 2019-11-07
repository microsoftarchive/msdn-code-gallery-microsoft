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
// RequestResponse.xaml.cpp
// Implementation of the RequestResponse class
//

#include "pch.h"
#include "RequestResponse.xaml.h"

using namespace concurrency;
using namespace Platform;
using namespace Windows::Networking::NetworkOperators;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace SDKSample::UssdApi;

RequestResponse::RequestResponse()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void RequestResponse::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

/// <summary>
/// This is the click handler for the 'SendButton' button.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void SDKSample::UssdApi::RequestResponse::SendButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Verify the USSD message text.
    if (MessageText->Text == L"")
    {
        rootPage->NotifyUser("Message cannot be empty", NotifyType::ErrorMessage);
        return;
    }

    try
    {
        // Get the network account ID.
        auto networkAccIds = MobileBroadbandAccount::AvailableNetworkAccountIds;
        if (networkAccIds->Size == 0)
        {
            rootPage->NotifyUser("No network account ID found", NotifyType::ErrorMessage);
            return;
        }
        // For the sake of simplicity, assume we want to use the first account.
        // Refer to the MobileBroadbandAccount API's for how to select a specific account ID.
        auto networkAccountId = networkAccIds->GetAt(0);

        SendButton->IsEnabled = false;
        rootPage->NotifyUser("Sending USSD request", NotifyType::StatusMessage);

        // Create a USSD session for the specified network acccount ID.
        auto ussdSession = UssdSession::CreateFromNetworkAccountId(networkAccountId);

        // Create a USSD message to be send to the network. This message is specific to
        // the network operator.
        auto message = ref new UssdMessage(MessageText->Text);

        // Execute asynchronous operation as task
        task<UssdReply^> getUssdReply(ussdSession->SendMessageAndGetReplyAsync(message));
        getUssdReply.then([=](task<UssdReply^> replyTask)
        {
            try
            {
                // Get will throw an exception if the task failed with an error.
                auto reply = replyTask.get();
                auto code = reply->ResultCode;
                if (code == UssdResultCode::ActionRequired || code == UssdResultCode::NoActionRequired)
                {
                    // If the actionRequired or noActionRequired ResultCode is returned, the reply contains
                    // a message from the network.
                    auto replyMessage = reply->Message;
                    auto payloadAsText = replyMessage->PayloadAsText;
                    if (payloadAsText != "")
                    {
                        // The message may be sent using various encodings. If Windows supports
                        // the encoding, the message can be accessed as text and will not be empty.
                        // Therefore, the test for an empty string is sufficient.
                        rootPage->NotifyUser("Response: " + payloadAsText, NotifyType::StatusMessage);
                    }
                    else
                    {
                        // If Windows does not support the encoding, the application may check
                        // the DataCodingScheme used for encoding and access the raw message
                        // through replyMessage->GetPayload()
                        wchar_t buf[100];
                        HRESULT hr = StringCchPrintf(buf, ARRAYSIZE(buf), L"Unsupported data coding scheme 0x%x", replyMessage->DataCodingScheme);
                        if (FAILED(hr))
                        {
                            throw Exception::CreateException(hr);
                        }
                        String^ s = ref new String(buf);
                        rootPage->NotifyUser(s, NotifyType::StatusMessage);
                    }
                }
                else
                {
                    rootPage->NotifyUser("No response message", NotifyType::StatusMessage);
                }
                if (code == UssdResultCode::ActionRequired)
                {
                    ussdSession->Close(); // Close the session from our end
                }
            }
            catch (Platform::Exception^ ex)
            {
                // Handle errors
                rootPage->NotifyUser("An unexpected exception occured: " + ex->Message, NotifyType::ErrorMessage);
            }
            SendButton->IsEnabled = true;
        });
    }
    catch (Exception^ ex)
    {
        rootPage->NotifyUser("An unexpected exception occured: " + ex->Message, NotifyType::ErrorMessage);
        SendButton->IsEnabled = true;
    }
}
