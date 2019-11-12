// Copyright (c) Microsoft. All rights reserved.

using System;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Action_Center_Quickstart
{
    /// <summary>
    /// How to use the NotificationManager.History to remove notifications from this app in the action center.
    /// </summary>
    public sealed partial class Scenario4 : Page
    {
        private MainPage rootPage;

        public Scenario4()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
        }

        private void SampleToasts_Click(object sender, RoutedEventArgs e)
        {
            // Use a helper method to check whether I can send a toast.
            // Note: If this check wasn't here, the code would fail silently.
            if (MySampleHelper.CanSendToasts())
            {
                // Create the notifier once
                var notifier = ToastNotificationManager.CreateToastNotifier();

                // Use a helper method to create a new ToastNotification.
                ToastNotification toast = MySampleHelper.CreateTextOnlyToast("Scenario 4", "msg Tag=\"T1\", Group=\"G1\"");

                // Set SuppressPopup = true on the toast in order to send it directly to action center without 
                // producing a popup on the user's phone.
                toast.SuppressPopup = true;

                toast.Tag = "T1";
                toast.Group = "G1";

                // Send the toast.
                notifier.Show(toast);

                toast = MySampleHelper.CreateTextOnlyToast("Scenario 4", "msg with Tag=\"T2\"");

                // Set SuppressPopup = true on the toast in order to send it directly to action center without 
                // producing a popup on the user's phone.
                toast.SuppressPopup = true;

                toast.Tag = "T2";

                // Send the toast.
                notifier.Show(toast);

                toast = MySampleHelper.CreateTextOnlyToast("Scenario 4", "msg with Tag=\"T1\", Group=\"G2\"");

                // Set SuppressPopup = true on the toast in order to send it directly to action center without 
                // producing a popup on the user's phone.
                toast.SuppressPopup = true;

                toast.Tag = "T1";
                toast.Group = "G2";

                // Send the toast.
                notifier.Show(toast);

                toast = MySampleHelper.CreateTextOnlyToast("Scenario 4", "message with no Tag or Group");

                // Set SuppressPopup = true on the toast in order to send it directly to action center without 
                // producing a popup on the user's phone.
                toast.SuppressPopup = true;

                // Send the toast.
                notifier.Show(toast);

                // Tell the user that the toast was sent successfully. 
                // This can be ommitted in a real app.
                rootPage.NotifyUser("Sample toasts sent.\nYou can now try the remove actions.", NotifyType.StatusMessage);

            }
        }

        private void RemoveByTag_Click(object sender, RoutedEventArgs e)
        {
            // Use the Remove(string) overload to remove all notifications with the given Tag property value and no value set for the Group property.
            // Note: Any toasts that have this Tag property value, but which also have the Group property set will not be removed using this method.
            ToastNotificationManager.History.Remove("T2");
            rootPage.NotifyUser("Notifications with Tag=\"T2\" have been removed.\nOpen action center to verify.", NotifyType.StatusMessage);
        }

        private void RemoveByGroup_Click(object sender, RoutedEventArgs e)
        {
            // Use the RemoveGroup(string) method to remove all notifications with the given group id.
            ToastNotificationManager.History.RemoveGroup("G1");
            rootPage.NotifyUser("Notifications with Group=\"G1\" have been removed.\nOpen action center to verify.", NotifyType.StatusMessage);
        }

        private void RemoveByTagAndGroup_Click(object sender, RoutedEventArgs e)
        {
            // Use the RemoveGroup(string, string) method to remove all notifications with the given tag id and group id.
            ToastNotificationManager.History.Remove("T1", "G2");
            rootPage.NotifyUser("Notifications with Tag=\"T1\" and Group=\"G2\" have been removed.\nOpen action center to verify.", NotifyType.StatusMessage);
        }

        private void RemoveAll_Click(object sender, RoutedEventArgs e)
        {
            ToastNotificationManager.History.Clear();
            rootPage.NotifyUser("All notifications have been removed. Open action center to verify that there is no longer any entries for this app.", NotifyType.StatusMessage);
        }

    }
}
