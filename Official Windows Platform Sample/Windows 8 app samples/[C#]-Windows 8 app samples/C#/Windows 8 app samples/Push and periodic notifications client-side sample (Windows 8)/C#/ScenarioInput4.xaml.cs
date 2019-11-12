// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using SDKTemplateCS;
using System;
using System.Collections.Generic;
using Windows.Networking.PushNotifications;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace PushAndPeriodicNotificationsSampleCS
{
	public sealed partial class ScenarioInput4 : Page
	{
		// A pointer back to the main page which is used to gain access to the input and output frames and their content.
		MainPage rootPage = null;

		public ScenarioInput4()
		{
			InitializeComponent();
		}

        void StartTilePolling_Click(object sender, RoutedEventArgs e)
        {
            List<Uri> urisToPoll = new List<Uri>(5);
            foreach (TextBox input in new TextBox[] { PollURL1, PollURL2, PollURL3, PollURL4, PollURL5 })
            {
                String polledUrl = input.Text;

                // The default string for this text box is "http://".
                // Make sure the user has entered some data.
                if (polledUrl != "http://" && polledUrl != "") {
                    urisToPoll.Add(new Uri(polledUrl));
                }
            }
            
            PeriodicUpdateRecurrence recurrence = (PeriodicUpdateRecurrence)PeriodicRecurrence.SelectedIndex;

            if (urisToPoll.Count == 1)
            {
                TileUpdateManager.CreateTileUpdaterForApplication().StartPeriodicUpdate(urisToPoll[0], recurrence);
                rootPage.NotifyUser("Started polling " + urisToPoll[0].AbsolutePath + ". Look at the application’s tile on the Start menu to see the latest update.", NotifyType.StatusMessage);
            }
            else if (urisToPoll.Count > 1)
            {
                TileUpdateManager.CreateTileUpdaterForApplication().StartPeriodicUpdateBatch(urisToPoll, recurrence);
                rootPage.NotifyUser("Started polling the specified URLs. Look at the application’s tile on the Start menu to see the latest update.", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Specify a URL that returns tile XML to begin tile polling.", NotifyType.ErrorMessage);
            }
        }

        void StopTilePolling_Click(object sender, RoutedEventArgs e)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().StopPeriodicUpdate();
            rootPage.NotifyUser("Stopped polling.", NotifyType.StatusMessage);
        }
		
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
            // IMPORTANT NOTE: call this only if you plan on polling several different URLs, and only
            // once after the user installs the app or creates a secondary tile
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);

			// Get a pointer to our main page
			rootPage = e.Parameter as MainPage;

			// We want to be notified with the OutputFrame is loaded so we can get to the content.
			rootPage.OutputFrameLoaded += new System.EventHandler(rootPage_OutputFrameLoaded);
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
