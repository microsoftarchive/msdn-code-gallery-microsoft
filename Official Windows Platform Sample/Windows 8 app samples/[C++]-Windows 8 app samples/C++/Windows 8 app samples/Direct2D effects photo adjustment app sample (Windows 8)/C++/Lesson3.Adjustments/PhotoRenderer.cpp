//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "PhotoRenderer.h"

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

static const float SaturationDefault = 1.0f; // The default value of D2D1_SATURATION_PROP_SATURATION
static const float BrightnessDefault = 0.5f; // Equivalent to default D2D1_BRIGHTNESS_PROP_WHITEPOINT
                                             // and D2D1_BRIGHTNESS_PROP_BLACKPOINT values

PhotoRenderer::PhotoRenderer() :
    m_bitmapPixelSize(),
    m_contextSize(),

    // Lesson 3:
    // Set the defaults for the new photo adjustment values.
    m_brightness(BrightnessDefault),
    m_saturation(SaturationDefault)
{
}

void PhotoRenderer::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    // Load the source image using a Windows Imaging Component decoder.
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
        m_wicFormatConverter->Initialize(
            frame.Get(),
            GUID_WICPixelFormat32bppPBGRA,
            WICBitmapDitherTypeNone,
            nullptr,
            0.0f,
            WICBitmapPaletteTypeCustom // premultiplied BGRA has no paletting, so this is ignored
            )
        );

    UINT width;
    UINT height;
    DX::ThrowIfFailed(
        m_wicFormatConverter->GetSize(&width, &height)
        );

    // Store the WIC bitmap size in pixels - this remains constant regardless of the app's DPI.
    m_bitmapPixelSize = D2D1::SizeU(width, height);
}

void PhotoRenderer::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D photo adjustment sample lesson 3: photo adjustments"
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1BitmapSource, &m_bitmapSourceEffect)
        );

    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(
            D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE,
            m_wicFormatConverter.Get()
            )
        );

    // The mipmap linear interpolation mode on the BitmapSource effect instructs it to construct
    // a software (CPU memory) mipmap. If Direct2D requests the image at a small scale factor
    // (<= 0.5), BitmapSource only needs to perform the scale operation from the nearest
    // mip level, which is a performance optimization. In addition, for very large images,
    // performing scaling on the CPU reduces GPU bus bandwidth consumption and GPU memory
    // consumption.
    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(
            D2D1_BITMAPSOURCE_PROP_INTERPOLATION_MODE,
            D2D1_BITMAPSOURCE_INTERPOLATION_MODE_MIPMAP_LINEAR
            )
        );

    // Lesson 3:
    // Initialize the brightness and saturation effects.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1Brightness, &m_brightnessEffect)
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1Saturation, &m_saturationEffect)
        );

    // Lesson 3:
    // Add the effects to the end of the effect graph.
    m_saturationEffect->SetInputEffect(0, m_bitmapSourceEffect.Get());
    m_brightnessEffect->SetInputEffect(0, m_saturationEffect.Get());
}

void PhotoRenderer::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    m_contextSize = m_d2dContext->GetSize();

    // Compute a zoom value so that the entire image fits on the screen.
    float zoom = max(
        m_renderTargetSize.Width / m_bitmapPixelSize.width,
        m_renderTargetSize.Height / m_bitmapPixelSize.height
        );

    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(
            D2D1_BITMAPSOURCE_PROP_SCALE,
            D2D1::Vector2F(
                zoom,
                zoom
                )
            )
        );
}

void PhotoRenderer::Render()
{
    m_d2dContext->BeginDraw();

    // Now we render the end of the graph (brightness). The source image we are rendering does
    // not contain alpha; as an optimization we use the source copy composite mode which
    // ignores alpha. If we were rendering an image with alpha (e.g. a PNG image with alpha)
    // then we would need to use another composite mode such as source over.
    // The interpolation mode is ignored here because there is no world transform
    // (no scaling/interpolation in the draw operation).
    m_d2dContext->DrawImage(
        m_brightnessEffect.Get(),
        D2D1_INTERPOLATION_MODE_LINEAR,
        D2D1_COMPOSITE_MODE_SOURCE_COPY
        );

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    m_sampleOverlay->Render();

    Present();
}

void PhotoRenderer::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &PhotoRenderer::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &PhotoRenderer::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &PhotoRenderer::OnResuming);
}

void PhotoRenderer::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(
            this,
            &PhotoRenderer::OnWindowSizeChanged
            );

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &PhotoRenderer::OnPointerPressed);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &PhotoRenderer::OnPointerReleased);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &PhotoRenderer::OnPointerMoved);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &PhotoRenderer::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &PhotoRenderer::OnDisplayContentsInvalidated);

    // We will use the gesture recognizer to translate touch gestures into changes in brightness
    // and saturation. We simply funnel any touch events into it and the gesture
    // recognizer will fire events when the transformations need to be updated or
    // a specific gesture (e.g. double tap) occurs.
    m_gestureRecognizer = ref new GestureRecognizer();

    m_gestureRecognizer->GestureSettings =
        GestureSettings::DoubleTap                     |
        GestureSettings::ManipulationTranslateX        |
        GestureSettings::ManipulationTranslateY        |
        GestureSettings::ManipulationTranslateInertia;

    m_gestureRecognizer->Tapped +=
        ref new TypedEventHandler<GestureRecognizer^, TappedEventArgs^>(this, &PhotoRenderer::OnTapped);

    m_gestureRecognizer->ManipulationUpdated +=
        ref new TypedEventHandler<GestureRecognizer^, ManipulationUpdatedEventArgs^>(
            this,
            &PhotoRenderer::OnManipulationUpdated
            );

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

