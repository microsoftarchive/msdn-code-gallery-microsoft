//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "CustomComputeShaderSample.h"

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

// Specified in DIPs since title height is DPI-independent.
static const float TitleHeight = 70.0f;

CustomComputeShaderSample::CustomComputeShaderSample() :
    m_zoom(0),
    m_imageSize(D2D1::SizeU())
{
}

void CustomComputeShaderSample::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Create WIC Decoder to read JPG file.
    ComPtr<IWICBitmapDecoder> decoder;
    DX::ThrowIfFailed(
        m_wicFactory->CreateDecoderFromFilename(
            L"mammoth.jpg",
            nullptr,
            GENERIC_READ,
            WICDecodeMetadataCacheOnDemand,
            &decoder
            )
        );

    ComPtr<IWICBitmapFrameDecode> frame;
    DX::ThrowIfFailed(
        decoder->GetFrame(0, &frame)
        );

    DX::ThrowIfFailed(
        m_wicFactory->CreateFormatConverter(&m_wicFormatConverter)
        );

    DX::ThrowIfFailed(
        m_wicFormatConverter->Initialize(
            frame.Get(),
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.0f,
            WICBitmapPaletteTypeCustom
            )
        );

    UINT width;
    UINT height;
    DX::ThrowIfFailed(m_wicFormatConverter->GetSize(&width, &height));
    m_imageSize = D2D1::SizeU(width, height);

    // Register sample Discrete Fourier Transform (DFT) effect with Direct2D.
    SampleDftEffect::Register(m_d2dFactory.Get());
}

void CustomComputeShaderSample::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D custom compute shader effect sample"
        );

    // Create BitmapSource effect to use the WIC image in Direct2D.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1BitmapSource, &m_bitmapSourceEffect)
        );

    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE, m_wicFormatConverter.Get())
        );

    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_INTERPOLATION_MODE, D2D1_BITMAPSOURCE_INTERPOLATION_MODE_FANT)
        );

    // In contrast to the Pixel and Vertex shader samples, this sample does not cache the BitmapSource effect. This is because
    // the image is only read from once at startup - the overhead added by caching is only worth while if the same unchanged
    // effect is read from multiple times.

    // Create custom effect.
    DX::ThrowIfFailed(m_d2dContext->CreateEffect(CLSID_CustomDFT, &m_dft));

    // Set input of custom effect to BitmapSource effect.
    m_dft->SetInputEffect(0, m_bitmapSourceEffect.Get());
}

void CustomComputeShaderSample::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // Scale image using BitmapSource effect.
    m_zoom = min(
        (m_renderTargetSize.Width / m_imageSize.width) / 2.0f,
        (m_renderTargetSize.Height / m_imageSize.height) / 2.0f
        );

    D2D1_VECTOR_2F scale = D2D1::Vector2F(m_zoom, m_zoom);

    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_SCALE, scale)
        );
}

void CustomComputeShaderSample::Render()
{
    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));

    // Calculate display positioning for image/DFT effect.
    float scaledWidth = m_imageSize.width * m_zoom;
    float scaledHeight = m_imageSize.height * m_zoom;
    float horizontalMargin = ((m_renderTargetSize.Width / 2 - scaledWidth) / 2.0f);

    // Clip values nearest whole pixel to prevent effect output from being anti-aliased.
    scaledWidth = floor(scaledWidth);
    scaledHeight = floor(scaledHeight);
    horizontalMargin = floor(horizontalMargin);
    float titleHeightPixels = TitleHeight * m_dpi / 96.0f;

    // Convert values to DIPs to preserve layout at different DPIs.
    scaledHeight *= 96.0f / m_dpi;
    scaledWidth *= 96.0f / m_dpi;
    horizontalMargin *= 96.0f / m_dpi;
    float titleHeightDips = titleHeightPixels * 96.0f / m_dpi;

    // Calculate whole pixel offset of screen midpoint before converting to DIPs to
    // prevent effect output from being anti-aliased.
    float screenMidpoint = floor(m_renderTargetSize.Width / 2.0f) * 96.0f / m_dpi;

    m_d2dContext->DrawImage(m_bitmapSourceEffect.Get(), D2D1::Point2F(horizontalMargin, titleHeightDips));

    // Because image output is non-transparent, it should be blended with BOUNDED_SOURCE_COPY, which allows
    // the compute shader output to be written directly to the render target instead of blended.
    m_d2dContext->DrawImage(
        m_dft.Get(),
        D2D1::Point2F(
            screenMidpoint + horizontalMargin,
            titleHeightDips
            ),
        D2D1::RectF(
            0.0f,
            0.0f,
            scaledWidth,
            scaledHeight
            ),
        D2D1_INTERPOLATION_MODE_LINEAR,
        D2D1_COMPOSITE_MODE_BOUNDED_SOURCE_COPY
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    // Display sample title and logo.
    m_sampleOverlay->Render();
}

void CustomComputeShaderSample::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &CustomComputeShaderSample::OnActivated);
}

void CustomComputeShaderSample::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &CustomComputeShaderSample::OnWindowSizeChanged);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &CustomComputeShaderSample::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &CustomComputeShaderSample::OnDisplayContentsInvalidated);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void CustomComputeShaderSample::Load(
    _In_ Platform::String^ entryPoint
    )
{
}

void CustomComputeShaderSample::Run()
{
    Render();
    Present();

    m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void CustomComputeShaderSample::Uninitialize()
{
}

void CustomComputeShaderSample::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
    Render();
    Present();
}

void CustomComputeShaderSample::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void CustomComputeShaderSample::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
    Present();
}

void CustomComputeShaderSample::OnActivated(
    _In_ CoreApplicationView^ sender,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new CustomComputeShaderSample();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
