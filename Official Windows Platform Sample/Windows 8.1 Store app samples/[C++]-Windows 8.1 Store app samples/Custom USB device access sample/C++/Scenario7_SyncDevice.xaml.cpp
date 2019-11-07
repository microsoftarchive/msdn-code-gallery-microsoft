//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// SyncDevice.xaml.cpp
// Implementation of the SyncDevice class
//

#include "pch.h"
#include "Scenario7_SyncDevice.xaml.h"
#include "MainPage.xaml.h"

using namespace Concurrency;
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Storage;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace SDKSample;
using namespace SDKSample::CustomUsbDeviceAccess;

SyncDevice::SyncDevice(void) :
    isSyncing(false),
    syncDeviceInformation(nullptr),
    syncDeviceSelector(nullptr)
{
    // Save trigger so that we may start the background task later
    // Only one instance of the trigger can exist at a time. Since the trigger does not implement
    // IDisposable, it may still be in memory when a new trigger is created.
    syncBackgroundTaskTrigger = ref new DeviceUseTrigger();

    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
///
/// We will enable/disable parts of the UI if the device doesn't support it.
/// </summary>
/// <param name="eventArgs">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void SyncDevice::OnNavigatedTo(NavigationEventArgs^ /* eventArgs */)
{
    // Both the OSRFX2 and the SuperMutt use the same scenario
    // If no devices are connected, none of the scenarios will be shown and an error will be displayed
    Map<DeviceType, UIElement^>^ deviceScenarios = ref new Map<DeviceType, UIElement^>();
    deviceScenarios->Insert(DeviceType::OsrFx2, GeneralScenario);
    deviceScenarios->Insert(DeviceType::SuperMutt, GeneralScenario);

    Utilities::SetUpDeviceScenarios(deviceScenarios, DeviceScenarioContainer);
    
    // Unregister any existing tasks that persisted; there should be none unless the app closed/crashed
    auto existingSyncTask = FindSyncTask();
    if (existingSyncTask != nullptr)
    {
        existingSyncTask->Unregister(true);
    }

    isSyncing = false;

    UpdateButtonStates();
}

/// <summary>
/// Cancel the background task so the device can be reopened
/// </summary>
/// <param name="eventArgs"></param>
void SyncDevice::OnNavigatedFrom(NavigationEventArgs^ /* eventArgs */) 
{
    // Unregister events when we leave this page so the cancel event doesn't call a callback method that 
    // doesn't exist (we left this page)
    if (isSyncing)
    {
        backgroundSyncTaskRegistration->Completed -= syncCompletedEventToken;
        syncCompletedEventToken.Value = 0;

        backgroundSyncTaskRegistration->Progress -= syncProgressEventToken;
        syncProgressEventToken.Value = 0;
    }

    CancelSyncWithDevice();
}

/// <summary>
/// Search all the existing background tasks for the sync task
/// </summary>
/// <returns>If found, the background task registration for the sync task; else, null.</returns>
BackgroundTaskRegistration^ SyncDevice::FindSyncTask()
{
    auto backgroundTaskIter = BackgroundTaskRegistration::AllTasks->First();
    auto isBackgroundTask = backgroundTaskIter->HasCurrent;

    while (isBackgroundTask)
    {
        auto backgroundTask = backgroundTaskIter->Current->Value;

        if (backgroundTask->Name == SyncBackgroundTaskInformation::Name)
        {
            return static_cast<BackgroundTaskRegistration^>(backgroundTask);
        }

        isBackgroundTask = backgroundTaskIter->MoveNext();
    }

    return nullptr;
}

void SyncDevice::CancelSyncWithDevice(void)
{
    if (isSyncing)
    {
        backgroundSyncTaskRegistration->Unregister(true);

        backgroundSyncTaskRegistration = nullptr;
   
        // We are canceling the task, so we are no longer syncing. If the task is registered but never run,
        // the cancel completion is never called
        isSyncing = false;
    }
}

/// <summary>
/// Registers for background task that will write to the device.
/// </summary>
void SyncDevice::SetupBackgroundTask(void)
{
    // Create background task to write
    auto backgroundTaskBuilder = ref new BackgroundTaskBuilder();

    backgroundTaskBuilder->Name = const_cast<String^>(SyncBackgroundTaskInformation::Name);
    backgroundTaskBuilder->TaskEntryPoint = const_cast<String^>(SyncBackgroundTaskInformation::TaskEntryPoint);
    backgroundTaskBuilder->SetTrigger(syncBackgroundTaskTrigger);
    backgroundSyncTaskRegistration = backgroundTaskBuilder->Register();

    // Make sure we're notified when the task completes or if there is an update
    syncCompletedEventToken = backgroundSyncTaskRegistration->Completed += ref new BackgroundTaskCompletedEventHandler(this, &SyncDevice::OnSyncWithDeviceCompleted);
    syncProgressEventToken = backgroundSyncTaskRegistration->Progress += ref new BackgroundTaskProgressEventHandler(this, &SyncDevice::OnSyncWithDeviceProgress);
}

/// <summary>
/// Starts the background task that syncs with the device
///
/// The trigger->RequestAsync() must be started on the UI thread because of the prompt that appears. The caller of StartSyncBackgroundTaskAsync()
/// is responsible for running this method in the UI thread.
/// </summary>
/// <returns></returns>
task<DeviceTriggerResult> SyncDevice::StartSyncBackgroundTaskAsync(void)
{
    // Save the current device information so that we can reopen it after syncing
    syncDeviceInformation = EventHandlerForDevice::Current->DeviceInformation;
    syncDeviceSelector = EventHandlerForDevice::Current->DeviceSelector;

    // Allow the background task to open the device and sync with it.
    // We must close the device because Background tasks need to create new handle to the device and cannot reuse the one from this app.
    // Since the device is opened exclusively, the app must close the device before the background task can use the device.
    EventHandlerForDevice::Current->CloseDevice();

    return create_task(syncBackgroundTaskTrigger->RequestAsync(syncDeviceInformation->Id));
}

/// <summary>
/// Starts the background task that will sync with the device.
/// </summary>
/// <returns></returns>
task<void> SyncDevice::SyncWithDeviceAsync(void)
{
    SetupBackgroundTask();

    return create_task(StartSyncBackgroundTaskAsync()).then([this] (DeviceTriggerResult deviceTriggerResult)
    {
        // Determine if we are allowed to sync
        String^ errorMessage = nullptr;

        switch (deviceTriggerResult)
        {
        case DeviceTriggerResult::Allowed:
            isSyncing = true;
            break;

        case DeviceTriggerResult::LowBattery:
            isSyncing = false;
            errorMessage = "Insufficient battery to start sync";
            break;

        case DeviceTriggerResult::DeniedByUser:
            isSyncing = false;
            errorMessage = "User declined the operation";
            break;

        case DeviceTriggerResult::DeniedBySystem:
            isSyncing = false;
            errorMessage = "Sync operation was denied by the system";
            break;

        default:
            isSyncing = false;
            errorMessage = "Failed to initiate sync";
            break;
        }

        // Reopen device and print error message
        if (!isSyncing && errorMessage != nullptr)
        {
            // Reopen the device once the background task is completed
            create_task(EventHandlerForDevice::Current->OpenDeviceAsync(syncDeviceInformation, syncDeviceSelector))
                .then([this, errorMessage](task<bool> openDeviceTask)
            {
                syncDeviceInformation = nullptr;
                syncDeviceSelector = nullptr;

                // Print this error message after the "device was opened" message
                MainPage::Current->NotifyUser("Unable to sync: " + errorMessage, NotifyType::ErrorMessage);
            });
        }
    });
}

/// <summary>
/// Reopen the device after the background task is done syncing. Notify the UI of how many bytes we wrote to the device.
/// </summary>
/// <param name="sender"></param>
/// <param name="args"></param>
void SyncDevice::OnSyncWithDeviceCompleted(BackgroundTaskRegistration^ /* sender */, BackgroundTaskCompletedEventArgs^ /* args */)
{
    MainPage::Current->Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler([this]()
        {    
            // Reopen the device once the background task is completed
            create_task(EventHandlerForDevice::Current->OpenDeviceAsync(syncDeviceInformation, syncDeviceSelector)).then([this](task<bool> openDeviceTask)
            {
                syncDeviceInformation = nullptr;
                syncDeviceSelector = nullptr;

                // Print our messages after the device is opened to avoid having our output overwritten by the EventHandlerForDevice
                auto taskCompleteStatus = safe_cast<String^>(ApplicationData::Current->LocalSettings->Values->Lookup(LocalSettingKeys::SyncBackgroundTaskStatus));

                if (taskCompleteStatus == SyncBackgroundTaskInformation::TaskCompleted)
                {
                    String^ totalBytesWritten = safe_cast<String^>(ApplicationData::Current->LocalSettings->Values->Lookup(LocalSettingKeys::SyncBackgroundTaskResult));

                    MainPage::Current->NotifyUser("Sync: Wrote " + totalBytesWritten + " bytes to the device", NotifyType::StatusMessage);
                }
                else if (taskCompleteStatus == SyncBackgroundTaskInformation::TaskCanceled)
                {
                    MainPage::Current->NotifyUser("Syncing was canceled", NotifyType::StatusMessage);
                }

                // Remove all local setting values
                ApplicationData::Current->LocalSettings->Values->Clear();

                isSyncing = false;

                UpdateButtonStates();
            });
        }));

    // Unregister the background task and let the remaining task finish until completion
    if (backgroundSyncTaskRegistration != nullptr)
    {
        backgroundSyncTaskRegistration->Unregister(false);
    }
}

