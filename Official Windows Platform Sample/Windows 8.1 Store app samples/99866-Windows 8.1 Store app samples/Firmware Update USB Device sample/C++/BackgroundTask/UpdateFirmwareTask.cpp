//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "UpdateFirmwareTask.h"

using namespace Concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Devices::Background;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Devices::Usb;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace BackgroundTask;
using namespace BackgroundTask::FirmwareUpdateUsbDevice;

UpdateFirmwareTask::UpdateFirmwareTask()
{
}

void UpdateFirmwareTask::Run(IBackgroundTaskInstance^ taskInstance)
{
    backgroundTaskInstance = taskInstance;

    // Trigger details contain device id and arguments that are provided by the caller in the main app
    // The taskInstance can always be casted to DeviceServicingDetails if this background task was started using a DeviceServicingTrigger
    deviceServicingDetails = static_cast<DeviceServicingDetails^>(taskInstance->TriggerDetails);

    auto deferral = Platform::Agile<BackgroundTaskDeferral^>(taskInstance->GetDeferral());

    backgroundTaskInstance->Progress = 0;

    cancellationTokenSource = cancellation_token_source();

    taskInstance->Canceled +=  ref new BackgroundTaskCanceledEventHandler(this, &UpdateFirmwareTask::OnCanceled);

    // For simplicity, no error checking will be done after opening the device. Ideally, one should always
    // check if the device was successfully opened and respond accordingly. For an example on how to do this,
    // please see Scenario 1 of this sample.
    OpenDeviceAsync().then([this] ()
    {
        // We opened the device, so notify the app that we've completed a bit of the background task
        backgroundTaskInstance->Progress = 10;

        // Setup the device for firmware update
        return SetupDeviceForFirmwareUpdateAsync();
    }).then([this] ()
    {              
        // Write firmware to the device 
        return WriteFirmwareAsync();
    }).then([this] (uint32 totalBytesWrittenForFirmware)
    {
        // Reset the device so the device loads the updated firmware
        return ResetDeviceAsync();
    }).then([this] (uint32 bytesWritten)
    {
        // The device was closed during reset, so clean up our UsbDevice object
        CloseDevice();

        // Wait for the device to be reenumerated. A device watcher can also be used instead to watch for the device to be reenumerated; that way
        // you know when the device is opened instead of guessing like we do here
        wait(4000);

        // Reopen the device so we can grab the new revision number because the revision number reflects the firmware verson of the SuperMutt device.
        // A consent prompt would normally appear here because we updated the device (revision number was changed in this case), but the Firmware Update API
        // allows the background task to open any device that is listed in the device metadata file (the one that allowed this firmware update background task to start)
        // without a consent prompt. The consent prompt will still appear if the same device is opened outside the background task.
        return OpenDeviceAsync();
    }).then([this, deferral] (task<void> firmwareUpdateTask) {
        try
        {
            firmwareUpdateTask.get();

            auto newFirmwareVersion = device->DeviceDescriptor->BcdDeviceRevision;

            CloseDevice();

            // We've completed firmware update
            backgroundTaskInstance->Progress = 100;

            ApplicationData::Current->LocalSettings->Values->Insert(LocalSettingKeys::FirmwareUpdateBackgroundTask::TaskStatus, FirmwareUpdateTaskInformation::TaskCompleted);
            ApplicationData::Current->LocalSettings->Values->Insert(LocalSettingKeys::FirmwareUpdateBackgroundTask::NewFirmwareVersion, newFirmwareVersion);
        }
        catch (const task_canceled& /* taskCanceled */)
        {
            ApplicationData::Current->LocalSettings->Values->Insert(LocalSettingKeys::FirmwareUpdateBackgroundTask::TaskStatus, FirmwareUpdateTaskInformation::TaskCanceled);
        }

        // Complete the background task (this raises the OnCompleted event on the corresponding BackgroundTaskRegistration)
        deferral->Complete();
    });
}

/// <summary>
/// Closes the device and sets the device member variable to null
/// </summary>
void UpdateFirmwareTask::CloseDevice()
{
    delete device;
    device = nullptr;
}

/// <summary>
/// Opens device and assigns member variable to the newly opened device
/// </summary>
/// <returns></returns>
task<void> UpdateFirmwareTask::OpenDeviceAsync()
{
    return create_task(UsbDevice::FromIdAsync(deviceServicingDetails->DeviceId), cancellationTokenSource.get_token()).then([this] (UsbDevice^ usbDevice)
        {
            device = usbDevice;
        });
}

task<uint32> UpdateFirmwareTask::ResetDeviceAsync()
{
    auto setupPacket = ControlTransferSetupPacketsFactory::CreateSetupPacket(ControlTransferSetupPacketsFactory::SetupPacketPurpose::ResetDevice);

    return create_task(device->SendControlOutTransferAsync(setupPacket), cancellationTokenSource.get_token());
}

/// <summary>
/// Sets up the device so that we can perform a firmware update on it
/// </summary>
/// <returns></returns>
task<void> UpdateFirmwareTask::SetupDeviceForFirmwareUpdateAsync()
{
    return task<uint32>([this] ()
        {
            auto setupPacket = ControlTransferSetupPacketsFactory::CreateSetupPacket(ControlTransferSetupPacketsFactory::SetupPacketPurpose::SetupDeviceForFirmwareUpdate);

            return device->SendControlOutTransferAsync(setupPacket);
        },
        cancellationTokenSource.get_token()).then([this] (uint32 bytesWritten)
        {
            // The device is setting itself up, so we'll wait until the device is done setting up (takes 30 seconds)
            wait(30000);
        },
        cancellationTokenSource.get_token());
}

