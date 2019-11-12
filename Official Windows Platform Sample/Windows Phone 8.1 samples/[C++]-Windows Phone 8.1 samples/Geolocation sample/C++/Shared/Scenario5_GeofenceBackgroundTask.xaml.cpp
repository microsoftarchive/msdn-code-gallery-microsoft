//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// Scenario5_BackgroundTask.xaml.cpp
// Implementation of the Scenario5 class
//

#include "pch.h"
#include "Scenario5_GeofenceBackgroundTask.xaml.h"

using namespace SDKSample;
using namespace SDKSample::GeolocationCPP;

using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Data::Json;
using namespace Windows::Devices::Geolocation;
using namespace Windows::Storage;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml::Navigation;

const int oneHundredNanosecondsPerSecond = 10000000;  // conversion from 100 nano-second resolution to seconds

Scenario5::Scenario5() : rootPage(MainPage::Current), sampleBackgroundTaskName("SampleGeofenceBackgroundTask"), sampleBackgroundTaskEntryPoint("BackgroundTask.GeofenceBackgroundTask")
{
    InitializeComponent();

#if (WINAPI_FAMILY == WINAPI_FAMILY_PC_APP)
    GeofenceBackgroundEventsListBox->Width = 480;
    GeofenceBackgroundEventsListBox->Height = 240;
#endif

    geolocator = ref new Geolocator();

    geofenceBackgroundEvents = ref new Platform::Collections::Vector<Platform::String^>();

    // using data binding to the root page collection of GeofenceItems associated with events
    GeofenceBackgroundEventsListBox->DataContext = geofenceBackgroundEvents;

    FillEventListBoxWithExistingEvents();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario5::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Look for an already registered background task
    geofenceTask = nullptr;

    auto iter = BackgroundTaskRegistration::AllTasks->First();
    while (iter->HasCurrent)
    {
        auto task = iter->Current;
        if (task->Value->Name == sampleBackgroundTaskName)
        {
            geofenceTask = safe_cast<BackgroundTaskRegistration^>(task->Value);
            break;
        }
        iter->MoveNext();
    }

    if (geofenceTask != nullptr)
    {
        FillEventListBoxWithExistingEvents();

        // Register for background task completion notifications
        taskCompletedToken = geofenceTask->Completed::add(ref new BackgroundTaskCompletedEventHandler(this, &Scenario5::OnCompleted));

        try
        {
            // Check the background access status of the application and display the appropriate status message
            switch (BackgroundExecutionManager::GetAccessStatus())
            {
            case BackgroundAccessStatus::Unspecified:
            case BackgroundAccessStatus::Denied:
#if (WINAPI_FAMILY == WINAPI_FAMILY_PC_APP)
                rootPage->NotifyUser("This application must be added to the lock screen before the background task will run.", NotifyType::ErrorMessage);
#else
                rootPage->NotifyUser("Not able to run in background.", NotifyType::ErrorMessage);
#endif
                break;

            default:
                rootPage->NotifyUser("Background task is already registered. Waiting for next update...", NotifyType::StatusMessage);
                break;
            }
        }
        catch (Exception^ ex)
        {
#if (WINAPI_FAMILY == WINAPI_FAMILY_PC_APP)
            if (ex->HResult == HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED))
            {
                rootPage->NotifyUser("Location Simulator not supported.  Could not determine lock screen status, be sure that the application is added to the lock screen.", NotifyType::StatusMessage);
            }
            else
#endif
            {
                rootPage->NotifyUser(ex->ToString(), NotifyType::ErrorMessage);
            }
        }

        UpdateButtonStates(/*registered:*/ true);
    }
    else
    {
        UpdateButtonStates(/*registered:*/ false);
    }
}

/// <summary>
/// Invoked when this page is no longer displayed.
/// </summary>
/// <param name="e"></param>
void Scenario5::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // Just in case the original GetGeopositionAsync call is still active
    geopositionTaskTokenSource.cancel();
}

void Scenario5::RegisterBackgroundTask(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    try
    {
        // Get permission for a background task from the user. If the user has already answered once,
        // this does nothing and the user must manually update their preference via PC Settings.
        task<BackgroundAccessStatus> requestAccessTask(BackgroundExecutionManager::RequestAccessAsync());
        requestAccessTask.then([this](BackgroundAccessStatus backgroundAccessStatus)
        {
            // Regardless of the answer, register the background task. If the user later adds this application
            // to the lock screen, the background task will be ready to run.

            // Create a new background task builder
            BackgroundTaskBuilder^ geofenceTaskBuilder = ref new BackgroundTaskBuilder();

            geofenceTaskBuilder->Name = sampleBackgroundTaskName;
            geofenceTaskBuilder->TaskEntryPoint = sampleBackgroundTaskEntryPoint;

            // Create a new location trigger
            auto trigger = ref new LocationTrigger(LocationTriggerType::Geofence);

            // Associate the location trigger with the background task builder
            geofenceTaskBuilder->SetTrigger(trigger);

            // If it is important that there is user presence and/or
            // internet connection when OnCompleted is called
            // the following could be called before calling Register()
            // SystemCondition^ condition = ref new SystemCondition(SystemConditionType::UserPresent | SystemConditionType::InternetAvailable);
            // geofenceTaskBuilder->AddCondition(condition);

            // Register the background task
            geofenceTask = geofenceTaskBuilder->Register();

            // Register for background task completion notifications
            taskCompletedToken = geofenceTask->Completed::add(ref new BackgroundTaskCompletedEventHandler(this, &Scenario5::OnCompleted));

            UpdateButtonStates(/*registered:*/ true);

            // Check the background access status of the application and display the appropriate status message
            switch (backgroundAccessStatus)
            {
            case BackgroundAccessStatus::Unspecified:
            case BackgroundAccessStatus::Denied:
#if (WINAPI_FAMILY == WINAPI_FAMILY_PC_APP)
                rootPage->NotifyUser("This application must be added to the lock screen before the background task will run.", NotifyType::ErrorMessage);
#else
                rootPage->NotifyUser("Not able to run in background.", NotifyType::ErrorMessage);
#endif
                break;

            default:
                GetGeopositionAsync();
                break;
            }
        });
    }
    catch (Exception^ ex)
    {
#if (WINAPI_FAMILY == WINAPI_FAMILY_PC_APP)
        if (ex->HResult == HRESULT_FROM_WIN32(ERROR_NOT_SUPPORTED))
        {
            rootPage->NotifyUser("Location Simulator not supported.  Could not get permission to add application to the lock screen, this application must be added to the lock screen before the background task will run.", NotifyType::StatusMessage);
        }
        else
#endif
        {
            rootPage->NotifyUser(ex->ToString(), NotifyType::ErrorMessage);
        }

        UpdateButtonStates(/*registered:*/ false);
    }
}

