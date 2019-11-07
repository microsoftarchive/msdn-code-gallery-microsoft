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

namespace RequestedTheme
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3 : Page
    {
       
        UserSettings userSettings = new UserSettings();
        
        public Scenario3()
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
            userSettings.SelectedTheme = ElementTheme.Light;
            panel.DataContext = userSettings;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RevertRequestedTheme();
        }

        private void RevertRequestedTheme()
        {
            if (userSettings.SelectedTheme == ElementTheme.Dark)
                userSettings.SelectedTheme = ElementTheme.Light;
            else
                userSettings.SelectedTheme = ElementTheme.Dark;

        }
    }
}
