//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "IoSyncBackgroundTask.h"

using namespace Concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Devices::Background;
using namespace Windows::Devices::Usb;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace BackgroundTasks;

IoSyncBackgroundTask::IoSyncBackgroundTask()
{
}

void IoSyncBackgroundTask::Run(IBackgroundTaskInstance^ taskInstance)
{
    auto deferral = Platform::Agile<BackgroundTaskDeferral^>(taskInstance->GetDeferral());
    backgroundTaskInstance = taskInstance;

    // Trigger details contain device id and arguments that are provided by the caller in the main app
    // The taskInstance can always be casted to DeviceUseDetails if this background task was started using a DeviceUseTrigger
    deviceSyncDetails = static_cast<DeviceUseDetails^>(taskInstance->TriggerDetails);

    backgroundTaskInstance->Progress = 0;

    cancellationTokenSource = cancellation_token_source();

    taskInstance->Canceled += ref new BackgroundTaskCanceledEventHandler(this, &IoSyncBackgroundTask::OnCanceled);

    // After opening the device, sync with the device.
    // For simplicity, no error checking will be done after opening the device. Ideally, one should always
    // check if the device was successfully opened and respond accordingly. For an example on how to do this,
    // please see Scenario 1 of this sample.
    //
    // The user may also block the device via the settings charm while we are syncing (in background task). In order to deal with
    // the user changing permissions, we have to listen for DeviceAccessInformation->AccessChanged events. See EventHandlerForDevice 
    // for how to handle DeviceAccessInformation->AccessChanged event.
    OpenDeviceAsync().then([this] ()
    {
        // The sample only demonstrates a bulk write for simplicity.
        // IO operations can be done after opening the device.
        // For more information on BackgroundTasks, please see the BackgroundTask sample on MSDN.
        return WriteToDeviceAsync();
    }, cancellationTokenSource.get_token()).then([this, deferral] (task<uint32> backgroundRunTask)
    {
        try
        {
            // Close the device because we are finished syncing and so that the app may reopen the device
            delete device;

            device = nullptr;

            auto bytesWritten = backgroundRunTask.get();

            ApplicationData::Current->LocalSettings->Values->Insert(LocalSettingKeys::SyncBackgroundTaskResult, bytesWritten.ToString());

            ApplicationData::Current->LocalSettings->Values->Insert(LocalSettingKeys::SyncBackgroundTaskStatus, BackgroundTaskInformation::TaskCompleted);
        }
        catch (const task_canceled& /* taskCanceled */)
        {
            ApplicationData::Current->LocalSettings->Values->Insert(LocalSettingKeys::SyncBackgroundTaskResult, "0");

            ApplicationData::Current->LocalSettings->Values->Insert(LocalSettingKeys::SyncBackgroundTaskStatus, BackgroundTaskInformation::TaskCanceled);
        }
        
        // Complete the background task (this raises the OnCompleted event on the corresponding BackgroundTaskRegistration)
        deferral->Complete();
    });
}

task<void> IoSyncBackgroundTask::OpenDeviceAsync()
{
    return create_task(UsbDevice::FromIdAsync(deviceSyncDetails->DeviceId), cancellationTokenSource.get_token()).then([this] (UsbDevice^ usbDevice)
    {
        device = usbDevice;

        backgroundTaskInstance->Progress = 10;
    });
}

/// <summary>
/// Cancels opening device and the IO operation, whichever is still running
/// </summary>
/// <param name="sender"></param>
/// <param name="reason"></param>
void IoSyncBackgroundTask::OnCanceled(IBackgroundTaskInstance^ /* sender */, BackgroundTaskCancellationReason /* reason */)
{
    cancellationTokenSource.cancel();

    backgroundTaskInstance->Progress = 0;
}

/// <summary>
/// Writes to device's first bulkOut endpoint of the default interface and updates progress per write.
/// When this method finishes, the progress will be 100.
/// </summary>
/// <returns>Total number of bytes written to the device</returns>
task<uint32> IoSyncBackgroundTask::WriteToDeviceAsync()
{
    return task<uint32>([this] ()
    {
        uint32 totalBytesWritten = 0;

        auto firstBulkOutEndpoint = device->DefaultInterface->BulkOutPipes->GetAt(0);

        // Distributes the remaining progress (out of 100) among each write
        double progressIncreasePerWrite = (100 - backgroundTaskInstance->Progress) / (static_cast<double>(Sync::NumberOfTimesToWrite));

        // This progress will be incremented by the more accurate progressIncreasePerWrite value and then rounded up before notifying the app
        double syncProgress = backgroundTaskInstance->Progress;

        auto dataWriter = ref new DataWriter(firstBulkOutEndpoint->OutputStream);

        // Create an array, all default initialized to 0, and write it to the buffer
        // The data inside the buffer will be garbage
        auto arrayBuffer = ref new Array<uint8>(Sync::BytesToWriteAtATime);

        for (uint32 timesWritten = 0; timesWritten < Sync::NumberOfTimesToWrite; timesWritten++)
        {
            if (!is_task_cancellation_requested())
            {
                dataWriter->WriteBytes(arrayBuffer);

                // Wait for the store to complete
                auto writeTask = create_task(dataWriter->StoreAsync(), cancellationTokenSource.get_token());
                writeTask.wait();

                totalBytesWritten += writeTask.get();

                syncProgress += progressIncreasePerWrite;

                // Rounding our value up because backgroundTaskInstance.progress is an unsigned int
                backgroundTaskInstance->Progress = static_cast<uint32>(ceil(syncProgress));
            }
            else
            {
                cancel_current_task();
            }
        }

        return totalBytesWritten;
    });
}