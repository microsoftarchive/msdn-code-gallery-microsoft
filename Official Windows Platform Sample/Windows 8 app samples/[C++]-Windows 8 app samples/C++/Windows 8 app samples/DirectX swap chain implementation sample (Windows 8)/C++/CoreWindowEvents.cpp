//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "CoreWindowEvents.h"

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

void CoreWindowEvents::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Create a DirectWrite text format object.
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI",
            NULL,
            DWRITE_FONT_WEIGHT_REGULAR,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            20.0f,
            L"en-US", //locale
            &m_eventTextFormat
            )
        );

    // Align the text to left.
    m_eventTextFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_LEADING);
    // Center the text vertically.
    m_eventTextFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER);
}

void CoreWindowEvents::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        L"CoreWindow Events Sample"
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Black),
            &m_blackBrush
            )
        );
}

void CoreWindowEvents::Render()
{
    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));

    m_d2dContext->DrawText(
        m_eventName->Data(),
        m_eventName->Length(),
        m_eventTextFormat.Get(),
        D2D1::RectF(0.0, 0.0, m_renderTargetSize.Width, m_renderTargetSize.Height),
        m_blackBrush.Get());

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    m_sampleOverlay->Render();
}

void CoreWindowEvents::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &CoreWindowEvents::OnActivated);
}

void CoreWindowEvents::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &CoreWindowEvents::OnWindowSizeChanged);
    window->CharacterReceived +=
        ref new TypedEventHandler<CoreWindow^, CharacterReceivedEventArgs^>(this, &CoreWindowEvents::OnCharacterReceived);
    window->KeyDown +=
        ref new TypedEventHandler<CoreWindow^, KeyEventArgs^>(this, &CoreWindowEvents::OnKeyDown);
    window->KeyUp +=
        ref new TypedEventHandler<CoreWindow^, KeyEventArgs^>(this, &CoreWindowEvents::OnKeyUp);
    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CoreWindowEvents::OnPointerPressed);
    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CoreWindowEvents::OnPointerReleased);
    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CoreWindowEvents::OnPointerMoved);
    window->Activated +=
        ref new TypedEventHandler<CoreWindow^, WindowActivatedEventArgs^>(this, &CoreWindowEvents::OnWindowActivated);
    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &CoreWindowEvents::OnLogicalDpiChanged);
    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &CoreWindowEvents::OnDisplayContentsInvalidated);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void CoreWindowEvents::Load(
    _In_ Platform::String^ entryPoint
    )
{
}

void CoreWindowEvents::Run()
{
    CoreWindow::GetForCurrentThread()->Activate();

    Render();
    Present();
    CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void CoreWindowEvents::Uninitialize()
{
}

void CoreWindowEvents::EventPrint(
    _In_ Platform::String^ eventText
    )
{
    m_eventName = eventText;
    Render();
    Present();
}

void CoreWindowEvents::PrintPointerInfo(
    _In_ Platform::String^ eventText,
    _In_ PointerEventArgs^ args
    )
{
    Windows::UI::Input::PointerPoint^ currentPoint = args->CurrentPoint;
    EventPrint(L"Event: " + eventText + "\nArgument: Pointer ID:  " + currentPoint->PointerId.ToString() +
                " Position: " + ((int)currentPoint->Position.X).ToString() + ":" + ((int)currentPoint->Position.Y).ToString());
}

void CoreWindowEvents::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
    EventPrint("Event: Window Size Changed\nArguments: Width: " + args->Size.Width.ToString() + " Height: "  + args->Size.Height.ToString());
}

void CoreWindowEvents::OnCharacterReceived(
    _In_ CoreWindow^ sender,
    _In_ CharacterReceivedEventArgs^ args
    )
{
    EventPrint("Event: Character Received\nArgument: Character: " + ((char16)args->KeyCode).ToString());
}

void CoreWindowEvents::OnKeyDown(
    _In_ CoreWindow^ sender,
    _In_ KeyEventArgs^ args
    )
{
    EventPrint("Event: Key Down\nArgument: Virtual Key: " + ((int)args->VirtualKey).ToString());
}

void CoreWindowEvents::OnKeyUp(
    _In_ CoreWindow^ sender,
    _In_ KeyEventArgs^ args
    )
{
    EventPrint("Event: Key Up\nArgument: Virtual Key: " + ((int)args->VirtualKey).ToString());
}

void CoreWindowEvents::OnPointerPressed(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    PrintPointerInfo(L"Pointer Pressed", args);
}

void CoreWindowEvents::OnPointerReleased(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    PrintPointerInfo(L"Pointer Released", args);
}

void CoreWindowEvents::OnPointerMoved(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    PrintPointerInfo(L"Pointer Moved", args);
}

void CoreWindowEvents::OnWindowActivated(
    _In_ CoreWindow^ sender,
    _In_ WindowActivatedEventArgs^ args
    )
{
    EventPrint("Event: Window Activated\nArgument: " + ((args->WindowActivationState == CoreWindowActivationState::Deactivated) ? "Deactivated" : "Activated"));
}

void CoreWindowEvents::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void CoreWindowEvents::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
    Present();
}

void CoreWindowEvents::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    CoreWindow::GetForCurrentThread()->Activate();
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new CoreWindowEvents();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}

