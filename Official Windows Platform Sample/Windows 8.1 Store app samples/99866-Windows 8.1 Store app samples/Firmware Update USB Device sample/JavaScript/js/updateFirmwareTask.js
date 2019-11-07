//// Copyright (c) Microsoft Corporation. All rights reserved

(function () {
    "use strict";

    importScripts("//Microsoft.WinJS.2.0/js/base.js");
    importScripts("//Microsoft.SDKSamples.FirmwareUpdateUsbDevice.js/js/superMuttFirmware.js");
    importScripts("//Microsoft.SDKSamples.FirmwareUpdateUsbDevice.js/js/constants.js");

    // This var is used to get information about the current instance of the background task.
    var backgroundTaskInstance = Windows.UI.WebUI.WebUIBackgroundTaskInstance.current;

    // Trigger details contain device id and arguments that are provided by the caller in the main app
    var deviceServicingDetails = backgroundTaskInstance.triggerDetails;

    var isBackgroundTaskCanceled = false;

    // Usb Device we are updating the firmware
    var device;

    // Promise for opening the device
    var openDevicePromise;

    // Promise for the whole firmware update process
    var firmwareUpdatePromise;

    // Promise for the individual sector writes to the device
    var individualWriteSectorPromise;

    function run() {
        backgroundTaskInstance.addEventListener("canceled", onCanceled);

        backgroundTaskInstance.progress = 0;

        isBackgroundTaskCanceled = false;

        // For simplicity, no error checking will be done after opening the device. Ideally, one should always
        // check if the device was successfully opened and respond accordingly. For an example on how to do this,
        // please see Scenario 1 of this sample.
        openDeviceAsync().then(function () {
            // We opened the device, so notify the app that we've completed a bit of the background task
            backgroundTaskInstance.progress = 10;

            // Setup the device for firmware update
            return setupDeviceForFirmwareUpdateAsync();
        }).then(function () {
            // Write firmware to the device
            return writeFirmwareAsync();
        }).then(function (bytesWritten) {
            // Reset the device so the device loads the updated firmware
            return resetDeviceAsync();
        }).then(function () {
            // The device was closed during reset, so clean up our UsbDevice object
            closeDevice();

            // Wait for the device to be reenumerated. A device watcher can also be used instead to watch for the device to be reenumerated; that way
            // you know when the device is opened instead of guessing like we do here
            return WinJS.Promise.timeout(4000);
        }).then(function () {
            // We've completed firmware update
            backgroundTaskInstance.progress = 100;

            // Reopen the device so we can grab the new revision number because the revision number reflects the firmware verson of the SuperMutt device.
            // A consent prompt would normally appear here because we updated the device (revision number was changed in this case), but the Firmware Update API
            // allows the background task to open any device that is listed in the device metadata file (the one that allowed this firmware update background task to start)
            // without a consent prompt. The consent prompt will still appear if the same device is opened outside the background task.
            return openDeviceAsync();
        }).then(function () {
            var newFirmwareVersion = device.deviceDescriptor.bcdDeviceRevision;

            closeDevice();

            Windows.Storage.ApplicationData.current.localSettings.values[SdkSample.Constants.localSettingKeys.firmwareUpdateBackgroundTask.taskStatus] =
                SdkSample.Constants.firmwareUpdateTaskInformation.taskCompleted;

            Windows.Storage.ApplicationData.current.localSettings.values[SdkSample.Constants.localSettingKeys.firmwareUpdateBackgroundTask.newFirmwareVersion] =
                newFirmwareVersion;
        }, function (error) {
            if (error.name === "Canceled") {
                Windows.Storage.ApplicationData.current.localSettings.values[SdkSample.Constants.localSettingKeys.firmwareUpdateBackgroundTask.taskStatus] =
                    SdkSample.Constants.firmwareUpdateTaskInformation.taskCanceled;
            }
        }).done(function () {
            // Complete the background task (this raises the OnCompleted event on the corresponding BackgroundTaskRegistration)
            close();
        });
    }

    /// <summary>
    /// Closes the device and sets the device member variable to null
    /// </summary>
    function closeDevice() {
        if (device) {
            device.close();
            device = null;
        }
    }

    /// <summary>
    /// Opens device and assigns member variable to the newly opened device
    /// </summary>
    /// <returns></returns>
    function openDeviceAsync() {
        openDevicePromise = Windows.Devices.Usb.UsbDevice.fromIdAsync(deviceServicingDetails.deviceId).then(function (usbDevice) {
            openDevicePromise = null;

            device = usbDevice;
        });

        return openDevicePromise;
    }

    function resetDeviceAsync() {
        var setupPacket = SdkSample.Constants.controlTransferSetupPacketsFactory
            .createSetupPacket(SdkSample.Constants.controlTransferSetupPacketsFactory.setupPacketPurpose.resetDevice);

        var tmpPromise = device.sendControlOutTransferAsync(setupPacket).then(function (bytesWritten) {
            firmwareUpdatePromise = null;
        });

        // Save promise so it can be canceled
        firmwareUpdatePromise = tmpPromise;

        return tmpPromise;
    }

    /// <summary>
    /// Sets up the device so that we can perform a firmware update on it
    /// </summary>
    /// <returns></returns>
    function setupDeviceForFirmwareUpdateAsync() {
        var setupPacket = SdkSample.Constants.controlTransferSetupPacketsFactory
            .createSetupPacket(SdkSample.Constants.controlTransferSetupPacketsFactory.setupPacketPurpose.setupDeviceForFirmwareUpdate);

        var tmpPromise = device.sendControlOutTransferAsync(setupPacket).then(function (bytesWritten) {
            // The device is setting itself up, so we'll wait until the device is done setting up
            return WinJS.Promise.timeout(30000).then(function () {
                firmwareUpdatePromise = null;
            });
        });

        // Save promise so it can be canceled
        firmwareUpdatePromise = tmpPromise;

        return tmpPromise;
    }

    /// <summary>
    /// Writes all the sectors to the device
    /// </summary>
    /// <returns>Total number of bytes that was written to the device</returns>
    function writeFirmwareAsync() {
        // Save how much progress remains to be completed before we start updating the firmware so we can figure out how much 
        // to increase the progress per byte written so that the progress can reach 100 by the end of the firmware update.
        var remainingProgressForFirmwareUpdate = 100 - backgroundTaskInstance.progress;
        var totalFirmwareSize = getTotalFirmwareSize();

        var totalBytesWritten = 0;

        SdkSample.Constants.firmware.sectors.forEach(function (sector) {
            if (!isBackgroundTaskCanceled) {
                if (firmwareUpdatePromise) {
                    firmwareUpdatePromise = firmwareUpdatePromise.then(function () {
                        return writeSectorAsync(sector);
                    });
                } else {
                    firmwareUpdatePromise = writeSectorAsync(sector);
                }

                firmwareUpdatePromise = firmwareUpdatePromise.then(function (bytesWritten) {
                    totalBytesWritten += bytesWritten;

                    backgroundTaskInstance.progress += ((bytesWritten * remainingProgressForFirmwareUpdate) / totalFirmwareSize);
                });
            }
        });

        // After writing the firmware
        var tmpPromise = firmwareUpdatePromise.then(function () {
            firmwareUpdatePromise = null;

            return totalBytesWritten;
        });

        firmwareUpdatePromise = tmpPromise;

        return tmpPromise;
    }

    /// <summary>
    /// Cancels opening device and the IO operation, whichever is still running
    /// </summary>
    /// <param name="reason"></param>
    function onCanceled(reason) {
        if (openDevicePromise) {
            openDevicePromise.cancel();

            openDevicePromise = null;
        }

        if (firmwareUpdatePromise) {
            firmwareUpdatePromise.cancel();

            firmwareUpdatePromise = null;
        }

        if (individualWriteSectorPromise) {
            individualWriteSectorPromise.cancel();

            individualWriteSectorPromise = null;
        }

        isBackgroundTaskCanceled = true;
    }

    /// <summary>
    /// This method is used to write the provided bits into their corresponding sectors on the device.
    /// 
    /// Due to chipset limitations, the control transfers cannot write the whole sector at once
    /// </summary>
    /// <returns>Total number of bytes written to the device</returns>
    function writeSectorAsync(firmwareSector) {
        var totalBytesWritten = 0;
            
        if (!isBackgroundTaskCanceled) {
            // Convert the binary array into a buffer so we can write pieces of the firmware at a time
            var dataWriter = new Windows.Storage.Streams.DataWriter();
            dataWriter.writeBytes(firmwareSector.binaryArray);

            var bufferFirmwareForSector = dataWriter.detachBuffer();

            // The data reader that will be used to read blocks of the firmware out of the firmware buffer
            var firmwareToWriteReader = Windows.Storage.Streams.DataReader.fromBuffer(bufferFirmwareForSector);

            // Setup packet that will be used for the control transfer
            var writeSectorSetupPacket = SdkSample.Constants.controlTransferSetupPacketsFactory.createSetupPacket(
                SdkSample.Constants.controlTransferSetupPacketsFactory.setupPacketPurpose.writeSector);

            var addressToWriteTo = firmwareSector.sector;

            // Sequentially write the sector in blocks because of chipset limitations
            return new WinJS.Promise(function (success, error) {
                var writeNextBlockAsync = function () {
                    if (!isBackgroundTaskCanceled && (totalBytesWritten < firmwareSector.binaryArray.length)) {
                        var bufferToWrite = null;

                        // Don't read more data than the buffer has
                        if (firmwareToWriteReader.unconsumedBufferLength < SdkSample.Constants.firmware.maxBytesToWritePerControlTransfer) {
                            bufferToWrite = firmwareToWriteReader.readBuffer(firmwareToWriteReader.unconsumedBufferLength);
                        } else {
                            bufferToWrite = firmwareToWriteReader.readBuffer(SdkSample.Constants.firmware.maxBytesToWritePerControlTransfer);
                        }

                        // The follow properties are device specific
                        writeSectorSetupPacket.value = addressToWriteTo >> 16;
                        writeSectorSetupPacket.index = addressToWriteTo & 0xFFFF;

                        // Amount of data to be written to the device
                        writeSectorSetupPacket.length = bufferToWrite.length;

                        if (!individualWriteSectorPromise) {
                            individualWriteSectorPromise = device.sendControlOutTransferAsync(writeSectorSetupPacket, bufferToWrite);
                        } else {
                            individualWriteSectorPromise = individualWriteSectorPromise.then(function () {
                                return device.sendControlOutTransferAsync(writeSectorSetupPacket, bufferToWrite);
                            });
                        }

                        individualWriteSectorPromise = individualWriteSectorPromise.then(function (bytesWritten) {
                            totalBytesWritten += bytesWritten;

                            addressToWriteTo += bytesWritten;

                            // Give the device the opportunity to write the data packet to the EEPROM
                            return WinJS.Promise.timeout(100);
                        }, error).then(writeNextBlockAsync);
                    } else {
                        individualWriteSectorPromise = null;

                        success(totalBytesWritten);

                        return;
                    }
                };

                writeNextBlockAsync();
            });
        } else {
            return WinJS.Promise.wrap(totalBytesWritten);
        }
    }

    function getTotalFirmwareSize() {
        var totalFirmwareSize = 0;

        SdkSample.Constants.firmware.sectors.forEach(function (sector) {
            totalFirmwareSize += sector.binaryArray.length;
        });

        return totalFirmwareSize;
    }

    run();
})();