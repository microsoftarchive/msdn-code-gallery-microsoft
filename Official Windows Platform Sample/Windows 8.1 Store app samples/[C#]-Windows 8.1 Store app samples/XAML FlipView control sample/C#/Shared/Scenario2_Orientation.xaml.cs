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
using Windows.UI.Xaml.Media;

namespace Controls_FlipView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // Used to track the orientation state of the FlipView for Scenario #2
        bool bHorizontal = true;

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
            var sampleData = new Controls_FlipView.Data.SampleDataSource();
            FlipView2Horizontal.ItemsSource = sampleData.Items;
            FlipView2Vertical.ItemsSource = sampleData.Items;
        }

        /// <summary>
        /// This is the click handler for the 'Orientation' button.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Orientation_Click(object sender, RoutedEventArgs e)
        {
            // Turn off the animations
            FlipView2Horizontal.UseTouchAnimationsForAllNavigation = false;

            bHorizontal = !bHorizontal;
            if (bHorizontal)
            {
                Orientation.Content = "Vertical";

                FlipView2Horizontal.SelectedIndex = FlipView2Vertical.SelectedIndex;
                FlipView2Vertical.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                FlipView2Horizontal.Visibility = Windows.UI.Xaml.Visibility.Visible;

            }
            else
            {
                Orientation.Content = "Horizontal";

                FlipView2Vertical.SelectedIndex = FlipView2Horizontal.SelectedIndex;
                FlipView2Vertical.Visibility = Windows.UI.Xaml.Visibility.Visible;
                FlipView2Horizontal.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }

            // Turn the programmatic animations back on
            FlipView2Horizontal.UseTouchAnimationsForAllNavigation = true;
        }
    }
}
