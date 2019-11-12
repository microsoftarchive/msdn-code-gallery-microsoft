//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "MainPage.xaml.h"
#include "App.g.h"

namespace Postcard
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class App sealed
    {
    public:
        App();

        virtual void OnLaunched(
            Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ args
            ) override;

    private:
        void OnSuspending(
            Platform::Object^ sender,
            Windows::ApplicationModel::SuspendingEventArgs^ args
            );

        void OnResuming(
            Platform::Object^ sender,
            Platform::Object^ args
            );

        MainPage^ m_mainPage;
    };
}