void Scenario5::UnregisterBackgroundTask(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Unregister the background task
    if (geofenceTask != nullptr)
    {
        geofenceTask->Unregister(true);
        geofenceTask = nullptr;
    }

    rootPage->NotifyUser("Background task unregistered", NotifyType::StatusMessage);

    UpdateButtonStates(/*registered:*/ false);
}

void Scenario5::OnCompleted(BackgroundTaskRegistration^ task, Windows::ApplicationModel::Background::BackgroundTaskCompletedEventArgs^ args)
{
    // Update the UI with progress reported by the background task
    // We need to dispatch to the UI thread to display the output
    Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler(
            [this, args]()
            {
                try
                {
                    // Throw an exception if the background task had an unrecoverable error
                    args->CheckResult();

                    // Update the UI with the completion status of the background task
                    auto settings = ApplicationData::Current->LocalSettings->Values;
                    if (settings->HasKey("Status"))
                    {
                        rootPage->NotifyUser(safe_cast<String^>(settings->Lookup("Status")), NotifyType::StatusMessage);
                    }

                    // add background events to listbox
                    FillEventListBoxWithExistingEvents();
                }
                catch (Exception^ ex)
                {
                    // The background task had an error
                    rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
                }
            },
            CallbackContext::Any
            )
        );
}

void Scenario5::GetGeopositionAsync()
{
    rootPage->NotifyUser("Checking permissions...", NotifyType::StatusMessage);

    task<Geoposition^> geopositionTask(geolocator->GetGeopositionAsync(), geopositionTaskTokenSource.get_token());
    geopositionTask.then([this](task<Geoposition^> getPosTask)
    {
        try
        {
            // Get will throw an exception if the task was canceled or failed with an error
            Geoposition^ pos = getPosTask.get();

            // clear status
            rootPage->NotifyUser("", NotifyType::StatusMessage);
        }
#if (WINAPI_FAMILY == WINAPI_FAMILY_PC_APP)
        catch (AccessDeniedException^)
        {
            rootPage->NotifyUser("Location has been disabled by the user. Enable access through the settings charm.", NotifyType::ErrorMessage);
        }
#endif
        catch (task_canceled&)
        {
            rootPage->NotifyUser("Operation canceled", NotifyType::StatusMessage);
        }
        catch (Exception^ ex)
        {
#if (WINAPI_FAMILY == WINAPI_FAMILY_PC_APP)
            // If there are no location sensors GetGeopositionAsync()
            // will timeout -- that is acceptable.

            if (ex->HResult == HRESULT_FROM_WIN32(WAIT_TIMEOUT))
            {
                rootPage->NotifyUser("Operation accessing location sensors timed out. Possibly there are no location sensors.", NotifyType::StatusMessage);
            }
            else
#endif
            {
                rootPage->NotifyUser(ex->ToString(), NotifyType::ErrorMessage);
            }
        }
    });
}

void Scenario5::FillEventListBoxWithExistingEvents()
{
    auto settings = ApplicationData::Current->LocalSettings->Values;
    if (settings->HasKey("BackgroundGeofenceEventCollection"))
    {
        String^ geofenceEvent = safe_cast<String^>(settings->Lookup("BackgroundGeofenceEventCollection"));

        if (0 != geofenceEvent->Length())
        {
            // remove all entries and repopulate
            geofenceBackgroundEvents->Clear();

            auto events = JsonValue::Parse(geofenceEvent)->GetArray();

            // NOTE: the events are accessed in reverse order
            // because the events were added to JSON from
            // newer to older.  geofenceBackgroundEvents->InsertAt(0,...) adds
            // each new entry to the beginning of the collection.
            for (int pos = events->Size - 1; pos >= 0; pos--)
            {
                auto element = events->GetAt(pos)->GetString();
                geofenceBackgroundEvents->InsertAt(0, element);
            }
        }
    }
}

/// <summary>
/// Update the enable state of the register/unregister buttons.
/// </summary>
void Scenario5::UpdateButtonStates(bool registered)
{
    auto uiDelegate = [this, registered]()
    {
        RegisterBackgroundTaskButton->IsEnabled = !registered;
        UnregisterBackgroundTaskButton->IsEnabled = registered;
    };
    auto handler = ref new Windows::UI::Core::DispatchedHandler(uiDelegate, Platform::CallbackContext::Any);

    Dispatcher->RunAsync(CoreDispatcherPriority::Normal, handler);
}
