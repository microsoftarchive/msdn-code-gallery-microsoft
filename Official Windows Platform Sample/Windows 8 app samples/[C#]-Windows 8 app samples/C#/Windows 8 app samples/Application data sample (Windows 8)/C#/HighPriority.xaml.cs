//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.Storage;

namespace ApplicationDataSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HighPriority : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        ApplicationDataContainer roamingSettings = null;

        public HighPriority()
        {
            this.InitializeComponent();

            ApplicationData applicationData = ApplicationData.Current;

            roamingSettings = applicationData.RoamingSettings;
            applicationData.DataChanged += new TypedEventHandler<ApplicationData, object>(DataChangedHandler);

            DisplayOutput(false);
        }

        // Guidance for using the HighPriority setting.
        //
        // Writing to the HighPriority setting enables a developer to store a small amount of
        // data that will be roamed out to the cloud with higher priority than other roaming
        // data, when possible.
        //
        // Applications should carefully consider which data should be stored in the 
        // HighPriority setting.  "Context" data such as the user's location within
        // media, or their current game-baord and high-score, can make the most sense to
        // roam with high priority.  By using the HighPriority setting, this information has
        // a higher likelihood of being available to the user when they begin to use another
        // machine.
        //
        // Applications should update their HighPriority setting when the user makes
        // a significant change to the data it represents.  Examples could include changing
        // music tracks, turning the page in a book, or finishing a level in a game.

        void IncrementHighPriority_Click(Object sender, RoutedEventArgs e)
        {
            int counter = Convert.ToInt32(roamingSettings.Values["HighPriority"]);

            roamingSettings.Values["HighPriority"] = counter + 1;

            DisplayOutput(false);
        }

        async void DataChangedHandler(Windows.Storage.ApplicationData appData, object o)
        {
            // DataChangeHandler may be invoked on a background thread, so use the Dispatcher to invoke the UI-related code on the UI thread.
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                DisplayOutput(true);
            });
        }

        void DisplayOutput(bool remoteUpdate)
        {
            int counter = Convert.ToInt32(roamingSettings.Values["HighPriority"]);

            OutputTextBlock.Text = "Counter: " + counter + (remoteUpdate ? " (updated remotely)" : "");
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
