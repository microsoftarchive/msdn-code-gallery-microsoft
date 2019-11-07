using System;
using System.Runtime.InteropServices.WindowsRuntime; // extension method byte[].AsBuffer()

//  Make it obvious which namespace provided each referenced type:
using ApplicationData = Windows.Storage.ApplicationData;
using ApplicationDataContainer = Windows.Storage.ApplicationDataContainer;
using BackgroundTaskBuilder = Windows.ApplicationModel.Background.BackgroundTaskBuilder;
using BackgroundTaskRegistration = Windows.ApplicationModel.Background.BackgroundTaskRegistration;
using BluetoothConnectionStatus = Windows.Devices.Bluetooth.BluetoothConnectionStatus;
using BluetoothLEDevice = Windows.Devices.Bluetooth.BluetoothLEDevice;
using DeviceConnectionChangeTrigger = Windows.ApplicationModel.Background.DeviceConnectionChangeTrigger;
using GattCharacteristic = Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristic;
using GattCharacteristicUuids = Windows.Devices.Bluetooth.GenericAttributeProfile.GattCharacteristicUuids;
using GattDeviceService = Windows.Devices.Bluetooth.GenericAttributeProfile.GattDeviceService;
using GattServiceUuids = Windows.Devices.Bluetooth.GenericAttributeProfile.GattServiceUuids;
using GattWriteOption = Windows.Devices.Bluetooth.GenericAttributeProfile.GattWriteOption;
using Task = System.Threading.Tasks.Task;

namespace KeepTheKeysCommon
{
    public class KeyFob
    {
        // static data
        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        // data members
        private BluetoothLEDevice device;           // constant, always non-null
        private string addressString;               // constant, Bluetooth address as 12 hex digits
        private GattDeviceService linkLossService;  // constant, may be null
        private bool alertOnPhone;                  // true iff we want a popup when this device disconnects
        private bool alertOnDevice;                 // true iff we want device to alert upon disconnection
        private AlertLevel alertLevel;              // alert level that device will set upon disconnection

        // trivial properties
        public BackgroundTaskRegistration TaskRegistration { get; set; }

        // readonly properties
        public bool HasLinkLossService { get { return linkLossService != null; } }
        public string Name { get { return device.Name; } }
        public string TaskName { get { return addressString; } }

        // settable properties, persisted in LocalSettings

        public bool AlertOnPhone
        {
            get { return alertOnPhone; }
            set
            {
                alertOnPhone = value;
                SaveSettings();
            }
        }

        public bool AlertOnDevice
        {
            get { return alertOnDevice; }
            set
            {
                alertOnDevice = value;
                SaveSettings();
            }
        }


        public AlertLevel AlertLevel
        {
            get { return alertLevel; }
            set
            {
                alertLevel = value;
                SaveSettings();
            }
        }

        // Constructor
        public KeyFob(BluetoothLEDevice device)
        {
            this.device = device;
            addressString = device.BluetoothAddress.ToString("x012");
            try
            {
                linkLossService = device.GetGattService(GattServiceUuids.LinkLoss);
            }
            catch (Exception)
            {
                // e.HResult == 0x80070490 means that the device doesn't have the requested service.
                // We can still alert on the phone upon disconnection, but cannot ask the device to alert.
                // linkLossServer will remain equal to null.
            }

            if (localSettings.Values.ContainsKey(addressString))
            {
                string[] values = ((string)localSettings.Values[addressString]).Split(',');
                alertOnPhone = bool.Parse(values[0]);
                alertOnDevice = bool.Parse(values[1]);
                alertLevel = (AlertLevel)Enum.Parse(typeof(AlertLevel), values[2]);
            }
        }

        // React to a change in configuration parameters:
        //    Save new values to local settings
        //    Set link-loss alert level on the device if appropriate
        //    Register or unregister background task if necessary
        private async void SaveSettings()
        {
            // Save this device's settings into nonvolatile storage
            localSettings.Values[addressString] = string.Join(",", alertOnPhone, alertOnDevice, alertLevel);

            // If the device is connected and wants to hear about the alert level on link loss, tell it
            if (alertOnDevice && device.ConnectionStatus == BluetoothConnectionStatus.Connected)
            {
                await SetAlertLevelCharacteristic();
            }

            // If we need a background task and one isn't already registered, create one
            if (TaskRegistration == null && (alertOnPhone || alertOnDevice))
            {
                DeviceConnectionChangeTrigger trigger = await DeviceConnectionChangeTrigger.FromIdAsync(device.DeviceId);
                trigger.MaintainConnection = true;
                BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
                builder.Name = TaskName;
                builder.TaskEntryPoint = "KeepTheKeysBackground.KeyFobTask";
                builder.SetTrigger(trigger);
                TaskRegistration = builder.Register();
            }

            // If we don't need a background task but have one, unregister it
            if (TaskRegistration != null && !alertOnPhone && !alertOnDevice)
            {
                TaskRegistration.Unregister(false);
                TaskRegistration = null;
            }
        }

        // Set the alert-level characteristic on the remote device
        public async Task SetAlertLevelCharacteristic()
        {
            // try-catch block protects us from the race where the device disconnects
            // just after we've determined that it is connected.
            try
            {
                byte[] data = new byte[1];
                data[0] = (byte)alertLevel;

                // The LinkLoss service should contain exactly one instance of the AlertLevel characteristic
                GattCharacteristic characteristic = linkLossService.GetCharacteristics(GattCharacteristicUuids.AlertLevel)[0];

                await characteristic.WriteValueAsync(data.AsBuffer(), GattWriteOption.WriteWithResponse);
            }
            catch (Exception)
            {
                // ignore exception
            }
        }

        // Provide a human-readable name for this object.
        public override string ToString()
        {
            return device.Name;
        }
    }
}
