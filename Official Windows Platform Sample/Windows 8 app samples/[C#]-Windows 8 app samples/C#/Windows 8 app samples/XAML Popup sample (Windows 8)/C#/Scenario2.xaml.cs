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

namespace XAMLPopup
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        Popup nonParentPopup;

        public Scenario2()
        {
            this.InitializeComponent();
        }

        // Handles the Click event of the Button demonstrating a parented Popup with input content
        private void ShowPopupWithParentClicked(object sender, RoutedEventArgs e)
        {
            if (!ParentedPopup.IsOpen) { ParentedPopup.IsOpen = true; }
        }

        // Handles the Click event of the Button demonstrating a non-parented Popup with input content
        private void ShowPopupWithoutParentClicked(object sender, RoutedEventArgs e)
        {
            // if we already have one showing, don't create another one
            if (nonParentPopup == null)
            {
                // create the Popup in code
                nonParentPopup = new Popup();

                // we are creating this in code and need to handle multiple instances
                // so we are attaching to the Popup.Closed event to remove our reference
                nonParentPopup.Closed += (senderPopup, argsPopup) =>
                    {
                        nonParentPopup = null;
                    };
                nonParentPopup.HorizontalOffset = 200;
                nonParentPopup.VerticalOffset = Window.Current.Bounds.Height - 200;

                // set the content to our UserControl
                nonParentPopup.Child = new PopupInputContent();

                // open the Popup
                nonParentPopup.IsOpen = true;
            }
        }
    }
}
