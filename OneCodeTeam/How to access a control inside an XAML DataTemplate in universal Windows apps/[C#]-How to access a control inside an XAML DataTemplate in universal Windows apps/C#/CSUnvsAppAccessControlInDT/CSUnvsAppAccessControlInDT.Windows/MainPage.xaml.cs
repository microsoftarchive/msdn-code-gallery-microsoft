/****************************** Module Header ******************************\
 * Module Name:    MainPage.xaml.cs
 * Project:        CSUnvsAppAccessControlInDT.Windows
 * Copyright (c) Microsoft Corporation.
 * 
 * This sample demonstrates how to access control inside DataTemplate in universal Windows apps.
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CSUnvsAppAccessControlInDT
{
    class VideoInfo
    {
        public string Name { get; set; }
        public string Link { get; set; }
        
    }
    public sealed partial class MainPage : Page
    {
        private List<Grid> m_renderedGrids = new List<Grid>();
        public MainPage()
        {
            this.InitializeComponent();

            List<VideoInfo> Videos = new List<VideoInfo> { 
            new VideoInfo { Name = "1. How to pick and manipulate a 3D object in universal Windows game apps", 
                Link = "http://video.ch9.ms/ch9/b994/95d5d113-f8d0-4489-91f1-6943eeabb994/OnecodeGridview_mid.mp4" }, 
            new VideoInfo { Name = "2. How to add wartermark text or image to a bitmap in Windows Store app", 
                Link = "http://video.ch9.ms/ch9/024c/988b0a77-acbf-4b8c-abc9-c460079c024c/AddWatermarkToBitmap_mid.mp4"},
            new VideoInfo {Name = "3. How to Add an Item Dynamically to a Grouped GridView in a Windows Store app",
                Link = "http://video.ch9.ms/ch9/b994/95d5d113-f8d0-4489-91f1-6943eeabb994/OnecodeGridview_mid.mp4"}
            };

            gridView.ItemsSource = Videos;
        }

        private async void Footer_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hyperlinkButton = sender as HyperlinkButton;
            if (hyperlinkButton != null)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(hyperlinkButton.Tag.ToString()));
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            // Add the Grids inside DataTemplate into a List.
            // You can get the controls inside DataTemplate from m_renderedGrids.
            m_renderedGrids.Add(sender as Grid);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {            
            try
            {
                int index = int.Parse(tbVideoIndex.Text) - 1;
                Grid currentGrid = m_renderedGrids[index] as Grid;

                MediaElement myVideo = (MediaElement)GetChildren(currentGrid).First(x => x.Name == "myVideo");

                myVideo.Play();
            }
            catch(Exception exception)
            {
                statusText.Text = exception.Message;
            }            
        }

        private List<FrameworkElement> GetChildren(DependencyObject parent)
        {
            List<FrameworkElement> controls = new List<FrameworkElement>();

            for(int i = 0; i< VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is FrameworkElement)
                {
                    controls.Add(child as FrameworkElement);
                }
                controls.AddRange(GetChildren(child));
            }

            return controls;
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
