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


using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothGattHeartRate
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario3_WriteCharacteristicValue : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario3_WriteCharacteristicValue()
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
                HeartRateService.Instance.ValueChangeCompleted += Instance_ValueChangeCompleted;
                if (HeartRateService.Instance.Service.GetCharacteristics(
                    GattCharacteristicUuids.HeartRateControlPoint).Count > 0)
                {
                    WriteCharacteristicValueButton.IsEnabled = true;
                }
                else
                {
                    rootPage.NotifyUser("The optional Heart Rate Control Point characteristic was not found on your " +
                        "device.", NotifyType.StatusMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("The Heart Rate Service is not initialized, please go to Scenario 1 to " +
                    "initialize the service before writing a Characteristic Value.", NotifyType.StatusMessage);
            }
        }
        
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HeartRateService.Instance.ValueChangeCompleted -= Instance_ValueChangeCompleted;
        }

        private async void Instance_ValueChangeCompleted(HeartRateMeasurement heartRateMeasurementValue)
        {
            if (heartRateMeasurementValue.HasExpendedEnergy)
            {
                // Serialize UI update to the the main UI thread.
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ExpendedEnergyTextBlock.Text = "Expended Energy: " + heartRateMeasurementValue.ExpendedEnergy + " kJ";
                });
            }
        }

        private async void WriteCharacteristicValue_Click(object sender, RoutedEventArgs args)
        {
            WriteCharacteristicValueButton.IsEnabled = false;
            try
            {
                var heartRateControlPointCharacteristic = HeartRateService.Instance.Service.GetCharacteristics(
                    GattCharacteristicUuids.HeartRateControlPoint)[0];
                 
                DataWriter writer = new DataWriter();
                writer.WriteByte(1);

                GattCommunicationStatus status = await heartRateControlPointCharacteristic.WriteValueAsync(
                    writer.DetachBuffer());

                if (status == GattCommunicationStatus.Success)
                {
                    rootPage.NotifyUser("Expended Energy successfully reset.", NotifyType.StatusMessage);
                }
                else
                {
                    rootPage.NotifyUser("Your device is unreachable, most likely the device is out of range, " +
                        "or is running low on battery, please make sure your device is working and try again.",
                        NotifyType.StatusMessage);
                }
            }
            catch (Exception e)
            {
                rootPage.NotifyUser("Error: " + e.Message, NotifyType.ErrorMessage);
            }
            WriteCharacteristicValueButton.IsEnabled = true;
        }
    }
}
