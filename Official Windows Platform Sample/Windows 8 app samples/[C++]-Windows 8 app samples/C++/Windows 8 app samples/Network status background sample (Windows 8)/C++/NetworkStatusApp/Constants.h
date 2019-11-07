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

#define SampleBackgroundTaskEntryPoint "NetworkStatusTask.BackgroundTask"
#define SampleBackgroundTaskWithConditionName "SampleBackgroundTaskWithCondition"

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
                return ref new Platform::String(L"Network Status");
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

    class BackgroundTaskSample
    {
    public:
        static BackgroundTaskRegistration^ RegisterBackgroundTask(String^ taskEntryPoint, String^ name, IBackgroundTrigger^ trigger, IBackgroundCondition^ condition);
        static void UnregisterBackgroundTasks(String^ name);
        static void UpdateBackgroundTaskStatus(Platform::String^ name, bool registered);

        static bool SampleBackgroundTaskWithConditionRegistered;
    };

}
