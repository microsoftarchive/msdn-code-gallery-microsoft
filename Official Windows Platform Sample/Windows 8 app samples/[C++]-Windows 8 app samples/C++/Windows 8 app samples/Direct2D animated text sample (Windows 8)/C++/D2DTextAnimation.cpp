//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "D2DTextAnimation.h"

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::Storage;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;

static const float MinZoom           = 0.5f;
static const float MaxZoom           = 9.0f;
static const float StartZoom         = 1.0f;
static const float OpacityMaskScaler = 2.0f; // Adjusts the resolution of the opacity mask texture.

D2DTextAnimation::D2DTextAnimation() :
    m_zoom(StartZoom),
    m_animating(false),
    m_firstRun(true)
{
    m_viewPosition.X = 0.0f;
    m_viewPosition.Y = 0.0f;
}

void D2DTextAnimation::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();

    Platform::String^ text = L"This sample demonstrates how to efficiently render animated text with D2D and DWrite. The text in this sample can be scaled and panned using touch and mouse controls. As the text's size and position are manipulated, this sample renders the animated text using a faster rendering mode than what is used when the text is static.";

    // Create a text format object to describe the text's style.
    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_NORMAL,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            18.0f,
            L"en-us",
            &m_textFormat
            )
        );

    DX::ThrowIfFailed(
        m_textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_LEADING)
        );

    DX::ThrowIfFailed(
        m_dwriteFactory->CreateTextLayout(
            text->Data(),
            text->Length(),
            m_textFormat.Get(),
            700, // maxWidth
            1000, // maxHeight
            &m_textLayout
            )
        );

    DX::ThrowIfFailed(
        m_textLayout->GetMetrics(&m_textMetrics)
        );
}

void D2DTextAnimation::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D animated text sample"
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Black),
            &m_blackBrush
            )
        );

    CreateOpacityMask();
}

void D2DTextAnimation::DrawTextLayout(float x, float y)
{
    m_d2dContext->DrawTextLayout(
        D2D1::Point2F(x, y),
        m_textLayout.Get(),
        m_blackBrush.Get(),
        D2D1_DRAW_TEXT_OPTIONS_NO_SNAP
        );
}

void D2DTextAnimation::CalculateCenterPosition()
{
    D2D1_SIZE_F size = m_d2dContext->GetSize();
    m_viewPosition.X = size.width/2 - m_textMetrics.widthIncludingTrailingWhitespace/2;
    m_viewPosition.Y = size.height/2 - m_textMetrics.height/2;
}

void D2DTextAnimation::CreateOpacityMask()
{
    D2D1_BITMAP_PROPERTIES1 properties = D2D1::BitmapProperties1(
        D2D1_BITMAP_OPTIONS_TARGET,
        D2D1::PixelFormat(DXGI_FORMAT_A8_UNORM, D2D1_ALPHA_MODE_PREMULTIPLIED)
        );

    D2D1_SIZE_U opacityBitmapSize = D2D1::SizeU(
        static_cast<UINT32>(OpacityMaskScaler * m_textMetrics.widthIncludingTrailingWhitespace),
        static_cast<UINT32>(OpacityMaskScaler * m_textMetrics.height)
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateBitmap(
            opacityBitmapSize,
            nullptr,
            0,
            &properties,
            &m_opacityBitmap
            )
        );

    D2D1::Matrix3x2F zoom = D2D1::Matrix3x2F::Scale(
        OpacityMaskScaler,
        OpacityMaskScaler
        );

    m_d2dContext->SetTarget(m_opacityBitmap.Get());
    m_d2dContext->BeginDraw();
    m_d2dContext->SetTransform(zoom);

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::Black, 0.0f));

    DrawTextLayout(0.0f, 0.0f);

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    m_d2dContext->SetTarget(nullptr);
}

