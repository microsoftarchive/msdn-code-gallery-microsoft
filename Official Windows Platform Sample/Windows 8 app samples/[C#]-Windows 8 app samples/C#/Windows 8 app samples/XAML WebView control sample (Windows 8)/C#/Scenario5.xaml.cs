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
using System.Collections.Generic;

namespace Controls_WebView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario5 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario5()
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
            WebView5.LoadCompleted += WebView5_LoadCompleted;
            WebView5.Navigate(new Uri("http://kexp.org/Playlist"));
        }

        void WebView5_LoadCompleted(object sender, NavigationEventArgs e)
        {
            List<Uri> allowedUris = new List<Uri>();
            allowedUris.Add(new Uri("http://kexp.org"));
            WebView5.AllowedScriptNotifyUris = allowedUris;
            WebView5.ScriptNotify += WebView5_ScriptNotify;
            string[] args = { "document.title;" };
            string foo = WebView5.InvokeScript("eval", args);
            rootPage.NotifyUser(String.Format("Title is: {0}", foo), NotifyType.StatusMessage);
        }

        void WebView5_ScriptNotify(object sender, NotifyEventArgs e)
        {
            rootPage.NotifyUser(e.Value, NotifyType.StatusMessage);
        }
    }
}
