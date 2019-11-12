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

namespace CustomDeviceAccess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeviceProperties : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public DeviceProperties()
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

        private void devicePropertiesGet_Click_1(object sender, RoutedEventArgs e)
        {
            if (DeviceList.Current.Fx2Device == null)
            {
                rootPage.NotifyUser("Fx2 device not connected or accessible", NotifyType.ErrorMessage);
                return;
            }

            try
            {
                var segment = DeviceList.Current.Fx2Device.SevenSegmentDisplay;
                rootPage.NotifyUser(
                    "The segment display value is " + segment,
                    NotifyType.StatusMessage
                    );
            }
            catch (ArgumentException)
            {
                rootPage.NotifyUser(
                    "The segment display value is not yet initialized",
                    NotifyType.StatusMessage
                    );
            }
            catch (Exception exception)
            {
                rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
                return;
            }
        }


        private void devicePropertiesSet_Click_1(object sender, RoutedEventArgs e)
        {
            if (DeviceList.Current.Fx2Device == null)
            {
                rootPage.NotifyUser("Fx2 device not connected or accessible", NotifyType.ErrorMessage);
                return;
            }

            byte val = (byte) (devicePropertiesSegmentInput.SelectedIndex + 1);

            try
            {
                DeviceList.Current.Fx2Device.SevenSegmentDisplay = val;
                rootPage.NotifyUser("The segement display is set to " + val, NotifyType.StatusMessage);
            }
            catch (Exception exception)
            {
                rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
            }
        }


    }
}
