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
using namespace Windows::System::Threading;
using namespace concurrency;

static const float sc_maxZoom = 4.0f;
static const float sc_recenterAnimationDuration = 0.25f; // 250ms

PhotoRenderer::PhotoRenderer() :
    // Initialize defaults for the transformation variables.
    m_viewPosition(),
    m_zoom(1.0f),
    m_minZoom(1.0f), // Min zoom is dynamically calculated
    m_thumbnailPixelSize(),
    m_thumbnailSize(),
    m_imagePixelSize(),
    m_imageSize(),
    m_contextSize(),
    m_isWindowClosed(false),
    m_renderingMode(RenderingMode::WaitForEvents),
    m_recenterStartZoom(),
    m_recenterStartPosition(),
    m_imageLoaded(false)
{
    m_timer = ref new BasicTimer();
}

void PhotoRenderer::HandleDeviceLost()
{
    m_fullResBitmapSourceEffect = nullptr;
    m_2dTransformEffectB = nullptr;
    m_2dTransformFullCached = nullptr;
    m_2dTransformHalfCached = nullptr;
    m_2dTransformQuarterCached = nullptr;

    DirectXBase::HandleDeviceLost();
}

void PhotoRenderer::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    CreateWICBitmapSourceFromFilename("mammoth_thumbnail.jpg", &m_thumbnailWicBitmapSource, &m_thumbnailPixelSize);
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
        "Direct2D photo adjustment sample lesson 5: asynchronous image load"
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D1BitmapSource, &m_thumbnailBitmapSourceEffect)
        );

    DX::ThrowIfFailed(
        m_thumbnailBitmapSourceEffect->SetValue(
            D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE,
            m_thumbnailWicBitmapSource.Get()
            )
        );

    // Create the 2DAffineTransform effect and add it to the end of the effect graph.
    // This effect will pan/zoom the initial thumbnail that is displayed while the
    // full-res image is decoded in the background.
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(CLSID_D2D12DAffineTransform, &m_2dTransformEffect)
        );

    m_2dTransformEffect->SetInputEffect(0, m_thumbnailBitmapSourceEffect.Get());

    // Lesson 5:
    // Create background D3D device that will load full-res image on a background thread.
    m_imageLoaded = false;
    CreateBackgroundDeviceResources();
}

void PhotoRenderer::CreateWindowSizeDependentResources()
{
    DirectXBase::CreateWindowSizeDependentResources();

    m_contextSize = m_d2dContext->GetSize();

    if (m_imageLoaded)
    {
        // If the full-res image has finished loading in the background,
        // compute new minimum zoom value based on full-res image size.
        m_imageSize = D2D1::SizeF(
            static_cast<float>(m_imagePixelSize.width) * 96.0f / m_dpi,
            static_cast<float>(m_imagePixelSize.height) * 96.0f / m_dpi
            );

        float newMinZoom = max(
            m_contextSize.width / m_imageSize.width,
            m_contextSize.height / m_imageSize.height
            );

        // Adjust zoom value so that effective zoom remains constant between thumbnail image and full-res image.
        m_zoom = m_zoom / m_minZoom * newMinZoom;
        m_minZoom = newMinZoom;

        // Ensure the image stays within the bounds of the window if it is resized.
        ClampViewPosition(&m_viewPosition, m_contextSize, m_imageSize, m_zoom);
    }
    else
    {
        // m_thumbnailSize is in DIPs and changes depending on the app DPI.
        // This app ignores the source bitmap's DPI and assumes it is 96, in order to
        // display it at full resolution.
        m_thumbnailSize = D2D1::SizeF(
            static_cast<float>(m_thumbnailPixelSize.width) * 96.0f / m_dpi,
            static_cast<float>(m_thumbnailPixelSize.height) * 96.0f / m_dpi
            );

        // Compute the minimum zoom to ensure the intial thumbnail fits on the screen.
        m_minZoom = max(
            m_contextSize.width / m_thumbnailSize.width,
            m_contextSize.height / m_thumbnailSize.height
            );

        m_zoom = m_minZoom;

        // Ensure the image stays within the bounds of the window if it is resized.
        ClampViewPosition(&m_viewPosition, m_contextSize, m_thumbnailSize, m_zoom);
    }
}

