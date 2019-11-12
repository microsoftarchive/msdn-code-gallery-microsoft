#pragma once

#include "pch.h"

using namespace Windows::ApplicationModel::Background;
using namespace Windows::System::Threading;

namespace NetworkStatusTask
{
    public ref class BackgroundTask sealed : public IBackgroundTask
    {
    public:
        BackgroundTask();

        virtual void Run(IBackgroundTaskInstance^ taskInstance);
        void OnCanceled(IBackgroundTaskInstance^ taskInstance, BackgroundTaskCancellationReason reason);

    private:
        ~BackgroundTask();
    };
}