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
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2()
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
            // Let's create a very simple HTML fragment
            string htmlFragment =
                @"<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
                <html>
                   <head>
                      <title>Map with valid credentials</title>
                      <meta http-equiv='Content-Type' content='text/html; charset=utf-8'/>
                      <script type='text/javascript' src='http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=7.0'></script>
                      <script type='text/javascript'>
                      var map = null;
                      function getMap()
                      {
                          map = new Microsoft.Maps.Map(document.getElementById('myMap'), {credentials: 'Your Bing Maps Key'});
                      } 
                      </script>
                   </head>
                   <body onload='getMap();'>
                      <div id='myMap' style='position:relative; width:400px; height:400px;'></div>
                   </body>
                </html>;";

            // And load it into the HTML text box so it will be visible
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
