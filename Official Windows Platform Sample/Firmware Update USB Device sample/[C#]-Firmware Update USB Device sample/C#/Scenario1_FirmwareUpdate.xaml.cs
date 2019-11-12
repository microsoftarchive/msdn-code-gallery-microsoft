//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Enumeration;
using Windows.Devices.Usb;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System.Globalization;

namespace FirmwareUpdateUsbDevice
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirmwareUpdate : SDKTemplate.Common.LayoutAwarePage
    {
        private Boolean isUpdatingFirmware;

        private DeviceServicingTrigger firmwareUpdateBackgroundTaskTrigger;
        private BackgroundTaskRegistration firmwareUpdateBackgroundTaskRegistration;

        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        private MainPage rootPage = MainPage.Current;

        public FirmwareUpdate()
        {
            // Save trigger so that we may start the background task later
            // Only one instance of the trigger can exist at a time. Since the trigger does not implement
            // IDisposable, it may still be in memory when a new trigger is created.
            firmwareUpdateBackgroundTaskTrigger = new DeviceServicingTrigger();
            
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// 
        /// Search for existing firmware update background task. If it already exists, unregister it.
        /// </summary>
        /// <param name="eventArgs">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            // Unregister any existing tasks that persisted; there should be none unless the app closed/crashed
            var existingFirmwareUpdateTask = FindFirmwareUpdateTask();
            if (existingFirmwareUpdateTask != null)
            {
                existingFirmwareUpdateTask.Unregister(true);
            }

            isUpdatingFirmware = false;

            UpdateButtonStates();
        }

        /// <summary>
        /// Finds all the the supermutt devices and returns the device information object for the first device.
        /// </summary>
        /// <returns>DeviceInformation for first device or null if there are no device found</returns>
        private async Task<DeviceInformation> FindFirstSuperMuttDeviceAsync()
        {
            var deviceInformationCollection = await DeviceInformation.FindAllAsync(UsbDevice.GetDeviceSelector(SuperMutt.Device.Vid, SuperMutt.Device.Pid));

            if (deviceInformationCollection.Count > 0)
            {
                return deviceInformationCollection[0];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Triggers the background task to update firmware for the device. The update will cancel if it's not completed within 2 minutes.
        /// 
        /// Before triggering the background task, all UsbDevices that will have their firmware updated must be closed. The background task will open
        /// the device to update the firmware, but if the app still has it open, the background task will fail. The reason why this happens is because
        /// when a UsbDevice is created, the corresponding device is opened exclusively (no one else can open this device).
        /// 
        /// The trigger.RequestAsync() must be started on the UI thread because of the prompt that appears. The caller of UpdateFirmwareForDeviceAsync()
        /// is responsible for running this method in the UI thread.
        /// </summary>
        /// <param name="deviceInformation"></param>
        /// <returns>An error message</returns>
        private async Task<String> StartFirmwareForDeviceAsync(DeviceInformation deviceInformation)
        {
            DeviceTriggerResult deviceTriggerResult = await firmwareUpdateBackgroundTaskTrigger.RequestAsync(deviceInformation.Id, 
                FirmwareUpdateTaskInformation.ApproximateFirmwareUpdateTime);

            // Determine if we are allowed to do firmware update
            String statusMessage = null;

            switch (deviceTriggerResult)
            {
                case DeviceTriggerResult.Allowed:
                    isUpdatingFirmware = true;
                    statusMessage = "Firmware update was allowed";
                    break;

                case DeviceTriggerResult.LowBattery:
                    isUpdatingFirmware = false;
                    statusMessage = "Insufficient battery to start firmware update";
                    break;

                case DeviceTriggerResult.DeniedByUser:
                    isUpdatingFirmware = false;
                    statusMessage = "User declined the operation";
                    break;

                case DeviceTriggerResult.DeniedBySystem:
                    // This can happen if the device metadata is not installed on the system.
                    // The app must be a privileged app
                    isUpdatingFirmware = false;
                    statusMessage = "Firmware update operation was denied by the system";
                    break;

                default:
                    isUpdatingFirmware = false;
                    statusMessage = "Failed to initiate firmware update - Unknown Reason";
                    break;
            }

            return statusMessage;
        }

        private void CancelFirmwareUpdate()
        {
            firmwareUpdateBackgroundTaskRegistration.Unregister(true);

            firmwareUpdateBackgroundTaskRegistration = null;

            // We are canceling the task, so we are no longer updating. If the task is registered but never run,
            // the cancel completion is never called
            isUpdatingFirmware = false;
        }

        /// <summary>
        /// Finds the first enumerated device, attempts to open it, and starts updating the firmare.
        /// 
        /// The device must be opened and closed before starting the background task because we must get permission from
        /// the user (the consent prompt) in the UI or else the background task will not be able to open the device.
        /// </summary>
        /// <returns></returns>
        private async Task UpdateFirmwareForFirstEnumeratedDeviceAsync()
        {
            DeviceInformation deviceToDoFirmwareUpdate = await FindFirstSuperMuttDeviceAsync();
            String firmwareStatusMessage = null;

            if (deviceToDoFirmwareUpdate != null)
            {
                // Open device here and get permission from the user
                var usbDevice = await UsbDevice.FromIdAsync(deviceToDoFirmwareUpdate.Id);

                if (usbDevice != null)
                {
                    // Firmware version before update (for the SuperMutt, the device revision is the firmware version)
                    var oldFirmwareVersion = "0x" + usbDevice.DeviceDescriptor.BcdDeviceRevision.ToString("X4", NumberFormatInfo.InvariantInfo);
                    UpdateOldFirmwareVersionInUI(oldFirmwareVersion);

                    // After getting permission, we need to close the device so that the background task can open
                    // the device. See comment for the function StartFirmwareForDeviceAsync().
                    usbDevice.Dispose();
                    usbDevice = null;

                    // Create a background task for the firmware update
                    RegisterForFirmwareUpdateBackgroundTask();

                    // Triggers the background task to update.
                    firmwareStatusMessage = await StartFirmwareForDeviceAsync(deviceToDoFirmwareUpdate);
                }
                else
                {
                    firmwareStatusMessage = "Could not open the device";
                }
            }
            else
            {
                firmwareStatusMessage = "No supported devices found";
            }

            // The firmware should be updating now, if not something went wrong
            if (isUpdatingFirmware)
            {
                rootPage.NotifyUser("Updating firmware...", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser("Unable to update firmware: " + firmwareStatusMessage, NotifyType.ErrorMessage);
            }
        }

        /// <summary>
        /// Registers for the firmware update background task
        /// </summary>
        private void RegisterForFirmwareUpdateBackgroundTask()
        {
            // Create background task to do the firmware update
            var backgroundTaskBuilder = new BackgroundTaskBuilder();

            backgroundTaskBuilder.Name = FirmwareUpdateTaskInformation.Name;
            backgroundTaskBuilder.TaskEntryPoint = FirmwareUpdateTaskInformation.TaskEntryPoint;
            backgroundTaskBuilder.SetTrigger(firmwareUpdateBackgroundTaskTrigger);
            firmwareUpdateBackgroundTaskRegistration = backgroundTaskBuilder.Register();

            // Make sure we're notified when the task completes or if there is an update
            firmwareUpdateBackgroundTaskRegistration.Completed += new BackgroundTaskCompletedEventHandler(OnFirmwareUpdateCompleted);
            firmwareUpdateBackgroundTaskRegistration.Progress += new BackgroundTaskProgressEventHandler(OnFirmwareUpdateProgress);
        }

        /// <summary>
        /// Search all the existing background tasks for the firmware update task
        /// </summary>
        /// <returns>If found, the background task registration for the firmware update task; else, null.</returns>
        BackgroundTaskRegistration FindFirmwareUpdateTask()
        {
            foreach (var backgroundTask in BackgroundTaskRegistration.AllTasks.Values)
            {
                if (backgroundTask.Name == FirmwareUpdateTaskInformation.Name)
                {
                    return (BackgroundTaskRegistration) backgroundTask;
                }
            }

            return null;
        }

        /// <summary>
        /// Print the version number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnFirmwareUpdateCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            // Exception may be thrown if an error occurs during running the background task
            args.CheckResult();

            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                new DispatchedHandler(() =>
                {
                    var taskCompleteStatus = (String)ApplicationData.Current.LocalSettings.Values[LocalSettingKeys.FirmwareUpdateBackgroundTask.TaskStatus];

                    if (taskCompleteStatus == FirmwareUpdateTaskInformation.TaskCompleted)
                    {
                        // Display firmware version after the firmware update
                        var newFirmwareVersion = (UInt32) ApplicationData.Current.LocalSettings.Values[LocalSettingKeys.FirmwareUpdateBackgroundTask.NewFirmwareVersion];

                        var newFirmwareVersionHex = "0x" + newFirmwareVersion.ToString("X4", NumberFormatInfo.InvariantInfo);

                        UpdateNewFirmwareVersionInUI(newFirmwareVersionHex);

                        rootPage.NotifyUser("Firmware update completed", NotifyType.StatusMessage);
                    }
                    else if (taskCompleteStatus == FirmwareUpdateTaskInformation.TaskCanceled)
                    {
                        rootPage.NotifyUser("Firmware update was canceled", NotifyType.StatusMessage);
                    }

                    // Remove all local setting values
                    ApplicationData.Current.LocalSettings.Values.Clear();

                    isUpdatingFirmware = false;

                    UpdateButtonStates();
                }));

            if (firmwareUpdateBackgroundTaskRegistration != null)
            {
                // Unregister the background task and let the remaining task finish until completion
                firmwareUpdateBackgroundTaskRegistration.Unregister(false);
            }
        }

        /// <summary>
        /// Updates the UI with the progress of the firmware update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void OnFirmwareUpdateProgress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {
            await rootPage.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                new DispatchedHandler(() =>
                {
                    FirmwareUpdateProgressBar.Value = args.Progress;
                }));
        }

        private async void UpdateFirmwareOnFirstEnumeratedDevice_Click(object sender, RoutedEventArgs e)
        {
            // Lock the firmware update button to prevent the user from trying multiple times while we are still creating the background task.
            ButtonUpdateFirmwareOnFirstEnumeratedDevice.IsEnabled = false;

            // Clear the device versions in the UI in case there was something there before
            UpdateOldFirmwareVersionInUI("");
            UpdateNewFirmwareVersionInUI("");

            await UpdateFirmwareForFirstEnumeratedDeviceAsync();

            UpdateButtonStates();
        }

        private void CancelFirmwareUpdate_Click(object sender, RoutedEventArgs e)
        {
            CancelFirmwareUpdate();

            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            ButtonUpdateFirmwareOnFirstEnumeratedDevice.IsEnabled = !isUpdatingFirmware;
            ButtonCancelFirmwareUpdate.IsEnabled = !ButtonUpdateFirmwareOnFirstEnumeratedDevice.IsEnabled;
        }

        private void UpdateOldFirmwareVersionInUI(String version)
        {
            OutputDeviceVersionBefore.Text = version;
        }

        private void UpdateNewFirmwareVersionInUI(String version)
        {
            OutputDeviceVersionAfter.Text = version;
        }

    }
}
