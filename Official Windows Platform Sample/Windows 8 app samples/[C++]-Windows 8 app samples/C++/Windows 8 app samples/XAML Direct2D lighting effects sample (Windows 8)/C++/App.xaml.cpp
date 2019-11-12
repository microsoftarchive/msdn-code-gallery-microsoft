//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "App.xaml.h"

using namespace D2DLightingEffects;

using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Media::Animation;
using namespace Windows::Graphics::Display;

App::App()
{
    InitializeComponent();
}

void App::OnLaunched(
    _In_ LaunchActivatedEventArgs^ args
    )
{
    m_mainPage = ref new MainPage();

    Window::Current->Content = m_mainPage;
    Window::Current->Activate();

    Suspending += ref new SuspendingEventHandler(this, &App::OnSuspending);
    Resuming += ref new EventHandler<Object^>(this, &App::OnResuming);
}

void App::OnResuming(
    _In_ Object^ sender,
    _In_ Object^ args
    )
{
    // Intentionally left blank. The settings are already preserved if an app is resuming
    // from a suspended state. Otherwise, the previous settings are loaded when MainPage is
    // constructed.
}

void App::OnSuspending(
    _In_ Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    m_mainPage->OnSuspending(sender, args);
}
