// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "pch.h"
#include "App.g.h"
#include "MainPage.g.h"
#include "MainPage.xaml.h"

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
#include "ContinuationManager.h"
#endif

using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Xaml::Controls;

namespace SDKSample
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
        void OnNavigationFailed(Platform::Object ^sender, Windows::UI::Xaml::Navigation::NavigationFailedEventArgs ^e);

        Frame^ CreateRootFrame();
        concurrency::task<void> RestoreStatus(ApplicationExecutionState previousExecutionState);

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
        ContinuationManager^ continuationManager;
#endif
    };
}
