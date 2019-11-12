//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace CustomDeviceAccess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeviceIO
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public DeviceIO()
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

        private async void DeviceIOGet_Click_1(object sender, RoutedEventArgs e)
        {
            var fx2Device = DeviceList.Current.GetSelectedDevice();

            if (fx2Device == null)
            {
                rootPage.NotifyUser("Fx2 device not connected or accessible", NotifyType.ErrorMessage);
                return;
            }

            try
            {
                byte[] outputBuffer = new byte[1];

                await fx2Device.SendIOControlAsync(Fx2Driver.GetSevenSegmentDisplay,
                                                   null,
                                                   outputBuffer.AsBuffer());
                
                var segment = Fx2Driver.SevenSegmentToDigit(outputBuffer[0]);

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


        private async void DeviceIOSet_Click_1(object sender, RoutedEventArgs e)
        {
            var fx2Device = DeviceList.Current.GetSelectedDevice();

            if (fx2Device == null)
            {
                rootPage.NotifyUser("Fx2 device not connected or accessible", NotifyType.ErrorMessage);
                return;
            }

            byte val = (byte)(DeviceIOSegmentInput.SelectedIndex + 1);

            byte[] input = new byte[] { 
                Fx2Driver.DigitToSevenSegment(val) 
                };

            try
            {
                await fx2Device.SendIOControlAsync(
                                    Fx2Driver.SetSevenSegmentDisplay,
                                    input.AsBuffer(),
                                    null
                                    );

                rootPage.NotifyUser("The segement display is set to " + val, NotifyType.StatusMessage);
            }
            catch (Exception exception)
            {
                rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);
            }
        }

    }
}
