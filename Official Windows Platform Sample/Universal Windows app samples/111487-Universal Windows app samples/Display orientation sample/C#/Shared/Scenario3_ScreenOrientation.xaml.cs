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
using SDKTemplate;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DisplayOrientation
{
    public sealed partial class Scenario3 : Page
    {
        public string BoxOneText { get; set; }
        public string BoxTwoText { get; set; }

        public Scenario3()
        {
            InitializeComponent();
            BoxOneText = "TextBlock One";
            BoxTwoText = "TextBlock Two";
            SideGrid.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DisplayInformation.GetForCurrentView().OrientationChanged += OnOrientationChanged;
            TransitionStoryboardState();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // If the navigation is external to the app don't deregister the accelerometer.
            // This can occur on Phone when suspending the app.
            if (e.NavigationMode == NavigationMode.Forward && e.Uri == null)
            {
                return;
            }

            DisplayInformation.GetForCurrentView().OrientationChanged -= OnOrientationChanged;
        }

        private void OnOrientationChanged(DisplayInformation sender, object args)
        {
            TransitionStoryboardState();
        }

        /// <summary>
        /// Invoked to determine the name of the visual state that corresponds to an application
        /// display orientation.
        /// </summary>
        private void TransitionStoryboardState()
        {
            string displayOrientation;

            switch (DisplayInformation.GetForCurrentView().CurrentOrientation)
            {
                case DisplayOrientations.Portrait:
                case DisplayOrientations.PortraitFlipped:
                    displayOrientation = "Portrait";
                    break;

                case DisplayOrientations.Landscape:
                case DisplayOrientations.LandscapeFlipped:
                default:
                    displayOrientation = "Landscape";
                    break;
            }

            VisualStateManager.GoToState(this, displayOrientation, false);
        }
    }
}
