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
using Windows.UI.Xaml.Documents;
using SDKTemplate;
using System;

namespace Controls_WebView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario1()
        {
            this.InitializeComponent();
            address.KeyUp += address_KeyUp;
            webView1.NavigationStarting += webView1_NavigationStarting;
            webView1.ContentLoading += webView1_ContentLoading;
            webView1.DOMContentLoaded += webView1_DOMContentLoaded;
            webView1.UnviewableContentIdentified += webView1_UnviewableContentIdentified;
            webView1.NavigationCompleted += webView1_NavigationCompleted;
        }

        /// <summary>
        /// Invoked when this xaml page is about to be displayed in a Frame.
        /// Note: This event is not related to the webview navigation.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            address.Text="http://www.microsoft.com";
            //NavigateWebview("http://www.microsoft.com");
        }

        /// <summary>
        /// This is the click handler for the 'Navigation' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void goButton_Click(object sender, RoutedEventArgs e)
        {
            if (!pageIsLoading)
            {
                NavigateWebview(address.Text);
            }
            else
            {
                webView1.Stop();
                pageIsLoading = false;
            }
        }

        /// <summary>
        /// This handles the enter key in the url address box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void address_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                NavigateWebview(address.Text);
            }
        }

        /// <summary>
        /// Helper to perform the navigation in webview
        /// </summary>
        /// <param name="url"></param>
        private void NavigateWebview(string url)
        {
            try
            {
                Uri targetUri = new Uri(url);
                webView1.Navigate(targetUri);
            }
            catch (FormatException myE)
            {
                // Bad address
                webView1.NavigateToString(String.Format("<h1>Address is invalid, try again.  Details --> {0}.</h1>", myE.Message));
            }
        }

        /// <summary>
        /// Property to control the "Go" button text, forward/backward buttons and progress ring
        /// </summary>
        private bool _pageIsLoading;
        bool pageIsLoading
        {
            get { return _pageIsLoading; }
            set
            {
                _pageIsLoading = value;
                goButton.Content = (value ? "Stop" : "Go");
                progressRing1.Visibility = (value ? Visibility.Visible : Visibility.Collapsed);

                if (!value)
                {
                    navigateBack.IsEnabled = webView1.CanGoBack;
                    navigateForward.IsEnabled = webView1.CanGoForward;
                }
            }
        }

        /// <summary>
        /// Event to indicate webview is starting a navigation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void webView1_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            string url = "";
            try { url = args.Uri.ToString(); }
            finally
            {
                address.Text = url;
                appendLog(String.Format("Starting navigation to: \"{0}\".\n", url));
                pageIsLoading = true;
            }
        }

        /// <summary>
        /// Event is fired by webview when the content is not a webpage, such as a file download
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        async void webView1_UnviewableContentIdentified(WebView sender, WebViewUnviewableContentIdentifiedEventArgs args)
        {
            appendLog(String.Format("Content for \"{0}\" cannot be loaded into webview. Invoking the default launcher instead.\n", args.Uri.ToString()));
            // We turn around and hand the Uri to the system launcher to launch the default handler for it
            await Windows.System.Launcher.LaunchUriAsync(args.Uri);
            pageIsLoading = false;
        }

        /// <summary>
        /// Event to indicate webview has resolved the uri, and that it is loading html content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void webView1_ContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            string url = (args.Uri != null) ? args.Uri.ToString() : "<null>";
            appendLog(String.Format("Loading content for \"{0}\".\n", url));
        }

        /// <summary>
        /// Event to indicate that the content is fully loaded in the webview. If you need to invoke script, it is best to wait for this event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void webView1_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            string url = (args.Uri != null) ? args.Uri.ToString() : "<null>";
            appendLog(String.Format("Content for \"{0}\" has finished loading.\n", url));
        }

        /// <summary>
        /// Event to indicate webview has completed the navigation, either with success or failure.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void webView1_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            pageIsLoading = false;
            if (args.IsSuccess)
            {
                string url = (args.Uri != null) ? args.Uri.ToString() : "<null>";
                appendLog(String.Format("Navigation to \"{0}\"completed successfully.\n", url));
            }
            else
            {
                string url = "";
                try { url = args.Uri.ToString(); }
                finally
                {
                    address.Text = url;
                    appendLog(String.Format("Navigation to: \"{0}\" failed with error code {1}.\n", url, args.WebErrorStatus.ToString()));
                }
            }
        }

        /// <summary>
        /// Helper for logging
        /// </summary>
        /// <param name="logEntry"></param>
        void appendLog(string logEntry)
        {
            Run r = new Run();
            r.Text = logEntry;
            Paragraph p = new Paragraph();
            p.Inlines.Add(r);
            logResults.Blocks.Add(p);
        }

        /// <summary>
        /// Handler for the GoBack button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navigateBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (webView1.CanGoBack) webView1.GoBack();
        }

        /// <summary>
        /// Handler for the GoForward button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navigateForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (webView1.CanGoForward) webView1.GoForward();
        }
    }
}
