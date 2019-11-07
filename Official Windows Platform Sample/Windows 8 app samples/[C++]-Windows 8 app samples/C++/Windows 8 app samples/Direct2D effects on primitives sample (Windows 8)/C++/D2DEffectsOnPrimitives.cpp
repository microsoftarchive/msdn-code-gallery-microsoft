//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "D2DEffectsOnPrimitives.h"

using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::System;
using namespace Windows::UI::Core;

// Define properties of the primitives (in units of DIPs).
static const D2D1_SIZE_F sc_inputBitmapSize             = D2D1::SizeF(450.0f, 250.0f);
static const D2D1_SIZE_F sc_largeRectangleSize          = D2D1::SizeF(400.0f, 200.0f);
static const D2D1_SIZE_F sc_smallRectangleSize          = D2D1::SizeF(300.0f, 150.0f);
static const float sc_roundingRadius                    = 30.0f;
static const float sc_strokeWidth                       = 10.0f;
static const float sc_offset                            = 150.0f;

// Define properties of the effects.
static const float sc_gaussianBlurStDev                 = 8.0f;
static const D2D_VECTOR_3F sc_specularLightPosition     = D2D1::Vector3F(300.0f, -1000.0f, 3000.0f);
static const float sc_specularExponent                  = 20.0f;
static const float sc_specularConstant                  = 0.75f;
static const float sc_specularSurfaceScale              = 5.0f;
static const D2D1_VECTOR_4F sc_arithmeticCoefficients   = D2D1::Vector4F(0.0f, 1.0f, 1.0f, 0.0f);

void D2DEffectsOnPrimitives::HandleDeviceLost()
{
    // Release window size-dependent resources prior to creating a new device and swap chain.
    m_inputImage = nullptr;
    m_arithmeticCompositeEffect = nullptr;

    DirectXBase::HandleDeviceLost();
}

void D2DEffectsOnPrimitives::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Create the string to display.
    m_string = L"Direct2D";

    // Create a text format object to define the default font,
    // weight, stretch, style, and locale.
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI",                    // Font family name.
            nullptr,                        // Font collection (use the system font collection).
            DWRITE_FONT_WEIGHT_REGULAR,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            64.0f,                          // Font size.
            L"en-us",                       // Locale.
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

void D2DEffectsOnPrimitives::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    // Create the sample overlay.
    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        L"Direct2D effects on primitives sample"
        );

    // Create brushes for rendering Direct2D.
    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::White), &m_whiteBrush)
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(D2D1::ColorF(D2D1::ColorF::Red), &m_coloredBrush)
        );
}

void D2DEffectsOnPrimitives::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // Create the input bitmap of primitives.
    CreatePrimitives();

    // Create the image effects and apply them to the primitives.
    CreateEffectGraph();
}

void D2DEffectsOnPrimitives::CreatePrimitives()
{
    // Convert from DIPs to pixels, since bitmaps are created in units of pixels.
    D2D1_SIZE_U bitmapSizeInPixels = D2D1::SizeU(
        static_cast<UINT32>(sc_inputBitmapSize.width / 96.0f * m_dpi),
        static_cast<UINT32>(sc_inputBitmapSize.height / 96.0f * m_dpi)
        );

    // Create the bitmap to which the effects will be applied.
    DX::ThrowIfFailed(
        m_d2dContext->CreateBitmap(
            bitmapSizeInPixels,
            nullptr,
            0,
            D2D1::BitmapProperties1(
                D2D1_BITMAP_OPTIONS_TARGET,
                D2D1::PixelFormat(
                    DXGI_FORMAT_B8G8R8A8_UNORM,
                    D2D1_ALPHA_MODE_PREMULTIPLIED
                    ),
                m_dpi,
                m_dpi
                ),
            &m_inputImage
            )
        );

    // Draw onto the input bitmap instead of the window's surface.
    m_d2dContext->SetTarget(m_inputImage.Get());

    m_d2dContext->BeginDraw();

    // Clear the bitmap with transparent white.
    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::White, 0.0f));

    // Create a small rounded rectangle centered in the bitmap.
    D2D1_RECT_F rectangle1 = D2D1::RectF(
        (sc_inputBitmapSize.width - sc_smallRectangleSize.width) / 2,
        (sc_inputBitmapSize.height - sc_smallRectangleSize.height) / 2,
        (sc_inputBitmapSize.width + sc_smallRectangleSize.width) / 2,
        (sc_inputBitmapSize.height + sc_smallRectangleSize.height) / 2
        );

    D2D1_ROUNDED_RECT roundedRect1 = D2D1::RoundedRect(rectangle1, sc_roundingRadius, sc_roundingRadius);

    m_d2dContext->FillRoundedRectangle(&roundedRect1, m_coloredBrush.Get());

    // Create a larger rounded rectangle centered in the bitmap.
    D2D1_RECT_F rectangle2 = D2D1::RectF(
        (sc_inputBitmapSize.width - sc_largeRectangleSize.width) / 2,
        (sc_inputBitmapSize.height - sc_largeRectangleSize.height) / 2,
        (sc_inputBitmapSize.width + sc_largeRectangleSize.width) / 2,
        (sc_inputBitmapSize.height + sc_largeRectangleSize.height) / 2
        );

    D2D1_ROUNDED_RECT roundedRect2 = D2D1::RoundedRect(rectangle2, sc_roundingRadius, sc_roundingRadius);

    m_d2dContext->DrawRoundedRectangle(&roundedRect2, m_coloredBrush.Get(), sc_strokeWidth);

    // Draw some text onto the rectangles.
    m_d2dContext->DrawText(
        m_string->Data(),
        m_string->Length(),
        m_textFormat.Get(),
        D2D1::RectF(
            0,
            0,
            sc_inputBitmapSize.width,
            sc_inputBitmapSize.height
            ),
        m_whiteBrush.Get()
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    // Reset the device context to render to the back buffer.
    m_d2dContext->SetTarget(m_d2dTargetBitmap.Get());
}

