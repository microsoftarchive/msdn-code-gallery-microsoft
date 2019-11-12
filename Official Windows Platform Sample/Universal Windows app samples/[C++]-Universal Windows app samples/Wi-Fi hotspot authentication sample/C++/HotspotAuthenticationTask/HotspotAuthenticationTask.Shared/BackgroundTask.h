//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
//
// BackgroundTask.h
// Implementation of the background task handler.
//

#pragma once

#include "pch.h"
#include <agile.h>

// The namespace for the background tasks.
namespace HotspotAuthenticationTask
{
    // A background task always implements the IBackgroundTask interface.
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class AuthenticationTask sealed : public Windows::ApplicationModel::Background::IBackgroundTask
    {
        Platform::String^ _foregroundAppId;
        bool _cancelRequested;;

    public:
        AuthenticationTask();
        virtual void Run(Windows::ApplicationModel::Background::IBackgroundTaskInstance^ taskInstance);
        void OnCanceled(Windows::ApplicationModel::Background::IBackgroundTaskInstance^ sender,
        Windows::ApplicationModel::Background::BackgroundTaskCancellationReason reason);

    private:
        Platform::Agile<Windows::ApplicationModel::Background::BackgroundTaskDeferral> Deferral;
    };
}
