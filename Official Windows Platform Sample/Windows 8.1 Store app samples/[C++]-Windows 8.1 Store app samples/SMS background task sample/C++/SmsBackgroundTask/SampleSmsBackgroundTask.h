#pragma once
#include <ppltasks.h>

using namespace Windows::ApplicationModel::Background;

namespace SmsBackgroundTask
{
    [Windows::Foundation::Metadata::WebHostHidden] 
    public ref class SampleSmsBackgroundTask sealed : public IBackgroundTask
    {
    public:
        void DisplayToast(IBackgroundTaskInstance^ taskInstance);
        virtual void Run(IBackgroundTaskInstance^ taskInstance);
    private:
        void OnCanceled(IBackgroundTaskInstance^ sender, BackgroundTaskCancellationReason reason);
    };
}