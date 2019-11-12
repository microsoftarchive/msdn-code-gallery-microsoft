// Copyright (c) Microsoft. All rights reserved.

using System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Action_Center_Quickstart
{
    /// <summary>
    /// This example demonstrates how to replace a notification. 
    /// By matching the tag and group properties of a notification, the replacement policy in the system will replace the old notification with the new one.
    /// </summary>
    public sealed partial class Scenario5 : Page
    {
        private MainPage rootPage;

        public Scenario5()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
        }

        private void SendOriginal_Click(object sender, RoutedEventArgs e)
        {
            // Use a helper method to check whether I can send a toast.
            // Note: If this check wasn't here, the code would fail silently.
            if (MySampleHelper.CanSendToasts())
            {
                // Use a helper method to create a new ToastNotification.
                ToastNotification toast = MySampleHelper.CreateTextOnlyToast("Scenario 5", OriginalText.Text);

                // Set SuppressPopup = true on the toast in order to send it directly to action center without 
                // producing a popup on the user's phone.
                toast.SuppressPopup = true;

                // Set the toast's Tag and Group properties so that it can be identified and replaced when we send another
                // toast with the same Tag and Group values.
                toast.Tag = "BYA";
                toast.Group = "DailyStockPrices";

                // Send the toast.
                ToastNotificationManager.CreateToastNotifier().Show(toast);

                rootPage.NotifyUser("Original notification sent.\nOpen the action center to view it", NotifyType.StatusMessage);

            }
        }

        private void SendUpdate_Click(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrEmpty(ReplacementText.Text))
            {
                rootPage.NotifyUser("Replacement text is empty.\nEnter a value in the TextBox.", NotifyType.ErrorMessage);
                return;
            }

            // Use a helper method to check whether I can send a toast.
            // Note: If this check wasn't here, the code would fail silently.
            if (MySampleHelper.CanSendToasts())
            {
                string updatedMessage = String.Format("Blue Yonder (BYA) ${0} {1}", ReplacementText.Text, DateTime.Now.ToString("t"));

                // Use a helper method to create a new ToastNotification.
                ToastNotification toast = MySampleHelper.CreateTextOnlyToast("Scenario 5", updatedMessage);

                // Set SuppressPopup = true on the toast in order to send the toast directly to action center without 
                // producing a popup on the user's phone.
                toast.SuppressPopup = true;

                // If a toast exists in action center with the following Tag and Group values, it will be replaced
                // when we send this toast.
                toast.Tag = "BYA";
                toast.Group = "DailyStockPrices";

                // Send the toast.
                ToastNotificationManager.CreateToastNotifier().Show(toast);

                rootPage.NotifyUser("Original notification has been updated.\nOpen the action center to view it", NotifyType.StatusMessage);

            }
        }

    }
}
