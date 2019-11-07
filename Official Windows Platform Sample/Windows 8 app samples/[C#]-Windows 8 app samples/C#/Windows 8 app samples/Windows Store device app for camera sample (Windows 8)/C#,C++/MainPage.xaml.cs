//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Navigation;


namespace SDKTemplate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public Windows.ApplicationModel.Activation.LaunchActivatedEventArgs LaunchArgs;

        public static MainPage Current;

        private Frame HiddenFrame = null;

        public MainPage()
        {
            this.InitializeComponent();

            // This is a static public property that will allow downstream pages to get 
            // a handle to the MainPage instance in order to call methods that are in this class.
            Current = this;

            // This frame is hidden, meaning it is never shown.  It is simply used to load
            // each scenario page and then pluck out the input and output sections and
            // place them into the UserControls on the main page.
            HiddenFrame = new Windows.UI.Xaml.Controls.Frame();
            HiddenFrame.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            LayoutRoot.Children.Add(HiddenFrame);

            // Populate the sample title from the constant in the GlobalVariables.cs file.
            SetFeatureName(FEATURE_NAME);

            SizeChanged += MainPage_SizeChanged;

        }

        void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateSize();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            InvalidateViewState();
        }

        private void InvalidateSize()
        {
            // Get the window width
            double windowWidth = this.ActualWidth;

            if (windowWidth != 0.0)
            {
                // Get the width of the ContentPanel.
                double contentPanelWidth = ContentPanel.ActualWidth;

                // Is the ContentPanel using any margins that we need to consider?
                double contentPanelMarginLeft = ContentPanel.Margin.Left;
                double contentPanelMarginRight = ContentPanel.Margin.Right;

                // Figure out how much room is left
                double availableWidth = windowWidth;

                // Is the top most child using margins?
                double layoutRootMarginLeft = ContentRoot.Margin.Left;
                double layoutRootMarginRight = ContentRoot.Margin.Right;

                // We have different widths to use depending on the view state
                if (ApplicationView.Value != ApplicationViewState.Snapped)
                {
                    // Make us as big as the the left over space
                    ContentPanel.Width = ((availableWidth) - (layoutRootMarginLeft + layoutRootMarginRight + contentPanelMarginLeft + contentPanelMarginRight));
                }
                else
                {
                    // Make us as big as the left over space
                    ContentPanel.Width = (windowWidth - (layoutRootMarginLeft + layoutRootMarginRight));
                }
            }
            InvalidateViewState();
        }

        private void InvalidateViewState()
        {

            if (ApplicationView.Value == ApplicationViewState.Snapped)
            {
                Grid.SetRow(FooterPanel, 2);
                Grid.SetColumn(FooterPanel, 0);
            }
            else
            {
                Grid.SetRow(FooterPanel, 1);
                Grid.SetColumn(FooterPanel, 1);
            }
        }

        async void Footer_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(((HyperlinkButton)sender).Tag.ToString()));
        }

        private void SetFeatureName(string str)
        {
            FeatureName.Text = str;
        }
    }
}
