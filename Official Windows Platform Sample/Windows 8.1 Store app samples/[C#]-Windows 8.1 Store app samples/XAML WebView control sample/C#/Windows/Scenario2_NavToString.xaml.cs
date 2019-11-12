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
    public sealed partial class Scenario2 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this xaml page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Using the storage classes to read the content from a file
            StorageFile f = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///html/html_example.html"));
            string htmlFragment = await FileIO.ReadTextAsync(f);

            // This is now a string so we can manipluate it before we use it.
            htmlFragment=htmlFragment.Replace(@"</body>", "    <p>This content will be handed to webview when you click the button.</p>\n  </body>");
            // Put the string into the textbox
            HTML2.Text = htmlFragment;
        }

        /// <summary>
        /// This is the click handler for the 'Load' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            // Grab the HTML from the text box and load it into the WebView
            WebView2.NavigateToString(HTML2.Text);
        }
    }
}
