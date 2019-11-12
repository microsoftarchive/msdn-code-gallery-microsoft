// Copyright (c) Microsoft. All rights reserved.

using System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Action_Center_Quickstart
{
    /// <summary>
    /// Sends multiple toasts, to illustrate the FIFO behavior of the action center 
    /// notification queue. 
    /// </summary>
    public sealed partial class Scenario3 : Page
    {
        private MainPage rootPage;

        public Scenario3()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
        }

        // Use this index to create unique toast messages.
        int toastIndex = 1;
        int numberToastsToSend = 25;
        private void Queue_Click(object sender, RoutedEventArgs e)
        {
            // Use a helper method to check whether I can send a toast.
            // Note: If this check wasn't here, the code would fail silently.
            if (MySampleHelper.CanSendToasts())
            {
                // Send multiple toasts to action center
                for (int i = 0; i < numberToastsToSend; i++)
                {
                    // Use a helper method to create a new ToastNotification.
                    ToastNotification toast = MySampleHelper.CreateTextOnlyToast("Scenario 3", String.Format("message {0}", toastIndex));

                    // Set SuppressPopup = true on the toast in order to send it directly to action center without 
                    // producing a popup on the user's phone.
                    toast.SuppressPopup = true;

                    // Send the toast.
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                    toastIndex++;
                }

                // Tell the user that the toast was sent successfully. 
                // This can be ommitted in a real app.
                MySampleHelper.ShowSuccessMessage(String.Format("We sent {0} toasts to action center.\nThe last 20 notifications will be shown in action center, followed by a \"More Notifications\" message to indicate that the app sent more toasts than are shown.", numberToastsToSend));

            }
        }

    }
}
