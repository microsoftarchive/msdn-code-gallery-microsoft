//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.Networking.NetworkOperators;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Collections.Generic;

namespace UssdApi
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RequestResponse : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public RequestResponse()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }


        /// <summary>
        /// This is the click handler for the 'SendButton' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the USSD message text.
            if (MessageText.Text == "")
            {
                rootPage.NotifyUser("Message cannot be empty", NotifyType.ErrorMessage);
                return;
            }

            try
            {
                // Get the network account ID.
                IReadOnlyList<string> networkAccIds = Windows.Networking.NetworkOperators.MobileBroadbandAccount.AvailableNetworkAccountIds;

                if (networkAccIds.Count == 0)
                {
                    rootPage.NotifyUser("No network account ID found", NotifyType.ErrorMessage);
                    return;
                }
                // For the sake of simplicity, assume we want to use the first account.
                // Refer to the MobileBroadbandAccount API's for how to select a specific account ID.
                string networkAccountId = networkAccIds[0];

                SendButton.IsEnabled = false;
                rootPage.NotifyUser("Sending USSD request", NotifyType.StatusMessage);

                // Create a USSD session for the specified network acccount ID.
                UssdSession session = UssdSession.CreateFromNetworkAccountId(networkAccountId);

                // Send a message to the network and wait for the reply. This message is
                // specific to the network operator and must be selected accordingly.
                UssdReply reply = await session.SendMessageAndGetReplyAsync(new UssdMessage(MessageText.Text));

                // Display the network reply. The reply always contains a ResultCode.
                UssdResultCode code = reply.ResultCode;
                if (code == UssdResultCode.ActionRequired || code == UssdResultCode.NoActionRequired)
                {
                    // If the actionRequired or noActionRequired ResultCode is returned, the reply contains
                    // a message from the network.
                    UssdMessage replyMessage = reply.Message;
                    string payloadAsText = replyMessage.PayloadAsText;
                    if (payloadAsText != "")
                    {
                        // The message may be sent using various encodings. If Windows supports
                        // the encoding, the message can be accessed as text and will not be empty.
                        // Therefore, the test for an empty string is sufficient.
                        rootPage.NotifyUser("Response: " + payloadAsText, NotifyType.StatusMessage);
                    }
                    else
                    {
                        // If Windows does not support the encoding, the application may check
                        // the DataCodingScheme used for encoding and access the raw message
                        // through replyMessage.GetPayload
                        rootPage.NotifyUser("Unsupported data coding scheme 0x" + replyMessage.DataCodingScheme.ToString("X"),
                            NotifyType.StatusMessage);
                    }
                }
                else
                {
                    rootPage.NotifyUser("Request failed: " + code.ToString(), NotifyType.StatusMessage);
                }
                if (code == UssdResultCode.ActionRequired)
                {
                    session.Close(); // Close the session from our end
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Unexpected exception occured: " + ex.ToString(), NotifyType.ErrorMessage);
            }
            SendButton.IsEnabled = true;
        }
    }
}
