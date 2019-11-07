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

bool BackgroundTaskSample::SampleBackgroundTaskWithConditionRegistered = false;


Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    // The format here is the following:
    //     { "Description for the sample", "Fully quaified name for the class that implements the scenario" } 
    { "Network Status Change with Internet Present", "SDKSample.NetworkStatusApp.NetworkStatusWithInternetPresent" }
};

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

    return task;
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
    if (name == SampleBackgroundTaskWithConditionName)
    {
        BackgroundTaskSample::SampleBackgroundTaskWithConditionRegistered = registered;
    }
}

