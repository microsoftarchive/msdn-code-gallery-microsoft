//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "ShapesPuzzle.h"

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

ShapesPuzzle::ShapesPuzzle()
{
}

void ShapesPuzzle::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Shapes Puzzle - Touch Hit Test Sample"
        );

    m_program->SetD2DContext(m_d2dContext.Get());
}

void ShapesPuzzle::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();
    m_program->CreateWindowSizeDependentResources();
}

void ShapesPuzzle::Render()
{
    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));
    m_program->RenderObjects();

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    m_sampleOverlay->Render();
}

void ShapesPuzzle::Initialize(CoreApplicationView^ applicationView)
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &ShapesPuzzle::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &ShapesPuzzle::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &ShapesPuzzle::OnResuming);
}

void ShapesPuzzle::SetWindow(CoreWindow^ window)
{
    m_program = ref new Program();
    m_program->Initialize(window);

    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &ShapesPuzzle::OnPointerPressed);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &ShapesPuzzle::OnPointerMoved);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &ShapesPuzzle::OnPointerReleased);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &ShapesPuzzle::OnWindowSizeChanged);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &ShapesPuzzle::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &ShapesPuzzle::OnDisplayContentsInvalidated);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void ShapesPuzzle::Load(Platform::String^ entryPoint)
{
}

void ShapesPuzzle::Run()
{
    m_window->Activate();

    Render();
    Present();

    m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void ShapesPuzzle::Uninitialize()
{
}

void ShapesPuzzle::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    Render();
    Present();
}

void ShapesPuzzle::OnPointerPressed(
    _In_ CoreWindow^,
    _In_ PointerEventArgs^ args)
{
    m_program->OnPointerPressed(args);
}

void ShapesPuzzle::OnPointerMoved(
    _In_ CoreWindow^,
    _In_ PointerEventArgs^ args)
{
    m_program->OnPointerMoved(args);
    Render();
    Present();
}

void ShapesPuzzle::OnPointerReleased(
    _In_ CoreWindow^,
    _In_ PointerEventArgs^ args)
{
    m_program->OnPointerReleased(args);
    Render();
    Present();
}

void ShapesPuzzle::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void ShapesPuzzle::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
    Present();
}

void ShapesPuzzle::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

void ShapesPuzzle::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
}

void ShapesPuzzle::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new ShapesPuzzle();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
