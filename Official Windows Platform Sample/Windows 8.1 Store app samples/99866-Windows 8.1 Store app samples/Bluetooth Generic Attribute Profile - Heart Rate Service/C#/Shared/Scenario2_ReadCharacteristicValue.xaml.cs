//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothGattHeartRate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2_ReadCharacteristicValue : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2_ReadCharacteristicValue()
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
            if (HeartRateService.Instance.IsServiceInitialized)
            {
                ReadCharacteristicValueButton.IsEnabled = true;
            }
            else
            {
                rootPage.NotifyUser("The Heart Rate Service is not initialized, please go to Scenario 1 to " +
                    "initialize the service before writing a Characteristic Value.", NotifyType.StatusMessage);
            }
        }

        /// <summary>
        /// Reads the Body Sensor Location characteristic value.
        /// </summary>
        /// <param name="sender">The button that generated this action.</param>
        /// <param name="e"></param>
        private async void ReadValueButton_Click(object sender, RoutedEventArgs args)
        {
            try
            {
                var bodySensorLocationCharacteristics = HeartRateService.Instance.Service.GetCharacteristics(
                    GattCharacteristicUuids.BodySensorLocation);

                if (bodySensorLocationCharacteristics.Count > 0)
                {
                    // Read the characteristic value
                    GattReadResult readResult = await bodySensorLocationCharacteristics[0].ReadValueAsync();
                    if (readResult.Status == GattCommunicationStatus.Success)
                    {
                        byte[] bodySensorLocationData = new byte[readResult.Value.Length];

                        DataReader.FromBuffer(readResult.Value).ReadBytes(bodySensorLocationData);

                        string bodySensorLocation = 
                            HeartRateService.Instance.ProcessBodySensorLocationData(bodySensorLocationData);
                        if (bodySensorLocation != "")
                        {
                            OutputTextBlock.Text = "The Body Sensor Location of your device is : " + bodySensorLocation;
                        } 
                        else
                        {
                            OutputTextBlock.Text = "The Body Sensor Location is not recognized by this version of " +
                                "the application";
                        }
                    }
                    else
                    {
                        rootPage.NotifyUser("Your device is unreachable, most likely the device is out of range, " +
                            "or is running low on battery, please make sure your device is working and try again.",
                            NotifyType.StatusMessage);
                    }
                }
                else
                {
                    rootPage.NotifyUser("Your device does not support the Body Sensor Location characteristic.", 
                        NotifyType.StatusMessage);
                }
            }
            catch (Exception e)
            {
                rootPage.NotifyUser("Error: " + e.Message, NotifyType.ErrorMessage);
            }
        }
    }
}
