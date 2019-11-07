//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace SDKTemplate
{
    public sealed partial class LastPrefetchTimeScenario : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public LastPrefetchTimeScenario()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void GetLastPrefetchTime_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                DateTimeOffset? lastPrefetchTime = Windows.Networking.BackgroundTransfer.ContentPrefetcher.LastSuccessfulPrefetchTime;
                if (lastPrefetchTime != null)
                {
                    LastPrefetchTime.Text = "The last successful prefetch time was " + lastPrefetchTime.ToString();
                }
                else
                {
                    LastPrefetchTime.Text = "There have been no successful prefetches yet";
                }
            }
        }

    }
}
