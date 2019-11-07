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

using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Storage;

#define SampleBackgroundTaskEntryPoint "Tasks.SampleBackgroundTask"
#define SampleBackgroundTaskName "SampleBackgroundTask"
#define SampleBackgroundTaskWithConditionName "SampleBackgroundTaskWithCondition"
#define ServicingCompleteTaskEntryPoint "Tasks.ServicingComplete"
#define ServicingCompleteTaskName "ServicingCompleteTask"
#define TimeTriggeredTaskName "TimeTriggeredTask"

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
        static property Platform::String^ FEATURE_NAME 
        {
            Platform::String^ get() 
            {  
                return ref new Platform::String(L"Background Task");
            }
        }

        static property Platform::Array<Scenario>^ scenarios 
        {
            Platform::Array<Scenario>^ get() 
            { 
                return scenariosInner; 
            }
        }
    private:
        static Platform::Array<Scenario>^ scenariosInner;
    };

    namespace BackgroundTask
    {
        class BackgroundTaskSample
        {
        public:
            static String^ GetBackgroundTaskStatus(Platform::String^ name);
            static void InitializeBackgroundTaskStatus();
            static BackgroundTaskRegistration^ RegisterBackgroundTask(String^ taskEntryPoint, String^ name, IBackgroundTrigger^ trigger, IBackgroundCondition^ condition);
            static void RegisterServicingCompleteBackgroundTask();
            static void UnregisterBackgroundTasks(String^ name);
            static void UpdateBackgroundTaskStatus(Platform::String^ name, bool registered);

            static String^ SampleBackgroundTaskProgress;
            static bool SampleBackgroundTaskRegistered;

            static String^ SampleBackgroundTaskWithConditionProgress;
            static bool SampleBackgroundTaskWithConditionRegistered;

            static String^ ServicingCompleteTaskProgress;
            static bool ServicingCompleteTaskRegistered;

            static String^ TimeTriggeredTaskProgress;
            static bool TimeTriggeredTaskRegistered;
        };

    }
}
