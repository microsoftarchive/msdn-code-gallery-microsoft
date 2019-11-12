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
     public sealed partial class SendPduMessage : SDKTemplate.Common.LayoutAwarePage
    {
        private SmsDevice device;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // Constructor
        public SendPduMessage()
        {
            this.InitializeComponent();
        }

        // Initialize variables and controls for the scenario.
        // This method is called just before the scenario page is displayed.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PduMessageText.Text = "";
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
                // Convert the entered message from hex to a byte array.
                byte[] data;
                ParseHexString(PduMessageText.Text, out data);

                // Create a binary message and set the data.
                SmsBinaryMessage msg = new SmsBinaryMessage();
                msg.SetData(data);

                // Set format based on the SMS device cellular type (GSM or CDMA)
                msg.Format = (device.CellularClass == CellularClass.Gsm) ? SmsDataFormat.GsmSubmit : SmsDataFormat.CdmaSubmit;

                // Send message asynchronously.
                rootPage.NotifyUser("Sending message ...", NotifyType.StatusMessage);
                await device.SendMessageAsync(msg);
                rootPage.NotifyUser("Sent message sent in PDU format", NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);

                // On failure, release the device. If the user revoked access or the device
                // is removed, a new device object must be obtained.
                device = null;
            }
        }

        // Convert a hex string to an equivalent byte array.
        // The string must contain an even number of hex digits.
        void ParseHexString(string hex, out byte[] data)
        {
            if (hex.Length > 0 && (hex.Length & 1) == 0)
            {
                int byteCount = hex.Length / 2;
                data = new byte[byteCount];

                for (int i = 0; i < byteCount; i++)
                {
                    data[i] = byte.Parse(hex.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
            }
            else
            {
                throw new FormatException("Input string must have an even number of hex digits");
            }
        }
    }
}
