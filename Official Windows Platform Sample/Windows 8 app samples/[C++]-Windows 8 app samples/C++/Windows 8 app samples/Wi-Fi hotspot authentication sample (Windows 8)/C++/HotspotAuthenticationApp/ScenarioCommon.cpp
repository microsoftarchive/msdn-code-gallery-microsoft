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
// ScenarioCommon.cpp
// Implementation of class shared by all scenarios
//

#include "pch.h"
#include "ScenarioCommon.h"

using namespace SDKSample::HotspotAuthenticationApp;
using namespace HotspotAuthenticationTask;
using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;

ScenarioCommon^ ScenarioCommon::scenarioCommonSingleton;

// This is used by the UI thread and does not need to be thread safe
ScenarioCommon^ ScenarioCommon::Instance::get()
{
    if(scenarioCommonSingleton == nullptr)
    {
        scenarioCommonSingleton = ref new ScenarioCommon();
    }
    return scenarioCommonSingleton;
}

// Default constructor
ScenarioCommon::ScenarioCommon()
{
    rootPage = MainPage::Current;
    coreDispatcher = Window::Current->CoreWindow->Dispatcher;
    backgroundTaskEntryPoint = "HotspotAuthenticationTask.AuthenticationTask";
    backgroundTaskName = "AuthenticationBackgroundTask";
    hasRegisteredBackgroundTaskHandler = false;
}

String^ ScenarioCommon::BackgroundTaskEntryPoint::get()
{
    return backgroundTaskEntryPoint;
}

String^ ScenarioCommon::BackgroundTaskName::get()
{
    return backgroundTaskName;
}

// Register completion handler for registered background task.
// Returns true if task is registered, false otherwise.
bool ScenarioCommon::RegisteredCompletionHandlerForBackgroundTask()
{
    if (!hasRegisteredBackgroundTaskHandler)
    {
        try
        {
            // Associate background task completed event handler with background task.
            auto iter = BackgroundTaskRegistration::AllTasks->First();
            while(iter->HasCurrent)
            {
                auto task = iter->Current;
                if (task->Value->Name == backgroundTaskName)
                {
                    task->Value->Completed += ref new BackgroundTaskCompletedEventHandler(this, &ScenarioCommon::OnBackgroundTaskCompleted);
                    hasRegisteredBackgroundTaskHandler = true;
                    break;
                }
                iter->MoveNext();
            }
        }
        catch (Exception^ ex)
        {
            rootPage->NotifyUser(ex->ToString(), NotifyType::StatusMessage);
        }
    }
    return hasRegisteredBackgroundTaskHandler;
}

// Background task completion handler. When authenticating through the foreground app, this triggers the
// authentication flow if the app is currently running.
void ScenarioCommon::OnBackgroundTaskCompleted(BackgroundTaskRegistration^ task, BackgroundTaskCompletedEventArgs^ args)
{
    if ((task != nullptr) && (args != nullptr))
    {
        if (task->Name == backgroundTaskName)
        {
            // The event handler may be invoked on a background thread, so use the Dispatcher to invoke the UI-related code on the UI thread.
            auto callback = ref new DispatchedHandler(
                [=]()
            {
                try
                {
                    // If the background task threw an exception, display the exception in the error text box.
                    args->CheckResult();

                    // Update the UI with the completion status of the background task
                    // The Run method of the background task sets this status.
                    rootPage->NotifyUser("Background task completed", NotifyType::StatusMessage);
                }
                catch (Exception^ ex)
                {
                    rootPage->NotifyUser(ex->ToString(), NotifyType::ErrorMessage);
                }
            }, CallbackContext::Any);
            coreDispatcher->RunAsync(CoreDispatcherPriority::Normal, callback);

            // Signal event for foreground authentication
            if (!ConfigStore::AuthenticateThroughBackgroundTask && foregroundAuthenticationCallback != nullptr)
            {
                coreDispatcher->RunAsync(CoreDispatcherPriority::Normal, foregroundAuthenticationCallback);
            }
        }
    }
}

void ScenarioCommon::SetForegroundAuthenticationEventHandler(Windows::UI::Core::DispatchedHandler^ handler)
{
    foregroundAuthenticationCallback = handler;
}
