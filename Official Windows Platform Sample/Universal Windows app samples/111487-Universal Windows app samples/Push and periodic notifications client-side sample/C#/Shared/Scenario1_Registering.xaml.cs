// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using PushNotificationsHelper;
using Windows.Networking.PushNotifications;

namespace SDKTemplate
{
    /// <summary>
    /// Registering channels for push notifications
    /// </summary>
    public sealed partial class Scenario1 : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        MainPage rootPage = null;

        public Scenario1()
        {
            InitializeComponent();
        }

        // The Notifier object allows us to use the same code in the maintenance task and this foreground application
        private async void OpenChannel_Click(Object sender, RoutedEventArgs e)
        {
            try
            {
                ChannelAndWebResponse channelAndWebResponse = await rootPage.Notifier.OpenChannelAndUploadAsync(ServerText.Text);
                rootPage.NotifyUser("Channel uploaded! Response:" + channelAndWebResponse.WebResponse, NotifyType.StatusMessage);
                rootPage.Channel = channelAndWebResponse.Channel;
            }
            catch (FormatException ex)
            {
                rootPage.NotifyUser("Channel not uploaded. An exception occurred: {0}" + ex.Message, NotifyType.ErrorMessage);
            }
            catch (AggregateException)
            {
                rootPage.NotifyUser("Channel not uploaded. Multiple exceptions occurred while uploading channel.", NotifyType.ErrorMessage);
            }
        }

        private void CloseChannel_Click(Object sender, RoutedEventArgs e)
        {
            PushNotificationChannel currentChannel = rootPage.Channel;
            if (currentChannel != null)
            {
                // Closing the channel prevents all future cloud notifications from 
                // being delivered to the application or application related UI
                currentChannel.Close();
                rootPage.Channel = null;

                rootPage.NotifyUser("Channel closed", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Channel not open", NotifyType.ErrorMessage);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Get a pointer to our main page
            rootPage = MainPage.Current;

            if (rootPage.Notifier == null)
            {
                rootPage.Notifier = new Notifier();
            }
        }
    }
}
