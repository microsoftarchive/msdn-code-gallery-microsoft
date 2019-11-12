//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using SDKTemplate;
using System;

namespace ApplicationViews
{
    public sealed partial class S1_Orientation
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public S1_Orientation()
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
            // Subscribe to window resize events
            Window.Current.SizeChanged += WindowSizeChanged;
            // Run layout logic when the page first loads
            InvalidateLayout();
        }

        /// <summary>
        /// Invoked when this page is no longer displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Unsubscribe from window resize events
            Window.Current.SizeChanged -= WindowSizeChanged;
        }

        /// <summary>
        /// Invoked when the main page is resized.
        /// </summary>
        private void WindowSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            InvalidateLayout();
        }

        /// <summary>
        /// Updates the layout when the window size changes
        /// </summary>
        private void InvalidateLayout()
        {
            // Get window orientation
            ApplicationViewOrientation winOrientation = ApplicationView.GetForCurrentView().Orientation;
            if (winOrientation == ApplicationViewOrientation.Landscape)
            {
                // Update grid to stack the boxes horizontally in landscape orientation
                Grid.SetColumn(Box1, 0);
                Grid.SetRow(Box1, 0);
                Grid.SetColumnSpan(Box1, 1);
                Grid.SetRowSpan(Box1, 2);

                Grid.SetColumn(Box2, 1);
                Grid.SetRow(Box2, 0);
                Grid.SetColumnSpan(Box2, 1);
                Grid.SetRowSpan(Box2, 2);
                
                rootPage.NotifyUser("Windows orientation is landscape.", NotifyType.StatusMessage);
            }
            else if (winOrientation == ApplicationViewOrientation.Portrait)
            {
                // Update grid to stack the boxes vertically in portrait orientation
                Grid.SetColumn(Box1, 0);
                Grid.SetRow(Box1, 0);
                Grid.SetColumnSpan(Box1, 2);
                Grid.SetRowSpan(Box1, 1);

                Grid.SetColumn(Box2, 0);
                Grid.SetRow(Box2, 1);
                Grid.SetColumnSpan(Box2, 2);
                Grid.SetRowSpan(Box2, 1);
                
                rootPage.NotifyUser("Windows orientation is portrait.", NotifyType.StatusMessage);
            }
        }
    }
}
