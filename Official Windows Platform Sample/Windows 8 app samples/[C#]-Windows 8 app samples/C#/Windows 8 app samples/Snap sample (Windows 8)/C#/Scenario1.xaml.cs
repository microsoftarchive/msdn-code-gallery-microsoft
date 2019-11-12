//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Input;
using Windows.UI.ViewManagement;
using SDKTemplate;
using System;

namespace Snap
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario1 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        #region Class Variables
        private bool programmaticUnsnapSucceeded = false;
        #endregion

        public Scenario1()
        {
            this.InitializeComponent();

            Scenario1Init();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Update view on load (because you can be launched into any view state)
            this.ShowCurrentViewState();
        }

        #region ApplicationView event handling logic
        void ShowCurrentViewState()
        {
            this.UpdateUnsnapButtonState();

            // Query for the current view state
            ApplicationViewState currentState = Windows.UI.ViewManagement.ApplicationView.Value;

            // Assign text according to view state
            if (currentState == ApplicationViewState.Snapped)
            {
                Scenario1OutputText.Text = "This app is snapped.";
            }
            else if (currentState == ApplicationViewState.Filled)
            {
                Scenario1OutputText.Text = "This app is in the fill state.";
            }
            else if (currentState == ApplicationViewState.FullScreenLandscape)
            {
                Scenario1OutputText.Text = "This app is full-screen landscape.";
            }
            else if (currentState == ApplicationViewState.FullScreenPortrait)
            {
                Scenario1OutputText.Text = "This app is full-screen portrait.";
            }
        }

        public void OnSizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs args)
        {
            switch (Windows.UI.ViewManagement.ApplicationView.Value)
            {
                case Windows.UI.ViewManagement.ApplicationViewState.Filled:
                    VisualStateManager.GoToState(this, "Fill", false);
                    break;
                case Windows.UI.ViewManagement.ApplicationViewState.FullScreenLandscape:
                    VisualStateManager.GoToState(this, "Full", false);
                    break;
                case Windows.UI.ViewManagement.ApplicationViewState.Snapped:
                    VisualStateManager.GoToState(this, "Snapped", false);
                    break;
                case Windows.UI.ViewManagement.ApplicationViewState.FullScreenPortrait:
                    VisualStateManager.GoToState(this, "Portrait", false);
                    break;
                default:
                    break;
            }

            this.ShowCurrentViewState();
        }

        private void UnsnapButton_Click(object sender, RoutedEventArgs e)
        {
            UnsnapButton.IsEnabled = false;
            this.programmaticUnsnapSucceeded = Windows.UI.ViewManagement.ApplicationView.TryUnsnap();
            this.UpdateUnsnapButtonState();

            if (!this.programmaticUnsnapSucceeded)
            {
                Scenario1OutputText.Text = "Programmatic unsnap did not work.";
            }
        }

        void UpdateUnsnapButtonState()
        {
            if (Windows.UI.ViewManagement.ApplicationView.Value == ApplicationViewState.Snapped)
            {
                UnsnapButton.Visibility = Visibility.Visible;
                UnsnapButton.IsEnabled = true;
            }
            else
            {
                UnsnapButton.Visibility = Visibility.Collapsed;
                UnsnapButton.IsEnabled = false;
            }
        }
        #endregion

        #region Adding and removing a SizeChanged event handler
        void Scenario1Init()
        {
            // Register for the window resize handler
            Window.Current.SizeChanged += OnSizeChanged;

            // Bind a click event to the unsnap button
            UnsnapButton.Click += new RoutedEventHandler(UnsnapButton_Click);
        }

        void Scenario1Reset()
        {
            Window.Current.SizeChanged -= OnSizeChanged;
        }
        #endregion
    }
}
