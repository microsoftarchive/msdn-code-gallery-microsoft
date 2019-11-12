//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

// LocationBackgroundTask.cpp
#include "pch.h"
#include "LocationBackgroundTask.h"

using namespace Concurrency;
using namespace BackgroundTask;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Devices::Geolocation;
using namespace Windows::Foundation;
using namespace Windows::Globalization::DateTimeFormatting;
using namespace Windows::Storage;

LocationBackgroundTask::LocationBackgroundTask()
{
}

LocationBackgroundTask::~LocationBackgroundTask()
{
}

void LocationBackgroundTask::Run(IBackgroundTaskInstance^ taskInstance)
{
    // Get the deferral object from the task instance
    Platform::Agile<BackgroundTaskDeferral> deferral(taskInstance->GetDeferral());

    // Associate a cancellation handler with the background task
    taskInstance->Canceled += ref new BackgroundTaskCanceledEventHandler(this, &LocationBackgroundTask::OnCanceled);

    Geolocator^ geolocator = ref new Geolocator();

    task<Geoposition^> geopositionTask(geolocator->GetGeopositionAsync(), geopositionTaskTokenSource.get_token());
    geopositionTask.then([this, deferral, geolocator](task<Geoposition^> getPosTask)
    {
        DateTimeFormatter^ dateFormatter = ref new DateTimeFormatter("longtime");
        auto settings = ApplicationData::Current->LocalSettings;

        try
        {
            // Get will throw an exception if the task was canceled or failed with an error
            Geoposition^ pos = getPosTask.get();

            // Write to LocalSettings to indicate that this background task ran
            settings->Values->Insert("Status", "Time: " + dateFormatter->Format(pos->Coordinate->Timestamp));
            settings->Values->Insert("Latitude", pos->Coordinate->Latitude.ToString());
            settings->Values->Insert("Longitude", pos->Coordinate->Longitude.ToString());
            settings->Values->Insert("Accuracy", pos->Coordinate->Accuracy.ToString());
        }
        catch (Platform::AccessDeniedException^)
        {
            // Write to LocalSettings to indicate that this background task ran
            settings->Values->Insert("Status", "Disabled");
            settings->Values->Insert("Latitude", "No data");
            settings->Values->Insert("Longitude", "No data");
            settings->Values->Insert("Accuracy", "No data");
        }
        catch (task_canceled&)
        {
        }

        // Indicate that the background task has completed
        deferral->Complete();
    });
}

// Handles background task cancellation
void LocationBackgroundTask::OnCanceled(IBackgroundTaskInstance^ taskInstance, BackgroundTaskCancellationReason reason)
{
    // Cancel the async operation
    geopositionTaskTokenSource.cancel();
}
