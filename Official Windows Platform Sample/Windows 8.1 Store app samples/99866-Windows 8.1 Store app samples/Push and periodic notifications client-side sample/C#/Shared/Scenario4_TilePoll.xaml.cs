// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
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
    /// Polling for tile notifications
    /// </summary>
    public sealed partial class Scenario4 : Page
    {
        private MainPage rootPage = null;

        public Scenario4()
        {
			InitializeComponent();
        }

        void StartTilePolling_Click(object sender, RoutedEventArgs e)
        {
            List<Uri> urisToPoll = new List<Uri>(5);
            foreach (TextBox input in new TextBox[] { PollURL1, PollURL2, PollURL3, PollURL4, PollURL5 })
            {
                string polledUrl = input.Text;

                // The default string for this text box is "http://".
                // Make sure the user has entered some data.
                if (Uri.IsWellFormedUriString(polledUrl,UriKind.Absolute)) 
                {
                    urisToPoll.Add(new Uri(polledUrl));
                }
                else
                {
                    rootPage.NotifyUser("Please enter a valid uri to poll.", NotifyType.ErrorMessage);
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
			rootPage = MainPage.Current;
		}
    }
}
