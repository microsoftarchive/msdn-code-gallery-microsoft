//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DWriteHitTesting.h"

using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;

DWriteHitTesting::DWriteHitTesting()
{
}

void DWriteHitTesting::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    m_text = "Touch Me To Change My Underline!";

    // Create a DirectWrite text format object.
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_LIGHT,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            64.0f,
            L"en-US", // locale
            &m_textFormat
            )
        );

    // Center the text horizontally.
    DX::ThrowIfFailed(
        m_textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER)
        );

    // Center the text vertically.
    DX::ThrowIfFailed(
        m_textFormat->SetParagraphAlignment(DWRITE_PARAGRAPH_ALIGNMENT_CENTER)
        );

    // Boolean array to keep track of which characters are underlined
    m_underlineArray = ref new Platform::Array<bool>(m_text->Length());

    // Initialize the array to all false in order to represent the initial un-underlined text
    for (unsigned int i = 0; i < m_text->Length(); i++)
    {
        m_underlineArray[i] = false;
    }
}

void DWriteHitTesting::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "DirectWrite hit testing sample"
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Black),
            &m_blackBrush
            )
        );
}

void DWriteHitTesting::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    D2D1_SIZE_F size = m_d2dContext->GetSize();

    // Create a DirectWrite Text Layout object
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextLayout(
            m_text->Data(),                     // Text to be displayed
            m_text->Length(),                   // Length of the text
            m_textFormat.Get(),                 // DirectWrite Text Format object
            size.width,                         // Width of the Text Layout
            size.height,                        // Height of the Text Layout
            &m_textLayout
            )
        );

    SetUnderlineOnRedraw();
}

void DWriteHitTesting::Render()
{
    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));
    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    m_d2dContext->DrawTextLayout(
        D2D1::Point2F(0.0f, 0.0f),
        m_textLayout.Get(),
        m_blackBrush.Get()
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    m_sampleOverlay->Render();
}

void DWriteHitTesting::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &DWriteHitTesting::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &DWriteHitTesting::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &DWriteHitTesting::OnResuming);
}

void DWriteHitTesting::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &DWriteHitTesting::OnWindowSizeChanged);

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &DWriteHitTesting::OnPointerPressed);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &DWriteHitTesting::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &DWriteHitTesting::OnDisplayContentsInvalidated);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void DWriteHitTesting::Load(
    _In_ Platform::String^ entryPoint
    )
{
    // Retrieve any stored variables from the LocalSettings collection.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("m_underlineArray"))
    {
        safe_cast<IPropertyValue^>(settingsValues->Lookup("m_underlineArray"))->GetBooleanArray(&m_underlineArray);
        SetUnderlineOnRedraw();
    }
}

void DWriteHitTesting::Run()
{
    Render();
    Present();

    m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void DWriteHitTesting::Uninitialize()
{
}

void DWriteHitTesting::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
    Render();
    Present();
}

void DWriteHitTesting::OnPointerPressed(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )

{
    DWRITE_HIT_TEST_METRICS hitTestMetrics;
    BOOL isTrailingHit;
    BOOL isInside;

    Windows::UI::Input::PointerPoint^ point = args->CurrentPoint;

    DX::ThrowIfFailed(
        m_textLayout->HitTestPoint(
            point->Position.X,
            point->Position.Y,
            &isTrailingHit,
            &isInside,
            &hitTestMetrics
            )
        );

    if (isInside)
    {
        BOOL underline;

        DX::ThrowIfFailed(
            m_textLayout->GetUnderline(hitTestMetrics.textPosition, &underline)
            );

        DWRITE_TEXT_RANGE textRange = {hitTestMetrics.textPosition, 1};

        DX::ThrowIfFailed(
            m_textLayout->SetUnderline(!underline, textRange)
            );

        m_underlineArray[hitTestMetrics.textPosition] = !underline;
    }

    Render();
    Present();
}

// This function is used to keep track of which elements are underlined - in order to redraw
// the underline if the text layout needs to be recreated

void DWriteHitTesting::SetUnderlineOnRedraw()
{
    for (unsigned int i = 0; i < m_text->Length(); i++)
    {
        DWRITE_TEXT_RANGE textRange = {i, 1};
        DX::ThrowIfFailed(
            m_textLayout->SetUnderline(m_underlineArray[i], textRange)
            );
    }
}

void DWriteHitTesting::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void DWriteHitTesting::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
    Present();
}

void DWriteHitTesting::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

void DWriteHitTesting::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("m_underlineArray"))
    {
        settingsValues->Remove("m_underlineArray");
    }
    settingsValues->Insert("m_underlineArray", PropertyValue::CreateBooleanArray(m_underlineArray));
}

void DWriteHitTesting::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new DWriteHitTesting();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
