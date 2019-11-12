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
    /// Recieving events for raw notification arrival
    /// </summary>
    public sealed partial class Scenario2 : Page
    {
        private MainPage rootPage = null;
        private CoreDispatcher _dispatcher;
        private bool eventAdded = false;

        public Scenario2()
        {
            this.InitializeComponent();
            _dispatcher = Window.Current.Dispatcher;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = MainPage.Current;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            UpdateListener(false);
        }

        private void Scenario2AddListener_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateListener(true))
            {
                rootPage.NotifyUser("Now listening for raw notifications", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Channel not open--open a channel in Scenario 1", NotifyType.ErrorMessage);
            }
        }

        private void Scenario2RemoveListener_Click(object sender, RoutedEventArgs e)
        {
            if (UpdateListener(false))
            {
                rootPage.NotifyUser("No longer listening for raw notifications", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Channel not open--open a channel in Scenario 1", NotifyType.ErrorMessage);
            }
        }

        private bool UpdateListener(bool add)
        {
            if (rootPage.Channel != null)
            {
                if (add && !eventAdded)
                {
                    rootPage.Channel.PushNotificationReceived += OnPushNotificationReceived;
                    eventAdded = true;
                }
                else if (!add && eventAdded)
                {
                    rootPage.Channel.PushNotificationReceived -= OnPushNotificationReceived;
                    eventAdded = false;
                }
                return true;
            }
        return false;
        }

        private async void OnPushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs e)
        {
            if (e.NotificationType == PushNotificationType.Raw)
            {
                e.Cancel = true;
                await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    rootPage.NotifyUser("Raw notification received with content: " + e.RawNotification.Content, NotifyType.StatusMessage);
                });
            }
        }
    }
}
