// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using PushNotificationsHelper;
using SDKTemplateCS;
using System;
using Windows.Networking.PushNotifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PushAndPeriodicNotificationsSampleCS
{
	public sealed partial class ScenarioInput1 : Page
	{
		// A pointer back to the main page which is used to gain access to the input and output frames and their content.
		MainPage rootPage = null;

		public ScenarioInput1()
		{
			InitializeComponent();
		}

        // The Notifier object allows us to use the same code in the maintenance task and this foreground application
        private async void OpenChannel_Click(Object sender, RoutedEventArgs e)
        {
            ChannelAndWebResponse channelAndWebResponse = await rootPage.Notifier.OpenChannelAndUploadAsync(ServerText.Text);
            rootPage.NotifyUser("Channel uploaded! Response:" + channelAndWebResponse.WebResponse, NotifyType.StatusMessage);
            rootPage.Channel = channelAndWebResponse.Channel;
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
			rootPage = e.Parameter as MainPage;

			// We want to be notified with the OutputFrame is loaded so we can get to the content.
			rootPage.OutputFrameLoaded += new System.EventHandler(rootPage_OutputFrameLoaded);

			if (rootPage.Notifier == null)
			{
				rootPage.Notifier = new Notifier();
			}
		}
		#region Template-Related Code - Do not remove
		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			rootPage.OutputFrameLoaded -= new System.EventHandler(rootPage_OutputFrameLoaded);
		}

		#endregion

		#region Use this code if you need access to elements in the output frame - otherwise delete
		void rootPage_OutputFrameLoaded(object sender, object e)
		{
			// At this point, we know that the Output Frame has been loaded and we can go ahead
			// and reference elements in the page contained in the Output Frame.

			// Get a pointer to the content within the OutputFrame.
			Page outputFrame = (Page)rootPage.OutputFrame.Content;

			// Go find the elements that we need for this scenario.
			// ex: flipView1 = outputFrame.FindName("FlipView1") as FlipView;
		}

		#endregion
	}
}