void D2DTextAnimation::Render()
{
    if (m_firstRun)
    {
        CalculateCenterPosition();
        m_firstRun = false;
    }

    D2D1::Matrix3x2F translation = D2D1::Matrix3x2F::Translation(
        m_viewPosition.X,
        m_viewPosition.Y
        );

    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());
    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));

    if (!m_animating)
    {
        // Use default (high quality) text rendering when the text is not being animated.
        D2D1::Matrix3x2F zoom = D2D1::Matrix3x2F::Scale(m_zoom, m_zoom);
        m_d2dContext->SetTransform(zoom * translation);
        DrawTextLayout(0.0f, 0.0f);
    }
    else
    {
        // Use fast opacity mask text rendering when the text is being animated.
        D2D1::Matrix3x2F defaultZoom = D2D1::Matrix3x2F::Scale(
            m_zoom / OpacityMaskScaler,
            m_zoom / OpacityMaskScaler
            );
        m_d2dContext->SetTransform(defaultZoom * translation);
        m_d2dContext->SetAntialiasMode(D2D1_ANTIALIAS_MODE_ALIASED);
        m_d2dContext->FillOpacityMask(m_opacityBitmap.Get(), m_blackBrush.Get(), D2D1_OPACITY_MASK_CONTENT_GRAPHICS);
        m_d2dContext->SetAntialiasMode(D2D1_ANTIALIAS_MODE_PER_PRIMITIVE);
    }

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    m_sampleOverlay->Render();
}

void D2DTextAnimation::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &D2DTextAnimation::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &D2DTextAnimation::OnSuspending);
}

void D2DTextAnimation::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &D2DTextAnimation::OnWindowSizeChanged);

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &D2DTextAnimation::OnPointerPressed);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &D2DTextAnimation::OnPointerReleased);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &D2DTextAnimation::OnPointerMoved);

    window->PointerWheelChanged +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &D2DTextAnimation::OnPointerWheelChanged);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &D2DTextAnimation::OnLogicalDpiChanged);

    DisplayProperties::DisplayContentsInvalidated +=
        ref new DisplayPropertiesEventHandler(this, &D2DTextAnimation::OnDisplayContentsInvalidated);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);

    // We will use the gesture recognizer to compute the touch gestures, like pinching and dragging,
    // into transformations we can use in Direct2D. We simply funnel any touch events into it
    // and the gesture recognizer will fire events when the transformations need to be updated or
    // a specific gesture (e.g. double tap) occurs.

    m_gestureRecognizer = ref new GestureRecognizer();

    m_gestureRecognizer->AutoProcessInertia = false;

    m_gestureRecognizer->GestureSettings =
        GestureSettings::DoubleTap                     |
        GestureSettings::ManipulationTranslateX        |
        GestureSettings::ManipulationTranslateY        |
        GestureSettings::ManipulationScale;

    m_gestureRecognizer->Tapped +=
        ref new TypedEventHandler<GestureRecognizer^, TappedEventArgs^>(this, &D2DTextAnimation::OnTapped);

    m_gestureRecognizer->ManipulationUpdated +=
        ref new TypedEventHandler<GestureRecognizer^, ManipulationUpdatedEventArgs^>(this, &D2DTextAnimation::OnManipulationUpdated);

    m_gestureRecognizer->ManipulationCompleted +=
        ref new TypedEventHandler<GestureRecognizer^, ManipulationCompletedEventArgs^>(this, &D2DTextAnimation::OnManipulationCompleted);

    m_gestureRecognizer->AutoProcessInertia = true;
}

void D2DTextAnimation::Load(
    _In_ Platform::String^ entryPoint
    )
{
    // Retrieve any stored variables from the LocalSettings collection.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("m_viewPosition"))
    {
        m_viewPosition = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_viewPosition"))->GetPoint();
    }

    if (settingsValues->HasKey("m_zoom"))
    {
        m_zoom = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_zoom"))->GetSingle();
    }

    if (settingsValues->HasKey("m_firstRun"))
    {
        m_firstRun = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_firstRun"))->GetBoolean();
    }
}

void D2DTextAnimation::Run()
{
    Render();
    Present();

    m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessUntilQuit);
}

void D2DTextAnimation::Uninitialize()
{
}