void D2DEffectsOnPrimitives::CreateEffectGraph()
{
    ComPtr<ID2D1Effect> gaussianBlurEffect;
    ComPtr<ID2D1Effect> specularLightingEffect;
    ComPtr<ID2D1Effect> compositeEffect;

    // Create the Gaussian Blur Effect
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1GaussianBlur, &gaussianBlurEffect)
        );

    // Set the blur amount
    DX::ThrowIfFailed(
        gaussianBlurEffect->SetValue(D2D1_GAUSSIANBLUR_PROP_STANDARD_DEVIATION, sc_gaussianBlurStDev)
        );

    // Create the Specular Lighting Effect
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1PointSpecular, &specularLightingEffect)
        );

    DX::ThrowIfFailed(
        specularLightingEffect->SetValue(D2D1_POINTSPECULAR_PROP_LIGHT_POSITION, sc_specularLightPosition)
        );

    DX::ThrowIfFailed(
        specularLightingEffect->SetValue(D2D1_POINTSPECULAR_PROP_SPECULAR_EXPONENT, sc_specularExponent)
        );

    DX::ThrowIfFailed(
        specularLightingEffect->SetValue(D2D1_POINTSPECULAR_PROP_SURFACE_SCALE, sc_specularSurfaceScale)
        );

    DX::ThrowIfFailed(
        specularLightingEffect->SetValue(D2D1_POINTSPECULAR_PROP_SPECULAR_CONSTANT, sc_specularConstant)
        );

    // Create the Composite Effects
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1Composite, &compositeEffect)
        );

    DX::ThrowIfFailed(
        compositeEffect->SetValue(D2D1_COMPOSITE_PROP_MODE, D2D1_COMPOSITE_MODE_SOURCE_IN)
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1ArithmeticComposite, &m_arithmeticCompositeEffect)
        );

    DX::ThrowIfFailed(
        m_arithmeticCompositeEffect->SetValue(D2D1_ARITHMETICCOMPOSITE_PROP_COEFFICIENTS, sc_arithmeticCoefficients)
        );

    // Connect the graph.
    // Apply a blur effect to the original image.
    gaussianBlurEffect->SetInput(0, m_inputImage.Get());

    // Apply a specular lighting effect to the result.
    specularLightingEffect->SetInputEffect(0, gaussianBlurEffect.Get());

    // Compose the original bitmap under the output from lighting and blur.
    compositeEffect->SetInput(0, m_inputImage.Get());
    compositeEffect->SetInputEffect(1, specularLightingEffect.Get());

    // Compose the original bitmap under the output from lighting and blur.
    m_arithmeticCompositeEffect->SetInput(0, m_inputImage.Get());
    m_arithmeticCompositeEffect->SetInputEffect(1, compositeEffect.Get());
}

void D2DEffectsOnPrimitives::Render()
{
    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));

    D2D1_SIZE_F size = m_d2dContext->GetSize();

    // Draw the bitmap with the original primitives.
    m_d2dContext->DrawImage(
        m_inputImage.Get(),
        D2D1::Point2F(
            (size.width - sc_inputBitmapSize.width) / 2,
            (size.height - sc_inputBitmapSize.height) / 2 - sc_offset
            )
        );

    // Draw the output of the effects graph.
    m_d2dContext->DrawImage(
        m_arithmeticCompositeEffect.Get(),
        D2D1::Point2F(
            (size.width - sc_inputBitmapSize.width) / 2,
            (size.height - sc_inputBitmapSize.height) / 2 + sc_offset
            )
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

void D2DEffectsOnPrimitives::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &D2DEffectsOnPrimitives::OnActivated);
}

void D2DEffectsOnPrimitives::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &D2DEffectsOnPrimitives::OnWindowSizeChanged);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &D2DEffectsOnPrimitives::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &D2DEffectsOnPrimitives::OnDisplayContentsInvalidated);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void D2DEffectsOnPrimitives::Load(
    _In_ Platform::String^ entryPoint
    )
{
    // This method is intentionally left empty.
}

void D2DEffectsOnPrimitives::Run()
{
    Render();
    Present();

    m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void D2DEffectsOnPrimitives::Uninitialize()
{
    // This method is intentionally left empty.
}

void D2DEffectsOnPrimitives::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
    Render();
    Present();
}

void D2DEffectsOnPrimitives::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void D2DEffectsOnPrimitives::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
    Present();
}

void D2DEffectsOnPrimitives::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new D2DEffectsOnPrimitives();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
