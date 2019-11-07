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
using System.Collections.Generic;
using SDKTemplate;
using System;

namespace Controls_WebView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario4 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario4()
        {
            this.InitializeComponent();
            WebView4.ScriptNotify += WebView4_ScriptNotify;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }


        void WebView4_ScriptNotify(object sender, NotifyEventArgs e)
        {
            rootPage.NotifyUser(string.Format("Response from script: '{0}'", e.Value), NotifyType.StatusMessage);
        }

        /// <summary>
        /// This is the click handler for the 'FireScript' button.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FireScript_Click(object sender, RoutedEventArgs e)
        {
            if (NavToString.IsChecked == true)
            {
                // We can run script that uses window.external.notify() to send data back to the app 
                // without having to set the AllowedScriptNotifyUris property because the app is
                // trusted and it owns the content of the script.
                WebView4.InvokeScript("SayGoodbye", null);
            }
            else
            {
                if (Nav.IsChecked == true)
                {
                    // Here we have to set the AllowedScriptNotifyUri property because we are navigating
                    // to some actual site where we don't own the content and we want to allow window.external.notify()
                    // to pass back data to our application.
                    List<Uri> allowedUris = new List<Uri>();
                    allowedUris.Add(new Uri("http://www.bing.com"));
                    WebView4.AllowedScriptNotifyUris = allowedUris;

                    // Notice that this is fairly contrived but for this example to work we need to be
                    // able to navigate to a real site, but since this site does not have a function that 
                    // we can call that actually uses window.external.notify() we have to inject that into
                    // the page using eval().  See the next scenario for more information on this technique.
                    string[] args = { "window.external.notify('GoodBye');" };
                    WebView4.InvokeScript("eval", args);
                }
                else
                {
                    rootPage.NotifyUser("Please choose a navigation method", NotifyType.ErrorMessage);
                }
            }
        }

        private void NavToString_Click(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("", NotifyType.StatusMessage);
            // Let's create an HTML fragment that contains some javascript code that we will fire using
            // InvokeScript().
            string htmlFragment =
                @"
<html>
    <head>
        <script type='text/javascript'>
            function SayGoodbye() 
            {
                window.external.notify('GoodBye'); 
            }
        </script>
    </head>
    <body>
        Page with 'Goodbye' script loaded.  Click the 'Fire Script' button to run the script and send data back to the application.
    </body>
</html>";

            // Load the fragment into the HTML text box so it will be visible.
            HTML4.Text = htmlFragment;
            WebView4.NavigateToString(HTML4.Text);
        }

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("", NotifyType.StatusMessage);
            WebView4.Navigate(new System.Uri("http://www.bing.com"));
        }

    }
}