void D2DTextAnimation::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
    Render();
    Present();
}

void D2DTextAnimation::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
    Render();
    Present();
}

void D2DTextAnimation::OnDisplayContentsInvalidated(
    _In_ Platform::Object^ sender
    )
{
    // Ensure the D3D Device is available for rendering.
    ValidateDevice();

    Render();
    Present();
}

void D2DTextAnimation::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

void D2DTextAnimation::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    // Store variables in the LocalSettings collection.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("m_viewPosition"))
    {
        settingsValues->Remove("m_viewPosition");
    }
    settingsValues->Insert("m_viewPosition", PropertyValue::CreatePoint(m_viewPosition));

    if (settingsValues->HasKey("m_zoom"))
    {
        settingsValues->Remove("m_zoom");
    }
    settingsValues->Insert("m_zoom", PropertyValue::CreateSingle(m_zoom));

    if (settingsValues->HasKey("m_firstRun"))
    {
        settingsValues->Remove("m_firstRun");
    }
    settingsValues->Insert("m_firstRun", PropertyValue::CreateBoolean(m_firstRun));
}

void D2DTextAnimation::OnPointerPressed(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessDownEvent(args->CurrentPoint);
}

void D2DTextAnimation::OnPointerReleased(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessUpEvent(args->CurrentPoint);
}

void D2DTextAnimation::OnPointerMoved(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessMoveEvents(args->GetIntermediatePoints());
}

void D2DTextAnimation::OnPointerWheelChanged(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    // Always zoom when the mouse wheel is used.
    m_gestureRecognizer->ProcessMouseWheelEvent(args->CurrentPoint, false, true);
}

void D2DTextAnimation::OnTapped(
    _In_ GestureRecognizer^ gestureRecognizer,
    _In_ TappedEventArgs^ args
    )
{
    if (args->TapCount == 2)
    {
        // A double tap means we should recenter.
        CalculateCenterPosition();
        m_zoom = StartZoom;

        Render();
        Present();
    }
}

void D2DTextAnimation::OnManipulationUpdated(
    _In_ GestureRecognizer^ gestureRecognizer,
    _In_ ManipulationUpdatedEventArgs^ args
    )
{
    Point position = args->Position;
    Point positionDelta = args->Delta.Translation;
    float zoomDelta = args->Delta.Scale;

    // In this method, we simply update the transformation variables to reflect how the user is manipulating the scene.
    // The gesture recognizer does the majority of the math; we simply need to update our internal variables.

    // Reposition the view to reflect translations.
    m_viewPosition.X += positionDelta.X;
    m_viewPosition.Y += positionDelta.Y;

    // We want to have any zoom operation be "centered" around the pointer position, which
    // requires recalculating the view position based on the new zoom and pointer position.
    // Step 1: Calculate the absolute pointer position (image position).
    D2D1_POINT_2F pointerAbsolutePosition = D2D1::Point2F(
        (m_viewPosition.X - position.X) / m_zoom,
        (m_viewPosition.Y - position.Y) / m_zoom
        );

    // Step 2: Apply the zoom operation and clamp to the min/max zoom.
    // zoomDelta is a coefficient for the change in zoom.
    m_zoom *= zoomDelta;
    m_zoom = Clamp(m_zoom, MinZoom, MaxZoom);

    // Step 3: Adjust the view position based on the new m_zoom value.
    m_viewPosition.X = pointerAbsolutePosition.x * m_zoom + position.X;
    m_viewPosition.Y = pointerAbsolutePosition.y * m_zoom + position.Y;

    // When the user is manipulating text via a gesture, text should be rendered using the opacity mask method.
    m_animating = true;

    Render();
    Present();
}

void D2DTextAnimation::OnManipulationCompleted(
    _In_ GestureRecognizer^ gestureRecognizer,
    _In_ ManipulationCompletedEventArgs^ args
    )
{
    // Once the user has finished a gesture, text should be rendered using the default method.
    m_animating = false;

    Render();
    Present();
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new D2DTextAnimation();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
