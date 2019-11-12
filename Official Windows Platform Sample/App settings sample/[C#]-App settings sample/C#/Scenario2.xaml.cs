// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.UI.ApplicationSettings;

namespace ApplicationSettings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2()
        {
            this.InitializeComponent();
            rootPage.NotifyUser("Swipe the right edge of the screen to invoke the Charms bar and select Settings.  Alternatively, press Windows+I.", NotifyType.StatusMessage);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += onCommandsRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            SettingsPane.GetForCurrentView().CommandsRequested -= onCommandsRequested;
        }

        /// <summary>
        /// Handler for the CommandsRequested event. Add custom SettingsCommands here.
        /// </summary>
        /// <param name="e">Event data that includes a vector of commands (ApplicationCommands)</param>
        void onCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs e)
        {
            SettingsCommand generalCommand = new SettingsCommand("general", "General",
                (handler) =>
                {
                    rootPage.NotifyUser("You selected the 'General' SettingsComand", NotifyType.StatusMessage);
                });
            e.Request.ApplicationCommands.Add(generalCommand);

            SettingsCommand helpCommand = new SettingsCommand("help", "Help",
                (handler) =>
                {
                    rootPage.NotifyUser("You selected the 'Help' SettingsComand", NotifyType.StatusMessage);
                });
            e.Request.ApplicationCommands.Add(helpCommand);
        }
    }
}
