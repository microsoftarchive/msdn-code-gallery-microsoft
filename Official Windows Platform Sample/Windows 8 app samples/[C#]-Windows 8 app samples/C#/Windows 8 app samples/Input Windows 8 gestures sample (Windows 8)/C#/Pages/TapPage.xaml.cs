//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using GesturesApp.Controls;
using System;

namespace GesturesApp.Pages
{
    public sealed partial class TapPage : GesturePageBase
    {
        private Windows.UI.Xaml.Controls.Primitives.ToggleButton[] _tapButtons;

        public TapPage() :
            base(
                "Tap",
                "Tap for primary action",
                "Tap an object to invoke the primary action, such as opening an app or executing a command. The size of the object affects how easy it is to tap accurately, so consider these guidelines when designing your touch targets.",
                "Similar to when you use a mouse and single left-click.",
                "Assets/tap.png")
        {
            this.InitializeComponent();

            this._tapButtons = new Windows.UI.Xaml.Controls.Primitives.ToggleButton[]  
            {
                tap00, tap01, tap02, tap10, tap11, tap12, tap20, tap21, tap22
            };

            // Configure the app bar items for this page
            // GesturePageBase.Selected uses this._nonContextualItems to populate the global app bar when the page is selected.

            // Links button
            this._links["Doc: Guidelines for targeting"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/hh465326.aspx");
            this._links["API: Tapped event"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.input.gesturerecognizer.tapped.aspx");
            this._links["TappedEventArgs class"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.input.tappedeventargs.aspx");
            this._nonContextualItems.Add(GesturePageBase.CreateLinksAppBarButton(this._links));
        }

        private void OnChangeTapButtonSize(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            double size = (double)this.Resources[(sender as Windows.UI.Xaml.Controls.RadioButton).Tag];

            foreach (var tapButton in _tapButtons)
            {
                tapButton.Height = size;
                tapButton.Width = size;
            }
        }
    }
}
