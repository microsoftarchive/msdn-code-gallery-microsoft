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
    public sealed partial class SendMessage : SDKTemplate.Common.LayoutAwarePage
    {
        private SmsDevice device;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // Constructor
        public SendMessage()
        {
            this.InitializeComponent();
        }

        // Initialize variables and controls for the scenario.
        // This method is called just before the scenario page is displayed.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SendToText.Text = "";
            SendMessageText.Text = "";
        }

        // Clean-up when scenario page is left. This is called when the
        // user navigates away from the scenario page.
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Release the device.
            device = null;
        }

        // Handle a request to send a text message.
        private async void Send_Click(object sender, RoutedEventArgs e)
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

            try
            {
                // Create a text message - set the entered destination number and message text.
                SmsTextMessage msg = new SmsTextMessage();
                msg.To = SendToText.Text;
                msg.Body = SendMessageText.Text;

                // Send the message asynchronously
                rootPage.NotifyUser("Sending message ...", NotifyType.StatusMessage);
                await device.SendMessageAsync(msg);
                rootPage.NotifyUser("Text message sent", NotifyType.StatusMessage);
            }
            catch (Exception err)
            {
                rootPage.NotifyUser(err.Message, NotifyType.ErrorMessage);

                // On failure, release the device. If the user revoked access or the device
                // is removed, a new device object must be obtained.
                device = null;
            }
        }
    }
}
