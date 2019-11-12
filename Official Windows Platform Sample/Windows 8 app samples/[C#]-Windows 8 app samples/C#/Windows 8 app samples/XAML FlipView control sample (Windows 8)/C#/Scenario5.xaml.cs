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

namespace Controls_FlipView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario5 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // Used to track the orientation state of the FlipView for Scenario #2
        bool bHorizontal = true;

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
            var sampleData = new Controls_FlipView.Data.SampleDataSource();
            FlipView5Horizontal.ItemsSource = sampleData.Items;
            FlipView5Vertical.ItemsSource = sampleData.Items;
        }

        /// <summary>
        /// This is the click handler for the 'Orientation' button.  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Orientation_Click(object sender, RoutedEventArgs e)
        {
            bHorizontal = !bHorizontal;
            if (bHorizontal)
            {
                Orientation.Content = "Vertical";
                FlipView5Horizontal.SelectedIndex = FlipView5Vertical.SelectedIndex;
                FlipView5Horizontal.Visibility = Windows.UI.Xaml.Visibility.Visible;
                FlipView5Vertical.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            else
            {
                Orientation.Content = "Horizontal";
                FlipView5Vertical.SelectedIndex = FlipView5Horizontal.SelectedIndex;
                FlipView5Vertical.Visibility = Windows.UI.Xaml.Visibility.Visible;
                FlipView5Horizontal.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }
    }
}
