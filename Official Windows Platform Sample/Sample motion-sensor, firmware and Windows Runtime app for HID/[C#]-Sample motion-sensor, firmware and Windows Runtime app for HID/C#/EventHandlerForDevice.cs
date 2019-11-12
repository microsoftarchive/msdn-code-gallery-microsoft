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
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using SDKTemplate;

namespace HidInfraredSensor
{
    /// <summary>
    /// The purpose of this class is to demonstrate what to do to a HidDevice when a specific app event
    /// is raised (e.g. suspension and resume), when the device is disconnected. The app events should be 
    /// handled in SuspensionManager instead. This class only exists to help isolate the events that are important
    /// to apps that use the Hid Api.
    /// 
    /// This class will also demonstrate how to handle device watcher events.
    /// </summary>
    class EventHandlerForDevice
    {
        /// <summary>
        /// Allows for singleton EventHandlerForDevice
        /// </summary>
        private static EventHandlerForDevice eventHandlerForDevice;

        private bool registeredForAppEvents;   // Did we register for events yet?

        private List<DeviceWatcher> deviceWatchers;

        private bool watchersSuspended;
        private bool watchersStarted;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        private MainPage rootPage = MainPage.Current;

        /// <summary>
        /// Enforces the singleton pattern so that there is only one object handling app events
        /// as it relates to the HidDevice and so that the event handlers do not get destroyed when going
        /// to another scenario page.
        /// </summary>
        public static EventHandlerForDevice Current
        {
            get
            {
                if (eventHandlerForDevice == null)
                {
                    eventHandlerForDevice = new EventHandlerForDevice();
                }

                return eventHandlerForDevice;
            }
        }

        public bool IsRegisteredForAppEvents
        {
            get
            {
                return registeredForAppEvents;
            }
        }

        /// <summary>
        /// Returns a list of device watchers so the user can see which device watchers were added. The read only property
        /// also enforces the use of AddDeviceWatcher(), which has logic in dealing with new device watchers (register events).
        /// </summary>
        public IReadOnlyList<DeviceWatcher> DeviceWatchers
        {
            get
            {
                return deviceWatchers;
            }
        }

        public bool WatchersStarted
        {
            get
            {
                return watchersStarted;
            }
        }

        /// <summary>
        /// Register for app suspension/resume events. See the comments
        /// for the event handlers for more information on what is being done to the device.
        ///
        /// We will also register for when the app exists so that we may close the device handle.
        /// </summary>
        public void StartHandlingAppEvents()
        {
            if (registeredForAppEvents == false)
            {
                // This event is raised when the app is exited and when the app is suspended
                App.Current.Suspending += new SuspendingEventHandler(EventHandlerForDevice.Current.OnAppSuspension);

                App.Current.Resuming += new EventHandler<Object>(EventHandlerForDevice.Current.OnAppResume);

                registeredForAppEvents = true;
            }
        }

        /// <summary>
        /// As each DeviceWatcher is added, we will register for Added, Removed, and EnumerationCompleted events.
        /// When the Added event is raised, we will make add it to the global list and have it displayed in the UI
        /// When the Removed event is raised, we will remove it from the UI.
        /// When the EnumerationCompleted event is raised, a message will be printed to the user.
        ///
        /// There is no way to remove a device watcher because it is not required by the scenarios. In order to
        /// stop events from being raised from a specific DeviceWatcher, the user must use the DeviceWatchers property
        /// to find the DeviceWatcher and call DeviceWatcher->Stop().
        /// </summary>
        /// <param name="deviceWatcher"></param>
        public void AddDeviceWatcher(DeviceWatcher deviceWatcher)
        {
            if (deviceWatcher != null
                && !deviceWatchers.Contains(deviceWatcher))
            {
                deviceWatchers.Add(deviceWatcher);

                deviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>(EventHandlerForDevice.Current.OnDeviceAdded);

                deviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>(EventHandlerForDevice.Current.OnDeviceRemoved);

                deviceWatcher.EnumerationCompleted += new TypedEventHandler<DeviceWatcher, Object>(EventHandlerForDevice.Current.OnDeviceEnumerationComplete);

                if (watchersStarted)
                {
                    deviceWatcher.Start();
                }
            }
        }

        /// <summary>
        /// Starts all device watchers including ones that have been individually stopped.
        /// </summary>
        public void StartDeviceWatchers()
        {
            rootPage.NotifyUser("Starting device watcher", NotifyType.StatusMessage);

            // Start all device watchers
            watchersStarted = true;

            foreach (DeviceWatcher deviceWatcher in deviceWatchers)
            {
                if ((deviceWatcher.Status != DeviceWatcherStatus.Started)
                    && (deviceWatcher.Status != DeviceWatcherStatus.EnumerationCompleted))
                {
                    deviceWatcher.Start();
                }
            }
        }

        /// <summary>
        /// Stops all device watchers.
        /// </summary>
        public void StopDeviceWatchers()
        {
            rootPage.NotifyUser("Stopping device watchers", NotifyType.StatusMessage);

            // Stop all device watchers
            foreach (DeviceWatcher deviceWatcher in deviceWatchers)
            {
                if ((deviceWatcher.Status == DeviceWatcherStatus.Started)
                    || (deviceWatcher.Status == DeviceWatcherStatus.EnumerationCompleted))
                {
                    deviceWatcher.Stop();
                }
            }

            // Close the device so we don't have a device connected when the list of devices is empty
            DeviceConnect.CloseDevice();

            // Clear the list of devices so we don't have potentially disconnected devices around
            DeviceList.Current.ClearDeviceEntries();

            watchersStarted = false;
        }

        /// <summary>
        /// Does nothing but default members to specific values and intentionally prevent users from instaniating this class
        /// multiple times. Only one instance of this type can exist because this class directly interacts with a DeviceList
        /// object. If multiple instances of this objects exists, then the HidDevice in DeviceList may be deleted multiple times.
        /// </summary>
        private EventHandlerForDevice()
        {
            registeredForAppEvents = false;
            watchersStarted = false;
            watchersSuspended = false;
            deviceWatchers = new List<DeviceWatcher>();
        }

        /// <summary>
        /// If a HidDevice object has been instantiated (a handle to the device is opened), we must close it before the app 
        /// goes into suspension because the API automatically closes it for us if we don't. When resuming, the API will
        /// not reopen the device automatically, so we need to explicitly open the device in the app (Scenario1_DeviceConnect).
        ///
        /// Since we have to reopen the device ourselves when the app resumes, it is good practice to explicitly call the close
        /// in the app as well (For every open there is a close).
        /// 
        /// We must stop the DeviceWatchers because device watchers will continue to raise events even if
        /// the app is in suspension, which is not desired. We resume the device watcher once the app resumes again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void OnAppSuspension(Object sender, SuspendingEventArgs args)
        {
            if (WatchersStarted)
            {
                watchersSuspended = true;
                StopDeviceWatchers();
            }
            else
            {
                watchersSuspended = false;
            }

            DeviceConnect.CloseDevice();
        }

        /// <summary>
        /// When resume into the application, we should reopen a handle to the Hid device again. Please see the comment in
        /// OnAppSuspension() for more details why we are doing this.
        /// 
        /// See OnAppSuspension for why we are starting the device watchers again
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void OnAppResume(Object sender, Object args)
        {
            if (watchersSuspended)
            {
                watchersSuspended = false;
                StartDeviceWatchers();
            }

            // Go to Scenario1_ConnectDevice to see the code for OpenDevice
            if (DeviceList.Current.PreviousDeviceId != null)
            {
                DeviceConnect.OpenDevice(DeviceList.Current.PreviousDeviceId);
            }
        }

        /// <summary>
        /// When a device is removed, we need to check if it's the device we're using. If it is, we need to close the device
        /// so that all pending operations are canceled properly.
        /// 
        /// We will also remove the device from the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="deviceInformationUpdate"></param>
        private async void OnDeviceRemoved(DeviceWatcher sender, DeviceInformationUpdate deviceInformationUpdate)
        {
            if ((DeviceList.Current.IsDeviceConnected)
                && (deviceInformationUpdate.Id == DeviceList.Current.CurrentDeviceEntry.Id))
            {
                await rootPage.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    new DispatchedHandler(() =>
                    {
                        rootPage.NotifyUser(
                            deviceInformationUpdate.Id + " was removed. Don't worry, we are closing the HidDevice",
                            NotifyType.StatusMessage);

                        DeviceList.Current.RemoveDeviceFromList(deviceInformationUpdate.Id);

                        DeviceConnect.CloseDevice();
                    }));
            }
        }

        /// <summary>
        /// This function will add the device to the DeviceList so that it shows up in the UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="deviceInformation"></param>
        private async void OnDeviceAdded(DeviceWatcher sender, DeviceInformation deviceInformation)
        {
            await rootPage.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                new DispatchedHandler(() =>
                {
                    rootPage.NotifyUser("OnDeviceAdded: " + deviceInformation.Id, NotifyType.StatusMessage);

                    DeviceList.Current.AddDeviceToList(deviceInformation);
                }));
        }

        /// <summary>
        /// Notifies the UI that we are done enumerating devices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnDeviceEnumerationComplete(DeviceWatcher sender, Object args)
        {
            await rootPage.Dispatcher.RunAsync(
                CoreDispatcherPriority.Low,
                new DispatchedHandler(() =>
                {
                    rootPage.NotifyUser("OnDeviceEnumerationComplete", NotifyType.StatusMessage);
                }));
        }
    }
}
