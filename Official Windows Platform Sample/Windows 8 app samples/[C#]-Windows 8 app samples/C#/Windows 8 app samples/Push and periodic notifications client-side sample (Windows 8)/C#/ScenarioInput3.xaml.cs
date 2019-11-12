// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using SDKTemplateCS;
using System;
using Windows.Foundation;
using Windows.Networking.PushNotifications;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PushAndPeriodicNotificationsSampleCS
{
	public sealed partial class ScenarioInput3 : Page
	{
		// A pointer back to the main page which is used to gain access to the input and output frames and their content.
		MainPage rootPage = null;

		public ScenarioInput3()
		{
			InitializeComponent();
		}

        private void AddCallback_Click(object sender, RoutedEventArgs e)
        {
            PushNotificationChannel currentChannel = rootPage.Channel;
            if (currentChannel != null)
            {
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
            String typeString = String.Empty;
            String notificationContent = String.Empty;
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
                    break;
                case PushNotificationType.Raw:
                    notificationContent = e.RawNotification.Content;
                    typeString = "Raw";
                    break;
            }

            // Setting the cancel property prevents the notification from being delivered. It's especially important to do this for toasts:
            // if your application is already on the screen, there's no need to display a toast from push notifications.
            e.Cancel = true;

            String text = "Received a " + typeString + " notification, containing: " + notificationContent;
            var ignored = Window.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
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

            rootPage.OutputFrameLoaded -= new System.EventHandler(rootPage_OutputFrameLoaded);
        }

		#region Template-Related Code - Do not remove
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			// Get a pointer to our main page
			rootPage = e.Parameter as MainPage;

			// We want to be notified with the OutputFrame is loaded so we can get to the content.
			rootPage.OutputFrameLoaded += new System.EventHandler(rootPage_OutputFrameLoaded);
		}


		#endregion

		#region Use this code if you need access to elements in the output frame - otherwise delete
		void rootPage_OutputFrameLoaded(object sender, object e)
		{
			// At this point, we know that the Output Frame has been loaded and we can go ahead
			// and reference elements in the page contained in the Output Frame.

			// Get a pointer to the content within the OutputFrame
			Page outputFrame = (Page)rootPage.OutputFrame.Content;

			// Go find the elements that we need for this scenario.
			// ex: flipView1 = outputFrame.FindName("FlipView1") as FlipView;

		}
		#endregion
	}
}
