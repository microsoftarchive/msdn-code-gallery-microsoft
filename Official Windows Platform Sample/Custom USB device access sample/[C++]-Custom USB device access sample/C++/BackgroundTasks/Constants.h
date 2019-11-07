//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

namespace BackgroundTasks
{
    namespace LocalSettingKeys
    {
        static Platform::String^ SyncBackgroundTaskStatus = "SyncBackgroundTaskStatus";
        static Platform::String^ SyncBackgroundTaskResult = "SyncBackgroundTaskResult";
    };

    namespace BackgroundTaskInformation
    {
        static Platform::String^ TaskCanceled = "Canceled";
        static Platform::String^ TaskCompleted = "Completed";
    };

    namespace Sync
    {
        static const uint32 BytesToWriteAtATime = 512;
        static const uint32 NumberOfTimesToWrite = 2;
    };
}
