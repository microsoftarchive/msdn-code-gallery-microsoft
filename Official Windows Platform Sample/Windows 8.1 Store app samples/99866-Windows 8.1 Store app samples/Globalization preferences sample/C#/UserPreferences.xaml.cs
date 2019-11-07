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
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.UserProfile;
using Windows.Globalization;

namespace GlobalizationPreferencesSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserPreferences : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public UserPreferences()
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
        }


        /// <summary>
        /// This is the click handler for the 'Display' button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display_Click(object sender, RoutedEventArgs e)
        {
            // This scenario uses the Windows.System.UserProfile.GlobalizationPreferences class to
            // obtain the user's globalization preferences.

            // Variable where we keep the results
            StringBuilder results = new StringBuilder();
            
            // Obtain the user preferences.
            results.AppendLine("User LanguageScenarios: " + string.Join(", ", GlobalizationPreferences.Languages));
            results.AppendLine("User calendars: " + string.Join(", ", GlobalizationPreferences.Calendars));
            results.AppendLine("User clocks: " + string.Join(", ", GlobalizationPreferences.Clocks));
            results.AppendLine("User home region: " + GlobalizationPreferences.HomeGeographicRegion);
            results.AppendLine("User first day of week: " + GlobalizationPreferences.WeekStartsOn.ToString());

            // Display the results
            rootPage.NotifyUser(results.ToString(), NotifyType.StatusMessage);
        }
    }
}
