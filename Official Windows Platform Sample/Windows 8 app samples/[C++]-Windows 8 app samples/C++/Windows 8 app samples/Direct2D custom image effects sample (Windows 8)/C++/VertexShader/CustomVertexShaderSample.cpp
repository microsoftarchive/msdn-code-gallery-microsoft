//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "CustomVertexShaderSample.h"

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;
using namespace Windows::System;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;

CustomVertexShaderSample::CustomVertexShaderSample() :
    m_skewX(0),
    m_skewY(0),
    m_isWindowClosed(false)
{
    m_timer = ref new BasicTimer();
}

void CustomVertexShaderSample::CreateDeviceIndependentResources()
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

    // Register the custom wave effect.
    DX::ThrowIfFailed(
        WaveEffect::Register(m_d2dFactory.Get())
        );
}

void CustomVertexShaderSample::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D custom vertex shader effect sample"
        );

    // Create BitmapSource effect to use WIC image in Direct2D.
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
    // The property system expects TRUE and FALSE rather than true or false to ensure that the size of
    // the value is consistent regardless of architecture.
    DX::ThrowIfFailed(m_bitmapSourceEffect->SetValue(D2D1_PROPERTY_CACHED, TRUE));

    DX::ThrowIfFailed(m_d2dContext->CreateEffect(CLSID_CustomWaveEffect, &m_waveEffect));

    // Set the BitmapSource effect as the input to the custom wave effect.
    m_waveEffect->SetInputEffect(0, m_bitmapSourceEffect.Get());
}

void CustomVertexShaderSample::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    // Scale image using BitmapSource effect.
    D2D1_VECTOR_2F scale;
    scale.x = scale.y = m_renderTargetSize.Width / m_imageSize.width;

    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_SCALE, scale)
        );
}

void CustomVertexShaderSample::Render()
{
    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));

    m_d2dContext->DrawImage(m_waveEffect.Get());

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

void CustomVertexShaderSample::Update()
{
    m_timer->Update();

    DX::ThrowIfFailed(
        m_waveEffect->SetValue(WAVE_PROP_OFFSET, m_timer->Total)
        );

    Render();
    Present();
}

void CustomVertexShaderSample::OnPointerPressed(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessDownEvent(args->CurrentPoint);
}

void CustomVertexShaderSample::OnPointerReleased(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessUpEvent(args->CurrentPoint);
}

void CustomVertexShaderSample::OnPointerMoved(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessMoveEvents(args->GetIntermediatePoints());
}

void CustomVertexShaderSample::OnPointerWheelChanged(
    _In_ CoreWindow^ sender,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessMouseWheelEvent(args->CurrentPoint, false, true);
}

void CustomVertexShaderSample::OnManipulationUpdated(
    _In_ GestureRecognizer^ sender,
    _In_ ManipulationUpdatedEventArgs^ args
    )
{
    m_skewX += args->Delta.Translation.X * .01f;
    m_skewY += args->Delta.Translation.Y * .01f;

    DX::ThrowIfFailed(
        m_waveEffect->SetValue(WAVE_PROP_SKEW_X, m_skewX)
        );

    DX::ThrowIfFailed(
        m_waveEffect->SetValue(WAVE_PROP_SKEW_Y, m_skewY)
        );
}

void CustomVertexShaderSample::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &CustomVertexShaderSample::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &CustomVertexShaderSample::OnSuspending);
}

void CustomVertexShaderSample::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &CustomVertexShaderSample::OnWindowSizeChanged);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &CustomVertexShaderSample::OnLogicalDpiChanged);

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CustomVertexShaderSample::OnPointerPressed);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CustomVertexShaderSample::OnPointerReleased);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CustomVertexShaderSample::OnPointerMoved);

    window->PointerWheelChanged +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CustomVertexShaderSample::OnPointerWheelChanged);

    // The gesture recognizer automatically recognizes actions such as zooms
    // (whether by pinching or using the scroll wheel) and cursor movement.
    m_gestureRecognizer = ref new GestureRecognizer();

    m_gestureRecognizer->GestureSettings =
        GestureSettings::ManipulationTranslateX        |
        GestureSettings::ManipulationTranslateY        |
        GestureSettings::ManipulationTranslateInertia;

    m_gestureRecognizer->ManipulationUpdated +=
        ref new TypedEventHandler<GestureRecognizer^, ManipulationUpdatedEventArgs^>(this, &CustomVertexShaderSample::OnManipulationUpdated);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void CustomVertexShaderSample::Load(
    _In_ Platform::String^ entryPoint
    )
{
    // Retrieve user-manipulated variables from the LocalSettings collection if the app was previously suspended.
    IPropertySet^ storedValues = ApplicationData::Current->LocalSettings->Values;

    if (storedValues->HasKey("m_skewX"))
    {
        m_skewX = safe_cast<IPropertyValue^>(storedValues->Lookup("m_skewX"))->GetSingle();
    }

    if (storedValues->HasKey("m_skewY"))
    {
        m_skewY = safe_cast<IPropertyValue^>(storedValues->Lookup("m_skewY"))->GetSingle();
    }

    // Set perspective values for custom wave effect.
    DX::ThrowIfFailed(m_waveEffect->SetValue(WAVE_PROP_SKEW_X, m_skewX));
    DX::ThrowIfFailed(m_waveEffect->SetValue(WAVE_PROP_SKEW_Y, m_skewY));
}

void CustomVertexShaderSample::Run()
{
    while (!m_isWindowClosed)
    {
        m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);
        Update();
    }
}

void CustomVertexShaderSample::Uninitialize()
{
}

void CustomVertexShaderSample::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    // Store user-manipulated properties in the LocalSettings collection.
    IPropertySet^ storedValues = ApplicationData::Current->LocalSettings->Values;

    // Check to ensure each key is not already in the collection. If it is present, remove
    // it, before storing in the new value. These values will be retrieved in the Load method.
    if (storedValues->HasKey("m_skewX"))
    {
        storedValues->Remove("m_skewX");
    }
    storedValues->Insert("m_skewX", PropertyValue::CreateSingle(m_skewX));

    if (storedValues->HasKey("m_skewY"))
    {
        storedValues->Remove("m_skewY");
    }
    storedValues->Insert("m_skewY", PropertyValue::CreateSingle(m_skewY));
}

void CustomVertexShaderSample::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
}

void CustomVertexShaderSample::OnWindowClosed(
    _In_ CoreWindow^ sender,
    _In_ CoreWindowEventArgs^ args
    )
{
    m_isWindowClosed = true;
}

void CustomVertexShaderSample::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    m_sampleOverlay->UpdateForWindowSizeChange();
    UpdateForWindowSizeChange();
}

void CustomVertexShaderSample::OnActivated(
    _In_ CoreApplicationView^ sender,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new CustomVertexShaderSample();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
