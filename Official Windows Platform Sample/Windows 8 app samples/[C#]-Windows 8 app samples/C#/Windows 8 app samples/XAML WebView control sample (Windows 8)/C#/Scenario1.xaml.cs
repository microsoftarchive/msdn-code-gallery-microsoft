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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace Controls_WebView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
        {
            this.InitializeComponent();
            Address.KeyUp += Address_KeyUp;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Address.Text = "http://www.msn.com";
        }

        /// <summary>
        /// This is the click handler for the 'Navigation' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NavigateButton_Click(object sender, RoutedEventArgs e)
        {
            ProgressRing1.IsActive = true;

            // Provide an indication as to where we are trying to navigate to
            rootPage.NotifyUser(String.Format("Navigating to: {0}", Address.Text), NotifyType.StatusMessage);

            // Hook the LoadCompleted event for the WebView to know when the URL is fully loaded
            WebView1.LoadCompleted += new Windows.UI.Xaml.Navigation.LoadCompletedEventHandler(WebView1_LoadCompleted);

            // Attempt to navigate to the specified URL.  Notice that a malformed URL will raise a FormatException
            // which we catch and let the user know that the URL is bad and to enter a new well-formed one.
            try
            {
                Uri targetUri = new Uri(Address.Text);
                WebView1.Navigate(targetUri);
            }
            catch (FormatException myE)
            {
                // Bad address
                rootPage.NotifyUser(String.Format("Address is invalid, try again.  Details --> {0}", myE.Message), NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// The LoadCompleted handler allows us to know when the requested URL is navigated to (the document is completely
        /// loaded into the WebView).
        /// </summary>
        /// <param name="sender">The initiator of the event (WebView1)</param>
        /// <param name="e">The standard NavigationEventArgs</param>
        void WebView1_LoadCompleted(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            WebView1.Visibility = Windows.UI.Xaml.Visibility.Visible;
            BlockingRect.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            ProgressRing1.IsActive = false;

            // Tell the user that the page has loaded
            rootPage.NotifyUser("Page loaded", NotifyType.StatusMessage);
        }

        void Address_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                NavigateButton_Click(this, new RoutedEventArgs());
            }
        }
    }
}
