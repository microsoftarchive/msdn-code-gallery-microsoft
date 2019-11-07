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
using Windows.UI.Xaml.Documents;
using System.Collections.Generic;
using Windows.Storage;
using SDKTemplate;
using System;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace Controls_WebView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario6 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario6()
        {
            this.InitializeComponent();
            string src = "ms-appx-web:///html/scriptNotify_example.html";
            //webViewLabel.Text = string.Format("Webview: {0}", src);
            webView6.Navigate(new Uri(src));
            webView6.ScriptNotify += webView6_ScriptNotify;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }


        void webView6_ScriptNotify(object sender, NotifyEventArgs e)
        {
            // Be sure to verify the source of the message when performing actions with the data.
            // As webview can be navigated, you need to check that the message is coming from a page/code
            // that you trust.
            Color c = Colors.Red;

            if (e.CallingUri.Scheme =="ms-appx-web")
            {
                if (e.Value.ToLower() == "blue") c = Colors.Blue;
                else if (e.Value.ToLower() == "green") c = Colors.Green;
            }
            rootPage.NotifyUser(string.Format("Response from script at '{0}': '{1}'", e.CallingUri, e.Value), NotifyType.StatusMessage);
        }
    }
}
