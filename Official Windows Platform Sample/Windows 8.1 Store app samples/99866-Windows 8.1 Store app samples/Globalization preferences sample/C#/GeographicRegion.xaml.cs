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
using Windows.Globalization;

namespace GlobalizationPreferencesSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GeographicRegionScenario : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public GeographicRegionScenario()
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
            // This scenario uses the Windows.Globalization.GeographicRegion class to
            // obtain the geographic region characteristics.

            // Stores results of the scenario
            StringBuilder results = new StringBuilder();

            // Display characteristics of user's geographic region.
            GeographicRegion userRegion = new GeographicRegion();
            results.AppendLine("User's region display name: " + userRegion.DisplayName);
            results.AppendLine("User's region native name: " + userRegion.NativeName);
            results.AppendLine("User's region currencies in use: " + string.Join(",", userRegion.CurrenciesInUse));
            results.AppendLine("User's region codes: " + userRegion.CodeTwoLetter + "," + userRegion.CodeThreeLetter + "," + userRegion.CodeThreeDigit);
            results.AppendLine();

            // Display characteristics of example region.
            GeographicRegion ruRegion = new GeographicRegion("RU");
            results.AppendLine("RU region display name: " + ruRegion.DisplayName);
            results.AppendLine("RU region native name: " + ruRegion.NativeName);
            results.AppendLine("RU region currencies in use: " + string.Join(",", ruRegion.CurrenciesInUse));
            results.AppendLine("RU region codes: " + ruRegion.CodeTwoLetter + "," + ruRegion.CodeThreeLetter + "," + ruRegion.CodeThreeDigit);

            // Display the results
            rootPage.NotifyUser(results.ToString(), NotifyType.StatusMessage);
        }
    }
}
