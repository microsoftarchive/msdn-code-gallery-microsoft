//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;

namespace HidInfraredSensor
{
    /// <summary>
    /// This scenario demonstrates how to set the sensor's report interval
    /// by issuing a HID output report.
    /// </summary>
    public sealed partial class SetReportInterval : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        private MainPage rootPage = MainPage.Current;

        public SetReportInterval()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        ///
        /// We will enable/disable parts of the UI if the device doesn't support it.
        /// </summary>
        /// <param name="eventArgs">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            // We will disable the scenario that is not supported by the device.
            // If no devices are connected, none of the scenarios will be shown and an error will be displayed
            Dictionary<DeviceModel, UIElement> deviceScenarios = new Dictionary<DeviceModel, UIElement>();
            deviceScenarios.Add(DeviceModel.IR_Sensor, IR_SensorScenario);

            DeviceList.Current.SetUpDeviceScenarios(deviceScenarios, DeviceScenarioContainer);
        }


        private async void SendNumericOutputReport_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs eventArgs)
        {
            if (DeviceList.Current.IsDeviceConnected)
            {
                ButtonSendNumericOutputReport.IsEnabled = false;

                Byte valueToWrite = (Byte) NumericValueToWrite.SelectedIndex;

                // valueToWrite is no longer 0-based; need to increment by 1
                await SendNumericOutputReportAsync(++valueToWrite);
             
                ButtonSendNumericOutputReport.IsEnabled = true;
            }
            else
            {
                DeviceList.Current.NotifyDeviceNotConnected();
            }
        }


        /// <summary>
        /// The app sends an output report to the motion sensor to set the sensor's report interval.
        /// </summary>
        /// <param name="valueToWrite"></param>
        /// <returns>A task that can be used to chain more methods after completing the scenario</returns>
        private async Task SendNumericOutputReportAsync(Byte valueToWrite)
        {
                var outputReport = DeviceList.Current.CurrentDevice.CreateOutputReport();

                Byte[] bytesToCopy = new Byte[1];
                bytesToCopy[0] = valueToWrite;

                WindowsRuntimeBufferExtensions.CopyTo(bytesToCopy, 0, outputReport.Data, 1, 1);

                uint bytesWritten = await DeviceList.Current.CurrentDevice.SendOutputReportAsync(outputReport);

                rootPage.NotifyUser("Bytes written:  " + bytesWritten.ToString() + "; Value Written: " + valueToWrite.ToString(), NotifyType.StatusMessage);
        }
                       
    }
}
