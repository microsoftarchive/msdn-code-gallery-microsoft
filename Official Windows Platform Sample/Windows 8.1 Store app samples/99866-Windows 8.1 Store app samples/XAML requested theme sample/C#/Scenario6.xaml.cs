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
using Windows.Storage;
using Windows.UI.Popups;

namespace SDKTemplate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario6 
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
       
        public Scenario6()
        {
            this.InitializeComponent();
        }

       
        async private void lightTheme_Click(object sender, RoutedEventArgs e)
        {
            // Store light or dark theme choice in app local settings. This value is read during app startup.
            ApplicationData.Current.LocalSettings.Values["IsLightTheme"] = true;
            
            // Changing the theme requires app restart. Notify user.
            MessageDialog md = new MessageDialog("Please restart the sample to see this theme applied to the output area below");
            await md.ShowAsync();
        }

        async private void darkTheme_Click(object sender, RoutedEventArgs e)
        {
            // Store light or dark theme choice in app local settings. This value is read during app startup.
            ApplicationData.Current.LocalSettings.Values["IsLightTheme"] = false;

            // Changing the theme requires app restart. Notify user.
            MessageDialog md = new MessageDialog("Please restart the sample to see this theme applied to the output area below");
            await md.ShowAsync();
        }

    }
}
