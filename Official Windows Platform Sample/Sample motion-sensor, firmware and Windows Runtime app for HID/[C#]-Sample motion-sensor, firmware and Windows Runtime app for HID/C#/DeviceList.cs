//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using SDKTemplate;
using HidInfraredSensor;

namespace HidInfraredSensor
{
    class DeviceList
    {
        /// <summary>
        /// Allows for singleton DeviceList
        /// </summary>
        private static DeviceList deviceList;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        private MainPage rootPage = MainPage.Current;

        /// <summary>
        /// Information on the current connected HidDevice
        /// </summary>
        private DeviceListEntry currentDeviceEntry;

        /// <summary>
        /// The device that is currently being used/connected by the app
        /// </summary>
        private HidDevice currentDevice;

        /// <summary>
        /// Device Id of the previously connected device. This is used to reconnect to the device after disconnecting from it.
        /// </summary>
        private String previousDeviceId;

        private ObservableCollection<DeviceListEntry> listOfDevices;

        /// <summary>
        /// Note that the below initialization is simplified for sample purposes and should be made thread-safe for 
        /// production code. (For information about making this thread safe, refer to the following page on MSDN
        /// http://msdn.microsoft.com/en-us/library/ff650316.aspx).
        /// </summary>
        public static DeviceList Current
        {
            get
            {
                if (deviceList == null)
                {
                    deviceList = new DeviceList();
                }

                return deviceList;
            }
        }

        /// <summary>
        /// Attempts to match the provided device with a well known device and returns the well known type
        /// </summary>
        /// <param name="device"></param>
        /// <returns>Device model of the current connected device or DeviceModel.None if there are no devices connected or is not recognized</returns>
        public static DeviceModel GetDeviceModel(HidDevice device)
        {
            if (device != null
                && device.VendorId == IR_Sensor.Device.Vid
                && device.ProductId == IR_Sensor.Device.Pid)
            {
                    return DeviceModel.IR_Sensor;
            }
            
            return DeviceModel.None;
        }

        public HidDevice CurrentDevice
        {
            get
            {
                return currentDevice;
            }
        }

        public DeviceListEntry CurrentDeviceEntry
        {
            get
            {
                return currentDeviceEntry;
            }
        }

        /// <summary>
        /// Device model of the current connected device or DeviceModel.None if there are no devices connected or is not recognized
        /// </summary>
        public DeviceModel CurrentDeviceModel
        {
            get
            {
                return DeviceList.GetDeviceModel(currentDevice);
            }
        }

        public bool IsDeviceConnected
        {
            get
            {
                return currentDevice != null;
            }
        }

        public String PreviousDeviceId
        {
            get
            {
                return previousDeviceId;
            }
        }

        public ObservableCollection<DeviceListEntry> Devices
        {
            get
            {
                return listOfDevices;
            }
        }

        public void AddDeviceToList(DeviceInformation deviceInformation)
        {
            // search the device list for a device with a matching interface ID
            var match = FindDevice(deviceInformation.Id);

            // Add the device if it's new
            if (match == null)
            {
                // Create a new element for this device interface, and queue up the query of its
                // device information
                match = new DeviceListEntry(deviceInformation);

                // Add the new element to the end of the list of devices
                listOfDevices.Add(match);
            }
        }

        public void RemoveDeviceFromList(String deviceId)
        {
            // Removes the device entry from the interal list; therefore the UI
            var deviceEntry = FindDevice(deviceId);

            listOfDevices.Remove(deviceEntry);
        }

        public void ClearDeviceEntries()
        {
            listOfDevices.Clear();
        }

        /// <summary>
        /// Saves the previously opened device's id so that it can be used to reopen a device.
        /// Sets the current device and it's device entry.
        /// </summary>
        public void SetCurrentDevice(String id, HidDevice device)
        {
            if (IsDeviceConnected)
            {
                previousDeviceId = currentDeviceEntry.Id;
            }

            currentDevice = device;

            currentDeviceEntry = FindDevice(id);
        }

        /// <summary>
        /// Displays the compatible scenarios and hides the non-compatible ones.
        /// If there are no supported devices, the scenarioContainer will be hidden and an error message
        /// will be displayed.
        /// </summary>
        /// <param name="scenarios">The key is the device model that the value, scenario, supports.</param>
        /// <param name="scenarioContainer">The container that encompasses all the scenarios that are specific to devices</param>
        public void SetUpDeviceScenarios(Dictionary<DeviceModel, UIElement> scenarios, UIElement scenarioContainer)
        {
            UIElement supportedScenario = null;

            if (DeviceList.Current.IsDeviceConnected)
            {
                foreach (KeyValuePair<DeviceModel, UIElement> deviceScenario in scenarios)
                {
                    // Enable the scenario if it's generic or the device model matches
                    if ((deviceScenario.Key == DeviceModel.Any)
                        || (deviceScenario.Key == DeviceList.Current.CurrentDeviceModel))
                    {
                        // Make the scenario visible in case other devices use the same scenario and collapsed it.
                        deviceScenario.Value.Visibility = Visibility.Visible;

                        supportedScenario = deviceScenario.Value;
                    }
                    else if (deviceScenario.Value != supportedScenario)    // Don't hide the scenario if it is supported by the current device and is shared by other devices
                    {
                        deviceScenario.Value.Visibility = Visibility.Collapsed;
                    }
                }
            }

            if (supportedScenario == null)
            {
                // Hide the container so that common elements shared across scenarios are also hidden
                scenarioContainer.Visibility = Visibility.Collapsed;

                rootPage.NotifyUser("No supported devices detected", NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// Prints an error message stating that device is not connected
        /// </summary>
        public void NotifyDeviceNotConnected()
        {
            MainPage.Current.NotifyUser("Device not connected or accessible", NotifyType.ErrorMessage);
        }
        
        private DeviceList()
        {
            listOfDevices = new ObservableCollection<DeviceListEntry>();
        }

        /// <summary>
        /// Searches through the existing list of devices for the first DeviceListEntry that has
        /// the specified Id.
        /// </summary>
        /// <param name="Id">Id of the device that is being searched for</param>
        /// <returns>DeviceListEntry that has the provided Id; else a nullptr</returns>
        private DeviceListEntry FindDevice(String id)
        {
            if (id != null)
            {
                foreach (DeviceListEntry entry in listOfDevices)
                {
                    if (entry.Id == id)
                    {
                        return entry;
                    }

                }
            }

            return null;
        }
    }
}
