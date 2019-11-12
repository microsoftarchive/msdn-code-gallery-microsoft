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

namespace DeviceCaps
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Mouse : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Mouse()
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
            Windows.Devices.Input.MouseCapabilities MouseCapabilities = new Windows.Devices.Input.MouseCapabilities();

            Buffer = string.Format("There is {0} mouse present\n", MouseCapabilities.MousePresent != 0 ? "a" : "no");
            Buffer += string.Format("There is {0} vertical mouse wheel present\n", MouseCapabilities.VerticalWheelPresent != 0 ? "a" : "no");
            Buffer += string.Format("There is {0} horizontal mouse wheel present\n", MouseCapabilities.HorizontalWheelPresent != 0 ? "a" : "no");
            Buffer += string.Format("The user has {0}opted to swap the mouse buttons\n", MouseCapabilities.SwapButtons != 0 ? "" : "not ");
            Buffer += string.Format("The mouse has {0} button(s)\n", MouseCapabilities.NumberOfButtons);

            MouseOutputTextBlock.Text = Buffer;
        }
    }
}
