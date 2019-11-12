//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DirectXApp.h"
#include "BasicTimer.h"

using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Storage;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;

DirectXApp::DirectXApp() :
    m_windowClosed(false),
    m_windowVisible(true)
{
}

void DirectXApp::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &DirectXApp::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &DirectXApp::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &DirectXApp::OnResuming);

    m_renderer = ref new BumpMapping();
}

void DirectXApp::SetWindow(
    _In_ CoreWindow^ window
    )
{
    // set cursor to stop spinning
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &DirectXApp::OnWindowSizeChanged);

    window->VisibilityChanged +=
        ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &DirectXApp::OnVisibilityChanged);

    window->Closed +=
        ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &DirectXApp::OnWindowClosed);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &DirectXApp::OnLogicalDpiChanged);


    m_renderer->Initialize(window, DisplayProperties::LogicalDpi);
}

void DirectXApp::Load(
    _In_ Platform::String^ entryPoint
    )
{
    IPropertySet^ appState = ApplicationData::Current->LocalSettings->Values;
    m_renderer->LoadInternalState(appState);
}

void DirectXApp::Run()
{
    BasicTimer^ timer = ref new BasicTimer();

    while (!m_windowClosed)
    {
        if (m_windowVisible)
        {
            timer->Update();
            CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);
            m_renderer->Update(timer->Total, timer->Delta);
            m_renderer->Render();
            m_renderer->Present(); // This call is synchronized to the display frame rate.
        }
        else
        {
            CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessOneAndAllPending);
        }
    }
}

void DirectXApp::Uninitialize()
{
}

void DirectXApp::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    m_renderer->UpdateForWindowSizeChange();
}

void DirectXApp::OnVisibilityChanged(
    _In_ CoreWindow^ sender,
    _In_ VisibilityChangedEventArgs^ args
    )
{
    m_windowVisible = args->Visible;
}

void DirectXApp::OnWindowClosed(
    _In_ CoreWindow^ sender,
    _In_ CoreWindowEventArgs^ args
    )
{
    m_windowClosed = true;
}

void DirectXApp::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    m_renderer->SetDpi(DisplayProperties::LogicalDpi);
}

void DirectXApp::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    CoreWindow::GetForCurrentThread()->Activate();
}

void DirectXApp::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    IPropertySet^ appState = ApplicationData::Current->LocalSettings->Values;
    m_renderer->SaveInternalState(appState);
}

void DirectXApp::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new DirectXApp();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
