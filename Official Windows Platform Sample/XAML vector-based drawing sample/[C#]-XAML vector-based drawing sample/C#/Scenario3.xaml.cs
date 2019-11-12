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
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.Foundation;

namespace Drawing
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

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Color1Selection.SelectedIndex = 0;
            Color2Selection.SelectedIndex = 1;
            Color3Selection.SelectedIndex = 2;
        }

        private void Color1SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Updates the first gradient stop's color based on the color selected in the ComboBox
            Scenario3GradientStop1.Color = rootPage.ConvertIndexToColor(Color1Selection.SelectedIndex);
        }


        private void Color2SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Updates the second GradientStop's color based on the color selected in the ComboBox
            Scenario3GradientStop2.Color = rootPage.ConvertIndexToColor(Color2Selection.SelectedIndex);
        }

        private void Color3SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Updates the third gradient stop's color based on the color selected in the ComboBox
            Scenario3GradientStop3.Color = rootPage.ConvertIndexToColor(Color3Selection.SelectedIndex);
        }

    }
}