// Retrieve application state from LocalSettings if the application was previously suspended.
void PhotoRenderer::Load(
    _In_ Platform::String^ entryPoint
    )
{
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("saturation"))
    {
        m_saturation = safe_cast<IPropertyValue^>(settingsValues->Lookup("saturation"))->GetSingle();
    }

    if (settingsValues->HasKey("brightness"))
    {
        m_brightness = safe_cast<IPropertyValue^>(settingsValues->Lookup("brightness"))->GetSingle();
    }

    // Update the effects properties state using the restored values.
    UpdateAdjustmentEffectValues();
}

void PhotoRenderer::Run()
{
    Render();

    // ProcessUntilQuit blocks until the app closes (while still firing input events).
    m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void PhotoRenderer::Uninitialize()
{
}

void PhotoRenderer::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
    Render();
}

void PhotoRenderer::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
}

void PhotoRenderer::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    UpdateAdjustmentEffectValues();
    Render();
}

void PhotoRenderer::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

// Save application state when suspending. If the application is closed by the process lifetime
// manager, this allows it to resume in the same state as the user left it.
void PhotoRenderer::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    // Check to ensure each key is not already in the collection before storing it.
    // These values are read in the Load method.
    if (settingsValues->HasKey("saturation"))
    {
        settingsValues->Remove("saturation");
    }
    settingsValues->Insert("saturation", PropertyValue::CreateSingle(m_saturation));

    if (settingsValues->HasKey("brightness"))
    {
        settingsValues->Remove("brightness");
    }
    settingsValues->Insert("brightness", PropertyValue::CreateSingle(m_brightness));
}

void PhotoRenderer::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

void PhotoRenderer::OnPointerPressed(
    _In_ Windows::UI::Core::CoreWindow^ window,
    _In_ Windows::UI::Core::PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessDownEvent(args->CurrentPoint);
}

void PhotoRenderer::OnPointerReleased(
    _In_ Windows::UI::Core::CoreWindow^ window,
    _In_ Windows::UI::Core::PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessUpEvent(args->CurrentPoint);
}

void PhotoRenderer::OnPointerMoved(
    _In_ Windows::UI::Core::CoreWindow^ window,
    _In_ Windows::UI::Core::PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessMoveEvents(args->GetIntermediatePoints());
}

// In this method, we use the gesture recognizer data to update the brightness and saturation values.
void PhotoRenderer::OnManipulationUpdated(
    _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
    _In_ Windows::UI::Input::ManipulationUpdatedEventArgs^ args
    )
{
    Point positionDelta = args->Delta.Translation;

    // Lesson 3:
    // Swiping left to right changes brightness, and up and down changes saturation.
    m_saturation = Clamp(m_saturation - positionDelta.Y / m_contextSize.height, 0.0f, 1.0f);
    // We limit the brightness range to "reasonable" values.
    m_brightness = Clamp(m_brightness + positionDelta.X / m_contextSize.width, 0.2f, 0.8f);

    UpdateAdjustmentEffectValues();

    Render();
}

void PhotoRenderer::UpdateAdjustmentEffectValues()
{
    // Lesson 3:
    // The brightness effect takes 2 points and determines the brightness transfer function
    // by drawing a straight line between them. The points computed here darken light values
    // when dragging to the right, and lighten dark values when dragging to the left.
    // Note that the default m_brightness value of 0.5f results in a whitepoint of (1, 1) and
    // a blackpoint of (0, 0), which are the default property values.
    D2D1_POINT_2F whitepointValue = D2D1::Point2F(1.0f, Clamp(m_brightness * 2.0f, 0, 1));
    D2D1_POINT_2F blackpointValue = D2D1::Point2F(0.0f, Clamp(m_brightness * 2.0f - 1.0f, 0, 1));

    DX::ThrowIfFailed(
        m_brightnessEffect->SetValue(D2D1_BRIGHTNESS_PROP_WHITE_POINT, whitepointValue)
        );

    DX::ThrowIfFailed(
        m_brightnessEffect->SetValue(D2D1_BRIGHTNESS_PROP_BLACK_POINT, blackpointValue)
        );

    DX::ThrowIfFailed(
        m_saturationEffect->SetValue(D2D1_SATURATION_PROP_SATURATION, m_saturation)
        );
}

void PhotoRenderer::OnTapped(
    _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
    _In_ Windows::UI::Input::TappedEventArgs^ args
    )
{
    if (args->TapCount == 2)
    {
        // Return the adjustment values to their defaults.
        ResetAdjustmentState();
    }
}

void PhotoRenderer::ResetAdjustmentState()
{
    // Reset the saturation and brightness effects to their default property values.
    m_saturation = SaturationDefault;
    m_brightness = BrightnessDefault;

    UpdateAdjustmentEffectValues();

    Render();
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new PhotoRenderer();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
