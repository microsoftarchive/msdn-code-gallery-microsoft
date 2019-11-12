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
    public sealed partial class LanguageScenario : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public LanguageScenario()
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
            // This scenario uses the Windows.Globalization.Language class to
            // obtain the Language characteristics.
            
            // Stores results of the scenario
            StringBuilder results = new StringBuilder();

            // Display characteristics of user's preferred Language.
            String topUserLanguage = GlobalizationPreferences.Languages[0];
            Language userLanguage = new Language(topUserLanguage);
            results.AppendLine("User's preferred Language display name: " + userLanguage.DisplayName);
            results.AppendLine("User's preferred Language tag: " + userLanguage.LanguageTag);
            results.AppendLine("User's preferred Language native name: " + userLanguage.NativeName);
            results.AppendLine("User's preferred Language script code: " + userLanguage.Script);
            results.AppendLine();

            // Display characteristics of the Russian Language.
            Language ruLanguage = new Language("ru-RU");
            results.AppendLine("Russian Language display name: " + ruLanguage.DisplayName);
            results.AppendLine("Russian Language tag: " + ruLanguage.LanguageTag);
            results.AppendLine("Russian Language native name: " + ruLanguage.NativeName);
            results.AppendLine("Russian Language script code: " + ruLanguage.Script);

            // Display results
            rootPage.NotifyUser(results.ToString(), NotifyType.StatusMessage);
        }
    }
}
