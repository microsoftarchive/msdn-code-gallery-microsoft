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

using Windows.Devices.Custom;
using System.Threading.Tasks;
using System.Threading;
using Windows.UI.Core;

namespace CustomDeviceAccess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DeviceEvents
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // saved copy of the switch state, used to highlight which entries have changed
        bool[] previousSwitchValues = null;

        public DeviceEvents()
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
            ClearSwitchStateTable();
            UpdateRegisterButton();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (cancelSource != null)
            {
                cancelSource.Cancel();
                cancelSource = null;
            }
        }

        private async void deviceEventsGet_Click_1(object sender, RoutedEventArgs e)
        {
            CustomDevice fx2Device = DeviceList.Current.GetSelectedDevice();

            bool[] switchStateArray = new bool[8];

            if (fx2Device == null)
            {
                rootPage.NotifyUser("Fx2 device not connected or accessible", NotifyType.ErrorMessage);
                return;
            }

            try
            {
                var output = new byte[1];

                await fx2Device.SendIOControlAsync(Fx2Driver.ReadSwitches,
                                                   null,
                                                   output.AsBuffer());

                switchStateArray = CreateSwitchStateArray(output);
            }
            catch (Exception exception)
            {
                rootPage.NotifyUser(exception.ToString(), NotifyType.ErrorMessage);
                return;
            }

            UpdateSwitchStateTable(switchStateArray);
        }

        private static bool[] CreateSwitchStateArray(byte[] output)
        {
            var switchStateArray = new bool[8];

            for (int i = 0; i < 8; i += 1)
            {
                switchStateArray[i] = (output[0] & (1 << i)) != 0;
            }

            return switchStateArray;
        }

        CancellationTokenSource cancelSource;
        byte[] switchMessageBuffer = new byte[1];

        private void deviceEventsBegin_Click_1(object sender, RoutedEventArgs e)
        {
            var fx2Device = DeviceList.Current.GetSelectedDevice();

            if (fx2Device == null)
            {
                rootPage.NotifyUser("Fx2 device not connected or accessible", NotifyType.ErrorMessage);
                return;
            }

            if (cancelSource == null)
            {
                ReadInterruptMessagesAsync();
            }

            UpdateRegisterButton();
        }

        private void deviceEventsCancel_Click_1(object sender, RoutedEventArgs e)
        {
            if (cancelSource != null)
            {
                cancelSource.Cancel();
            }
            UpdateRegisterButton();
        }

        private async void ReadInterruptMessagesAsync()
        {
            var fx2Device = DeviceList.Current.GetSelectedDevice();
            var switchMessageBuffer = new byte[1] { 0 };
            
            cancelSource = new CancellationTokenSource();

            bool failure = false;

            while (cancelSource.IsCancellationRequested == false) 
            {
                uint bytesRead;

                try
                {
                    bytesRead = await fx2Device.SendIOControlAsync(
                                                    Fx2Driver.GetInterruptMessage,
                                                    null,
                                                    switchMessageBuffer.AsBuffer()
                                                    ).AsTask(cancelSource.Token);
                }
                catch (TaskCanceledException)
                {
                    rootPage.NotifyUser("Pending GetInterruptMessage IO Control cancelled\n", NotifyType.StatusMessage);
                    failure = false;
                    break;
                }
                catch (Exception e)
                {
                    rootPage.NotifyUser("Error accessing Fx2 device:\n" + e.Message, NotifyType.ErrorMessage);
                    failure = true;
                    break;
                }

                if (bytesRead == 0)
                {
                    rootPage.NotifyUser("Fx2 device returned 0 byte interrupt message.  Stopping\n", NotifyType.ErrorMessage);
                    failure = true;
                    break;
                }

                var switchState = CreateSwitchStateArray(switchMessageBuffer);
                UpdateSwitchStateTable(switchState);

            }

            if (failure)
            {
                ClearSwitchStateTable();
            }

            cancelSource = null;
            UpdateRegisterButton();
        }

        private void UpdateRegisterButton()
        {
            deviceEventsBegin.IsEnabled = (cancelSource == null);
            deviceEventsCancel.IsEnabled = (cancelSource != null);
        }

        private void ClearSwitchStateTable()
        {
            deviceEventsSwitches.Inlines.Clear();
            previousSwitchValues = null;
        }

        private void UpdateSwitchStateTable(bool[] switchStateArray)
        {
            var output = deviceEventsSwitches;

            DeviceList.CreateBooleanTable(
                output.Inlines,
                switchStateArray,
                previousSwitchValues,
                "Switch Number",
                "Switch State",
                "off",
                "on"
                );
            previousSwitchValues = switchStateArray;
        }

    }
}
