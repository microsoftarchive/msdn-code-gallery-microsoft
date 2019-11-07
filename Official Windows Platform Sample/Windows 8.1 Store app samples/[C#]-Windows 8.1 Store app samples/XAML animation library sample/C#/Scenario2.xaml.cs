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

namespace Animation
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        bool isScenario2StoryboardRunning = false;

        public Scenario2()
        {
            this.InitializeComponent();
            Scenario2ToggleStoryboard.Click += Scenario2ToggleStoryboard_Click;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        void Scenario2ToggleStoryboard_Click(object sender, RoutedEventArgs e)
        {
            // toggle the storyboards to stop or begin them depending on the current state
            if (isScenario2StoryboardRunning)
            {
                ToggleScenario2(false);
            }
            else
            {
                ToggleScenario2(true);
            }
        }

        private void ToggleScenario2(bool startScenario)
        {
            if (!startScenario)
            {
                // stop the storyboards
                Scenario2ContinuousStoryboard.Stop();
                Scenario2KeyFrameStoryboard.Stop();
                Scenario2ToggleStoryboard.Content = "Begin storyboards";
            }
            else
            {
                // start the storyboards
                Scenario2ContinuousStoryboard.Begin();
                Scenario2KeyFrameStoryboard.Begin();
                Scenario2ToggleStoryboard.Content = "Stop storyboards";
            }
            isScenario2StoryboardRunning = startScenario;
        }

    }
}
