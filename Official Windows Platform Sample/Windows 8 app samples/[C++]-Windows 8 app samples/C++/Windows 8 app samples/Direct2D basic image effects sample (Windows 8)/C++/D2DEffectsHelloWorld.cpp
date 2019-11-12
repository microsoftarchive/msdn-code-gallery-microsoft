//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "D2DEffectsHelloWorld.h"

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::Storage::Pickers;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

// Define effects constants.
static const float sc_gaussianBlurStDev = 5.0f;

void D2DEffectsHelloWorld::HandleDeviceLost()
{
    // Release window size-dependent resources prior to creating a new device and swap chain.
    m_bitmapSourceEffect = nullptr;
    m_gaussianBlurEffect = nullptr;

    DirectXBase::HandleDeviceLost();
}

void D2DEffectsHelloWorld::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    DecodeImage();
}

void D2DEffectsHelloWorld::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D basic image effects sample"
        );
}

void D2DEffectsHelloWorld::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    CreateEffectGraph();
}

void D2DEffectsHelloWorld::DecodeImage()
{
    DX::ThrowIfFailed(
        m_wicFactory->CreateDecoderFromFilename(
            L"mammoth.jpg",
            nullptr,
            GENERIC_READ,
            WICDecodeMetadataCacheOnDemand,
            &m_wicDecoder
            )
        );

    DX::ThrowIfFailed(m_wicDecoder->GetFrame(0, &m_wicFrameDecode));

    DX::ThrowIfFailed(m_wicFactory->CreateFormatConverter(&m_wicFormatConverter));

    // We format convert to a pixel format that is compatible with Direct2D.
    // To optimize for performance when using WIC and Direct2D together, we need to
    // select the target pixel format based on the image's native precision:
    // - <= 8 bits per channel precision: use BGRA channel order
    //   (example: all JPEGs, including the image in this sample, are 8bpc)
    // -  > 8 bits per channel precision: use RGBA channel order
    //   (example: TIFF and JPEG-XR images support 32bpc float)
    // Note that a fully robust system will arbitrate between various WIC pixel formats
    // and hardware feature level support using the IWICPixelFormatInfo2 interface.
    DX::ThrowIfFailed(
        m_wicFormatConverter->Initialize(
            m_wicFrameDecode.Get(),
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.0f,
            WICBitmapPaletteTypeCustom
            )
        );
}

void D2DEffectsHelloWorld::CreateEffectGraph()
{
    UINT imageWidth;
    UINT imageHeight;
    m_wicFormatConverter->GetSize(&imageWidth, &imageHeight);

    D2D1_POINT_2F scale = D2D1::Point2F(
        m_renderTargetSize.Width/imageWidth,
        m_renderTargetSize.Height/imageHeight
        );

    // Create a Bitmap Source Effect.
    DX::ThrowIfFailed(m_d2dContext->CreateEffect(CLSID_D2D1BitmapSource, &m_bitmapSourceEffect));

    // Set the scale property to the scale amount.
    DX::ThrowIfFailed(m_bitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_SCALE, scale));

    // Set the BitmapSource Property to the BitmapSource generated earlier.
    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE, m_wicFormatConverter.Get())
        );

    // Create the Gaussian Blur Effect.
    DX::ThrowIfFailed(m_d2dContext->CreateEffect(CLSID_D2D1GaussianBlur, &m_gaussianBlurEffect));

    // Set the input to recieve the bitmap from the BitmapSourceEffect.
    m_gaussianBlurEffect->SetInputEffect(0, m_bitmapSourceEffect.Get());

    // Set the blur amount.
    DX::ThrowIfFailed(m_gaussianBlurEffect->SetValue(D2D1_GAUSSIANBLUR_PROP_STANDARD_DEVIATION, sc_gaussianBlurStDev));
}

void D2DEffectsHelloWorld::Render()
{
    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));

    // Draw the scaled and blurred image.
    m_d2dContext->DrawImage(m_gaussianBlurEffect.Get());

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    m_sampleOverlay->Render();
}

void D2DEffectsHelloWorld::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &D2DEffectsHelloWorld::OnActivated);
}

void D2DEffectsHelloWorld::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &D2DEffectsHelloWorld::OnWindowSizeChanged);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &D2DEffectsHelloWorld::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &D2DEffectsHelloWorld::OnDisplayContentsInvalidated);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void D2DEffectsHelloWorld::Load(
    _In_ Platform::String^ entryPoint
    )
{
    // This method is intentionally left empty.
}

void D2DEffectsHelloWorld::Run()
{
    Render();
    Present();

    m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void D2DEffectsHelloWorld::Uninitialize()
{
    // This method is intentionally left empty.
}

void D2DEffectsHelloWorld::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
    Render();
    Present();
}

void D2DEffectsHelloWorld::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void D2DEffectsHelloWorld::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D device is available for rendering.
    ValidateDevice();

    Render();
    Present();
}

void D2DEffectsHelloWorld::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new D2DEffectsHelloWorld();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
