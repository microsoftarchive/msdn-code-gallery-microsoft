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
using System.Threading.Tasks;

using Windows.Devices.Sms;
using SmsSendReceive;

namespace SmsSendReceive
{
    public sealed partial class DeleteMessage : SDKTemplate.Common.LayoutAwarePage
    {
        private SmsDevice device;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // Constructor
        public DeleteMessage()
        {
            this.InitializeComponent();
        }

        // Initialize variables and controls for the scenario.
        // This method is called just before the scenario page is displayed.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DeleteIdText.Text = "";
        }

        // Clean-up when scenario page is left. This is called when the
        // user navigates away from the scenario page.
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Release the device.
            device = null;
        }

        // Handle a request to delete a message.
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Parse the entered message ID and pass it to the common delete method
            uint id;
            if (uint.TryParse(DeleteIdText.Text, out id))
            {
                await DoDeleteAsync(id);
            }
            else
            {
                rootPage.NotifyUser("Invalid message ID entered", NotifyType.ErrorMessage);
            }
       }

        // Handle a request to delete all messages.
        private async void DeleteAll_Click(object sender, RoutedEventArgs e)
        {
            // Call the common delete method with MaxValue to indicate delete all.
            await DoDeleteAsync(uint.MaxValue);
        }

        // Delete one or all messages.
        // The ID of the message to delete is passed as a parameter. An ID of MaxValue
        // specifies that all messages should be deleted.
        private async Task DoDeleteAsync(uint messageId)
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
                // Delete one or all messages.
                if (messageId < uint.MaxValue)
                {
                    // Verify ID is within range (1 to message store capacity). Note that a SIM
                    // can have gaps in its message array, so all valid IDs do not necessarily map
                    // to messages.
                    if (messageId >= 1 && messageId <= device.MessageStore.MaxMessages)
                    {
                        // Delete the selected message asynchronously.
                        rootPage.NotifyUser("Deleting message ...", NotifyType.StatusMessage);
                        await device.MessageStore.DeleteMessageAsync(messageId);
                        rootPage.NotifyUser("Message " + messageId + " deleted", NotifyType.StatusMessage);
                    }
                    else
                    {
                        rootPage.NotifyUser("Message ID entered is out of range", NotifyType.ErrorMessage);
                    }
                }
                else
                {
                    // Delete all messages asynchronously.
                    rootPage.NotifyUser("Deleting all messages ...", NotifyType.StatusMessage);
                    await device.MessageStore.DeleteMessagesAsync(SmsMessageFilter.All);
                    rootPage.NotifyUser("All messages deleted", NotifyType.StatusMessage);
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
