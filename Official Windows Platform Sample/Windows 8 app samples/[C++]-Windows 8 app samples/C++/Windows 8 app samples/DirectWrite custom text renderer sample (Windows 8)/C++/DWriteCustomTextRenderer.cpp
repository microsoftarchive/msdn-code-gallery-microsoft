//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "DWriteCustomTextRenderer.h"

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

DWriteCustomTextRenderer::DWriteCustomTextRenderer()
{
}

void DWriteCustomTextRenderer::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Create a DirectWrite text format object.
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Gabriola",
            nullptr,
            DWRITE_FONT_WEIGHT_NORMAL,
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
}

void DWriteCustomTextRenderer::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "DirectWrite custom text renderer sample"
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Black),
            &m_blackBrush
            )
        );

    DX::ThrowIfFailed(
        CoCreateInstance(
            CLSID_WICImagingFactory,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(&m_wicFactory)
            )
        );

    DX::ThrowIfFailed(
        m_wicFactory->CreateDecoderFromFilename(
            L"mammoth_small.jpg",
            nullptr,
            GENERIC_READ,
            WICDecodeMetadataCacheOnDemand,
            &m_wicBitmapDecoder
            )
        );

    m_wicBitmapDecoder->GetFrame(
        0,
        &m_wicBitmapFrame
        );

    DX::ThrowIfFailed(
        m_wicFactory->CreateFormatConverter(
            &m_wicFormatConverter
            )
        );

    m_wicFormatConverter->Initialize(
        m_wicBitmapFrame.Get(),
        GUID_WICPixelFormat32bppPBGRA,
        WICBitmapDitherTypeNone,
        nullptr,
        0.0,
        WICBitmapPaletteTypeCustom
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateBitmapFromWicBitmap(
            m_wicFormatConverter.Get(),
            &m_logoBitmap
            )
        );

    D2D1_BITMAP_BRUSH_PROPERTIES properties = {D2D1_EXTEND_MODE_WRAP, D2D1_EXTEND_MODE_WRAP, D2D1_BITMAP_INTERPOLATION_MODE_NEAREST_NEIGHBOR};

    DX::ThrowIfFailed(
        m_d2dContext->CreateBitmapBrush(
            m_logoBitmap.Get(),
            properties,
            &m_bitmapBrush
            )
        );

    m_textRenderer = new (std::nothrow) CustomTextRenderer(
        m_d2dFactory.Get(),
        m_d2dContext.Get(),
        m_blackBrush.Get(),
        m_bitmapBrush.Get()
    );
}

void DWriteCustomTextRenderer::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    Platform::String^ text = "Hello World From ... DirectWrite!";

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextLayout(
            text->Data(),                       // Text to be displayed
            text->Length(),                     // Length of the text
            m_textFormat.Get(),                 // DirectWrite Text Format object
            m_d2dContext->GetSize().width,      // Width of the Text Layout
            m_d2dContext->GetSize().height,     // Height of the Text Layout
            &m_textLayout
            )
        );

    // Text range for the "DirectWrite!" at the end of the string
    DWRITE_TEXT_RANGE textRange = {21, 12};

    // Set the font size on that text range to 100
    m_textLayout->SetFontSize(100.0f, textRange);

    // Create a DirectWrite Typography object
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTypography(
            &m_textTypography
            )
        );

    // Enumerate a stylistic set 6 font feature for application to our text layout
    DWRITE_FONT_FEATURE fontFeature = {DWRITE_FONT_FEATURE_TAG_STYLISTIC_SET_6, 1};

    // Apply the previously enumerated font feature to our Text Typography object
    m_textTypography->AddFontFeature(fontFeature);

    // Move our text range to the entire length of the string
    textRange.length = text->Length();
    textRange.startPosition = 0;

    // Apply our recently defined typography to our entire text range
    DX::ThrowIfFailed(
        m_textLayout->SetTypography(
            m_textTypography.Get(),
            textRange
            )
        );

    // Move the text range to the end again
    textRange.length = 12;
    textRange.startPosition = 21;

    // Set the underline and the font weight on the text range
    m_textLayout->SetUnderline(TRUE, textRange);
    m_textLayout->SetFontWeight(DWRITE_FONT_WEIGHT_BOLD, textRange);
}

void DWriteCustomTextRenderer::Render()
{
    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));
    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    D2D1_POINT_2F origin = D2D1::Point2F(0.0f, 0.0f);

    m_textLayout->Draw(
        m_d2dContext.Get(),
        m_textRenderer.Get(),
        origin.x,
        origin.y
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

void DWriteCustomTextRenderer::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &DWriteCustomTextRenderer::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &DWriteCustomTextRenderer::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &DWriteCustomTextRenderer::OnResuming);
}

void DWriteCustomTextRenderer::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &DWriteCustomTextRenderer::OnWindowSizeChanged);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &DWriteCustomTextRenderer::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &DWriteCustomTextRenderer::OnDisplayContentsInvalidated);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void DWriteCustomTextRenderer::Load(
    _In_ Platform::String^ entryPoint
    )
{
}

void DWriteCustomTextRenderer::Run()
{
    Render();
    Present();

    m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void DWriteCustomTextRenderer::Uninitialize()
{
}

void DWriteCustomTextRenderer::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
    Render();
    Present();
}

void DWriteCustomTextRenderer::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void DWriteCustomTextRenderer::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
    Present();
}

void DWriteCustomTextRenderer::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

void DWriteCustomTextRenderer::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
}

void DWriteCustomTextRenderer::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new DWriteCustomTextRenderer();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