/// <summary>
/// Updates the UI with the progress of the sync
/// </summary>
/// <param name="sender"></param>
/// <param name="args"></param>
void SyncDevice::OnSyncWithDeviceProgress(BackgroundTaskRegistration^ /* sender */, BackgroundTaskProgressEventArgs^ args)
{
    MainPage::Current->Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler([this, args]()
        {    
            SyncProgressBar->Value = args->Progress;
        }));
}

void SyncDevice::Sync_Click(Platform::Object^ /* sender */, Windows::UI::Xaml::RoutedEventArgs^ /* e */)
{
    if (EventHandlerForDevice::Current->IsDeviceConnected)
    {
        MainPage::Current->NotifyUser("Syncing in the background...", NotifyType::StatusMessage);

        // Reset progress bar 
        SyncProgressBar->Value = 0;

        SyncWithDeviceAsync().then([this]()
        {
            UpdateButtonStates();
        });
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

void SyncDevice::CancelSync_Click(Platform::Object^ /* sender */, Windows::UI::Xaml::RoutedEventArgs^ /* e */)
{
    if (isSyncing)
    {
        CancelSyncWithDevice();

        UpdateButtonStates();
    }
    else
    {
        Utilities::NotifyDeviceNotConnected();
    }
}

void SyncDevice::UpdateButtonStates(void)
{
    ButtonSync->IsEnabled = !isSyncing;
    ButtonCancelSync->IsEnabled = isSyncing;
}
