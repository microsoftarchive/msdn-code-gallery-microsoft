//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace AppBarControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : Windows.UI.Xaml.Controls.Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        StackPanel leftPanel = null;
        StackPanel rightPanel = null;
        Brush originalBackgroundBrush = null;
        Brush originalSeparatorBrush = null;
        Style originalButtonStyle = null;

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
            // Save off original background
            originalBackgroundBrush = rootPage.BottomAppBar.Background;

            // Set the new AppBar Background
            rootPage.BottomAppBar.Background = new SolidColorBrush(Color.FromArgb(255, 20, 20, 90));

            // Find our stack panels that contain our AppBarButtons

            leftPanel = rootPage.FindName("LeftPanel") as StackPanel;
            rightPanel = rootPage.FindName("RightPanel") as StackPanel;

            // Change the color of all AppBarButtons in each panel
            ColorButtons(leftPanel);
            ColorButtons(rightPanel);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            rootPage.BottomAppBar.Background = originalBackgroundBrush;
            RestoreButtons(leftPanel);
            RestoreButtons(rightPanel);
        }

        /// <summary>
        /// This method will change the style of each AppBarButton to use a green foreground color
        /// </summary>
        /// <param name="panel"></param>
        private void ColorButtons(StackPanel panel)
        {
            int count = 0;
            foreach (var item in panel.Children)
            {
                // For AppBarButton, change the style
                if (item.GetType() == typeof(AppBarButton))
                {
                    if (count == 0)
                    {
                        originalButtonStyle = ((AppBarButton)item).Style;
                    }
                    ((AppBarButton)item).Style = rootPage.Resources["GreenAppBarButtonStyle"] as Style;
                    count++;
                }
                else
                {
                    // For AppBarSeparator(s), just change the foreground color
                    if (item.GetType() == typeof(AppBarSeparator))
                    {
                        originalSeparatorBrush = ((AppBarSeparator)item).Foreground;
                        ((AppBarSeparator)item).Foreground = new SolidColorBrush(Color.FromArgb(255, 90, 200, 90));
                    }
                }
            }
        }

        /// <summary>
        /// This method will restore the style of each AppBarButton
        /// </summary>
        /// <param name="panel"></param>
        private void RestoreButtons(StackPanel panel)
        {
            foreach (var item in panel.Children)
            {
                // For AppBarButton, change the style
                if (item.GetType() == typeof(AppBarButton))
                {
                    ((AppBarButton)item).Style = originalButtonStyle;
                }
                else
                {
                    // For AppBarSeparator(s), just change the foreground color
                    if (item.GetType() == typeof(AppBarSeparator))
                        ((AppBarSeparator)item).Foreground = originalSeparatorBrush;
                }
            }
        }
    }
}
