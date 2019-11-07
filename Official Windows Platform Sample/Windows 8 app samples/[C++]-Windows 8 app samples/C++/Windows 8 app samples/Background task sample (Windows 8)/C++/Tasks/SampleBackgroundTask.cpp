// SampleBackgroundTask.cpp
#include "pch.h"
#include "SampleBackgroundTask.h"

using namespace Tasks;
using namespace Windows::Foundation;
using namespace Windows::Storage;

SampleBackgroundTask::SampleBackgroundTask() :
    CancelRequested(false), Deferral(nullptr), PeriodicTimer(nullptr), Progress(0), TaskInstance(nullptr)
{
}

SampleBackgroundTask::~SampleBackgroundTask()
{
}

void SampleBackgroundTask::Run(IBackgroundTaskInstance^ taskInstance)
{
    //
    // Associate a cancellation handler with the background task.
    //
    taskInstance->Canceled += ref new BackgroundTaskCanceledEventHandler(this, &SampleBackgroundTask::OnCanceled);

    //
    // Get the deferral object from the task instance, and take a reference to the taskInstance.
    //
    Deferral = taskInstance->GetDeferral();
    TaskInstance = taskInstance;

    auto timerDelegate = [this](ThreadPoolTimer^ timer)
    {
        if ((CancelRequested == false) &&
            (Progress < 100))
            {
                Progress += 10;
                TaskInstance->Progress = Progress;
            }
            else
            {
                PeriodicTimer->Cancel();

                //
                // Write to LocalSettings to indicate that this background task ran.
                //
                auto settings = ApplicationData::Current->LocalSettings;
                auto key = TaskInstance->Task->Name;
                settings->Values->Insert(key, (Progress < 100) ? "Canceled" : "Completed");

                //
                // Indicate that the background task has completed.
                //
                Deferral->Complete();
            }
    };

    TimeSpan period;
    period.Duration = 500 * 10000; // 500 milliseconds
    PeriodicTimer = ThreadPoolTimer::CreatePeriodicTimer(ref new TimerElapsedHandler(timerDelegate), period);
}

//
// Handles background task cancellation.
//
void SampleBackgroundTask::OnCanceled(IBackgroundTaskInstance^ taskInstance, BackgroundTaskCancellationReason reason)
{
    //
    // Indicate that the background task is canceled.
    //
    CancelRequested = true;
}
