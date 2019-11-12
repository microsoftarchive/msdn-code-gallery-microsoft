// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SDKTemplate
{
    /// <summary>
    /// Listening for notification events via callback
    /// </summary>
    public sealed partial class Scenario3 : Page
    {
        private MainPage rootPage = null;
        private CoreDispatcher dispatcher = null;

        public Scenario3()
        {
			InitializeComponent();
        }

        private void AddCallback_Click(object sender, RoutedEventArgs e)
        {
            PushNotificationChannel currentChannel = rootPage.Channel;
            if (currentChannel != null)
            {
                dispatcher = Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher;
                currentChannel.PushNotificationReceived += OnPushNotificationReceived;
                rootPage.NotifyUser("Callback added.", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Channel not open. Open the channel in scenario 1.", NotifyType.ErrorMessage);
            }
        }

        private void RemoveCallback_Click(object sender, RoutedEventArgs e)
        {
            PushNotificationChannel currentChannel = rootPage.Channel;
            if (currentChannel != null)
            {
                currentChannel.PushNotificationReceived -= OnPushNotificationReceived;
                rootPage.NotifyUser("Callback removed.", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Channel not open. Open the channel in scenario 1.", NotifyType.StatusMessage);
            }
        }

        void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs e)
        {
            string typeString = String.Empty;
            string notificationContent = String.Empty;
            switch (e.NotificationType)
            {
                case PushNotificationType.Badge:
                    typeString = "Badge";
                    notificationContent = e.BadgeNotification.Content.GetXml();
                    break;
                case PushNotificationType.Tile:
                    notificationContent = e.TileNotification.Content.GetXml();
                    typeString = "Tile";
                    break;
                case PushNotificationType.Toast:
                    notificationContent = e.ToastNotification.Content.GetXml();
                    typeString = "Toast";
                    // Setting the cancel property prevents the notification from being delivered. It's especially important to do this for toasts:
                    // if your application is already on the screen, there's no need to display a toast from push notifications.
                    e.Cancel = true;
                    break;
                case PushNotificationType.Raw:
                    notificationContent = e.RawNotification.Content;
                    typeString = "Raw";
                    break;
            }

            

            string text = "Received a " + typeString + " notification, containing: " + notificationContent;
            var ignored = dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                rootPage.NotifyUser(text, NotifyType.StatusMessage);
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            PushNotificationChannel currentChannel = rootPage.Channel;

            if (currentChannel != null)
            {
                currentChannel.PushNotificationReceived -= OnPushNotificationReceived;
            }
		}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
        }
    }
}
