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
using namespace Windows::UI::Input;
using namespace Windows::System;
using namespace Windows::Storage;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Devices::Input;

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

    m_renderer = ref new StereoSimpleD3D();
}

void DirectXApp::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &DirectXApp::OnWindowSizeChanged);

    window->VisibilityChanged +=
        ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &DirectXApp::OnVisibilityChanged);

    window->Closed +=
        ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &DirectXApp::OnWindowClosed);

    window->KeyDown +=
        ref new TypedEventHandler<CoreWindow^, KeyEventArgs^>(this, &DirectXApp::OnKeyDown);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &DirectXApp::OnLogicalDpiChanged);

    m_renderer->Initialize(window, DisplayProperties::LogicalDpi);
}

void DirectXApp::Load(
    _In_ Platform::String^ entryPoint
    )
{
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

            // render the mono content or the left eye view of the stereo content
            m_renderer->Update(0, timer->Total, timer->Delta);
            m_renderer->RenderEye(0);
            // render the right eye view of the stereo content
            if (m_renderer->GetStereoEnabledStatus())
            {
                m_renderer->Update(1, timer->Total, timer->Delta);
                m_renderer->RenderEye(1);
            }
            m_renderer->Present(); // this call is sychronized to the display frame rate
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

void DirectXApp::OnKeyDown(
    _In_ CoreWindow^ sender,
    _In_ KeyEventArgs^ args
    )
{
    Windows::System::VirtualKey Key;
    Key = args->VirtualKey;

    // if the image is in stereo, adjust for user keystrokes increasing/decreasing the stereo effect
    if (m_renderer->GetStereoEnabledStatus())
    {
        float stereoExaggeration = m_renderer->GetStereoExaggeration();
        // figure out the command from the keyboard
        if (Key == VirtualKey::Up)             // increase stereo effect
        {
            stereoExaggeration += 0.1f;
        }
        if (Key == VirtualKey::Down)           // descrease stereo effect
        {
            stereoExaggeration -= 0.1f;
        }
        stereoExaggeration = min(stereoExaggeration, 2.0f);
        stereoExaggeration = max(stereoExaggeration, 0.0f);
        m_renderer->SetStereoExaggeration(stereoExaggeration);
    }
}

void DirectXApp::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    if (args->Kind == ActivationKind::Launch)
    {
        // Load previously saved state only if the application shut down cleanly last time.
        if (args->PreviousExecutionState != ApplicationExecutionState::NotRunning)
        {
            // When this application is suspended, it stores the drawing state.
            // This code attempts to restore the saved state.
            IPropertySet^ set = ApplicationData::Current->LocalSettings->Values;
            // an int called StereoExaggerationFactor is used as a key
            if (set->HasKey("StereoExaggerationFactor"))
            {
                float tempStereoExaggerationFactor = (safe_cast<IPropertyValue^>(set->Lookup("StereoExaggerationFactor")))->GetSingle();
                m_renderer->SetStereoExaggeration(tempStereoExaggerationFactor);
            }
        }
    }
    else
    {
        DX::ThrowIfFailed(E_UNEXPECTED);
    }
    CoreWindow::GetForCurrentThread()->Activate();
}

void DirectXApp::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    // This is also a good time to save your application's state in case the process gets terminated.
    // That way, when the user relaunches the application, they will return to the position they left.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;
    if (settingsValues->HasKey("StereoExaggerationFactor"))
    {
        settingsValues->Remove("StereoExaggerationFactor");
    }

    float tempStereoExaggerationFactor = m_renderer->GetStereoExaggeration();
    settingsValues->Insert("StereoExaggerationFactor", PropertyValue::CreateSingle(tempStereoExaggerationFactor));
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
