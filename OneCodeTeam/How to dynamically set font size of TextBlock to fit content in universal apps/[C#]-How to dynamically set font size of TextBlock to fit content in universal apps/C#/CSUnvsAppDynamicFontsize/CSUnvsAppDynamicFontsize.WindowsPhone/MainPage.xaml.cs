/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.cs
* Project:      CSUnvsAppDynamicFontsize.WindowsPhone
* Copyright (c) Microsoft Corporation.
*
* This code sample shows you how to dynamically set the font size of a 
* TextBlock to fit its content in universal Windows apps.
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
using Windows.UI.Xaml.Navigation;


namespace CSUnvsAppDynamicFontsize
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


        private async void Footer_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hyperlinkButton = sender as HyperlinkButton;
            if (hyperlinkButton != null)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(hyperlinkButton.Tag.ToString()));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ContentTextBlock.FontSize = 30;
            ContentTextBlock.Text = ContentTextBox.Text;
        }

        private void ContentTextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            TextBlock contentTextBlock = sender as TextBlock;
            
            if (contentTextBlock != null)
            {
                double height = contentTextBlock.Height;
                if (this.ContentTextBlock.ActualHeight > height)
                {
                    // Get how many times the TextBlock's height compares with the existing one
                    double fontsizeMultiplier = Math.Sqrt(height / this.ContentTextBlock.ActualHeight);

                    // Set the new FontSize
                    this.ContentTextBlock.FontSize = Math.Floor(this.ContentTextBlock.FontSize * fontsizeMultiplier);
                }
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ContentTextBlock.Height = ContentTextBox.ActualHeight;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > e.NewSize.Height)
            {
                VisualStateManager.GoToState(this, "LandscapeLayout", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "DefaultLayout", true);
            }      
        }
    }
}
