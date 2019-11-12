//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Background;
using Windows.Devices.Usb;
using Windows.Storage;
using Windows.Storage.Streams;

namespace BackgroundTask
{
    public sealed class UpdateFirmwareTask : IBackgroundTask, IDisposable
    {
        // Deferral must be used to prevent the background task from terminating 
        // while an asynchronous operation is still active
        private BackgroundTaskDeferral deferral;
        private IBackgroundTaskInstance backgroundTaskInstance;

        private DeviceServicingDetails deviceServicingDetails;

        private UsbDevice device;

        private CancellationTokenSource cancellationTokenSource;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            backgroundTaskInstance = taskInstance;

            // Trigger details contain device id and arguments that are provided by the caller in the main app
            // The taskInstance can always be casted to DeviceServicingDetails if this background task was started using a DeviceServicingTrigger
            deviceServicingDetails = (DeviceServicingDetails)taskInstance.TriggerDetails;

            deferral = taskInstance.GetDeferral();

            try
            {
                backgroundTaskInstance.Progress = 0;

                cancellationTokenSource = new CancellationTokenSource();

                taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

                // For simplicity, no error checking will be done after opening the device. Ideally, one should always
                // check if the device was successfully opened and respond accordingly. For an example on how to do this,
                // please see Scenario 1 of this sample.
                await OpenDeviceAsync();

                // We opened the device, so notify the app that we've completed a bit of the background task
                backgroundTaskInstance.Progress = 10;

                // Setup the device for firmware update
                await SetupDeviceForFirmwareUpdateAsync();

                // Write firmware to the device
                UInt32 totalBytesWrittenForFirmware = await WriteFirmwareAsync();

                // Reset the device so the device loads the updated firmware
                await ResetDeviceAsync();

                // The device was closed during reset, so clean up our UsbDevice object
                CloseDevice();

                // Wait for the device to be reenumerated. A device watcher can also be used instead to watch for the device to be reenumerated; that way
                // you know when the device is opened instead of guessing like we do here
                await Task.Delay(4000);

                // We've completed firmware update
                backgroundTaskInstance.Progress = 100;

                // Reopen the device so we can grab the new revision number because the revision number reflects the firmware verson of the SuperMutt device.
                // A consent prompt would normally appear here because we updated the device (revision number was changed in this case), but the Firmware Update API
                // allows the background task to open any device that is listed in the device metadata file (the one that allowed this firmware update background task to start)
                // without a consent prompt. The consent prompt will still appear if the same device is opened outside the background task.
                await OpenDeviceAsync();

                var newFirmwareVersion = device.DeviceDescriptor.BcdDeviceRevision;

                CloseDevice();

                ApplicationData.Current.LocalSettings.Values[LocalSettingKeys.FirmwareUpdateBackgroundTask.TaskStatus] = FirmwareUpdateTaskInformation.TaskCompleted;
                ApplicationData.Current.LocalSettings.Values[LocalSettingKeys.FirmwareUpdateBackgroundTask.NewFirmwareVersion] = newFirmwareVersion;
            }
            catch (OperationCanceledException /*ex*/)
            {
                ApplicationData.Current.LocalSettings.Values[LocalSettingKeys.FirmwareUpdateBackgroundTask.TaskStatus] = FirmwareUpdateTaskInformation.TaskCanceled;
            }
            finally
            {
                // Complete the background task (this raises the OnCompleted event on the corresponding BackgroundTaskRegistration)
                deferral.Complete();
            }
        }

