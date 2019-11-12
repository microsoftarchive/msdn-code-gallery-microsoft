//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.IO;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;

namespace HidInfraredSensor
{
    /// <summary>
    /// Page containing working sample code to demonstrate how to connect to a hid device and how to respond to
    /// the device disconnecting, the app suspending, and the app resuming.
    /// 
    /// In general, in order to make a device work this sample (to use the generic scenarios):
    /// 1) The Package.appxmanifest needs to be
    ///     modified to include the new device. Please see the Package.appxmanifest for more information on how to do that.
    /// 2) Create a DeviceWatcher for your device. To do so, please read the comments in InitializeDeviceWatchers.
    /// </summary>
    public sealed partial class DeviceConnect : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        private MainPage rootPage = MainPage.Current;

        /// <summary>
        /// This method opens the device using the WinRT Hid API. After the device is opened, we will save the device
        /// so that it can be used across scenarios.
        ///
        /// The same code should be used to reopen the device when the app resumes.
        /// 
        /// This method is static because EventHandlerForDevice calls this method when the app is resumed.
        /// </summary>
        /// <param name="id"></param>
        public static async void OpenDevice(String id)
        {
            // It is important that the FromIdAsync call is made on the UI thread because the consent prompt can only be displayed
            // on the UI thread.
            await MainPage.Current.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                HidDevice device = await HidDevice.FromIdAsync(id, FileAccessMode.ReadWrite);

                // Save the device from the task to a global object so that it can be used later from other scenarios
                DeviceList.Current.SetCurrentDevice(id, device);

                MainPage.Current.NotifyUser("Device " + id + " opened", NotifyType.StatusMessage);
            });
        }

        /// <summary>
        /// This method demonstrates how to close the device properly using the WinRT Hid API.
        ///
        /// When the HidDevice is closing, it will cancel all IO operations that are still pending (not complete) and
        /// then waits for them to all cancel/complete before continuing. The pending IO operations will still call their respective
        /// completion callbacks with either a task cancelled error or the operation completed.
        /// 
        /// This method is static because EventHandlerForDevice calls this method when the app is suspended.
        /// </summary>
        public static void CloseDevice()
        {
            if (DeviceList.Current.IsDeviceConnected)
            {
                MainPage.Current.NotifyUser("Device is closed", NotifyType.StatusMessage);

                // This closes the handle to the device
                DeviceList.Current.CurrentDevice.Dispose();

                DeviceList.Current.SetCurrentDevice(null, null);
            }
        }

        public DeviceConnect()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// 
        /// Create the necessary device watchers when we navigate to this page for the first time. We only want to create 
        /// the device watchers once because the DeviceList object, which is never deallocated until the app is closed, will 
        /// keep track of all added device watchers.
        /// The device watchers can be later started by using the start/stop buttons on the UI.
        ///
        /// Please see the EventHandlerForDevice constructor and the app suspension/resume event handlers in EventHandlerForDevice.cpp for more 
        /// information on how to handle app suspension/resume.
        /// </summary>
        /// <param name="eventArgs">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Initalize the desired device watchers so that we can watch for when devices are connected/removed
            if (EventHandlerForDevice.Current.DeviceWatchers.Count == 0) // Don't reinitialize device watchers if we already initialized them
            {
                InitalizeDeviceWatchers();
            }

            // Begin watching out for events
            if (!(EventHandlerForDevice.Current.IsRegisteredForAppEvents))
            {
                EventHandlerForDevice.Current.StartHandlingAppEvents();
            }

            DeviceListSource.Source = DeviceList.Current.Devices;

            // Reselect the device that we are connected to. When we navigate away and back, the selection changes to the first element on the devices list
            // Don't select anything if the device isn't on the list or no devices are connected
            SelectDeviceInList(DeviceList.Current.IsDeviceConnected ? DeviceList.Current.CurrentDeviceEntry.Id : "");

            // Add the SelectionChange after setting the source because the UI tends to automatically select the first item
            // on the list whenever it goes from having no data to having data.
            ConnectDevices.SelectionChanged += new SelectionChangedEventHandler(this.ConnectDevices_SelectChanged);

            UpdateStartStopButtons();
        }

        private void StartDeviceWatcher_Click(Object sender, RoutedEventArgs eventArgs)
        {
            EventHandlerForDevice.Current.StartDeviceWatchers();

            UpdateStartStopButtons();
        }

        private void StopDeviceWatcher_Click(Object sender, RoutedEventArgs eventArgs)
        {
            EventHandlerForDevice.Current.StopDeviceWatchers();

            UpdateStartStopButtons();
        }

        /// <summary>
        /// When an entry is selected from the list of devices, we will connect to the device.
        /// If a device was previously connected, disconnect from that device first before connecting to the new device.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ConnectDevices_SelectChanged(Object sender, SelectionChangedEventArgs eventArgs)
        {
            var deviceList = (ListBox)sender;
            var selection = deviceList.SelectedItems;
            DeviceListEntry entry = null;

            if (selection.Count > 0)
            {
                var obj = selection[0];
                entry = (DeviceListEntry)obj;
            }

            var currentlySelectedId = (DeviceList.Current.IsDeviceConnected
                ? DeviceList.Current.CurrentDeviceEntry.Id
                : null);
            var newlySelectedId = (entry != null) ? entry.Id : null;

            // If we were already connected to another device, disconnect from that device.
            if (currentlySelectedId != null)
            {
                CloseDevice();
            }

            if (newlySelectedId != null)
            {
                OpenDevice(newlySelectedId);
            }
        }

        /// <summary>
        /// Initialize device watchers to watch for the IR_Sensor.
        /// Several other ways of using a device selector to get the desired devices (the devices are really DeviceInformation objects):
        /// 1) Using only the Usage Page and the Usage Id of the device (enumerates any device with the same usage page and usage id)
        /// 2) Using the Usage Page, Usage Id, Vendor Id, Product Id (Same as above except it also matches the VIDs and PIDs)
        ///
        /// GetDeviceSelector return an AQS string that can be passed directly into DeviceWatcher.createWatcher() or  DeviceInformation.createFromIdAsync(). 
        ///
        /// In this sample, a DeviceWatcher will be used to watch for devices because we can detect surprise device removals.
        /// </summary>
        private void InitalizeDeviceWatchers()
        {
            // IR_Sensor
            var IR_SensorSelector = HidDevice.GetDeviceSelector(IR_Sensor.Device.UsagePage, IR_Sensor.Device.UsageId);

            // Create a device watcher to look for instances of the IR_Sensor device
            var IR_SensorWatcher = DeviceInformation.CreateWatcher(IR_SensorSelector);

            // Allow the EventHandlerForDevice to handle device watcher events that relates or effects our device (i.e. device removal, addition, app suspension/resume)
            EventHandlerForDevice.Current.AddDeviceWatcher(IR_SensorWatcher);
        }

        /// <summary>
        /// Selects the item in the UI's listbox that corresponds to the provided device id. If there are no
        /// matches, we will not select anything.
        /// </summary>
        /// <param name="deviceIdToSelect">The device id of the device to select on the list box</param>
        private void SelectDeviceInList(String deviceIdToSelect)
        {
            // Don't select anything by default. Changing the selection opens the device and there will not be a selection change if
            // the device is selected before register for the SelectionChange event
            ConnectDevices.SelectedIndex = -1;

            var deviceList = DeviceList.Current.Devices;

            for (int deviceListIndex = 0; deviceListIndex < deviceList.Count; deviceListIndex++)
            {
                if (deviceList[deviceListIndex].Id == deviceIdToSelect)
                {
                    ConnectDevices.SelectedIndex = deviceListIndex;

                    break;
                }
            }
        }

        /// <summary>
        /// Toggle the Start/Stop Device Watcher buttons. Only one can be enabled at a time.
        /// </summary>
        private void UpdateStartStopButtons()
        {
            ButtonStartDeviceWatcher.IsEnabled = !EventHandlerForDevice.Current.WatchersStarted;
            ButtonStopDeviceWatcher.IsEnabled = EventHandlerForDevice.Current.WatchersStarted;
        }

    }
}
