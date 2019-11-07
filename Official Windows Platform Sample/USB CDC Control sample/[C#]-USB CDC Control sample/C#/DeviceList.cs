//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.ApplicationModel;

namespace UsbCdcControl
{
    public class DeviceListEntry
    {
        /// <summary>
        /// The DeviceInformation object (device interface) for this  device
        /// </summary>
        public Windows.Devices.Enumeration.DeviceInformation Device;

        /// <summary>
        /// The device interface path
        /// </summary>
        public string Id { get { return Device.Id; } }

        /// <summary>
        /// The device's instance ID
        /// </summary>
        public string InstanceId { get { return (string)Device.Properties["System.Devices.DeviceInstanceId"]; } }

        /// <summary>
        /// The device name
        /// </summary>
        public string Name { get { return Device.Name; } }

        /// <summary>
        /// Has the device been found during the current enumeration
        /// </summary>
        public bool Matched = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="DeviceInterface">The device interface for this entry</param>
        public DeviceListEntry(Windows.Devices.Enumeration.DeviceInformation DeviceInterface)
        {
            this.Device = DeviceInterface;
        }
    }

    sealed class UsbDeviceInfo
    {
        internal UsbDeviceInfo(String id, String name)
        {
            Id = id;
            Name = name;
        }

        internal UsbDeviceInfo(DeviceListEntry info)
        {
            Id = info.Id;
            Name = info.Name;
        }

        public String Name;
        public String Id;
    };

    class DeviceList
    {
        private static List<DeviceList> instances = null;
        public static List<DeviceList> Instances
        {
            get
            {
                if (DeviceList.instances == null)
                {
                    DeviceList.instances = new List<DeviceList>();
                }
                return DeviceList.instances;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        internal DeviceList(String deviceSelector)
        {
            this.deviceSelector = deviceSelector;
            this.InitDeviceWatcher();

            // Register for app suspend/resume handlers
            // Ideally, one should close the device on app suspend and reopen the device on app resume because the API will close the device
            // for you if you don't explicitly close the device. Please see CustomUsbDeviceAccess sample for an example of how that should 
            // be done.
            SDKTemplate.App.Current.Suspending += new SuspendingEventHandler(this.SuspendDeviceWatcher);
            SDKTemplate.App.Current.Resuming += this.ResumeDeviceWatcher;

            // Add this to the static list.
            DeviceList.Instances.Add(this);
        }

        /// <summary>
        /// List of  devices currently attached to the system.  This
        /// list can be updated by the PNP watcher.
        /// </summary>
        public ObservableCollection<DeviceListEntry> Devices { get { return this.devices; } }

        /// <summary>
        /// Flag to keep track of whether the device watcher has been started.
        /// </summary>
        public bool WatcherStarted = false;

        public void StartWatcher()
        {
            foreach (var entry in this.devices)
            {
                entry.Matched = false;
            }

            WatcherStarted = true;
            this.watcher.Start();

            return;
        }

        public void StopWatcher()
        {
            this.watcher.Stop();
            WatcherStarted = false;
        }

        public event EventHandler<UsbDeviceInfo> DeviceAdded;
        public event EventHandler<UsbDeviceInfo> DeviceRemoved;

        //
        // private internal state
        //

        /// <summary>
        /// The device watcher that we setup to look for  devices
        /// </summary>
        private Windows.Devices.Enumeration.DeviceWatcher watcher = null;

        /// <summary>
        /// Internal list of devices.
        /// </summary>
        private ObservableCollection<DeviceListEntry> devices = new ObservableCollection<DeviceListEntry>();

        /// <summary>
        /// Device selector for CreateWatcher.
        /// </summary>
        readonly private String deviceSelector;

        private void InitDeviceWatcher()
        {
            // Create a device watcher to look for instances of the  device interface
            this.watcher = Windows.Devices.Enumeration.DeviceInformation.CreateWatcher(
                            this.deviceSelector,
                            new string[] { "System.Devices.DeviceInstanceId" }
                            );

            this.watcher.Added += this.OnAdded;
            this.watcher.Removed += this.OnRemoved;
            this.watcher.EnumerationCompleted += this.OnEnumerationComplete;
        }

        /// <summary>
        /// Event handler for arrival of  devices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="devInterface">The device interface which was added</param>
        private void OnAdded(Windows.Devices.Enumeration.DeviceWatcher sender, Windows.Devices.Enumeration.DeviceInformation devInterface)
        {
            // search the device list for a device with a matching interface ID
            DeviceListEntry match = FindInList(devInterface.Id);

            // If we found a match then mark it as verified and return
            if (match != null)
            {
                if (match.Matched == false)
                {
                    DeviceAdded(this, new UsbDeviceInfo(match));
                }
                match.Matched = true;
                return;
            }


            // Create a new element for this device interface, and queue up the query of its
            // device information
            match = new DeviceListEntry(devInterface);

            // Add the new element to the end of the list of devices
            this.devices.Add(match);

            DeviceAdded(this, new UsbDeviceInfo(match));
        }

        /// <summary>
        /// Event handler for the removal of an  device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="devInformation">The device interface that was removed</param>
        private void OnRemoved(Windows.Devices.Enumeration.DeviceWatcher sender, Windows.Devices.Enumeration.DeviceInformationUpdate devInformation)
        {
            var deviceId = devInformation.Id;

            // Search the list of devices for one with a matching ID
            var match = FindInList(deviceId);
            if (match != null)
            {
                // Remove the matched item
                this.devices.Remove(match);

                DeviceRemoved(this, new UsbDeviceInfo(match));
            }
        }

        /// <summary>
        /// Event handler for the end of an enumeration/reenumeration started
        /// by calling Start on the device watcher.  This culls out any entries
        /// in the list which are no longer matched.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnEnumerationComplete(Windows.Devices.Enumeration.DeviceWatcher sender, object args)
        {
            DeviceListEntry removedDevice;

            while ((removedDevice = this.devices.FirstOrDefault(e => e.Matched == false)) != null)
            {
                this.devices.Remove(removedDevice);
            }
        }

        private bool watcherSuspended = false;

        private DeviceListEntry FindInList(string id)
        {
            return this.devices.FirstOrDefault(entry => entry.Id == id);
        }

        private void SuspendDeviceWatcher(object sender, SuspendingEventArgs e)
        {
            if (WatcherStarted)
            {
                this.watcherSuspended = WatcherStarted;
                StopWatcher();
            }
        }

        private async void ResumeDeviceWatcher(object sender, object e)
        {
            if (this.watcherSuspended)
            {
                StartWatcher();
                this.watcherSuspended = false;
            }

            // Notify the user that they have to go back to scenario 1 to reconnect to the device.
            await SDKTemplate.MainPage.Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
            {
                SDKTemplate.MainPage.Current.NotifyUser("If device was connected on app suspension, then it was closed. Please go to scenario 1 to reconnect to the device.", SDKTemplate.NotifyType.StatusMessage);
            }));
        }
    }
}
