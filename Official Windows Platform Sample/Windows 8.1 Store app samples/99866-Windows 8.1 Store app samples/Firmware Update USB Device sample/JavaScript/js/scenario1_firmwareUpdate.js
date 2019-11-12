//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    var FirmwareUpdateClass = WinJS.Class.define(null, {
        isUpdatingFirmware: false,
        /// <summary>
        /// Save trigger so that we may start the background task later
        /// Only one instance of the trigger can exist at a time. Since the trigger does not implement
        /// IDisposable, it may still be in memory when a new trigger is created.
        /// </summary>
        firmwareUpdateBackgroundTaskTrigger: new Windows.ApplicationModel.Background.DeviceServicingTrigger(),
        firmwareUpdateBackgroundTaskRegistration: null,
        /// <summary>
        /// Finds all the the supermutt devices and returns the device information object for the first device.
        /// </summary>
        /// <returns>DeviceInformation for first device or null if there are no device found</returns>
        findFirstSuperMuttDeviceAsync: function () {
            return Windows.Devices.Enumeration.DeviceInformation.findAllAsync(Windows.Devices.Usb.UsbDevice.getDeviceSelector(
                SdkSample.Constants.superMutt.device.vid, SdkSample.Constants.superMutt.device.pid), null).then(function (deviceInformationCollection) {
                    if (deviceInformationCollection.length > 0) {
                        return deviceInformationCollection[0];
                    } else {
                        return null;
                    }
                });
        },
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
        startFirmwareForDeviceAsync: function (deviceInformation) {
            return this.firmwareUpdateBackgroundTaskTrigger.requestAsync(deviceInformation.id, SdkSample.Constants.firmwareUpdateTaskInformation.approximateFirmwareUpdateTime).then(function (deviceTriggerResult) {
                // Determine if we are allowed to do firmware update
                var statusMessage = null;

                switch (deviceTriggerResult)
                {
                    case Windows.ApplicationModel.Background.DeviceTriggerResult.allowed:
                        firmwareUpdate.isUpdatingFirmware = true;
                        statusMessage = "Firmware update was allowed";
                        break;

                    case Windows.ApplicationModel.Background.DeviceTriggerResult.lowBattery:
                        firmwareUpdate.isUpdatingFirmware = false;
                        statusMessage = "Insufficient battery to start firmware update";
                        break;

                    case Windows.ApplicationModel.Background.DeviceTriggerResult.deniedByUser:
                        firmwareUpdate.isUpdatingFirmware = false;
                        statusMessage = "User declined the operation";
                        break;

                    case Windows.ApplicationModel.Background.DeviceTriggerResult.deniedBySystem:
                        // This can happen if the device metadata is not installed on the system.
                        // The app must be a privileged app
                        firmwareUpdate.isUpdatingFirmware = false;
                        statusMessage = "Firmware update operation was denied by the system";
                        break;

                    default:
                        firmwareUpdate.isUpdatingFirmware = false;
                        statusMessage = "Failed to initiate firmware update";
                        break;
                }

                return statusMessage;
            });
        },
        cancelFirmwareUpdate: function () {
            this.firmwareUpdateBackgroundTaskRegistration.unregister(true);

            this.firmwareUpdateBackgroundTaskRegistration = null;

            // We are canceling the task, so we are no longer updating. If the task is registered but never run,
            // the cancel completion is never called
            this.isUpdatingFirmware = false;
        },
        /// <summary>
        /// Finds the first enumerated device, attempts to open it, and starts updating the firmare.
        /// 
        /// The device must be opened and closed before starting the background task because we must get permission from
        /// the user (the consent prompt) in the UI or else the background task will not be able to open the device.
        /// </summary>
        updateFirmwareForFirstEnumeratedDeviceAsync: function () {
            return this.findFirstSuperMuttDeviceAsync().then(function (firstSuperMuttDeviceInformation) {
                if (firstSuperMuttDeviceInformation) {
                    // Open device here and get permission from the user
                    return Windows.Devices.Usb.UsbDevice.fromIdAsync(firstSuperMuttDeviceInformation.id).then(function (usbDevice) {
                        if (usbDevice) {
                            // Firmware version before update (for the SuperMutt, the device revision is the firmware version)
                            var oldFirmwareVersion = "0x" + usbDevice.deviceDescriptor.bcdDeviceRevision.toString(16);
                            updateOldFirmwareVersionInUI(oldFirmwareVersion);

                            // After getting permission, we need to close the device so that the background task can open
                            // the device. See comment for the function StartFirmwareForDeviceAsync().
                            usbDevice.close();
                            usbDevice = null;

                            // Create a background task for the firmware update
                            firmwareUpdate.registerForFirmwareUpdateBackgroundTask();

                            // Triggers the background task to update.
                            return firmwareUpdate.startFirmwareForDeviceAsync(firstSuperMuttDeviceInformation);
                        } else {
                            return WinJS.Promise.wrap("Could not open the device");
                        }
                    });
                } else {
                    return WinJS.Promise.wrap("No supported devices found");
                }
            }).then(function (firmwareStatusMessage) {
                // The firmware should be updating now, if not something went wrong
                if (firmwareUpdate.isUpdatingFirmware) {
                    WinJS.log && WinJS.log("Updating firmware...", "sample", "status");
                } else {
                    WinJS.log && WinJS.log("Unable to update firmware: " + firmwareStatusMessage, "sample", "error");
                }
            });
        },
        /// <summary>
        /// Registers for the firmware update background task
        /// </summary>
        registerForFirmwareUpdateBackgroundTask: function () {
            // Create background task to do the firmware update
            var backgroundTaskBuilder = new Windows.ApplicationModel.Background.BackgroundTaskBuilder();

            backgroundTaskBuilder.name = SdkSample.Constants.firmwareUpdateTaskInformation.name;
            backgroundTaskBuilder.taskEntryPoint = SdkSample.Constants.firmwareUpdateTaskInformation.taskEntryPoint;
            backgroundTaskBuilder.setTrigger(this.firmwareUpdateBackgroundTaskTrigger);
            this.firmwareUpdateBackgroundTaskRegistration = backgroundTaskBuilder.register();

            // Make sure we're notified when the task completes or if there is an update
            this.firmwareUpdateBackgroundTaskRegistration.addEventListener("completed", firmwareUpdate.onFirmwareUpdateCompleted);
            this.firmwareUpdateBackgroundTaskRegistration.addEventListener("progress", firmwareUpdate.onFirmwareUpdateProgress);
        },
        /// <summary>
        /// Search all the existing background tasks for the firmware update task
        /// </summary>
        /// <returns>If found, the background task registration for the firmware update task; else, null.</returns>
        findFirmwareUpdateTask: function () {
            // Unregister and cancel any background tasks may have persisted from this scenario
            var backgroundTaskIter = Windows.ApplicationModel.Background.BackgroundTaskRegistration.allTasks.first();

            while (backgroundTaskIter.hasCurrent) {
                var backgroundTask = backgroundTaskIter.current.value;

                if (backgroundTask.name === SdkSample.Constants.firmwareUpdateTaskInformation.name) {
                    return backgroundTask;
                }

                backgroundTaskIter.moveNext();
            }

            return null;
        },
        /// <summary>
        /// Print the version number
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        onFirmwareUpdateCompleted: function (args) {
            // Exception may be thrown if an error occurs during running the background task
            args.checkResult();
            
            var taskCompleteStatus = Windows.Storage.ApplicationData.current.localSettings.values[SdkSample.Constants.localSettingKeys.firmwareUpdateBackgroundTask.taskStatus];

            if (taskCompleteStatus === SdkSample.Constants.firmwareUpdateTaskInformation.taskCompleted) {
                // Display firmware version after the firmware update
                var newFirmwareVersion = Windows.Storage.ApplicationData.current.localSettings.values[SdkSample.Constants.localSettingKeys.firmwareUpdateBackgroundTask.newFirmwareVersion];
                var newFirmwareVersionHex = "0x" + newFirmwareVersion.toString(16);
                
                updateNewFirmwareVersionInUI(newFirmwareVersionHex);

                WinJS.log && WinJS.log("Firmware update completed", "sample", "status");
            } else if (taskCompleteStatus === SdkSample.Constants.firmwareUpdateTaskInformation.taskCanceled) {
                WinJS.log && WinJS.log("Firmware update was canceled", "sample", "status");
            }

            // Remove all local setting values
            Windows.Storage.ApplicationData.current.localSettings.values.clear();

            firmwareUpdate.isUpdatingFirmware = false;

            updateButtonStates();

            if (firmwareUpdate.firmwareUpdateBackgroundTaskRegistration) {
                // Unregister the background task and let the remaining task finish until completion
                firmwareUpdate.firmwareUpdateBackgroundTaskRegistration.unregister(false);
            }
        },
        /// <summary>
        /// Updates the UI with the progress of the firmware update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        onFirmwareUpdateProgress: function (args) {
            window.firmwareUpdateProgressBar.value = args.progress;
        }
    }, null);

    var firmwareUpdate = new FirmwareUpdateClass();

    var page = WinJS.UI.Pages.define("/html/scenario1_firmwareUpdate.html", {
        ready: function (element, options) {
            document.getElementById("buttonUpdateFirmwareOnFirstEnumeratedDevice").addEventListener("click", updateFirmwareOnFirstEnumeratedDeviceClick, false);
            document.getElementById("buttonCancelFirmwareUpdate").addEventListener("click", cancelFirmwareUpdateClick, false);

            // Unregister any existing tasks that persisted; there should be none unless the app closed/crashed
            var existingFirmwareUpdateTask = firmwareUpdate.findFirmwareUpdateTask();
            if (existingFirmwareUpdateTask) {
                existingFirmwareUpdateTask.unregister(true);
            }

            firmwareUpdate.isUpdatingFirmware = false;

            updateButtonStates();
        }
    });

    function updateFirmwareOnFirstEnumeratedDeviceClick() {
        // Lock the firmware update button to prevent the user from trying multiple times while we are still creating the background task.
        window.buttonUpdateFirmwareOnFirstEnumeratedDevice.disabled = true;

        // Clear the device versions in the UI in case there was something there before
        updateOldFirmwareVersionInUI("");
        updateNewFirmwareVersionInUI("");

        firmwareUpdate.updateFirmwareForFirstEnumeratedDeviceAsync().done(function () {
            updateButtonStates();
        });
    }

    function cancelFirmwareUpdateClick() {
        firmwareUpdate.cancelFirmwareUpdate();

        updateButtonStates();
    }

    function updateButtonStates() {
        window.buttonUpdateFirmwareOnFirstEnumeratedDevice.disabled = firmwareUpdate.isUpdatingFirmware;
        window.buttonCancelFirmwareUpdate.disabled = !window.buttonUpdateFirmwareOnFirstEnumeratedDevice.disabled;
    }

    function updateOldFirmwareVersionInUI(version)
    {
        window.outputDeviceVersionBefore.innerHTML = version;
    }

    function updateNewFirmwareVersionInUI(version)
    {
        window.outputDeviceVersionAfter.innerHTML = version;
    }
})();
