#include "pch.h"
using namespace Platform;
using namespace Windows::ApplicationModel::Background;

namespace Background
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class NetworkChangeTask sealed :IBackgroundTask
    {
    public:
        virtual void Run(IBackgroundTaskInstance^ taskInstance);
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class KATask sealed : IBackgroundTask
    {
    public:
        virtual void Run(IBackgroundTaskInstance^ taskInstance);
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class PushNotifyTask sealed : IBackgroundTask
    {
    public:
        virtual void Run(IBackgroundTaskInstance^ taskInstance);
        void InvokeSimpleToast(String^ messageReceived);
    };
}