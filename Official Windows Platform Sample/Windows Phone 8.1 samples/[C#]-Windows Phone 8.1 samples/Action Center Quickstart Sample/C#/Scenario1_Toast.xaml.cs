// Copyright (c) Microsoft. All rights reserved.

using System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace Action_Center_Quickstart
{
    /// <summary>
    /// Demonstrates how to send a toast that can be viewed in the action center.
    /// </summary>
    public sealed partial class Scenario1 : Page
    {
        private MainPage rootPage;

        public Scenario1()
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
                // Each time we run this code, we'll append the count (toastIndex in this example) to the message
                // so that it can be seen as a unique message in the action center. This is not mandatory - we
                // do it here for educational purposes.
                ToastNotification toast = MySampleHelper.CreateTextOnlyToast("Scenario 1",String.Format("message {0}", toastIndex));

                // Optional. Setting an expiration time on a toast notification defines the maximum
                // time the notification will be displayed in action center before it expires and is removed. 
                // If this property is not set, the notification expires after 7 days and is removed.
                // Tapping on a toast in action center launches the app and removes it immediately from action center.
                toast.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(3600);

                // Display the toast.
                ToastNotificationManager.CreateToastNotifier().Show(toast); 
                toastIndex++;

                // Tell the user that the toast was sent successfully. 
                // This can be ommitted in a real app.
                MySampleHelper.ShowSuccessMessage();

            }
        }

    }
}
