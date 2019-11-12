#pragma once
#include <ppltasks.h>


namespace SmsBackgroundTask
{
    public ref class SampleSmsBackgroundTask sealed : public Windows::ApplicationModel::Background::IBackgroundTask
    {
    public:
        virtual void Run(Windows::ApplicationModel::Background::IBackgroundTaskInstance^ taskInstance);
        void DisplayToast(Windows::ApplicationModel::Background::IBackgroundTaskInstance^ taskInstance);

    private:
        void OnCanceled(Windows::ApplicationModel::Background::IBackgroundTaskInstance^ sender, Windows::ApplicationModel::Background::BackgroundTaskCancellationReason reason);
    };
}