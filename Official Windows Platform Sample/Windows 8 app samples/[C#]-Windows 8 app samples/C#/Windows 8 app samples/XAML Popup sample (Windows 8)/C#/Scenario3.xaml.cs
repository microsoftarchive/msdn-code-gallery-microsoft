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

namespace XAMLPopup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario3()
        {
            this.InitializeComponent();
        }

        // handles the Click event of the Button for showing the light dismiss behavior
        private void ShowPopupLightDismissClicked(object sender, RoutedEventArgs e)
        {
            if (!LightDismissSimplePopup.IsOpen) { LightDismissSimplePopup.IsOpen = true; }
        }

        // Handles the Click event on the Button within the simple Popup control and simply closes it.
        private void ClosePopupClicked(object sender, RoutedEventArgs e)
        {
            // if the Popup is open, then close it
            if (LightDismissSimplePopup.IsOpen) { LightDismissSimplePopup.IsOpen = false; }
        }

        // handles the Click event of the Button for showing the light dismiss with animations behavior
        private void ShowPopupAnimationClicked(object sender, RoutedEventArgs e)
        {
            if (!LightDismissAnimatedPopup.IsOpen) { LightDismissAnimatedPopup.IsOpen = true; }
        }

        // Handles the Click event on the Button within the simple Popup control and simply closes it.
        private void CloseAnimatedPopupClicked(object sender, RoutedEventArgs e)
        {
            if (LightDismissAnimatedPopup.IsOpen) { LightDismissAnimatedPopup.IsOpen = false; }
        }

        // handles the Click event of the Button for showing the light dismiss with settings behavior
        private void ShowPopupSettingsClicked(object sender, RoutedEventArgs e)
        {
            if (!SettingsAnimatedPopup.IsOpen) 
            {
                /* The UI guidelines for a proper 'Settings' flyout are such that it should fill the height of the 
                current Window and be either narrow (346px) or wide (646px)
                Using the measurements of the Window.Curent.Bounds will help you position correctly.
                This sample here shows a simple *example* of this using the Width to get the HorizontalOffset but
                the app developer will have to perform these measurements depending on the structure of the app's 
                views in their code */
                RootPopupBorder.Width = 646;
                SettingsAnimatedPopup.HorizontalOffset = Window.Current.Bounds.Width - 646;

                SettingsAnimatedPopup.IsOpen = true; 
            }
        }

        // Handles the Click event on the Button within the simple Popup control and simply closes it.
        private void CloseSettingsPopupClicked(object sender, RoutedEventArgs e)
        {
            if (SettingsAnimatedPopup.IsOpen) { SettingsAnimatedPopup.IsOpen = false; }
        }
    }
}
