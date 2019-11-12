// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "pch.h"
#include "App.g.h"
#include "MainPage.g.h"
#include "MainPage.xaml.h"

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
#include "ContinuationManager.h"
#endif

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

        virtual void OnFileActivated(Windows::ApplicationModel::Activation::FileActivatedEventArgs^ e) override;
        virtual void OnActivated(Windows::ApplicationModel::Activation::IActivatedEventArgs^ e) override;

    private:
        void OnSuspending(Platform::Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ e);
        void OnNavigationFailed(Platform::Object ^sender, Windows::UI::Xaml::Navigation::NavigationFailedEventArgs ^e);

        Windows::UI::Xaml::Controls::Frame^ CreateRootFrame();
        concurrency::task<void> RestoreStatus(Windows::ApplicationModel::Activation::ApplicationExecutionState previousExecutionState);

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
        ContinuationManager^ continuationManager;
#endif

    };
}
