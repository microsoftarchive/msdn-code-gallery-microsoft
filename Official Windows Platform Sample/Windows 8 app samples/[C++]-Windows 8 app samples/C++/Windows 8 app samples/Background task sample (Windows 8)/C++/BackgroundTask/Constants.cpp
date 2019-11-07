//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::BackgroundTask;

String^ BackgroundTaskSample::SampleBackgroundTaskProgress = "";
bool BackgroundTaskSample::SampleBackgroundTaskRegistered = false;

String^ BackgroundTaskSample::SampleBackgroundTaskWithConditionProgress = "";
bool BackgroundTaskSample::SampleBackgroundTaskWithConditionRegistered = false;

String^ BackgroundTaskSample::ServicingCompleteTaskProgress = "";
bool BackgroundTaskSample::ServicingCompleteTaskRegistered = false;

String^ BackgroundTaskSample::TimeTriggeredTaskProgress = "";
bool BackgroundTaskSample::TimeTriggeredTaskRegistered = false;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    // The format here is the following:
    //     { "Description for the sample", "Fully quaified name for the class that implements the scenario" }
    { "Sample background task in C++", "SDKSample.BackgroundTask.SampleBackgroundTask" }, 
    { "Sample background task in C++ with a condition", "SDKSample.BackgroundTask.SampleBackgroundTaskWithCondition" },
    { "Servicing complete background task in C++", "SDKSample.BackgroundTask.ServicingCompleteTask" },
    { "Sample background task with time trigger", "SDKSample.BackgroundTask.TimeTriggeredTask" }
};

String^ BackgroundTaskSample::GetBackgroundTaskStatus(Platform::String^ name)
{
    auto registered = false;
    if (name == SampleBackgroundTaskName)
    {
        registered = BackgroundTaskSample::SampleBackgroundTaskRegistered;
    }
    else if (name == SampleBackgroundTaskWithConditionName)
    {
        registered = BackgroundTaskSample::SampleBackgroundTaskWithConditionRegistered;
    }
    else if (name == ServicingCompleteTaskName)
    {
        registered = BackgroundTaskSample::ServicingCompleteTaskRegistered;
    }
    else if (name == TimeTriggeredTaskName)
    {
        registered = BackgroundTaskSample::TimeTriggeredTaskRegistered;
    }

    String^ status = registered ? "Registered" : "Unregistered";

    auto settings = ApplicationData::Current->LocalSettings->Values;
    if (settings->HasKey(name))
    {
        status += " - " + dynamic_cast<String^>(settings->Lookup(name));
    }

    return status;
}

//
// Update the UI state for all registered background tasks.
//
void BackgroundTaskSample::InitializeBackgroundTaskStatus()
{
    auto iter = BackgroundTaskRegistration::AllTasks->First();
    auto hascur = iter->HasCurrent;
    while (hascur)
    {
        auto cur = iter->Current->Value;
        UpdateBackgroundTaskStatus(cur->Name, true);
        hascur = iter->MoveNext();
    }
}

BackgroundTaskRegistration^ BackgroundTaskSample::RegisterBackgroundTask(String^ taskEntryPoint, String^ name, IBackgroundTrigger^ trigger, IBackgroundCondition^ condition)
{
    auto builder = ref new BackgroundTaskBuilder();

    builder->Name = name;
    builder->TaskEntryPoint = taskEntryPoint;
    builder->SetTrigger(trigger);

    if (condition != nullptr)
    {
        builder->AddCondition(condition);
    }

    auto task = builder->Register();

    UpdateBackgroundTaskStatus(name, true);

    //
    // Remove previous completion status from local settings.
    //
    auto settings = ApplicationData::Current->LocalSettings->Values;
    settings->Remove(name);

    return task;
}

//
// Registers a background task for the servicing-complete system event. This event occurs when the application is updated.
//
void BackgroundTaskSample::RegisterServicingCompleteBackgroundTask()
{
    //
    // Check whether the servicing-complete background task is already registered.
    //
    auto iter = BackgroundTaskRegistration::AllTasks->First();
    auto hascur = iter->HasCurrent;
    while (hascur)
    {
        auto cur = iter->Current->Value;

        if (cur->Name == ServicingCompleteTaskName)
        {
            //
            // The task is already registered.
            //
            return;
        }

        hascur = iter->MoveNext();
    }

    //
    // The servicing-complete background task is not already registered.
    //
    RegisterBackgroundTask(ServicingCompleteTaskEntryPoint,
                           ServicingCompleteTaskName,
                           ref new SystemTrigger(SystemTriggerType::ServicingComplete, false),
                           nullptr);
}

//
// Unregister background tasks with specified name.
//
void BackgroundTaskSample::UnregisterBackgroundTasks(String^ name)
{
    //
    // Loop through all background tasks and unregister any that have a name that matches
    // the name passed into this function.
    //
    auto iter = BackgroundTaskRegistration::AllTasks->First();
    auto hascur = iter->HasCurrent;
    while (hascur)
    {
        auto cur = iter->Current->Value;

        if(cur->Name == name)
        {
            cur->Unregister(true);
            UpdateBackgroundTaskStatus(name, false);
        }

        hascur = iter->MoveNext();
    }
}

void BackgroundTaskSample::UpdateBackgroundTaskStatus(Platform::String^ name, bool registered)
{
    if (name == SampleBackgroundTaskName)
    {
        BackgroundTaskSample::SampleBackgroundTaskRegistered = registered;
    }
    else if (name == SampleBackgroundTaskWithConditionName)
    {
        BackgroundTaskSample::SampleBackgroundTaskWithConditionRegistered = registered;
    }
    else if (name == ServicingCompleteTaskName)
    {
        BackgroundTaskSample::ServicingCompleteTaskRegistered = registered;
    }
    else if (name == TimeTriggeredTaskName)
    {
        BackgroundTaskSample::TimeTriggeredTaskRegistered = registered;
    }
}

