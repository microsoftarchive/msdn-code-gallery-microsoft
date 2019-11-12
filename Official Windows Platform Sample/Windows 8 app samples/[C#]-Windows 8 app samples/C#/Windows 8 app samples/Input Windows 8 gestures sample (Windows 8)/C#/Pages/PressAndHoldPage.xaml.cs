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
    public sealed partial class PressAndHoldPage : GesturePageBase
    {
        private Windows.UI.Xaml.Media.Brush[] _brushes;
        private uint _brushIndex;

        public PressAndHoldPage() :
            base(
                "PressAndHold",
                "Press and hold to learn",
                "Press and hold an item to bring up detailed info or teaching visuals without invoking an action. The Tooltip control is the easiest way to implement this. This action might also bring up a context menu.",
                "Similar to when you use a mouse and hover over an item.",
                "Assets/press_and_hold.png")
        {
            this.InitializeComponent();

            // Configure the app bar items for this page
            // GesturePageBase.Selected uses this._nonContextualItems to populate the global app bar when the page is selected.

            // Links button
            this._links["Doc: Guidelines for targeting"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/hh465326.aspx");
            this._links["API: Holding event"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.input.gesturerecognizer.holding.aspx");
            this._links["API: HoldingEventArgs"] = new Uri("http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.input.holdingeventargs.aspx");
            this._nonContextualItems.Add(GesturePageBase.CreateLinksAppBarButton(this._links));

            this._brushes = new Windows.UI.Xaml.Media.Brush[]
            {
                App.Current.Resources["AppWhiteBrush"] as Windows.UI.Xaml.Media.Brush,
                App.Current.Resources["AppBlueBrush"] as Windows.UI.Xaml.Media.Brush,
                App.Current.Resources["AppOrangeBrush"] as Windows.UI.Xaml.Media.Brush
            };
            this._brushIndex = 0;
        }

        private void OnLeftButtonTapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this._brushIndex = (uint)((this._brushIndex + 1) % this._brushes.Length);
            this.leftButton.Foreground = this._brushes[this._brushIndex];
        }
    }
}
