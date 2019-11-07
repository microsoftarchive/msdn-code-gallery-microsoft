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
using Windows.ApplicationModel.DataTransfer;

namespace Controls_WebView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario7 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario7()
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
            webView7.NavigationCompleted += webView7_NavigationCompleted;
            webView7.NavigationStarting += webView7_NavigationStarting;
            webView7.Navigate(new Uri("http://www.microsoft.com"));

            // Register for the share event
            DataTransferManager.GetForCurrentView().DataRequested += dataTransferManager_DataRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Unregister for the share event
            DataTransferManager.GetForCurrentView().DataRequested -= dataTransferManager_DataRequested;
        }

        void webView7_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            ProgressRing1.IsActive = true;
        }

        void webView7_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            ProgressRing1.IsActive = false;
        }

        /// <summary>
        /// This is the click handler for the 'Share Content' button.
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Share_Click(object sender, RoutedEventArgs e)
        {
            // Show the share UI
            DataTransferManager.ShowShareUI();
        }

        /// <summary>
        /// Called when a share is instigated either through the charms bar or the button in the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async void dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            //We are going to use an async API to talk to the webview, so get a deferral for the results
            DataRequestDeferral deferral = args.Request.GetDeferral();
            DataPackage dp = await webView7.CaptureSelectedContentToDataPackageAsync();
           
            if (dp != null && dp.GetView().AvailableFormats.Count >0)
            {
                // Webview has a selection, so we'll share its data package
                dp.Properties.Title = "This is the selection from the webview control";
                request.Data = dp;
            }
            else
            {
                // No selection, so we'll share the url of the webview
                DataPackage myData = new DataPackage();
                myData.SetWebLink(webView7.Source);
                myData.Properties.Title = "This is the URI from the webview control";
                myData.Properties.Description = webView7.Source.ToString();
                request.Data = myData;
            }
            deferral.Complete();
        }
    }
}
