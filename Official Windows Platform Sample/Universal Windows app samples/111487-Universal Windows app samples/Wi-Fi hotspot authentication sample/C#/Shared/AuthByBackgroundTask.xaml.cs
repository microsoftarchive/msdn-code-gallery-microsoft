//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using HotspotAuthenticationTask;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace HotspotAuthenticationApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AuthByBackgroundTask : Page
    {
        // A pointer back to the main page.  This is needed if you call methods in MainPage such as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public AuthByBackgroundTask()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Configure background task handler to perform authentication
            ConfigStore.AuthenticateThroughBackgroundTask = true;

            // Setup completion handler
            ScenarioCommon.Instance.RegisteredCompletionHandlerForBackgroundTask();
        }
    }
}
