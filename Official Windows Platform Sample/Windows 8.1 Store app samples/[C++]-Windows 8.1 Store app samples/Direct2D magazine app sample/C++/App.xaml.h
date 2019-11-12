//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "App.g.h"

namespace Magazine
{
    ref class App sealed
    {
    public:
        App();
        virtual void OnLaunched(
            _In_ Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ args
            ) override;

    private:
        ~App();
        void OnSuspending(
            _In_ Platform::Object^ sender,
            _In_ Windows::ApplicationModel::SuspendingEventArgs^ args
            );

        void OnResuming(
            _In_ Platform::Object^ sender,
            _In_ Platform::Object^ args
            );
    };
}
