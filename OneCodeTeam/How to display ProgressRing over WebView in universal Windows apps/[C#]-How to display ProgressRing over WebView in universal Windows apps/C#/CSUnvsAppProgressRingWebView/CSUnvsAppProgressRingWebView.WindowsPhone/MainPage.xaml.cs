/****************************** Module Header ******************************\
 * Module Name:  MainPage.xaml.cs
 * Project:      CSUnvsAppProgressRingWebView.WindowsPhone
 * Copyright (c) Microsoft Corporation.
 * 
 * This sample demonstrates how to display ProgressRing over WebView.
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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;


namespace CSUnvsAppProgressRingWebView
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        #region Common methods

        /// <summary>
        /// Click event handler for the link in the footer. 
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private async void Footer_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hyperlinkButton = sender as HyperlinkButton;
            if (hyperlinkButton != null)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(hyperlinkButton.Tag.ToString()));
            }
        }

        public void NotifyUser(string message)
        {
            this.statusText.Text = message;
        }

        #endregion
        /// <summary>
        /// WebView navigation completed event hanlder.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void DisplayWebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs e)
        {
            LoadingProcessProgressRing.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            LoadingProcessProgressRing.IsActive = false;
            LoadButton.IsEnabled = true;
            DisplayWebView.Visibility = Visibility.Visible;

        }
        /// <summary>
        /// Load button click event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = ValidateAndGetUri(UrlTextBox.Text);
            if (uri != null)
            {
                try
                {
                    LoadingProcessProgressRing.IsActive = true;
                    LoadingProcessProgressRing.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    LoadButton.IsEnabled = false;

                    DisplayWebView.Navigate(uri);
                }
                catch (Exception ex)
                {
                    NotifyUser(ex.ToString());
                }
            }
        }
        /// <summary>
        /// Validate if the uri is available.
        /// </summary>
        /// <param name="uriString">
        /// The input string.
        /// </param>
        /// <returns></returns>
        private Uri ValidateAndGetUri(string uriString)
        {
            Uri uri = null;
            try
            {
                uri = new Uri(uriString);
                HintTextBlock.Visibility = Visibility.Collapsed;
            }
            catch (FormatException e)
            {
                HintTextBlock.Text = e.Message;
                HintTextBlock.Visibility = Visibility.Visible;
            }
            return uri;
        }

        private void UrlTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(UrlTextBox.Text))
            {
                LoadButton.IsEnabled = true;
            }
            else
            {
                LoadButton.IsEnabled = false;
            }
        }
        /// <summary>
        /// When the user presses "Enter" key, call LoadButton_Click method to load the content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UrlTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                LoadButton_Click(sender, e);
            }
        }
    }
}
