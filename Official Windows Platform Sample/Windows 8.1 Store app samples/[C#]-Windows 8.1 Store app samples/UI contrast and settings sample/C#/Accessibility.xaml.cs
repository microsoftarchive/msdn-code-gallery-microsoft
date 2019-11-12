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

namespace HighContrast
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Accessibility : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Accessibility()
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
            string Buffer;
            Windows.UI.ViewManagement.AccessibilitySettings AccessibilitySettings = new Windows.UI.ViewManagement.AccessibilitySettings();

            Buffer = string.Format("High Contrast mode is currently {0}\n", AccessibilitySettings.HighContrast ? "enabled" : "disabled");
            Buffer += string.Format("The specific high contrast scheme is {0}\n", AccessibilitySettings.HighContrast ? AccessibilitySettings.HighContrastScheme : "currently undefined");
            AccessibilityOutputTextBlock.Text = Buffer;
        }
    }
}
