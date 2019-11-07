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
using SDKTemplate.Common;

namespace ScanRuntimeAPI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1EnumerateScanners : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page. This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1EnumerateScanners()
        {
            this.InitializeComponent();
            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateButtons();
        }

        /// <summary>
        /// Invoked when user nagivates away from this page
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ModelDataContext.UnLoad();
        }


        /// <summary>
        /// Updates Start and Stop Enumeration buttons
        /// </summary>
        private void UpdateButtons()
        {
            StartEnumerationWatcher.IsEnabled = !ModelDataContext.ScannerDataContext.WatcherStarted;
            StopEnumerationWatcher.IsEnabled = ModelDataContext.ScannerDataContext.WatcherStarted;
        }
        
        /// <summary>
        /// Starts the enumeration of scanner devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Enumeration_Watcher_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                rootPage.NotifyUser("Scanner enumeration watcher started.", NotifyType.StatusMessage);
                ModelDataContext.ScannerDataContext.StartScannerWatcher();
                UpdateButtons();
            }
        }

        /// <summary>
        /// Stops the enumeration of scanner devices.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop_Enumeration_Watcher_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                rootPage.NotifyUser("Scanner enumeration watcher stopped.", NotifyType.StatusMessage);
                ModelDataContext.ScannerDataContext.StopScannerWatcher();
                UpdateButtons();
            }
        }

    }
}
