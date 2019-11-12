#pragma once

using namespace Windows::ApplicationModel::Background;

namespace OperatorNotificationTask
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class OperatorNotification sealed : public IBackgroundTask
    {
    public:
        //
        // The Run method is the entry point of a background task
        //
        virtual void Run(IBackgroundTaskInstance^ taskInstance);
    };
}