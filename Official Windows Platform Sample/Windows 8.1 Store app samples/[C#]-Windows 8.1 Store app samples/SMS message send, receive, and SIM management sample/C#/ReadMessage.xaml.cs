//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

using Windows.Devices.Sms;
using SmsSendReceive;

namespace SmsSendReceive
{
    public sealed partial class ReadMessage : SDKTemplate.Common.LayoutAwarePage
    {
        private SmsDevice device;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // Constructor
        public ReadMessage()
        {
            this.InitializeComponent();
        }

        // Initialize variables and controls for the scenario.
        // This method is called just before the scenario page is displayed.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ReadIdText.Text = "";
            DateText.Text = "";
            ReadFromText.Text = "";
            ReadMessageText.Text = "";
        }

        // Clean-up when scenario page is left. This is called when the
        // user navigates away from the scenario page.
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Release the device.
            device = null;
        }

         // Handle a request to read a message.
        private async void Read_Click(object sender, RoutedEventArgs e)
        {
            // If this is the first request, get the default SMS device. If this
            // is the first SMS device access, the user will be prompted to grant
            // access permission for this application.
            if (device == null)
            {
                try
                {
                    rootPage.NotifyUser("Getting default SMS device ...", NotifyType.StatusMessage);
                    device = await SmsDevice.GetDefaultAsync();
                }
                catch (Exception ex)
                {
                    rootPage.NotifyUser("Failed to find SMS device\n" + ex.Message, NotifyType.ErrorMessage);
                    return;
                }
            }

            // Clear message display.
            DateText.Text = "";
            ReadFromText.Text = "";
            ReadMessageText.Text = "";

            try
            {
                // Parse the message ID - must be number between 1 and maximum message count.
                uint id;
                if (uint.TryParse(ReadIdText.Text, out id) &&
                   (id >= 1) && (id <= device.MessageStore.MaxMessages))
                {
                    rootPage.NotifyUser("Reading message ...", NotifyType.StatusMessage);

                    // Get the selected message from message store asynchronously.
                    ISmsMessage msg = await device.MessageStore.GetMessageAsync(id);

                    // See if this is a text message by querying for the text message interface.
                    ISmsTextMessage textMsg = msg as ISmsTextMessage;
                    if (textMsg == null)
                    {
                        // If it is a binary message then try to convert it to a text message.
                        if (msg is SmsBinaryMessage)
                        {
                            textMsg = SmsTextMessage.FromBinaryMessage(msg as SmsBinaryMessage);
                        }
                    }

                    // Display the text message information.
                    if (textMsg != null)
                    {
                        DateText.Text = textMsg.Timestamp.DateTime.ToString();
                        ReadFromText.Text = textMsg.From;
                        ReadMessageText.Text = textMsg.Body;

                        rootPage.NotifyUser("Message read.", NotifyType.StatusMessage);
                    }
                }
                else
                {
                    rootPage.NotifyUser("Invalid ID number entered.", NotifyType.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);

                // On failure, release the device. If the user revoked access or the device
                // is removed, a new device object must be obtained.
                device = null;
            }
        }
    }
}
