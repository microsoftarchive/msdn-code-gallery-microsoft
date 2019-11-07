//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1_FirmwareUpdate.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1_FirmwareUpdate.xaml.h"
#include "MainPage.xaml.h"

using namespace Concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Devices::Usb;
using namespace Windows::Foundation;
using namespace Windows::Storage;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace SDKSample;
using namespace SDKSample::FirmwareUpdateUsbDevice;

FirmwareUpdate::FirmwareUpdate() :
    isUpdatingFirmware(false)
{
    // Save trigger so that we may start the background task later
    // Only one instance of the trigger can exist at a time. Since the trigger does not implement
    // IDisposable, it may still be in memory when a new trigger is created.
    firmwareUpdateBackgroundTaskTrigger = ref new DeviceServicingTrigger();

    InitializeComponent();
}


/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// 
/// Search for existing firmware update background task. If it already exists, unregister it.
/// </summary>
/// <param name="eventArgs">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void FirmwareUpdate::OnNavigatedTo(NavigationEventArgs^ eventArgs)
{
    // Unregister any existing tasks that persisted; there should be none unless the app closed/crashed
    auto existingFirmwareUpdateTask = FindFirmwareUpdateTask();
    if (existingFirmwareUpdateTask != nullptr)
    {
        existingFirmwareUpdateTask->Unregister(true);
    }

    isUpdatingFirmware = false;

    UpdateButtonStates();
}

