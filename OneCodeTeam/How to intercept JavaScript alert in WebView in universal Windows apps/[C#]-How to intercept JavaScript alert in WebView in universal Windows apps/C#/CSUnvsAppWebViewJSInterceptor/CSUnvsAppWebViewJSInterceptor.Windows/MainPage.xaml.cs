/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.cs
* Project:      CSUnvsAppWebViewJSInterceptor.Windows
* Copyright (c) Microsoft Corporation.
*
* This code sample shows you how to intercept JavaScript alert in WebView and 
* display the alert message in Universal apps.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace CSUnvsAppWebViewJSInterceptor
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        
        private async void WebViewWithJSInjection_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            string result = await this.WebViewWithJSInjection.InvokeScriptAsync("eval", new string[] { "window.alert = function (AlertMessage) {window.external.notify(AlertMessage)}" });
        }

        private async void WebViewWithJSInjection_ScriptNotify(object sender, NotifyEventArgs e)
        {
            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog(e.Value);
            await dialog.ShowAsync();
        }

        private async void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri((sender as HyperlinkButton).Tag.ToString()));
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width <= 500)
            {
                VisualStateManager.GoToState(this, "MinimalLayout", true);
            }
            else if (e.NewSize.Width > e.NewSize.Height)
            {
                VisualStateManager.GoToState(this, "DefaultLayout", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "PortraitLayout", true);
            }
        }


    }

}
