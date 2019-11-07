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
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace BasicControls
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MiscControls : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public MiscControls()
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

        private void StretchModeButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton button = sender as RadioButton;
            if (Image1 != null)
            {
                switch (button.Name)
                {
                    case "FillButton":
                        Image1.Stretch = Windows.UI.Xaml.Media.Stretch.Fill;
                        break;
                    case "NoneButton":
                        Image1.Stretch = Windows.UI.Xaml.Media.Stretch.None;
                        break;
                    case "UniformButton":
                        Image1.Stretch = Windows.UI.Xaml.Media.Stretch.Uniform;
                        break;
                    case "UniformToFillButton":
                        Image1.Stretch = Windows.UI.Xaml.Media.Stretch.UniformToFill;
                        break;
                    default:
                        break;
                }
            }
        }


    }
}
