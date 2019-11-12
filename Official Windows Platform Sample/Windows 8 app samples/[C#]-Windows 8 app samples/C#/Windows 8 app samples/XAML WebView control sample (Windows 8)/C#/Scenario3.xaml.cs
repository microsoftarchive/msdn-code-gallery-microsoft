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

namespace Controls_WebView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3 : SDKTemplate.Common.LayoutAwarePage
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
            // Let's create an HTML fragment that contains some javascript code that we will fire using
            // InvokeScript().
            string htmlFragment = 
                @"
<html>
    <head>
        <script type='text/javascript'>
            function SayGoodbye() 
            { 
                document.getElementById('myDiv').innerText = 'GoodBye'; 
            }
        </script>
    </head>
    <body>
        <div id='myDiv'>Hello</div>
     </body>
</html>";

            // Load it into the HTML text box so it will be visible.
            HTML3.Text = htmlFragment;
        }

        /// <summary>
        /// This is the click handler for the 'Load' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            // Grab the HTML from the text box and load it into the WebView
            WebView3.NavigateToString(HTML3.Text);
        }

        /// <summary>
        /// This is the click handler for the 'Script' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Script_Click(object sender, RoutedEventArgs e)
        {
            // Invoke the javascript function called 'SayGoodbye' that is loaded into the WebView.
            WebView3.InvokeScript("SayGoodbye", null);
        }
    }
}
