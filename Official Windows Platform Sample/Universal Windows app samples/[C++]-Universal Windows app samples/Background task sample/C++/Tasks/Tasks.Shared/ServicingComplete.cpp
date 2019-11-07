// ServicingComplete.cpp
#include "pch.h"
#include "ServicingComplete.h"

using namespace Tasks;
using namespace Windows::Storage;

ServicingComplete::ServicingComplete() :
    CancelRequested(false)
{
}

ServicingComplete::~ServicingComplete()
{
}

void ServicingComplete::Run(IBackgroundTaskInstance^ taskInstance)
{
    //
    // Do the background task activity.
    //
    unsigned int progress;
    for (progress = 0; progress <= 100; progress += 10)
    {
        //
        // If the cancellation handler indicated that the task was canceled, stop doing the task.
        //
        if (CancelRequested)
        {
            break;
        }

        //
        // Indicate progress to the foreground application.
        //
        taskInstance->Progress = progress;
    }

    //
    // Write to LocalSettings to indicate that this background task ran.
    //
    auto settings = ApplicationData::Current->LocalSettings;
	auto key = taskInstance->Task->Name;
    settings->Values->Insert(key, (progress < 100) ? "Canceled" : "Completed");
}

void ServicingComplete::OnCanceled(IBackgroundTaskInstance^ taskInstance, BackgroundTaskCancellationReason reason)
{
    //
    // Indicate that the background task is canceled.
    //
    CancelRequested = true;
}
