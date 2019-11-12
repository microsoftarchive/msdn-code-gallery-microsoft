// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "App.g.h"

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
#include "ContinuationManager.h"
#endif

namespace OCR
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    ref class App sealed
    {
    public:
        virtual void OnLaunched(Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ e) override;
        App();

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
        virtual void OnActivated(Windows::ApplicationModel::Activation::IActivatedEventArgs^ e) override;
#endif

    private:
        void OnSuspending(Platform::Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ e);

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
        ContinuationManager^ continuationManager;

        Frame^ CreateRootFrame();
        concurrency::task<void> RestoreStatus(ApplicationExecutionState previousExecutionState);
#endif
    };
}