        public void Dispose()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }

        /// <summary>
        /// Closes the device and sets the device member variable to null
        /// </summary>
        private void CloseDevice()
        {
            device.Dispose();
            device = null;
        }

        /// <summary>
        /// Opens device and assigns member variable to the newly opened device
        /// </summary>
        /// <returns></returns>
        private async Task OpenDeviceAsync()
        {
            device = await UsbDevice.FromIdAsync(deviceServicingDetails.DeviceId).AsTask(cancellationTokenSource.Token);
        }

        private async Task ResetDeviceAsync()
        {
            var setupPacket = ControlTransferSetupPacketsFactory.CreateSetupPacket(ControlTransferSetupPacketsFactory.SetupPacketPurpose.ResetDevice);

            await device.SendControlOutTransferAsync(setupPacket).AsTask(cancellationTokenSource.Token);
        }

        /// <summary>
        /// Sets up the device so that we can perform a firmware update on it
        /// </summary>
        /// <returns></returns>
        private Task SetupDeviceForFirmwareUpdateAsync()
        {
            return Task.Run(async () =>
                {
                    var setupPacket = ControlTransferSetupPacketsFactory.CreateSetupPacket(ControlTransferSetupPacketsFactory.SetupPacketPurpose.SetupDeviceForFirmwareUpdate);

                    await device.SendControlOutTransferAsync(setupPacket);

                    // The device is setting itself up, so we'll wait until the device is done setting up
                    await Task.Delay(TimeSpan.FromSeconds(30));
                },
                cancellationTokenSource.Token);
        }

        /// <summary>
        /// Writes all the sectors to the device
        /// </summary>
        /// <returns>Total number of bytes that was written to the device</returns>
        private Task<UInt32> WriteFirmwareAsync()
        {
            return Task.Run(async () =>
                {
                    // Save how much progress remains to be completed before we start updating the firmware so we can figure out how much 
                    // to increase the progress per byte written so that the progress can reach roughly 100 by the end of the firmware update.
                    var remainingProgressForFirmwareUpdate = 100 - backgroundTaskInstance.Progress;
                    var totalFirmwareSize = GetTotalFirmwareSize();

                    // Will contain all the control transfer tasks so we can wait for them all to complete later
                    var listControlTransferTasks = new List<Task>();

                    UInt32 totalBytesWritten = 0;

                    // Write all the sectors
                    foreach (var firmwareSector in Firmware.Sectors)
                    {
                        if (!cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            await WriteSectorAsync(firmwareSector).ContinueWith((Task<UInt32> bytesWrittenTask) =>
                            {
                                var bytesWritten = bytesWrittenTask.Result;

                                totalBytesWritten += bytesWritten;

                                backgroundTaskInstance.Progress += ((bytesWritten * remainingProgressForFirmwareUpdate) / totalFirmwareSize);
                            },
                            cancellationTokenSource.Token);
                        }
                        else
                        {
                            break;
                        }

                    }

                    return totalBytesWritten;
                },
                cancellationTokenSource.Token);
        }

        /// <summary>
        /// Cancels opening device and the IO operation, whichever is still running
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="reason"></param>
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            cancellationTokenSource.Cancel();

            backgroundTaskInstance.Progress = 0;
        }

        /// <summary>
        /// This method is used to write the provided bits into their corresponding sectors on the device.
        /// 
        /// Due to chipset limitations, the control transfers cannot write the whole sector at once
        /// </summary>
        /// <returns>Total number of bytes written to the device</returns>
        private async Task<UInt32> WriteSectorAsync(FirmwareSector firmwareSector)
        {
            UInt32 totalBytesWritten = 0;
            
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                // Convert the binary array into a buffer so we can write pieces of the firmware at a time
                var dataWriter = new DataWriter();
                dataWriter.WriteBytes(firmwareSector.BinaryArray);

                var bufferFirmwareForSector = dataWriter.DetachBuffer();

                // The data reader that will be used to read blocks of the firmware out of the firmware buffer
                var firmwareToWriteReader = DataReader.FromBuffer(bufferFirmwareForSector);

                // Setup packet that will be used for the control transfer
                var writeSectorSetupPacket = ControlTransferSetupPacketsFactory.CreateSetupPacket(ControlTransferSetupPacketsFactory.SetupPacketPurpose.WriteSector);

                var addressToWriteTo = firmwareSector.Sector;

                // Sequentially write the sector in blocks because of chipset limitations
                while (totalBytesWritten < firmwareSector.BinaryArray.Length)
                {
                    if (!cancellationTokenSource.IsCancellationRequested)
                    {
                        IBuffer bufferToWrite = null;

                        // Don't read more data than the buffer has
                        if (firmwareToWriteReader.UnconsumedBufferLength < Firmware.MaxBytesToWritePerControlTransfer)
                        {
                            bufferToWrite = firmwareToWriteReader.ReadBuffer(firmwareToWriteReader.UnconsumedBufferLength);
                        }
                        else
                        {
                            bufferToWrite = firmwareToWriteReader.ReadBuffer(Firmware.MaxBytesToWritePerControlTransfer);
                        }

                        // The follow properties are device specific
                        writeSectorSetupPacket.Value = addressToWriteTo >> 16;
                        writeSectorSetupPacket.Index = addressToWriteTo & 0xFFFF;

                        // Amount of data to be written to the device
                        writeSectorSetupPacket.Length = bufferToWrite.Length;

                        var bytesWritten = await device.SendControlOutTransferAsync(writeSectorSetupPacket, bufferToWrite).AsTask(cancellationTokenSource.Token);

                        totalBytesWritten += bytesWritten;

                        addressToWriteTo += bytesWritten;

                        // Give the device the opportunity to write the data packet to the EEPROM
                        await Task.Delay(100);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return totalBytesWritten;
        }

        private UInt32 GetTotalFirmwareSize()
        {
            UInt32 totalFirmwareSize = 0;

            foreach (var firmwareSector in Firmware.Sectors)
            {
                totalFirmwareSize += (UInt32) firmwareSector.BinaryArray.Length;
            }

            return totalFirmwareSize;
        }
    }
}
