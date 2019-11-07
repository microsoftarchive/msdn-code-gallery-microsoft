//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using SDKTemplate;
using System;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace MultipleViews
{
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Normally, you would hard code calling DisableShowingMainViewOnActivation
            // in the activation/launch handlers of App.xaml.cs. The sample allows 
            // the user to change the preference for demonstration purposes

            // Check the data stored from last run. Restart if you change this. See
            // App.xaml.cs for calling DisableShowingMainViewOnActivation
            var shouldDisable = ApplicationData.Current.LocalSettings.Values[App.DISABLE_MAIN_VIEW_KEY];
            if (shouldDisable != null && (bool) shouldDisable)
            {
                DisableMainBox.IsChecked = true;
            }
        }

        private void DisableMainBox_Checked(object sender, RoutedEventArgs e)
        {
            // Normally, you would hard code calling DisableShowingMainViewOnActivation
            // in the activation/launch handlers of App.xaml.cs. The sample allows 
            // the user to change the preference for demonstration purposes
            ApplicationData.Current.LocalSettings.Values[App.DISABLE_MAIN_VIEW_KEY] = true;
        }

        private void DisableMainBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Normally, you would hard code calling DisableShowingMainViewOnActivation
            // in the activation/launch handlers of App.xaml.cs. The sample allows 
            // the user to change the preference for demonstration purposes
            ApplicationData.Current.LocalSettings.Values[App.DISABLE_MAIN_VIEW_KEY] = false;
        }
    }
}
