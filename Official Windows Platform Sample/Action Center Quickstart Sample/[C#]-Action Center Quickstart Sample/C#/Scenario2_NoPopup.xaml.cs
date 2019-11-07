// Copyright (c) Microsoft. All rights reserved.

using System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Action_Center_Quickstart
{
    /// <summary>
    /// Demonstrates how to send local toasts directly to action center, without showing the user a popup.
    /// </summary>
    public sealed partial class Scenario2 : Page
    {
        private MainPage rootPage;

        public Scenario2()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
        }

        // Use this index to create unique toast messages.
        int toastIndex = 1;
        private void DisplayToast_Click(object sender, RoutedEventArgs e)
        {
            // Use a helper method to check whether I can send a toast.
            // Note: If this check wasn't here, the code would fail silently.
            if (MySampleHelper.CanSendToasts())
            {
                // Use a helper method to create a new ToastNotification.
                ToastNotification toast = MySampleHelper.CreateTextOnlyToast("Scenario 2", String.Format("message {0}", toastIndex));

                // Set SuppressPopup = true on the toast in order to send it directly to action center without 
                // producing a popup on the user's phone.
                toast.SuppressPopup = true;

                // Send the toast.
                ToastNotificationManager.CreateToastNotifier().Show(toast);
                toastIndex++;

                // Tell the user that the toast was sent successfully. 
                // This can be ommitted in a real app.
                MySampleHelper.ShowSuccessMessage();

            }
        }
    }
}
