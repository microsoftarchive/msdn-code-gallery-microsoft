// Copyright (c) Microsoft Corporation. All rights reserved.

#pragma once
#include "pch.h"
#include <sal.h>

namespace BackgroundTaskHelper
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class NetworkChangeTask sealed : Windows::ApplicationModel::Background::IBackgroundTask
    {
    public:
        virtual void Run(_In_ Windows::ApplicationModel::Background::IBackgroundTaskInstance^ taskInstance);
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class KATask sealed : Windows::ApplicationModel::Background::IBackgroundTask
    {
    public:
        virtual void Run(_In_ Windows::ApplicationModel::Background::IBackgroundTaskInstance^ taskInstance);
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class PushNotifyTask sealed : Windows::ApplicationModel::Background::IBackgroundTask
    {
    public:
        virtual void Run(_In_ Windows::ApplicationModel::Background::IBackgroundTaskInstance^ taskInstance);
        void InvokeSimpleToast(_In_ Platform::String^ messageReceived);
    };
}
