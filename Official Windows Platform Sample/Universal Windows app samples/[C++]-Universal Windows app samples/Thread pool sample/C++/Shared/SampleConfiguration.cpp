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
#include "SampleConfiguration.h"
#include "Scenario1_DelayTimer.xaml.h"

using namespace SDKSample;
using namespace SDKSample::ThreadPool;

Platform::String^ ThreadPoolSample::GetStatusString(SDKSample::ThreadPool::Status status)
{
    switch (status)
    {
    case SDKSample::ThreadPool::Status::Unregistered:
        return "Unregistered";
        break;
    case SDKSample::ThreadPool::Status::Started:
        return "Started";
        break;
    case SDKSample::ThreadPool::Status::Canceled:
        return "Canceled";
        break;
    case SDKSample::ThreadPool::Status::Completed:
        return "Completed";
        break;
    default:
        return "Error";
        break;
    }
}

Windows::Foundation::TimeSpan ThreadPoolSample::MillisecondsToTimeSpan(long milliseconds)
{
    Windows::Foundation::TimeSpan time;
    time.Duration = milliseconds * 10000;
    return time;
}

void ThreadPoolSample::ParseTimeValue(Platform::String^ text, unsigned long *value)
{
    auto time = _wtol(text->Data());

    if ((time.ToString() == text) && (time >= 0))
    {
        *value = time;
    }
}

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    // The format here is the following:
    //     { "Description for the sample", "Fully quaified name for the class that implements the scenario" }
    { "Thread pool delay timer", "SDKSample.ThreadPool.DelayTimerScenario" }, 
    { "Thread pool periodic timer", "SDKSample.ThreadPool.PeriodicTimerScenario" },
    { "Thread pool work item", "SDKSample.ThreadPool.WorkItemScenario" }
}; 
