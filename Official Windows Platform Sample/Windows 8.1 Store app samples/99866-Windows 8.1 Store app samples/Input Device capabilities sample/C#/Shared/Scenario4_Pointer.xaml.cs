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
    public sealed partial class Pointer : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Pointer()
        {
            this.InitializeComponent();
        }

        private string PointerType(Windows.Devices.Input.PointerDevice PointerDevice)
        {
            if (PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse)
            {
                return "mouse";
            }
            else if (PointerDevice.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen)
            {
                return "pen";
            }
            else
            {
                return "touch";
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string Buffer;

            Buffer = "List of all pointer devices: \n\n";

            var PointerDeviceList = Windows.Devices.Input.PointerDevice.GetPointerDevices();
            int displayIndex = 1;

            foreach (Windows.Devices.Input.PointerDevice PointerDevice in PointerDeviceList)
            {
                Buffer += string.Format("Pointer device " + displayIndex + ":\n");
                Buffer += string.Format("This pointer device type is " + PointerType(PointerDevice) + "\n");
                Buffer += string.Format("This pointer device is " + (PointerDevice.IsIntegrated ? "not " : "") + "external\n");
                Buffer += string.Format("This pointer device has a maximum of " + PointerDevice.MaxContacts + " contacts\n");
                Buffer += string.Format("The physical device rect is " +
                    PointerDevice.PhysicalDeviceRect.X.ToString() + ", " +
                    PointerDevice.PhysicalDeviceRect.Y.ToString() + ", " +
                    PointerDevice.PhysicalDeviceRect.Width.ToString() + ", " +
                    PointerDevice.PhysicalDeviceRect.Height.ToString() + "\n");
                Buffer += string.Format("The screen rect is " +
                    PointerDevice.ScreenRect.X.ToString() + ", " +
                    PointerDevice.ScreenRect.Y.ToString() + ", " +
                    PointerDevice.ScreenRect.Width.ToString() + ", " +
                    PointerDevice.ScreenRect.Height.ToString() + "\n\n");
            }

            PointerOutputTextBlock.Text = Buffer;
        }
    }
}
