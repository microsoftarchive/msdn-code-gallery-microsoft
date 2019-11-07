//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "D2DPerspectiveTransform.h"

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;

D2DPerspectiveTransform::D2DPerspectiveTransform():
    m_bitmapPerspective(nullptr),
    m_rotationAngle(0.0f)
{
}

void D2DPerspectiveTransform::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();
}

void D2DPerspectiveTransform::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D perspective transform sample"
        );

    // Load a bitmap from the Windows Imaging Component decoder.
    ComPtr<IWICBitmapDecoder> decoder;
    DX::ThrowIfFailed(
        m_wicFactory->CreateDecoderFromFilename(
            L"mammoth_small.jpg",
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

    ComPtr<IWICFormatConverter> converter;
    DX::ThrowIfFailed(
        m_wicFactory->CreateFormatConverter(&converter)
        );

    // We format convert to a pixel format that is compatible with Direct2D.
    // To optimize for performance when using WIC and Direct2D together, we need to
    // select the target pixel format based on the image's native precision:
    // - <= 8 bits per channel precision: use BGRA channel order
    //   (example: all JPEGs, including the image in this sample, are 8bpc)
    // -  > 8 bits per channel precision: use RGBA channel order
    //   (example: TIFF and JPEG-XR images support 32bpc float
    // Note that a fully robust system will arbitrate between various WIC pixel formats and
    // hardware feature level support using the IWICPixelFormatInfo2 interface.
    DX::ThrowIfFailed(
        converter->Initialize(
            frame.Get(),
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.0f,
            WICBitmapPaletteTypeCustom
            )
        );

    // Create bitmap from WIC bitmap.
    DX::ThrowIfFailed(
        m_d2dContext->CreateBitmapFromWicBitmap(
            converter.Get(),
            nullptr,
            &m_bitmapPerspective
            )
        );
}

void D2DPerspectiveTransform::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();
}

void D2DPerspectiveTransform::Render()
{
    m_d2dContext->BeginDraw();
    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));

    D2D1_SIZE_F size = m_d2dContext->GetSize();

    D2D1_RECT_F destRect = D2D1::RectF(
        size.width / 4,
        size.height / 4,
        3 * size.width / 4,
        3 * size.height / 4
        );

    // Center the Image and apply the perspective transformation with rotation along the Y-axis.
    D2D1::Matrix4x4F matrix =
        D2D1::Matrix4x4F::Translation(-size.width/ 2, -size.height / 2, 0.0f) *
        D2D1::Matrix4x4F::RotationY(m_rotationAngle) *
        D2D1::Matrix4x4F::PerspectiveProjection(size.width/ 2) *
        D2D1::Matrix4x4F::Translation(size.width/ 2, size.height / 2, 0.0f);

    m_d2dContext->DrawBitmap(
        m_bitmapPerspective.Get(),
        &destRect,
        1.0f, // Draw the bitmap fully opaque.
        D2D1_INTERPOLATION_MODE_LINEAR,
        nullptr, // Use the full source bitmap for sampling.
        &matrix
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

void D2DPerspectiveTransform::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &D2DPerspectiveTransform::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &D2DPerspectiveTransform::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &D2DPerspectiveTransform::OnResuming);
}

void D2DPerspectiveTransform::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &D2DPerspectiveTransform::OnWindowSizeChanged);

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &D2DPerspectiveTransform::OnPointerPressed);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &D2DPerspectiveTransform::OnPointerReleased);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &D2DPerspectiveTransform::OnPointerMoved);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &D2DPerspectiveTransform::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &D2DPerspectiveTransform::OnDisplayContentsInvalidated);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);

    m_gestureRecognizer = ref new GestureRecognizer();

    m_gestureRecognizer->AutoProcessInertia = false;

    m_gestureRecognizer->GestureSettings = GestureSettings::ManipulationTranslateX;

    m_gestureRecognizer->ManipulationUpdated +=
        ref new TypedEventHandler<GestureRecognizer^, ManipulationUpdatedEventArgs^>(this, &D2DPerspectiveTransform::OnManipulationUpdated);
}

void D2DPerspectiveTransform::Load(
    _In_ Platform::String^ entryPoint
    )
{
    // Retrieve any stored variables from the LocalSettings collection.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("m_rotationAngle"))
    {
        m_rotationAngle = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_rotationAngle"))->GetSingle();
    }
}

void D2DPerspectiveTransform::Run()
{
    Render();
    Present();

    m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void D2DPerspectiveTransform::Uninitialize()
{
}

void D2DPerspectiveTransform::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
    Render();
    Present();
}

void D2DPerspectiveTransform::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void D2DPerspectiveTransform::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
    Present();
}

void D2DPerspectiveTransform::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

void D2DPerspectiveTransform::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    // Store rotation angle in the LocalSettings collection.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("m_rotationAngle"))
    {
        settingsValues->Remove("m_rotationAngle");
    }
    settingsValues->Insert("m_rotationAngle", PropertyValue::CreateSingle(m_rotationAngle));
}

void D2DPerspectiveTransform::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

void D2DPerspectiveTransform::OnPointerPressed(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessDownEvent(args->CurrentPoint);
}

void D2DPerspectiveTransform::OnPointerReleased(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessUpEvent(args->CurrentPoint);
}

void D2DPerspectiveTransform::OnPointerMoved(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessMoveEvents(args->GetIntermediatePoints());
}

void D2DPerspectiveTransform::OnManipulationUpdated(
    _In_ GestureRecognizer^ gestureRecognizer,
    _In_ ManipulationUpdatedEventArgs^ args)
{
    m_rotationAngle += args->Delta.Translation.X / 3.0f; // The divisor in the equation slows the pace of the rotation.

    Render();
    Present();
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new D2DPerspectiveTransform();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}