void PhotoRenderer::Render()
{
    m_d2dContext->BeginDraw();

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

// Update the animation state for a single frame.
void PhotoRenderer::UpdateRecenterAnimation()
{
    // Track timing information.
    // We collect timing information so we can tell the renderer how much time has passed
    // from frame to frame. This ensures consistent, smooth animations.
    m_timer->Update();
    float delta = m_timer->Total  / sc_recenterAnimationDuration;
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

    // Update the effects properties state using the restored values.
    UpdatePanZoomEffectValues();
}

void PhotoRenderer::Run()
{
    Render();

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

    // Lesson 5:
    // Since the thumbnail will be displayed before the full-res image,
    // scale the zoom value to match the thumbnail image's size.
    float adjustedZoom = m_zoom * m_imageSize.width / m_thumbnailSize.width;
    settingsValues->Insert("zoom", PropertyValue::CreateSingle(adjustedZoom));

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
    // We are ignoring the gesture that wheel movement means panning.
    // Zooming is indicated by holding the Control key down, so we
    // override this method to always detect a zoom wheel event.
    m_gestureRecognizer->ProcessMouseWheelEvent(args->CurrentPoint, false, true);
}

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
    if (m_imageLoaded == true)
    {
        ClampViewPosition(&m_viewPosition, m_contextSize, m_imageSize, m_zoom);
    }
    else
    {
        ClampViewPosition(&m_viewPosition, m_contextSize, m_thumbnailSize, m_zoom);
    }

    UpdatePanZoomEffectValues();
    Render();
}

// This method is separate from OnManipulationUpdated to allow us to manually call it
// when we need to regenerate the effects state (e.g. when resuming from previously saved state).
void PhotoRenderer::UpdatePanZoomEffectValues()
{
    D2D1::Matrix3x2F zoomFull = D2D1::Matrix3x2F::Scale(m_zoom, m_zoom);
    D2D1::Matrix3x2F zoomHalf = D2D1::Matrix3x2F::Scale(m_zoom * 2.0f, m_zoom * 2.0f);
    D2D1::Matrix3x2F zoomQuarter = D2D1::Matrix3x2F::Scale(m_zoom * 4.0f, m_zoom * 4.0f);
    D2D1::Matrix3x2F translation = D2D1::Matrix3x2F::Translation(m_viewPosition.x, m_viewPosition.y);

    if (m_imageLoaded == true)
    {
        // Lesson 5:
        // Once background resources have been created, set the input of m_2dTransformEffect to the
        // appropriate mip level based on the zoom level.
        if (m_zoom <= 0.25f)
        {
            m_2dTransformEffect->SetInputEffect(0, m_2dTransformQuarterCached.Get());
            DX::ThrowIfFailed(
                m_2dTransformEffect->SetValue(D2D1_2DAFFINETRANSFORM_PROP_TRANSFORM_MATRIX, zoomQuarter * translation)
                );
        }
        else if (m_zoom <= 0.5f)
        {
            m_2dTransformEffect->SetInputEffect(0, m_2dTransformHalfCached.Get());
            DX::ThrowIfFailed(
                m_2dTransformEffect->SetValue(D2D1_2DAFFINETRANSFORM_PROP_TRANSFORM_MATRIX, zoomHalf * translation)
                );
        }
        else
        {
            m_2dTransformEffect->SetInputEffect(0, m_2dTransformFullCached.Get());
            DX::ThrowIfFailed(
                m_2dTransformEffect->SetValue(D2D1_2DAFFINETRANSFORM_PROP_TRANSFORM_MATRIX, zoomFull * translation)
                );
        }
    }
    else
    {
        DX::ThrowIfFailed(
            m_2dTransformEffect->SetValue(D2D1_2DAFFINETRANSFORM_PROP_TRANSFORM_MATRIX, zoomFull * translation)
            );
    }
}

