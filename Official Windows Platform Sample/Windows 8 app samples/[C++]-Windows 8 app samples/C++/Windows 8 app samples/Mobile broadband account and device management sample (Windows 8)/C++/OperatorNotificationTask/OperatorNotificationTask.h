#pragma once

namespace OperatorNotificationTask
{
    public ref class OperatorNotification sealed : public Windows::ApplicationModel::Background::IBackgroundTask
    {
    public:
        //
        // The Run method is the entry point of a background task
        //
        virtual void Run(Windows::ApplicationModel::Background::IBackgroundTaskInstance^ taskInstance);
    };
}