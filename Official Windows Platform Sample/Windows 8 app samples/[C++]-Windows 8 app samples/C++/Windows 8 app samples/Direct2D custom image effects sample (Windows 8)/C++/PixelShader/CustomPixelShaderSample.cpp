//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "CustomPixelShaderSample.h"

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

CustomPixelShaderSample::CustomPixelShaderSample() :
    m_rippleAnimating(false),
    m_isWindowClosed(false)
{
    m_timer = ref new BasicTimer();
}

void CustomPixelShaderSample::CreateDeviceIndependentResources()
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

    // Register the ripple effect.
    DX::ThrowIfFailed(
        RippleEffect::Register(m_d2dFactory.Get())
        );
}

void CustomPixelShaderSample::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D custom pixel shader effect sample"
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

    // Because the image will not be changing, the BitmapSource effect should be cached for better performance.
    // The property system expects TRUE and FALSE rather than true or false.
    DX::ThrowIfFailed(m_bitmapSourceEffect->SetValue(D2D1_PROPERTY_CACHED, TRUE));

    DX::ThrowIfFailed(m_d2dContext->CreateEffect(CLSID_CustomRippleEffect, &m_rippleEffect));

    // Set the BitmapSource effect as the input to the custom Ripple effect.
    m_rippleEffect->SetInputEffect(0, m_bitmapSourceEffect.Get());
}

void CustomPixelShaderSample::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // Scale image using BitmapSource effect. In certain cases (such as when the display
    // is in portrait mode) the ratio of screen height to image height is greater than the
    // ratio of screen width to image width and needs to be used to guarantee full screen coverage.
    D2D1_VECTOR_2F scale;
    scale.x = scale.y = max(
        m_renderTargetSize.Width / m_imageSize.width,
        m_renderTargetSize.Height / m_imageSize.height
        );

    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_SCALE, scale)
        );
}

void CustomPixelShaderSample::Render()
{
    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));

    m_d2dContext->DrawImage(m_rippleEffect.Get());

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

void CustomPixelShaderSample::Update()
{
    m_timer->Update();
    float delta = m_timer->Total;

    // Stop animating after four seconds.
    if (delta > 4)
    {
        delta = 4;
        m_rippleAnimating = false;
    }

    DX::ThrowIfFailed(
        m_rippleEffect->SetValue(RIPPLE_PROP_FREQUENCY, 140.0f - delta * 30.0f)
        );

    DX::ThrowIfFailed(
        m_rippleEffect->SetValue(RIPPLE_PROP_AMPLITUDE, 60.0f - delta * 15.0f)
        );

    DX::ThrowIfFailed(
        m_rippleEffect->SetValue(RIPPLE_PROP_PHASE, -delta * 20.0f)
        );

    DX::ThrowIfFailed(
        m_rippleEffect->SetValue(RIPPLE_PROP_SPREAD, 0.01f + delta / 10.0f)
        );

    Render();
    Present();
}

void CustomPixelShaderSample::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &CustomPixelShaderSample::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &CustomPixelShaderSample::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &CustomPixelShaderSample::OnResuming);
}

void CustomPixelShaderSample::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CustomPixelShaderSample::OnPointerPressed);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &CustomPixelShaderSample::OnWindowSizeChanged);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &CustomPixelShaderSample::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &CustomPixelShaderSample::OnDisplayContentsInvalidated);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void CustomPixelShaderSample::Load(
    _In_ Platform::String^ entryPoint
    )
{
}

void CustomPixelShaderSample::Run()
{
    Render();
    Present();

    while (!m_isWindowClosed)
    {
        if (m_rippleAnimating)
        {
            // Check to see if any new input events have been fired.
            m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);

            // Update and render the ripple animation.
            Update();
        }
        else
        {
            // Blocks until an input event is fired.
            m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessOneAndAllPending);
        }
    }
}

void CustomPixelShaderSample::Uninitialize()
{
}

void CustomPixelShaderSample::OnPointerPressed(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    // Reset the timer to the start of the animation.
    m_timer->Reset();

    // Since the custom ripple effect expects positioning in pixels, convert the event position from DIPs to pixels.
    D2D1_POINT_2F position = D2D1::Point2F(
        args->CurrentPoint->Position.X * m_dpi / 96.0f,
        args->CurrentPoint->Position.Y * m_dpi / 96.0f
        );

    DX::ThrowIfFailed(
        m_rippleEffect->SetValue(RIPPLE_PROP_CENTER, position)
        );

    // While ripple is animating, the program needs to both continuously render
    // and continuously poll for inputs.
    m_rippleAnimating = true;
}

void CustomPixelShaderSample::OnWindowClosed(
    _In_ CoreWindow^ sender,
    _In_ CoreWindowEventArgs^ args
    )
{
    m_isWindowClosed = true;
}

void CustomPixelShaderSample::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
    Render();
    Present();
}

void CustomPixelShaderSample::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void CustomPixelShaderSample::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
    Present();
}

void CustomPixelShaderSample::OnActivated(
    _In_ CoreApplicationView^ sender,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

void CustomPixelShaderSample::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
}

void CustomPixelShaderSample::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new CustomPixelShaderSample();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
