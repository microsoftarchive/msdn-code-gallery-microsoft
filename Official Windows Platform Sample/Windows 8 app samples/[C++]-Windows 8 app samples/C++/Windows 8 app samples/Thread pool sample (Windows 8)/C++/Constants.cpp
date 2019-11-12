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
#include "DelayTimerScenario.xaml.h"

using namespace SDKSample;
using namespace SDKSample::ThreadPool;

int DelayTimerMilliseconds = 0;
Platform::String^ DelayTimerInfo = "";
Status DelayTimerStatus = SDKSample::ThreadPool::Status::Unregistered;
int DelayTimerSelectedIndex = 0;

int PeriodicTimerCount = 0;
int PeriodicTimerMilliseconds = 0;
Platform::String^ PeriodicTimerInfo = "";
Status PeriodicTimerStatus = SDKSample::ThreadPool::Status::Unregistered;
int PeriodicTimerSelectedIndex = 0;

Status WorkItemStatus = SDKSample::ThreadPool::Status::Unregistered;
Platform::String^ WorkItemInfo = "";
Windows::System::Threading::WorkItemPriority WorkPriority = Windows::System::Threading::WorkItemPriority::Normal;
int WorkItemSelectedIndex = 1;

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

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    // The format here is the following:
    //     { "Description for the sample", "Fully quaified name for the class that implements the scenario" }
    { "Thread pool delay timer", "SDKSample.ThreadPool.DelayTimerScenario" }, 
    { "Thread pool periodic timer", "SDKSample.ThreadPool.PeriodicTimerScenario" },
    { "Thread pool work item", "SDKSample.ThreadPool.WorkItemScenario" }
}; 
