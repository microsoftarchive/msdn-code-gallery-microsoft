// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using SDKTemplateCS;
using System;
using Windows.Networking.PushNotifications;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PushAndPeriodicNotificationsSampleCS
{
	public sealed partial class ScenarioInput5 : Page
	{
		// A pointer back to the main page which is used to gain access to the input and output frames and their content.
		MainPage rootPage = null;

		public ScenarioInput5()
		{
			InitializeComponent();
		}

        void StartBadgePolling_Click(object sender, RoutedEventArgs e)
        {
            String polledUrl = BadgePollingURL.Text;

            // The default string for this text box is "http://".
            // Make sure the user has entered some data.
            if (polledUrl != "http://")
            {
                PeriodicUpdateRecurrence recurrence = (PeriodicUpdateRecurrence)PeriodicRecurrence.SelectedIndex;

                // You can also specify a time you would like to start polling. Secondary tiles can also receive
                // polled updates using BadgeUpdateManager.createBadgeUpdaterForSecondaryTile(tileId).
                BadgeUpdateManager.CreateBadgeUpdaterForApplication().StartPeriodicUpdate(new Uri(polledUrl), recurrence);

                 rootPage.NotifyUser("Started polling " + polledUrl + ". Look at the application’s tile on the Start menu to see the latest update.", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Specify a URL that returns badge XML to begin badge polling.", NotifyType.ErrorMessage);
            }
        }

        void StopBadgePolling_Click(object sender, RoutedEventArgs e)
        {
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().StopPeriodicUpdate();
            rootPage.NotifyUser("Stopped polling.", NotifyType.StatusMessage);
        }

		#region Template-Related Code - Do not remove
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			// Get a pointer to our main page
			rootPage = e.Parameter as MainPage;

			// We want to be notified with the OutputFrame is loaded so we can get to the content.
			rootPage.OutputFrameLoaded += new System.EventHandler(rootPage_OutputFrameLoaded);
		}

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
