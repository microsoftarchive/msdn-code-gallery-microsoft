#pragma once

#include "pch.h"

using namespace Windows::ApplicationModel::Background;

namespace Tasks
{
    public ref class ServicingComplete sealed : public IBackgroundTask
    {

    public:
        ServicingComplete();

        virtual void Run(IBackgroundTaskInstance^ taskInstance);
        void OnCanceled(IBackgroundTaskInstance^ taskInstance, BackgroundTaskCancellationReason reason);

    private:
        ~ServicingComplete();

        volatile bool CancelRequested;
    };
}
