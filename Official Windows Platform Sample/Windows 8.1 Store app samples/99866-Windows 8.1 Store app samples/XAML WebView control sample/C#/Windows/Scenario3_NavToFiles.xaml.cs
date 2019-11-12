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
using Windows.Storage;
using SDKTemplate;
using System;

namespace Controls_WebView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario3()
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
            // Initialize the local state directory with content
            createHtmlFileInLocalState();
        }

        // Copies the file "html\html_example2.html" from this package's installed location to
        // a new file "NavigateToState\test.html" in the local state folder.  When this is
        // done, enables the 'Load HTML' button.
        async void createHtmlFileInLocalState()
        {
            StorageFolder stateFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("NavigateToState", CreationCollisionOption.OpenIfExists);
            StorageFile htmlFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("html\\html_example2.html");

            await htmlFile.CopyAsync(stateFolder, "test.html", NameCollisionOption.ReplaceExisting);
            loadFromLocalState.IsEnabled = true;
        }

        /// <summary>
        /// Navigates the webview to the application package
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadFromPackage_Click(object sender, RoutedEventArgs e)
        {
            string url = "ms-appx-web:///html/html_example2.html";
            webView2.Navigate(new Uri(url));
            webViewLabel.Text = string.Format("Webview: {0}", url);
        }

        /// <summary>
        /// Navigates the webview to the local state directory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadFromLocalState_Click(object sender, RoutedEventArgs e)
        {
            string url = "ms-appdata:///local/NavigateToState/test.html";
            webView2.Navigate(new Uri(url));
            webViewLabel.Text = string.Format("Webview: {0}", url);
        }
    }
}



