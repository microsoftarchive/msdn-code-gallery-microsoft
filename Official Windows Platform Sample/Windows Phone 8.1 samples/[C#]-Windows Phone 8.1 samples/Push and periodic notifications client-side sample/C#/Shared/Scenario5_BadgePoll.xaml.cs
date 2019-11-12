// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
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
    /// Polling for badge notifications
    /// </summary>
    public sealed partial class Scenario5 : Page
    {        
        private MainPage rootPage = null;

        public Scenario5()
        {
			InitializeComponent();
        }
		
		void StartBadgePolling_Click(object sender, RoutedEventArgs e)
        {
            string polledUrl = BadgePollingURL.Text;

            // The default string for this text box is "http://".
            // Make sure the user has entered some data.
            if (Uri.IsWellFormedUriString(polledUrl, UriKind.Absolute))
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
		
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {            
            rootPage = MainPage.Current;
        }
    }
}
