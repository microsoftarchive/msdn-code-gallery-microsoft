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

static const float sc_maxZoom = 4.0f;
static const float sc_recenterAnimationDuration = 0.25f; // 250ms

PhotoRenderer::PhotoRenderer() :
    // Lesson 2:
    // Initialize defaults for the transformation variables.
    m_viewPosition(),
    m_zoom(1.0f),
    m_minZoom(1.0f), // Min zoom is dynamically calculated
    m_bitmapPixelSize(),
    m_imageSize(),
    m_contextSize(),
    m_isWindowClosed(false),
    m_renderingMode(RenderingMode::WaitForEvents),
    m_recenterStartZoom(),
    m_recenterStartPosition()
{
    m_timer = ref new BasicTimer();
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
        "Direct2D photo adjustment sample lesson 2: pan & zoom"
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

    // Lesson 2:
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

    // Lesson 2:
    // Create the 2DAffineTransform effect and add it to the end of the effect graph.
    // Note that, although 2DAffineTransform supports GPU accelerated scaling, for this
    // scenario we are using BitmapSource to scale the image.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D12DAffineTransform, &m_2dTransformEffect)
        );

    m_2dTransformEffect->SetInputEffect(0, m_bitmapSourceEffect.Get());
}

void PhotoRenderer::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    m_contextSize = m_d2dContext->GetSize();

    // Lesson 2:
    // m_imageSize is in DIPs and changes depending on the app DPI.
    // This app ignores the source bitmap's DPI and assumes it is 96, in order to
    // display it at full resolution.
    m_imageSize = D2D1::SizeF(
        static_cast<float>(m_bitmapPixelSize.width) * 96.0f / m_dpi,
        static_cast<float>(m_bitmapPixelSize.height) * 96.0f / m_dpi
        );

    // Lesson 2:
    // Compute the minimum zoom to ensure the image fits on the screen.
    m_minZoom = max(
        m_contextSize.width / m_imageSize.width,
        m_contextSize.height / m_imageSize.height
        );

    m_zoom = m_minZoom;

    // Ensure the image stays within the bounds of the window if it is resized.
    ClampViewPosition(&m_viewPosition, m_contextSize, m_imageSize, m_zoom);
}

void PhotoRenderer::Render()
{
    m_d2dContext->BeginDraw();

    // Lesson 2:
    // Now we render the 2DAffineTransform effect. The source image we are rendering does
    // not contain alpha; as an optimization we use the source copy composite mode which
    // ignores alpha. If we were rendering an image with alpha (e.g. a PNG image with alpha)
    // then we would need to use another composite mode such as source over.
    // The interpolation mode is ignored here because there is no world transform
    // (no scaling/interpolation in the draw operation).
    m_d2dContext->DrawImage(
        m_2dTransformEffect.Get(),
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

// Lesson 2:
// Update the animation state for a single frame.
void PhotoRenderer::UpdateRecenterAnimation()
{
    // Track timing information.
    // We collect timing information so we can tell the renderer how much time has passed
    // from frame to frame. This ensures consistent, smooth animations.
    m_timer->Update();
    float delta = m_timer->Total / sc_recenterAnimationDuration;
    delta = Clamp(delta, 0.0f, 1.0f);

    // Linearly interpolate between the starting position and the reset position.
    m_viewPosition.x = LinearInterpolate(m_recenterStartPosition.x, 0, delta);
    m_viewPosition.y = LinearInterpolate(m_recenterStartPosition.y, 0, delta);
    m_zoom = LinearInterpolate(m_recenterStartZoom, m_minZoom, delta);

    // Prevent the view from going outside the image boundaries while zooming out.
    ClampViewPosition(&m_viewPosition, m_contextSize, m_imageSize, m_zoom);

    UpdatePanZoomEffectValues();

    if (delta >= 1.0f)
    {
        m_renderingMode = RenderingMode::WaitForEvents;
    }
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

    window->Closed +=
        ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &PhotoRenderer::OnWindowClosed);

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &PhotoRenderer::OnPointerPressed);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &PhotoRenderer::OnPointerReleased);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &PhotoRenderer::OnPointerMoved);

    window->PointerWheelChanged +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &PhotoRenderer::OnPointerWheelChanged);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &PhotoRenderer::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &PhotoRenderer::OnDisplayContentsInvalidated);

    // Lesson 2:
    // We will use the gesture recognizer to compute the touch gestures, like pinching and dragging,
    // into transformations we can use in Direct2D. We simply funnel any touch events into it
    // and the gesture recognizer will fire events when the transformations need to be updated or
    // a specific gesture (e.g. double tap) occurs.
    m_gestureRecognizer = ref new GestureRecognizer();

    m_gestureRecognizer->GestureSettings =
        GestureSettings::DoubleTap                     |
        GestureSettings::ManipulationTranslateX        |
        GestureSettings::ManipulationTranslateY        |
        GestureSettings::ManipulationScale             |
        GestureSettings::ManipulationTranslateInertia  |
        GestureSettings::ManipulationScaleInertia;

    m_gestureRecognizer->Tapped +=
        ref new TypedEventHandler<GestureRecognizer^, TappedEventArgs^>(this, &PhotoRenderer::OnTapped);

    m_gestureRecognizer->ManipulationUpdated +=
        ref new TypedEventHandler<GestureRecognizer^, ManipulationUpdatedEventArgs^>(
            this,
            &PhotoRenderer::OnManipulationUpdated
            );

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

