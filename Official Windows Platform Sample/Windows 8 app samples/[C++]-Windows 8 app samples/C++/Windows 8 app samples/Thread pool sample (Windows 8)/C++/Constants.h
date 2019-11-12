//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include <collection.h>

namespace SDKSample
{
    public value struct Scenario
    {
        Platform::String^ Title;
        Platform::String^ ClassName;
    };

    partial ref class MainPage
    {
    public:
        static property Platform::String^ FEATURE_NAME {
            Platform::String^ get() {  return ref new Platform::String(L"ThreadPool"); }
        }

        static property Platform::Array<Scenario>^ scenarios {
            Platform::Array<Scenario>^ get() 
            { 
                return scenariosInner; 
            }
        }
    private:
        static Platform::Array<Scenario>^ scenariosInner;
    };

    namespace ThreadPool
    {
        ref class DelayTimerScenario;
        ref class PeriodicTimerScenario;
        ref class WorkItemScenario;

        public enum class Status
        {
            Unregistered = 0,
            Started = 1,
            Canceled = 2,
            Completed = 3
        };

        struct ThreadPoolSample {
        public:
            static Windows::Foundation::TimeSpan MillisecondsToTimeSpan(long milliseconds);
            static Platform::String^ GetStatusString(ThreadPool::Status status);
        };

        static DelayTimerScenario^ DelayScenario;
        static Windows::System::Threading::ThreadPoolTimer^ DelayTimer;
        static int DelayTimerMilliseconds;
        static Platform::String^ DelayTimerInfo;
        static Status DelayTimerStatus;
        static int DelayTimerSelectedIndex;

        static PeriodicTimerScenario^ PeriodicScenario;
        static int PeriodicTimerCount;
        static Windows::System::Threading::ThreadPoolTimer^ PeriodicTimer;
        static int PeriodicTimerMilliseconds;
        static Platform::String^ PeriodicTimerInfo;
        static Status PeriodicTimerStatus;
        static int PeriodicTimerSelectedIndex;

        static WorkItemScenario^ WorkScenario;
        static Windows::Foundation::IAsyncAction^ WorkItem;
        static Platform::String^ WorkItemInfo;
        static Windows::System::Threading::WorkItemPriority WorkPriority;
        static Status WorkItemStatus;
        static int WorkItemSelectedIndex;
    }
}
