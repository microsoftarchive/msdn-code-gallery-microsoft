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
    public sealed partial class Scenario7 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        DataTransferManager dataTransferManager = null;
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
            WebView7.LoadCompleted += WebView7_LoadCompleted;
            WebView7.Navigate(new Uri("http://www.wsj.com"));
        }

        void WebView7_LoadCompleted(object sender, NavigationEventArgs e)
        {
            WebView7.Visibility = Windows.UI.Xaml.Visibility.Visible;
            BlockingRect.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ProgressRing1.IsActive = false;
        }

        /// <summary>
        /// This is the click handler for the 'Share Content' button.
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Share_Click(object sender, RoutedEventArgs e)
        {
            dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += dataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        void dataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            DataPackage p = WebView7.DataTransferPackage;

            if (p.GetView().Contains(StandardDataFormats.Text))
            {
                p.Properties.Title = "WebView Sharing Excerpt";
                p.Properties.Description = "This is a snippet from the content hosted in the WebView control";
                request.Data = p;
            }
            else
            {
                request.FailWithDisplayText("Nothing to share");
            }
            dataTransferManager.DataRequested -= dataTransferManager_DataRequested;
        }
    }
}