/// <summary>
/// Finds all the the supermutt devices and returns the device information object for the first device.
/// </summary>
/// <returns>DeviceInformation for first device or null if there are no device found</returns>
task<DeviceInformation^> FirmwareUpdate::FindFirstSuperMuttDeviceAsync()
{
    return create_task(DeviceInformation::FindAllAsync(UsbDevice::GetDeviceSelector(SuperMutt::Device::Vid, SuperMutt::Device::Pid)))
        .then([this](DeviceInformationCollection^ deviceInformationCollection) -> DeviceInformation^
        {
            if (deviceInformationCollection->Size > 0)
            {
                return deviceInformationCollection->GetAt(0);
            }
            else
            {
                return nullptr;
            }
        });
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
task<String^> FirmwareUpdate::StartFirmwareForDeviceAsync(DeviceInformation^ deviceInformation)
{
    return create_task(firmwareUpdateBackgroundTaskTrigger->RequestAsync(deviceInformation->Id, FirmwareUpdateTaskInformation::ApproximateFirmwareUpdateTime))
        .then([this](DeviceTriggerResult deviceTriggerResult)
        {
            // Determine if we are allowed to do firmware update
            String^ statusMessage = nullptr;

            switch (deviceTriggerResult)
            {
            case DeviceTriggerResult::Allowed:
                isUpdatingFirmware = true;
                statusMessage = "Firmware update was allowed";
                break;

            case DeviceTriggerResult::LowBattery:
                isUpdatingFirmware = false;
                statusMessage = "Insufficient battery to start firmware update";
                break;

            case DeviceTriggerResult::DeniedByUser:
                isUpdatingFirmware = false;
                statusMessage = "User declined the operation";
                break;

            case DeviceTriggerResult::DeniedBySystem:
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
        });
}

void FirmwareUpdate::CancelFirmwareUpdate()
{
    firmwareUpdateBackgroundTaskRegistration->Unregister(true);

    firmwareUpdateBackgroundTaskRegistration = nullptr;

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
task<void> FirmwareUpdate::UpdateFirmwareForFirstEnumeratedDeviceAsync()
{
    return FindFirstSuperMuttDeviceAsync().then([this] (DeviceInformation^ firstSuperMuttDeviceFound)
        {
            if (firstSuperMuttDeviceFound != nullptr)
            {
                // Open device here and get permission from the user
                return create_task(UsbDevice::FromIdAsync(firstSuperMuttDeviceFound->Id)).then([this, firstSuperMuttDeviceFound](UsbDevice^ usbDevice)
                    {
                        if (usbDevice != nullptr)
                        {
                            // Firmware version before update
                            auto oldFirmwareVersion = Utilities::ConvertToHex16Bit(usbDevice->DeviceDescriptor->BcdDeviceRevision);
                            UpdateOldFirmwareVersionInUI(oldFirmwareVersion);

                            // After getting permission, we need to close the device so that the background task can open
                            // the device. See comment for the function StartFirmwareForDeviceAsync().
                            delete usbDevice;
                            usbDevice = nullptr;

                            // Create a background task for the firmware update
                            RegisterForFirmwareUpdateBackgroundTask();

                            // Triggers the background task to update.
                            return StartFirmwareForDeviceAsync(firstSuperMuttDeviceFound);
                        }
                        else
                        {
                            return task<String^>([] ()
                                {
                                    return ref new String(L"Could not open the device");
                                }); 
                        }
                    });
            }
            else
            {
                return task<String^>([] ()
                    {
                        return ref new String(L"No supported devices found");
                    });
            }
        }).then([this] (String^ firmwareStatusMessage)
        {
            // The firmware should be updating now, if not something went wrong
            if (isUpdatingFirmware)
            {
                MainPage::Current->NotifyUser("Updating firmware...", NotifyType::StatusMessage);
            }
            else
            {
                MainPage::Current->NotifyUser("Unable to update firmware: " + firmwareStatusMessage, NotifyType::ErrorMessage);
            }
        });
}

/// <summary>
/// Registers for the firmware update background task
/// </summary>
void FirmwareUpdate::RegisterForFirmwareUpdateBackgroundTask()
{
    // Create background task to do the firmware update
    auto backgroundTaskBuilder = ref new BackgroundTaskBuilder();

    backgroundTaskBuilder->Name = FirmwareUpdateTaskInformation::Name;
    backgroundTaskBuilder->TaskEntryPoint = FirmwareUpdateTaskInformation::TaskEntryPoint;
    backgroundTaskBuilder->SetTrigger(firmwareUpdateBackgroundTaskTrigger);
    firmwareUpdateBackgroundTaskRegistration = backgroundTaskBuilder->Register();

    // Make sure we're notified when the task completes or if there is an update
    firmwareUpdateBackgroundTaskRegistration->Completed += ref new BackgroundTaskCompletedEventHandler(this, &FirmwareUpdate::OnFirmwareUpdateCompleted);
    firmwareUpdateBackgroundTaskRegistration->Progress += ref new BackgroundTaskProgressEventHandler(this, &FirmwareUpdate::OnFirmwareUpdateProgress);
}

/// <summary>
/// Search all the existing background tasks for the firmware update task
/// </summary>
/// <returns>If found, the background task registration for the firmware update task; else, null.</returns>
BackgroundTaskRegistration^ FirmwareUpdate::FindFirmwareUpdateTask()
{
    auto backgroundTaskIter = BackgroundTaskRegistration::AllTasks->First();
    auto isBackgroundTask = backgroundTaskIter->HasCurrent;

    while (isBackgroundTask)
    {
        auto backgroundTask = backgroundTaskIter->Current->Value;

        if (backgroundTask->Name == FirmwareUpdateTaskInformation::Name)
        {
            return static_cast<BackgroundTaskRegistration^>(backgroundTask);
        }

        isBackgroundTask = backgroundTaskIter->MoveNext();
    }

    return nullptr;
}

void FirmwareUpdate::OnFirmwareUpdateCompleted(
    Windows::ApplicationModel::Background::BackgroundTaskRegistration^ /* sender */, 
    Windows::ApplicationModel::Background::BackgroundTaskCompletedEventArgs^ /* args */)
{
    MainPage::Current->Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler([this]()
        {    
            // Print our messages after the device is opened to avoid having our output overwritten by the EventHandlerForDevice
            auto taskCompleteStatus = safe_cast<String^>(ApplicationData::Current->LocalSettings->Values->Lookup(LocalSettingKeys::FirmwareUpdateBackgroundTask::TaskStatus));

            if (taskCompleteStatus == FirmwareUpdateTaskInformation::TaskCompleted)
            {
                // Display firmware version after the firmware update
                auto newFirmwareVersion = safe_cast<uint32>(ApplicationData::Current->LocalSettings->Values->Lookup(LocalSettingKeys::FirmwareUpdateBackgroundTask::NewFirmwareVersion));
                auto newFirmwareVersionHex = Utilities::ConvertToHex16Bit(newFirmwareVersion);

                UpdateNewFirmwareVersionInUI(newFirmwareVersionHex);

                MainPage::Current->NotifyUser("Firmware update completed", NotifyType::StatusMessage);
            }
            else if (taskCompleteStatus == FirmwareUpdateTaskInformation::TaskCanceled)
            {
                MainPage::Current->NotifyUser("Firmware update was canceled", NotifyType::StatusMessage);
            }

            // Remove all local setting values
            ApplicationData::Current->LocalSettings->Values->Clear();

            isUpdatingFirmware = false;

            UpdateButtonStates();
        }));

    if (firmwareUpdateBackgroundTaskRegistration != nullptr)
    {
        // Unregister the background task and let the remaining task finish until completion
        firmwareUpdateBackgroundTaskRegistration->Unregister(false);
    }
}

/// <summary>
/// Updates the UI with the progress of the firmware update
/// </summary>
/// <param name="sender"></param>
/// <param name="args"></param>
void FirmwareUpdate::OnFirmwareUpdateProgress(BackgroundTaskRegistration^ sender, BackgroundTaskProgressEventArgs^ args)
{
    MainPage::Current->Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler([this, args]()
        {    
            FirmwareUpdateProgressBar->Value = args->Progress;
        }));
}

void FirmwareUpdate::UpdateFirmwareOnFirstEnumeratedDevice_Click(Object^ sender, RoutedEventArgs^ e)
{
    // Lock the firmware update button to prevent the user from trying multiple times while we are still creating the background task.
    ButtonUpdateFirmwareOnFirstEnumeratedDevice->IsEnabled = false;

    // Clear the device versions in the UI in case there was something there before
    UpdateOldFirmwareVersionInUI("");
    UpdateNewFirmwareVersionInUI("");

    UpdateFirmwareForFirstEnumeratedDeviceAsync().then([this] ()
    {
        UpdateButtonStates();
    });
}

void FirmwareUpdate::CancelFirmwareUpdate_Click(Object^ sender, RoutedEventArgs^ e)
{
    CancelFirmwareUpdate();

    UpdateButtonStates();
}

void FirmwareUpdate::UpdateButtonStates()
{
    ButtonUpdateFirmwareOnFirstEnumeratedDevice->IsEnabled = !isUpdatingFirmware;
    ButtonCancelFirmwareUpdate->IsEnabled = !ButtonUpdateFirmwareOnFirstEnumeratedDevice->IsEnabled;
}

void FirmwareUpdate::UpdateOldFirmwareVersionInUI(String^ version)
{
    OutputDeviceVersionBefore->Text = version;
}

void FirmwareUpdate::UpdateNewFirmwareVersionInUI(String^ version)
{
    OutputDeviceVersionAfter->Text = version;
}