// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "BackgroundTask.h"

using namespace BackgroundTasks;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Networking::PushNotifications;
using namespace Windows::Storage;

void SampleBackgroundTask::Run(IBackgroundTaskInstance^ taskInstance)
{
    // Get the background task details
    auto settings = ApplicationData::Current->LocalSettings;
    auto taskName = taskInstance->Task->Name;

    // Store the content received from the notification so it can be retrieved from the UI
    auto notificationDetails = dynamic_cast<IRawNotification^>(taskInstance->TriggerDetails);
    settings->Values->Insert(taskName, notificationDetails->Content);
}
