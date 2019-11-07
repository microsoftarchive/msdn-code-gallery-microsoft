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
// BackgroundTask.xaml.cpp
// Implementation of the BackgroundTask class
//

#include "pch.h"
#include "BackgroundTask.xaml.h"

using namespace SDKSample::SmsBackgroundTaskSample;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Networking::NetworkOperators;
using namespace Windows::UI::Core;
using namespace Windows::Storage;
using namespace Windows::Devices::Sms;
using namespace concurrency;

BackgroundTask::BackgroundTask():
    backgroundTaskEntryPoint("SmsBackgroundTask.SampleSmsBackgroundTask"),
    backgroundTaskName("SampleSmsBackgroundTask"),
    hasDeviceAccess(false)
{
    InitializeComponent();

    // Get dispatcher for dispatching updates to the UI thread.
    sampleDispatcher = Window::Current->Dispatcher;

    try
    {
        // Initialize state-based registration of currently registered background tasks.
        InitializeRegisteredSmsBackgroundTasks();
    }
    catch (Platform::Exception^ ex)
    {
        rootPage->NotifyUser(ex->ToString(), NotifyType::ErrorMessage);
    }
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void BackgroundTask::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

//Handle request to register the background task
void SDKSample::SmsBackgroundTaskSample::BackgroundTask::RegisterBackgroundTask_Click(Platform::Object^ sender, RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        // SMS is a sensitive capability and the user may be prompted for consent. If the app
        // does not obtain permission for the package to have access to SMS before the background
        // work item is run (perhaps after the app is suspended or terminated), the background
        // work item will not have access to SMS and will have no way to prompt the user for consent
        // without an active window. Here, any available SMS device is activated in order to ensure
        // consent. Your app will likely do something with the SMS device as part of its features.

        if (!hasDeviceAccess)
        {
            create_task(SmsDevice::GetDefaultAsync()).then([this] (SmsDevice^ getSmsDevice){
                rootPage->NotifyUser("Successfully connnected to SMS device with account number: " + getSmsDevice->AccountPhoneNumber, NotifyType::StatusMessage);
                hasDeviceAccess = true;
            }).then([this] (task<void> catcherrors) {
                try
                {
                    catcherrors.get();
                }
                catch (Platform::Exception^ ex)
                {
                    rootPage->NotifyUser("Failed to find SMS device\n" + ex->Message, NotifyType::ErrorMessage);
                    cancel_current_task();
                }

            }).then([this] ()
            {
                // Create a new background task builder.
                BackgroundTaskBuilder^ taskBuilder = ref new ::BackgroundTaskBuilder();

                // Create a new SmsReceived trigger.
                SystemTrigger^ trigger = ref new SystemTrigger(SystemTriggerType::SmsReceived, false);

                // Associate the SmsReceived trigger with the background task builder.
                taskBuilder->SetTrigger(trigger);

                // Specify the background task to run when the trigger fires.
                taskBuilder->TaskEntryPoint = backgroundTaskEntryPoint;

                // Name the background task.
                taskBuilder->Name = backgroundTaskName;

                // Register the background task.
                BackgroundTaskRegistration^ taskRegistration = taskBuilder->Register();

                // Associate completed event handler with the new background task.
                taskRegistration->Completed += ref new BackgroundTaskCompletedEventHandler(this, &BackgroundTask::OnCompleted);

                UpdateBackgroundTaskUIState(true);
                rootPage->NotifyUser("Registered SMS background task", NotifyType::StatusMessage);
            }).then([this] (task<void> catcherrors) {
                try
                {
                    catcherrors.get();
                }
                catch (Platform::Exception^ ex)
                {
                    rootPage->NotifyUser(ex->Message, NotifyType::ErrorMessage);
                }

            });
        }

    }
}

// Handle request to unregister the background task
void SDKSample::SmsBackgroundTaskSample::BackgroundTask::UnregisterBackgroundTask_Click(Platform::Object^ sender, RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        // Loop through all background tasks and unregister our background task
        auto iter = BackgroundTaskRegistration::AllTasks->First();
        auto hascur = iter->HasCurrent;
        while (hascur)
        {
            auto cur = iter->Current;
            if (cur->Value->Name == backgroundTaskName)
            {
                cur->Value->Unregister(true);
            }
            hascur = iter->MoveNext();
        }

        UpdateBackgroundTaskUIState(false);
        rootPage->NotifyUser("Unregistered SMS background task.", NotifyType::StatusMessage);
    }
}

// Update the registration status text and the button states based on the background task's
// registration state.
void BackgroundTask::UpdateBackgroundTaskUIState(bool Registered)
{
    if (Registered)
    {
        BackgroundTaskStatus->Text = "Registered";
        RegisterBackgroundTaskButton->IsEnabled = false;
        UnregisterBackgroundTaskButton->IsEnabled = true;
    }
    else
    {
        BackgroundTaskStatus->Text = "Unregistered";
        RegisterBackgroundTaskButton->IsEnabled = true;
        UnregisterBackgroundTaskButton->IsEnabled = false;
    }
}

// Initialize state based on currently registered background tasks
void BackgroundTask::InitializeRegisteredSmsBackgroundTasks()
{
    try
    {
        //
        // Initialize UI elements based on currently registered background tasks
        // and associate background task completed event handler with each background task.
        //
        UpdateBackgroundTaskUIState(false);

        auto iter = BackgroundTaskRegistration::AllTasks->First();
        auto hascur = iter->HasCurrent;
        while (hascur)
        {
            auto cur = iter->Current;
            IBackgroundTaskRegistration^ task = cur->Value;
            if (task->Name == backgroundTaskName)
            {
                UpdateBackgroundTaskUIState(true);
                task->Completed += ref new BackgroundTaskCompletedEventHandler(this, &BackgroundTask::OnCompleted);
            }
            hascur = iter->MoveNext();
        }

    }
    catch (Platform::Exception^ ex)
    {
        rootPage->NotifyUser(ex->ToString(), NotifyType::ErrorMessage);
    }
}

// Handle background task completion event.
void BackgroundTask::OnCompleted(BackgroundTaskRegistration^ sender, BackgroundTaskCompletedEventArgs^ e)
{
    // Update the UI with the complrtion status reported by the background task.
    // Dispatch an anonymous task to the UI thread to do the update.
    sampleDispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, sender, e] ()
    {
        try
        {
            if ((sender != nullptr) && (e != nullptr))
            {
                //Check completion status
                e->CheckResult();

                // Update the UI with the background task's completion status.
                // The task stores status in the application data settings indexed by the task ID.
                auto key = sender->TaskId.ToString();
                auto settings = ApplicationData::Current->LocalSettings;
                BackgroundTaskStatus->Text = settings->Values->Lookup(key)->ToString();
            }
        }
        catch (Platform::Exception^ ex)
        {
            rootPage->NotifyUser(ex->ToString(), NotifyType::ErrorMessage);
        }
    }));
}
