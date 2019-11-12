//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// App.xaml.h
// Declaration of the App.xaml class.
//

#pragma once

#include "pch.h"
#include "App.g.h"
#include "MainPage.g.h"

#define DISABLE_MAIN_VIEW_KEY "DisableShowingMainViewOnActivation"

namespace SDKSample
{
    ref class App
    {
    internal:
        App();
        virtual void OnSuspending(Platform::Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ pArgs);

        property Windows::Foundation::Collections::IVector<SecondaryViewsHelpers::ViewLifetimeControl^>^ SecondaryViews
        {
            Windows::Foundation::Collections::IVector<SecondaryViewsHelpers::ViewLifetimeControl^>^ get();
        };

        void UpdateTitle(Platform::String^ newTitle, int viewId);
        property Windows::UI::Core::CoreDispatcher^ MainDispatcher
        {
            Windows::UI::Core::CoreDispatcher^ get();
        };
        property int MainViewId
        {
            int get();
        }
    protected:
        virtual void OnLaunched(Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ pArgs) override;
        virtual void OnActivated(Windows::ApplicationModel::Activation::IActivatedEventArgs^ pArgs) override;
    private:
        bool TryFindViewLifetimeControlForViewId(int viewId, SecondaryViewsHelpers::ViewLifetimeControl^* foundData);
        Concurrency::task<void> InitializeMainPage(Windows::ApplicationModel::Activation::ApplicationExecutionState previousExecutionState,
                                Platform::String^ arguments);
        Platform::Collections::Vector<SecondaryViewsHelpers::ViewLifetimeControl^>^ secondaryViews;
        Windows::UI::Core::CoreDispatcher^ mainDispatcher;
        int mainViewId;
    };
}
