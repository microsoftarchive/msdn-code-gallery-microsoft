/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.cs
* Project:      CSUnvsAppDynamicFontsize.Windows
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


namespace CSUnvsAppDynamicFontsize
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();


        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ContentTextBlock.Height = ContentTextBox.ActualHeight;
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
                    // Get the ratio of the TextBlock's height to that of the TextBox’s
                    double fontsizeMultiplier = Math.Sqrt(height / this.ContentTextBlock.ActualHeight);

                    // Set the new FontSize
                    this.ContentTextBlock.FontSize = Math.Floor(this.ContentTextBlock.FontSize * fontsizeMultiplier);
                }                
            }  
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width < 800.0)
            {
                VisualStateManager.GoToState(this, "MinimalLayout", true);
            }
            else if (e.NewSize.Width < e.NewSize.Height)
            {
                VisualStateManager.GoToState(this, "PortraitLayout", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "DefaultLayout", true);
            }
        }

    }
}
