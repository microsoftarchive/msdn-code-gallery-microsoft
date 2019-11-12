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
    public sealed partial class ReceiveMessage : SDKTemplate.Common.LayoutAwarePage
    {
        private SmsDevice device;
        private int msgCount;
        private bool listening;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // Constructor
        public ReceiveMessage()
        {
            this.InitializeComponent();
        }

        // Initialize variables and controls for the scenario.
        // This method is called just before the scenario page is displayed.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            listening = false;
            msgCount = 0;

            ReceivedFromText.Text = "";
            ReceivedMessageText.Text = "";
        }

        // Clean-up when scenario page is left. This is called when the
        // user navigates away from the scenario page.
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Detach event handler
            if (listening)
            {
                device.SmsMessageReceived -= device_SmsMessageReceived;
            }

            // Release the device
            device = null;
        }


        // Handle a request to listen for received messages.
        private async void Receive_Click(object sender, RoutedEventArgs e)
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

            // Attach a message received handler to the device, if not already listening
            if (!listening)
            {
                try
                {
                    msgCount = 0;
                    device.SmsMessageReceived += device_SmsMessageReceived;
                    listening = true;
                    rootPage.NotifyUser("Waiting for message ...", NotifyType.StatusMessage);
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

        // Handle a received message event.
        async void device_SmsMessageReceived(SmsDevice sender, SmsMessageReceivedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                () =>
                {
                    try
                    {
                        // Get message from the event args.
                        SmsTextMessage msg = args.TextMessage;
                        msgCount += 1;

                        ReceivedCountText.Text = msgCount.ToString();
                        ReceivedFromText.Text = msg.From;
                        ReceivedMessageText.Text = msg.Body;
                        rootPage.NotifyUser(msgCount.ToString() + ((msgCount == 1) ? " message" : " messages") + " received", NotifyType.StatusMessage);
                    }
                    catch (Exception ex)
                    {
                        rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
                    }
                });
        }
    }
}
