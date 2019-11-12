//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"

using namespace Magazine;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::UI::Input;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::Storage;

App::App()
{
    InitializeComponent();
}

App::~App()
{
}

void App::OnLaunched(
    _In_ LaunchActivatedEventArgs^ args
    )
{
    Window::Current->Content = ref new MainPage();
    Window::Current->Activate();

    Suspending += ref new SuspendingEventHandler(this, &App::OnSuspending);
    Resuming += ref new EventHandler<Object^>(this, &App::OnResuming);
}

void App::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    // This is a good time to save your application's state in case the process gets terminated.
    // That way, when the user relaunches the application, they will return to the position they left.
    auto mainPage = dynamic_cast<MainPage^>(Window::Current->Content);
    mainPage->SaveState(ApplicationData::Current->LocalSettings->Values);
}

void App::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
    // No extra operation required on resume as the application is only visually updated.
    // If the application is suspended for a longer period and is eventually terminated,
    // the application's activation logic will ensure that the application saved state is
    // properly restored.
}