/// <summary>
/// Writes all the sectors to the device
/// </summary>
/// <returns>Total number of bytes that was written to the device</returns>
task<uint32> UpdateFirmwareTask::WriteFirmwareAsync()
{
    return task<uint32>([this] ()
        {
            // Save how much progress remains to be completed before we start updating the firmware so we can figure out how much 
            // to increase the progress per byte written so that the progress can reach 100 by the end of the firmware update.
            auto remainingProgressForFirmwareUpdate = 100 - backgroundTaskInstance->Progress;
            auto totalFirmwareSize = GetTotalFirmwareSize();

            auto cancellationTokenForIo = cancellationTokenSource.get_token();

            uint32 totalBytesWritten = 0;

            // Write all the sectors
            for each (auto firmwareSector in Firmware::Sectors)
            {
                if (!cancellationTokenForIo.is_canceled())
                {
                    auto writeSectorTask = WriteSectorAsync(firmwareSector)
                        .then([this, remainingProgressForFirmwareUpdate, totalFirmwareSize, &totalBytesWritten] (uint32 bytesWritten)
                        {
                            totalBytesWritten += bytesWritten;

                            backgroundTaskInstance->Progress += ((bytesWritten * remainingProgressForFirmwareUpdate) / totalFirmwareSize);
                        }, 
                        cancellationTokenForIo);

                    writeSectorTask.wait();
                }
                else
                {
                    break;
                }
            }

            return totalBytesWritten;
        },
        cancellationTokenSource.get_token());
}

/// <summary>
/// Cancels opening device and the IO operation, whichever is still running
/// </summary>
/// <param name="sender"></param>
/// <param name="reason"></param>
void UpdateFirmwareTask::OnCanceled(IBackgroundTaskInstance^ /* sender */, BackgroundTaskCancellationReason /* reason */)
{
    cancellationTokenSource.cancel();

    backgroundTaskInstance->Progress = 0;
}

/// <summary>
/// This method is used to write the provided bits into their corresponding sectors on the device.
/// 
/// Due to chipset limitations, the control transfers cannot write the whole sector at once
/// </summary>
/// <returns>Total number of bytes written to the device</returns>
task<uint32> UpdateFirmwareTask::WriteSectorAsync(FirmwareSector^ firmwareSector)
{
    return task<uint32>([this, firmwareSector] ()
    {
        uint32 totalBytesWritten = 0;

        auto cancellationToken = cancellationTokenSource.get_token();

        if (!cancellationToken.is_canceled())
        {
            // Convert the binary array into a buffer so we can write pieces of the firmware at a time
            auto dataWriter = ref new DataWriter();
            dataWriter->WriteBytes(firmwareSector->BinaryArray);

            auto bufferFirmwareForSector = dataWriter->DetachBuffer();

            // The data reader that will be used to read blocks of the firmware out of the firmware buffer
            auto firmwareToWriteReader = DataReader::FromBuffer(bufferFirmwareForSector);

            // Setup packet that will be used for the control transfer
            auto writeSectorSetupPacket = ControlTransferSetupPacketsFactory::CreateSetupPacket(ControlTransferSetupPacketsFactory::SetupPacketPurpose::WriteSector);

            auto addressToWriteTo = firmwareSector->Sector;

            // Sequentially write the sector in blocks because of chipset limitations
            while (totalBytesWritten < firmwareSector->BinaryArray->Length)
            {
                if (!cancellationToken.is_canceled())
                {
                    IBuffer^ bufferToWrite = nullptr;

                    // Don't read more data than the buffer has
                    if (firmwareToWriteReader->UnconsumedBufferLength < Firmware::MaxBytesToWritePerControlTransfer)
                    {
                        bufferToWrite = firmwareToWriteReader->ReadBuffer(firmwareToWriteReader->UnconsumedBufferLength);
                    }
                    else
                    {
                        bufferToWrite = firmwareToWriteReader->ReadBuffer(Firmware::MaxBytesToWritePerControlTransfer);
                    }    

                    // The follow properties are device specific
                    writeSectorSetupPacket->Value = addressToWriteTo >> 16;
                    writeSectorSetupPacket->Index = addressToWriteTo & 0xFFFF;

                    // Amount of data to be written to the device
                    writeSectorSetupPacket->Length = bufferToWrite->Length;

                    // Wait for the write to complete
                    auto writeTask = create_task(device->SendControlOutTransferAsync(writeSectorSetupPacket, bufferToWrite), cancellationTokenSource.get_token());
                    writeTask.wait();

                    auto bytesWritten = writeTask.get();

                    totalBytesWritten += bytesWritten;

                    addressToWriteTo += bytesWritten;

                    // Give the device the opportunity to write the data packet to the EEPROM
                    wait(100);
                }
                else
                {
                    break;
                }
            }
        }

        return totalBytesWritten;
    });
}

uint32 UpdateFirmwareTask::GetTotalFirmwareSize()
{
    uint32 totalFirmwareSize = 0;

    for each (auto firmwareSector in Firmware::Sectors)
    {
        totalFirmwareSize += firmwareSector->BinaryArray->Length;
    }

    return totalFirmwareSize;
}