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
    public sealed partial class S2_Edges
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public S2_Edges()
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
            // Get an instance of ApplicationView for the current window
            ApplicationView currentAppView = ApplicationView.GetForCurrentView();
            if (currentAppView.IsFullScreen)
            {
                // If app is full screen, center the control
                Output.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
                rootPage.NotifyUser("App window is full screen.", NotifyType.StatusMessage);
            }
            else if (currentAppView.AdjacentToLeftDisplayEdge)
            {
                // If app is adjacent to the left edge, align control to the left
                Output.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
                rootPage.NotifyUser("App window is adjacent to the left display edge.", NotifyType.StatusMessage);
            }
            else if (currentAppView.AdjacentToRightDisplayEdge)
            {
                // If app is adjacent to the right edge, align control to the right
                Output.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
                rootPage.NotifyUser("App window is adjacent to the right display edge.", NotifyType.StatusMessage);
            }
            else
            {
                // If app is not adjacent to either side of the screen, center the control
                Output.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
                rootPage.NotifyUser("App window is not adjacent to any edges.", NotifyType.StatusMessage);
            }
        }
    }
}
