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

namespace SDKSample
{
    ref class App
    {
    internal:
        App();
        virtual void OnSuspending(Platform::Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ pArgs);
    protected:
        virtual void OnLaunched(Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ pArgs) override;
        virtual void OnActivated(Windows::ApplicationModel::Activation::IActivatedEventArgs^ pArgs) override;
    private:
        void InitializeApp(Windows::ApplicationModel::Activation::IActivatedEventArgs^ args);
        void InitializeWindowContent(Windows::UI::Xaml::Controls::Frame^ rootFrame, Windows::ApplicationModel::Activation::IActivatedEventArgs^ args);
    };
}
