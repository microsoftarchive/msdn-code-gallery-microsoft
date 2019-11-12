// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// App.xaml.h
// Declaration of the App class
//

#pragma once

#include "App.g.h"
#include "HiloPage.h"
#include "ViewModelLocator.h"  // Required by generated header
#include "DesignTimeData.h" // Required by generated header

namespace Hilo
{
    // See http://go.microsoft.com/fwlink/?LinkId=267274 for info about this app.

    class ExceptionPolicy;
    class TileUpdateScheduler;
    class Repository;

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    ref class App sealed
    {
    public:
        App();
        virtual void OnLaunched(Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ args) override;

    internal:
        std::shared_ptr<Repository> GetRepository() { return m_repository; }

    private:
        void OnSuspending(Platform::Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ e);
        void App::OnResume(Object^ sender, Platform::Object^ e);
        std::shared_ptr<ExceptionPolicy> m_exceptionPolicy;
        std::shared_ptr<TileUpdateScheduler> m_tileUpdateScheduler;
        std::shared_ptr<Repository> m_repository;
    };
}