void PhotoRenderer::OnTapped(
    _In_ Windows::UI::Input::GestureRecognizer^ recognizer,
    _In_ Windows::UI::Input::TappedEventArgs^ args
    )
{
    if (args->TapCount == 2)
    {
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

void PhotoRenderer::CreateWICBitmapSourceFromFilename(Platform::String^ filename, IWICBitmapSource** formatConverter, D2D1_SIZE_U* size)
{
    // Load the source image using a Windows Imaging Component decoder.
    ComPtr<IWICBitmapDecoder> decoder;
    DX::ThrowIfFailed(
        m_wicFactory->CreateDecoderFromFilename(
            filename->Data(),
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

    // An IWICFormatConverter is used here to implement the
    // IWICBitmapSource that is returned.
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
            WICBitmapPaletteTypeCustom // premultiplied BGRA has no paletting, so this is ignored
            )
        );

    UINT width, height;
    DX::ThrowIfFailed(converter->GetSize(&width, &height));

    *formatConverter = converter.Detach();

    // Store the WIC bitmap size in pixels - this remains constant regardless of the app's DPI.
    *size = D2D1::SizeU(width, height);
}


void PhotoRenderer::CreateBackgroundDeviceResources()
{
    // Lesson 5:
    // Create new Direct3D device which will process the full-res image in the background
    // while the thumbnail is initially displayed. This enables a faster load time
    // for the application, while maintaining the ability for the user to pan and zoom
    // the thumbnail in the meantime.

    // This flag adds support for surfaces with a different color channel ordering than the API default.
    // It is recommended usage, and is required for compatibility with Direct2D.
    UINT creationFlags = D3D11_CREATE_DEVICE_BGRA_SUPPORT;

#if defined(_DEBUG)
    // If the project is in a debug build, enable debugging via SDK Layers with this flag.
    creationFlags |= D3D11_CREATE_DEVICE_DEBUG;
#endif

    // This array defines the set of DirectX hardware feature levels this app will support.
    D3D_FEATURE_LEVEL featureLevels[] =
    {
        D3D_FEATURE_LEVEL_11_1,
        D3D_FEATURE_LEVEL_11_0,
        D3D_FEATURE_LEVEL_10_1,
        D3D_FEATURE_LEVEL_10_0,
        D3D_FEATURE_LEVEL_9_3,
        D3D_FEATURE_LEVEL_9_2,
        D3D_FEATURE_LEVEL_9_1
    };

    // Create the Direct3D API device object and corresponding context.
    ComPtr<ID3D11Device> device;
    ComPtr<ID3D11DeviceContext> context;
    DX::ThrowIfFailed(
        D3D11CreateDevice(
            nullptr,                    // specify null to use the default adapter
            D3D_DRIVER_TYPE_HARDWARE,
            0,                          // leave as 0 unless software device
            creationFlags,              // optionally set debug and Direct2D compatibility flags
            featureLevels,              // list of feature levels this app can support
            ARRAYSIZE(featureLevels),   // number of entries in above list
            D3D11_SDK_VERSION,          // always set this to D3D11_SDK_VERSION for modern
            &device,                    // returns the Direct3D device created
            &m_featureLevel,            // returns feature level of device created
            &context                    // returns the device immediate context
            )
        );

    // Get the Direct3D 11.1 device by QI off the Direct3D 11 one.
    DX::ThrowIfFailed(
        device.As(&m_d3dDeviceB)
        );

    // And get the corresponding device context in the same way.
    DX::ThrowIfFailed(
        context.As(&m_d3dContextB)
        );

    // Obtain the underlying DXGI device of the Direct3D 11.1 device.
    DX::ThrowIfFailed(
        m_d3dDeviceB.As(&m_dxgiDeviceB)
        );

    // Create seperate ID2D1Factory1 for background thread. It is single-threaded
    // as a performance optimization, since our app's logic guarantees that its devices
    // are only accessed from one thread at a time.
    D2D1_FACTORY_OPTIONS options;
    ZeroMemory(&options, sizeof(D2D1_FACTORY_OPTIONS));

#if defined(_DEBUG)
    // If the project is in a debug build, enable Direct2D debugging via SDK Layers
    options.debugLevel = D2D1_DEBUG_LEVEL_INFORMATION;
#endif

    DX::ThrowIfFailed(
        D2D1CreateFactory(
            D2D1_FACTORY_TYPE_SINGLE_THREADED,
            __uuidof(ID2D1Factory1),
            &options,
            &m_d2dFactoryB
            )
        );

    // Obtain the Direct2D device for 2-D rendering.
    DX::ThrowIfFailed(
        m_d2dFactoryB->CreateDevice(m_dxgiDeviceB.Get(), &m_d2dDeviceB)
        );

    // And get its corresponding device context object.
    DX::ThrowIfFailed(
        m_d2dDeviceB->CreateDeviceContext(
            D2D1_DEVICE_CONTEXT_OPTIONS_NONE,
            &m_d2dContextB
            )
        );

    // Raise the limit on the amount of cached tiles stored in Direct2D to 256MB.
    // This enables the app to store the complete a mip chain of the full-res image in GPU memory
    // (which takes (21MP + 10.5MP + 5.25MP) * 4Bpp = 147MB. If the limit is not raised from its
    // default (128MB) existing cache tiles will be evicted to make room for new ones, impacting performance.
    m_d2dDeviceB->SetMaximumTextureMemory(256000000);

    // Start loading full-res image on background thread / device. Once
    // background processing is done call Render to update the image.
    auto backgroundTask = task<void>([this]()
    {
        BackgroundProcessing();
    }).then([this]() {
        // After background processing has completed, change devices so
        // that the background device is now being drawn from. Because
        // this happens on the UI thread, there is no race condition.
        ChangeDevices();

        // After the change, force a render to display the full-res image.
        Render();
    }, task_continuation_context::use_current()); // Force render to occur on UI thread.
}

// Lesson 5:
// Once processing on the background device has completed,
// switch devices to begin drawing from that.
void PhotoRenderer::ChangeDevices()
{
    // All references to the original swap chain must be removed before it can be released.
    // We must first release the existing swap chain before creating the new one since only
    // one swapchain can be assigned to a CoreWindow at a time.
    m_d2dContext->SetTarget(nullptr);
    m_d3dRenderTargetView = nullptr;
    m_d2dTargetBitmap = nullptr;
    m_d3dContext->ClearState();
    m_d3dContext->Flush();
    m_swapChain = nullptr;
    m_dxgiDeviceB = nullptr;

    // Assign background devices to 'in use' references. This releases the existing objects
    // and allows the app to take advantage of DirectXBase's built-in swap chain creation.
    m_d3dDevice = m_d3dDeviceB.Detach();
    m_d3dContext = m_d3dContextB.Detach();
    m_d2dDevice = m_d2dDeviceB.Detach();
    m_d2dContext = m_d2dContextB.Detach();
    m_d2dFactory = m_d2dFactoryB.Detach();

    // Need to preserve DPI on new device.
    m_d2dContext->SetDpi(m_dpi, m_dpi);

    m_2dTransformEffect = m_2dTransformEffectB.Detach();

    m_imageLoaded = true;

    // Re-intialize SampleOverlay on new device.
    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D photo adjustment sample lesson 5: asynchronous image load"
        );

    // Recreate swapchain on new device.
    CreateWindowSizeDependentResources();

    UpdatePanZoomEffectValues();
}

// Lesson 5:
// Perform background processing in a seperate method, on its own thread.
// This allows us to render the thumbnail without the expensive
// background operations blocking the foreground.
void PhotoRenderer::BackgroundProcessing()
{
    ComPtr<IWICBitmapSource> WICFormatConverter;
    CreateWICBitmapSourceFromFilename("mammoth.jpg", &WICFormatConverter, &m_imagePixelSize);

    // Create and initialize a BitmapSource effect.
    DX::ThrowIfFailed(m_d2dContextB->CreateEffect(CLSID_D2D1BitmapSource, &m_fullResBitmapSourceEffect));

    // Set the input of the bitmap source effect to the returned WICFormatConverter.
    DX::ThrowIfFailed(m_fullResBitmapSourceEffect->SetValue(D2D1_BITMAPSOURCE_PROP_WIC_BITMAP_SOURCE, WICFormatConverter.Get()));

    DX::ThrowIfFailed(m_d2dContextB->CreateEffect(CLSID_D2D12DAffineTransform, &m_2dTransformFullCached));
    DX::ThrowIfFailed(m_d2dContextB->CreateEffect(CLSID_D2D12DAffineTransform, &m_2dTransformHalfCached));
    DX::ThrowIfFailed(m_d2dContextB->CreateEffect(CLSID_D2D12DAffineTransform, &m_2dTransformQuarterCached));
    DX::ThrowIfFailed(m_d2dContextB->CreateEffect(CLSID_D2D12DAffineTransform, &m_2dTransformEffectB));

    // The GPU mip chain is constructed here. The full, half, and quarter cached effects
    // are scaled at 1x, 0.5x, 0.25x respectively. These effects are cached to keep them
    // available on the GPU so that the app doesn't need to continuously be decoding/transferring
    // image data from the CPU. The app then uses the non-cached effect to incrementally
    // scale beyond the mip levels and to render to the screen.

    // Linear scaling can be used on the mip chain since we are scaling exactly at 0.5x.
    // The leaf node is unrestricted, but is limited by the size of the image. If we
    // want it to not go below 0.5x, the image can be as large as ~40MP on a 1366x768
    // screen. Otherwise additional mip levels need to be added.

    m_2dTransformFullCached->SetInputEffect(0, m_fullResBitmapSourceEffect.Get());
    m_2dTransformHalfCached->SetInputEffect(0, m_2dTransformFullCached.Get());
    m_2dTransformQuarterCached->SetInputEffect(0, m_2dTransformHalfCached.Get());

    DX::ThrowIfFailed(
        m_2dTransformFullCached->SetValue(
            D2D1_2DAFFINETRANSFORM_PROP_INTERPOLATION_MODE,
            D2D1_2DAFFINETRANSFORM_INTERPOLATION_MODE_LINEAR
            )
        );

    DX::ThrowIfFailed(
        m_2dTransformHalfCached->SetValue(
            D2D1_2DAFFINETRANSFORM_PROP_INTERPOLATION_MODE,
            D2D1_2DAFFINETRANSFORM_INTERPOLATION_MODE_LINEAR
            )
        );

    DX::ThrowIfFailed(
        m_2dTransformQuarterCached->SetValue(
            D2D1_2DAFFINETRANSFORM_PROP_INTERPOLATION_MODE,
            D2D1_2DAFFINETRANSFORM_INTERPOLATION_MODE_LINEAR
            )
        );

    DX::ThrowIfFailed(
        m_2dTransformEffectB->SetValue(
            D2D1_2DAFFINETRANSFORM_PROP_INTERPOLATION_MODE,
            D2D1_2DAFFINETRANSFORM_INTERPOLATION_MODE_LINEAR
            )
        );

    // D2D1_BORDER_MODE_HARD is used to avoid unnecessary edge anti-aliasing work (edges are never visible).
    DX::ThrowIfFailed(m_2dTransformFullCached->SetValue(D2D1_2DAFFINETRANSFORM_PROP_BORDER_MODE, D2D1_BORDER_MODE_HARD));
    DX::ThrowIfFailed(m_2dTransformHalfCached->SetValue(D2D1_2DAFFINETRANSFORM_PROP_BORDER_MODE, D2D1_BORDER_MODE_HARD));
    DX::ThrowIfFailed(m_2dTransformQuarterCached->SetValue(D2D1_2DAFFINETRANSFORM_PROP_BORDER_MODE, D2D1_BORDER_MODE_HARD));
    DX::ThrowIfFailed(m_2dTransformEffectB->SetValue(D2D1_2DAFFINETRANSFORM_PROP_BORDER_MODE, D2D1_BORDER_MODE_HARD));

    // Ensure mip chain is cached on GPU.
    DX::ThrowIfFailed(m_2dTransformFullCached->SetValue(D2D1_PROPERTY_CACHED, TRUE));
    DX::ThrowIfFailed(m_2dTransformHalfCached->SetValue(D2D1_PROPERTY_CACHED, TRUE));
    DX::ThrowIfFailed(m_2dTransformQuarterCached->SetValue(D2D1_PROPERTY_CACHED, TRUE));

    D2D_MATRIX_3X2_F downScaleMatrix = D2D1::Matrix3x2F::Scale(0.5f, 0.5f);
    DX::ThrowIfFailed(m_2dTransformHalfCached->SetValue(D2D1_2DAFFINETRANSFORM_PROP_TRANSFORM_MATRIX, downScaleMatrix));
    DX::ThrowIfFailed(m_2dTransformQuarterCached->SetValue(D2D1_2DAFFINETRANSFORM_PROP_TRANSFORM_MATRIX, downScaleMatrix));

    // Shrink the output of the last cached node before drawing to ensure the
    // complete image is drawn/populated in GPU cache.
    m_2dTransformEffectB->SetInputEffect(0, m_2dTransformQuarterCached.Get());
    DX::ThrowIfFailed(
        m_2dTransformEffectB->SetValue(
            D2D1_2DAFFINETRANSFORM_PROP_TRANSFORM_MATRIX,
            D2D1::Matrix3x2F::Scale(0.1f, 0.1f)
            )
        );

    // Create temporary bitmap to render to in the background.
    ComPtr<ID2D1Bitmap1> renderTarget;
    D2D1_BITMAP_PROPERTIES1 renderTargetProperties = D2D1::BitmapProperties1(
        D2D1_BITMAP_OPTIONS_TARGET,
        D2D1::PixelFormat(
            DXGI_FORMAT_B8G8R8A8_UNORM,
            D2D1_ALPHA_MODE_PREMULTIPLIED
            )
        );

    DX::ThrowIfFailed(
        m_d2dContextB->CreateBitmap(
            D2D1::SizeU(128, 128), // Large enough to contain scaled down image.
            nullptr,
            0,
            &renderTargetProperties,
            &renderTarget
            )
        );

    // Draw to temporary target in background. This ensures GPU processes image / populates mip levels.
    // This drawing code is blocking, which is why we have to perform it on a background thread to allow
    // the thumbnail to be continously rendered in the foreground.
    m_d2dContextB->SetTarget(renderTarget.Get());

    m_d2dContextB->BeginDraw();
    m_d2dContextB->DrawImage(
        m_2dTransformEffectB.Get(),
        D2D1_INTERPOLATION_MODE_LINEAR,
        D2D1_COMPOSITE_MODE_SOURCE_COPY
        );

    HRESULT hr = m_d2dContextB->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
        // is lost. It will be handled during the next call to Present.
        DX::ThrowIfFailed(hr);
    }

    // Create the event handle that will later signal when background processing has completed.
    HANDLE processingHandle = CreateEventEx(
        nullptr, // Default security attributes
        nullptr, // Manual-reset event
        false,   // Initial state is non-signaled
        0x1F0003 // Allow other threads to interact with handle.
        );

    // Tell the DXGI device to signal the HANDLE object once all processing
    // is done on the device.
    DX::ThrowIfFailed(m_dxgiDeviceB->EnqueueSetEvent(processingHandle));

    // The INFINITE parameter causes this to only return once the GPU has signaled
    // it has completed processing. This allows the app to force a render once
    // the background work has completed.
    WaitForSingleObjectEx(processingHandle, INFINITE, false);
    CloseHandle(processingHandle);
}