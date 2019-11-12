#pragma once

#include "pch.h"
#include <agile.h>

using namespace Windows::ApplicationModel::Background;
using namespace Windows::System::Threading;

namespace Tasks
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class SampleBackgroundTask sealed : public IBackgroundTask
    {

    public:
        SampleBackgroundTask();

        virtual void Run(IBackgroundTaskInstance^ taskInstance);
        void OnCanceled(IBackgroundTaskInstance^ taskInstance, BackgroundTaskCancellationReason reason);

    private:
        ~SampleBackgroundTask();

        volatile bool CancelRequested;
        Platform::Agile<Windows::ApplicationModel::Background::BackgroundTaskDeferral> Deferral;
        ThreadPoolTimer^ PeriodicTimer;
        unsigned int Progress;
        IBackgroundTaskInstance^ TaskInstance;
    };
}