// Lesson 2:
// Retrieve application state from LocalSettings if the application was previously suspended.
void PhotoRenderer::Load(
    _In_ Platform::String^ entryPoint
    )
{
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("viewPositionX"))
    {
        m_viewPosition.x = safe_cast<IPropertyValue^>(settingsValues->Lookup("viewPositionX"))->GetSingle();
    }

    if (settingsValues->HasKey("viewPositionY"))
    {
        m_viewPosition.y = safe_cast<IPropertyValue^>(settingsValues->Lookup("viewPositionY"))->GetSingle();
    }

    if (settingsValues->HasKey("zoom"))
    {
        m_zoom = safe_cast<IPropertyValue^>(settingsValues->Lookup("zoom"))->GetSingle();
    }

    // Lesson 2:
    // Update the effects properties state using the restored values.
    UpdatePanZoomEffectValues();
}

void PhotoRenderer::Run()
{
    Render();

    // Lesson 2:
    // Most of the time we wait for events (RenderingMode::WaitForEvents), but when the pan/zoom
    // recenter animation is running (RunRecenterAnimation) we use a realtime rendering loop.
    while (!m_isWindowClosed)
    {
        switch (m_renderingMode)
        {
            case RenderingMode::WaitForEvents:
                // ProcessOneAndAllPending blocks until one or more events are fired, and processes
                // all of them at once.
                m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessOneAndAllPending);
                break;

            case RenderingMode::RunRecenterAnimation:
                // ProcessAllIfPresent processes all queued events and then returns.
                m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);
                UpdateRecenterAnimation();
                Render();
                break;
        }
    }
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
    UpdatePanZoomEffectValues();
    Render();
}

void PhotoRenderer::OnWindowClosed(
    _In_ CoreWindow^ window,
    _In_ CoreWindowEventArgs^ args
    )
{
    m_isWindowClosed = true;
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

    UpdatePanZoomEffectValues();
    Render();
}

void PhotoRenderer::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

// Lesson 2:
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
    if (settingsValues->HasKey("zoom"))
    {
        settingsValues->Remove("zoom");
    }
    settingsValues->Insert("zoom", PropertyValue::CreateSingle(m_zoom));

    if (settingsValues->HasKey("viewPositionX"))
    {
        settingsValues->Remove("viewPositionX");
    }
    settingsValues->Insert("viewPositionX", PropertyValue::CreateSingle(m_viewPosition.x));

    if (settingsValues->HasKey("viewPositionY"))
    {
        settingsValues->Remove("viewPositionY");
    }
    settingsValues->Insert("viewPositionY", PropertyValue::CreateSingle(m_viewPosition.y));
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

void PhotoRenderer::OnPointerWheelChanged(
    _In_ Windows::UI::Core::CoreWindow^ window,
    _In_ Windows::UI::Core::PointerEventArgs^ args
    )
{
    // Lesson 2:
    // We are ignoring the gesture that wheel movement means panning.
    // Zooming is indicated by holding the Control key down, so we
    // override this method to always detect a zoom wheel event.
    m_gestureRecognizer->ProcessMouseWheelEvent(args->CurrentPoint, FALSE, TRUE);
}

// Lesson 2:
// In this method, we use the gesture recognizer data to update the transformation variables
// to reflect how the user is manipulating the image.
// The gesture recognizer provides the following data:
// 1. position (pixel offset from the screen origin to the pointer's current position)
// 2. positionDelta (pixel offset from the previous event's pointer to the current pointer)
// 3. zoomDelta (coefficient indicating the change in zoom)
void PhotoRenderer::OnManipulationUpdated(
    _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
    _In_ Windows::UI::Input::ManipulationUpdatedEventArgs^ args
    )
{
    Point position = args->Position;
    Point positionDelta = args->Delta.Translation;
    float zoomDelta = args->Delta.Scale;

    // Reposition the view to reflect translations.
    m_viewPosition.x += positionDelta.X;
    m_viewPosition.y += positionDelta.Y;

    // We want to have any zoom operation be "centered" around the pointer position, which
    // requires recalculating the view position based on the new zoom and pointer position.
    // Step 1: Calculate the absolute pointer position (image position).
    D2D1_POINT_2F pointerAbsolutePosition = D2D1::Point2F(
        (m_viewPosition.x - position.X) / m_zoom,
        (m_viewPosition.y - position.Y) / m_zoom
        );

    // Step 2: Apply the zoom operation and clamp to the min/max zoom.
    // zoomDelta is a coefficient for the change in zoom.
    m_zoom *= zoomDelta;
    m_zoom = Clamp(m_zoom, m_minZoom, sc_maxZoom);

    // Step 3: Adjust the view position based on the new m_zoom value.
    m_viewPosition.x = pointerAbsolutePosition.x * m_zoom + position.X;
    m_viewPosition.y = pointerAbsolutePosition.y * m_zoom + position.Y;

    // Prevent the view from going outside the image boundaries.
    ClampViewPosition(&m_viewPosition, m_contextSize, m_imageSize, m_zoom);

    UpdatePanZoomEffectValues();
    Render();
}

// Lesson 2:
// This method is separate from OnManipulationUpdated to allow us to manually call it
// when we need to regenerate the effects state (e.g. when resuming from previously saved state).
void PhotoRenderer::UpdatePanZoomEffectValues()
{
    // Lesson 2:
    // Recall that zoom/scaling is handled by BitmapSource.
    D2D1_VECTOR_2F zoom = D2D1::Vector2F(m_zoom, m_zoom);
    DX::ThrowIfFailed(
        m_bitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_SCALE, zoom)
        );

    D2D1::Matrix3x2F translation = D2D1::Matrix3x2F::Translation(m_viewPosition.x, m_viewPosition.y);
    DX::ThrowIfFailed(
        m_2dTransformEffect->SetValue(D2D1_2DAFFINETRANSFORM_PROP_TRANSFORM_MATRIX, translation)
        );
}

void PhotoRenderer::OnTapped(
    _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
    _In_ Windows::UI::Input::TappedEventArgs^ args
    )
{
    if (args->TapCount == 2)
    {
        // Lesson 2:
        // A double tap means we want to reset the position to (0, 0) and zoom to minimum.
        // Initialize the animation state variables and trigger the animation mode.
        InitializeRecenterAnimation();
    }
}

void PhotoRenderer::InitializeRecenterAnimation()
{
    m_recenterStartPosition = m_viewPosition;
    m_recenterStartZoom = m_zoom;

    m_timer->Reset();

    m_renderingMode = RenderingMode::RunRecenterAnimation;
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
