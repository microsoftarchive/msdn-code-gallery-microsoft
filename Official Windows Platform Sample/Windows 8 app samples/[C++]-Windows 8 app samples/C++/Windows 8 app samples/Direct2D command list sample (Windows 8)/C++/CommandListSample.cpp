//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "CommandListSample.h"
#include "BasicTimer.h"

using namespace D2D1;

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::Storage;
using namespace Windows::System;

static const float MinZoom = 0.0625f;
static const float MaxZoom = 8.0f;

CommandListRenderer::CommandListRenderer():
    m_zoom(1.0f),
    m_recenterStartTime(0.0f),
    m_recenter(false),
    m_recenterStartZoom(1.0f),
    m_windowClosed(false),
    m_windowVisible(true)
{
    m_recenterStartPosition.x = 0;
    m_recenterStartPosition.y = 0;
    m_viewPosition.X = 0;
    m_viewPosition.Y = 0;
}

void CommandListRenderer::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D command list and image brush sample"
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::DarkBlue),
            &m_blueBrush
            )
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateCommandList(&m_commandList)
        );

    m_d2dContext->SetTarget(m_commandList.Get());
    m_d2dContext->BeginDraw();
    DrawPattern();

    // We ignore D2DERR_RECREATE_TARGET here. This error indicates that the device
    // is lost. It will be handled during the next call to Present.
    HRESULT hr = m_d2dContext->EndDraw();
    if (hr != D2DERR_RECREATE_TARGET)
    {
        DX::ThrowIfFailed(hr);
    }

    DX::ThrowIfFailed(
        m_commandList->Close()
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateImageBrush(
            m_commandList.Get(),
            D2D1::ImageBrushProperties(
                D2D1::RectF(198, 298, 370, 470),
                D2D1_EXTEND_MODE_WRAP,
                D2D1_EXTEND_MODE_WRAP,
                D2D1_INTERPOLATION_MODE_LINEAR
                ),
            &m_imageBrush
            )
        );
}

void CommandListRenderer::Render()
{
    D2D1_SIZE_F size = m_d2dContext->GetSize();

    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));
    Matrix3x2F translation = Matrix3x2F::Translation(m_viewPosition.X / m_zoom, m_viewPosition.Y / m_zoom);
    Matrix3x2F zoom = Matrix3x2F::Scale(m_zoom, m_zoom);

    m_imageBrush->SetTransform(translation * zoom);

    m_d2dContext->FillRectangle(
        D2D1::RectF(0, 0, size.width, size.height),
        m_imageBrush.Get()
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

void CommandListRenderer::DrawPattern()
{
    Microsoft::WRL::ComPtr<ID2D1LinearGradientBrush>        linearGradientBrush;
    Microsoft::WRL::ComPtr<ID2D1GradientStopCollection>     gradientStopCollection;
    D2D1_GRADIENT_STOP gradientStops[4];
    gradientStops[0].color = D2D1::ColorF(D2D1::ColorF::Aqua, 0.7f);
    gradientStops[0].position = 0.0f;
    gradientStops[1].color = D2D1::ColorF(D2D1::ColorF::DarkMagenta, 0.5f);
    gradientStops[1].position = 0.3f;
    gradientStops[2].color = D2D1::ColorF(D2D1::ColorF::Olive, 0.5f);
    gradientStops[2].position = 0.6f;
    gradientStops[3].color = D2D1::ColorF(D2D1::ColorF::Aqua, 0.7f);
    gradientStops[3].position = 1.0f;

    DX::ThrowIfFailed(
        m_d2dContext->CreateGradientStopCollection(
            gradientStops,
            ARRAYSIZE(gradientStops),
            &gradientStopCollection
            )
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateLinearGradientBrush(
            D2D1::LinearGradientBrushProperties(
                D2D1::Point2F(250, 350),
                D2D1::Point2F(275, 375)
                ),
            gradientStopCollection.Get(),
            &linearGradientBrush
            )
        );

    m_d2dContext->FillEllipse(
        D2D1::Ellipse(
            D2D1::Point2F(287.5, 387.5),
            75.0f,
            75.0f
            ),
        m_blueBrush.Get()
        );

    m_d2dContext->FillEllipse(
        D2D1::Ellipse(
            D2D1::Point2F(262.5f, 362.5f),
            12.5f,
            12.5f
            ),
        linearGradientBrush.Get()
        );

    linearGradientBrush->SetTransform(
        D2D1::Matrix3x2F::Translation(D2D1::SizeF(50, 50))
        );

    m_d2dContext->FillEllipse(
        D2D1::Ellipse(
            D2D1::Point2F(312.5f, 412.5f),
            12.5f,
            12.5f
            ),
        linearGradientBrush.Get()
        );

    linearGradientBrush->SetTransform(
        D2D1::Matrix3x2F::Translation(D2D1::SizeF(50, 0))
        );

    m_d2dContext->FillEllipse(
        D2D1::Ellipse(
            D2D1::Point2F(312.5f, 362.5f),
            12.5f,
            12.5f
            ),
        linearGradientBrush.Get()
        );

    linearGradientBrush->SetTransform(
        D2D1::Matrix3x2F::Translation(D2D1::SizeF(0, 50))
        );

    m_d2dContext->FillEllipse(
        D2D1::Ellipse(
            D2D1::Point2F(262.5f, 412.5f),
            12.5f,
            12.5f
            ),
        linearGradientBrush.Get()
        );
}

void CommandListRenderer::Update(float timeTotal)
{
    // A douple tap will set this variable to true, meaning we should start recentering.
    if (m_recenter)
    {
        // If the start time of the animation isn't known yet, set it to the current time.
        if (m_recenterStartTime == 0.0f)
        {
            m_recenterStartTime = timeTotal;
        }

        // Determine how far along in the animation we should be (we want the animation to take a
        // quarter of a second)
        float delta = (timeTotal - m_recenterStartTime) * 4.0f;

        // If we're at the end of the animation, ensure we've completely reset the position and zoom,
        // and clear the animation flag
        if (delta > 1.0f)
        {
            m_viewPosition.X = 0;
            m_viewPosition.Y = 0;
            m_zoom = 1.0f;
            m_recenterStartTime = 0.0f;
            m_recenter = false;
        }

        // Otherwise, linearly interpolate between the starting position and the reset position.
        else
        {
            m_viewPosition.X = LinearInterpolate(m_recenterStartPosition.x, 0, delta);
            m_viewPosition.Y = LinearInterpolate(m_recenterStartPosition.y, 0, delta);
            m_zoom = LinearInterpolate(m_recenterStartZoom, 1.0f, delta);
        }
    }
}

void CommandListRenderer::HandlePointerDoubleTapped(Point position)
{
    // A double tap means we should recenter.
    m_recenter = true;
    m_recenterStartPosition.x = m_viewPosition.X;
    m_recenterStartPosition.y = m_viewPosition.Y;
    m_recenterStartZoom = m_zoom;
}

void CommandListRenderer::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &CommandListRenderer::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &CommandListRenderer::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &CommandListRenderer::OnResuming);
}

void CommandListRenderer::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &CommandListRenderer::OnWindowSizeChanged);

    window->VisibilityChanged +=
        ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &CommandListRenderer::OnVisibilityChanged);

    window->PointerPressed +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CommandListRenderer::OnPointerPressed);

    window->PointerReleased +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CommandListRenderer::OnPointerReleased);

    window->PointerMoved +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CommandListRenderer::OnPointerMoved);

    window->PointerWheelChanged +=
        ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this, &CommandListRenderer::OnPointerWheelChanged);

    window->Closed +=
        ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &CommandListRenderer::OnWindowClosed);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &CommandListRenderer::OnLogicalDpiChanged);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);

    m_gestureRecognizer = ref new GestureRecognizer();

    m_gestureRecognizer->AutoProcessInertia = true;

    m_gestureRecognizer->GestureSettings =
        GestureSettings::DoubleTap                     |
        GestureSettings::ManipulationTranslateX        |
        GestureSettings::ManipulationTranslateY        |
        GestureSettings::ManipulationScale             |
        GestureSettings::ManipulationTranslateInertia  |
        GestureSettings::ManipulationScaleInertia;

    m_gestureRecognizer->Tapped +=
        ref new TypedEventHandler<GestureRecognizer^, TappedEventArgs^>(this, &CommandListRenderer::OnTapped);

    m_gestureRecognizer->ManipulationUpdated +=
        ref new TypedEventHandler<GestureRecognizer^, ManipulationUpdatedEventArgs^>(this, &CommandListRenderer::OnManipulationUpdated);
}

void CommandListRenderer::Load(
    _In_ Platform::String^ entryPoint
    )
{
    // Retrieve stored variables from the LocalSettings collection if the app was terminated after suspension.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    if (settingsValues->HasKey("m_zoom"))
    {
        m_zoom = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_zoom"))->GetSingle();
    }

    if (settingsValues->HasKey("m_viewPosition"))
    {
        m_viewPosition = safe_cast<IPropertyValue^>(settingsValues->Lookup("m_viewPosition"))->GetPoint();
    }
}

void CommandListRenderer::Run()
{
    BasicTimer^ timer = ref new BasicTimer();

    while (!m_windowClosed)
    {
        if (m_windowVisible)
        {
            timer->Update();
            m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);
            Update(timer->Total);
            Render();
            Present(); // This call is synchronized to the display frame rate.
        }
        else
        {
            m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessOneAndAllPending);
        }
    }
}

void CommandListRenderer::Uninitialize()
{
}

void CommandListRenderer::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
}

void CommandListRenderer::OnVisibilityChanged(
    _In_ CoreWindow^ sender,
    _In_ VisibilityChangedEventArgs^ args
    )
{
    m_windowVisible = args->Visible;
}

void CommandListRenderer::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
}

void CommandListRenderer::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

void CommandListRenderer::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
    // Store user-manipulated properties in the LocalSettings collection.
    IPropertySet^ settingsValues = ApplicationData::Current->LocalSettings->Values;

    // Check to ensure each key is not already in the collection. If it is present, remove
    // it before storing in the new value. These values will be retrieved in the Load method.

    if (settingsValues->HasKey("m_zoom"))
    {
        settingsValues->Remove("m_zoom");
    }
    settingsValues->Insert("m_zoom", PropertyValue::CreateSingle(m_zoom));

    if (settingsValues->HasKey("m_viewPosition"))
    {
        settingsValues->Remove("m_viewPosition");
    }
    settingsValues->Insert("m_viewPosition", PropertyValue::CreatePoint(m_viewPosition));
}

void CommandListRenderer::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

void CommandListRenderer::OnWindowClosed(
    _In_ CoreWindow^ sender,
    _In_ CoreWindowEventArgs^ args
    )
{
    m_windowClosed = true;
}

void CommandListRenderer::OnPointerPressed(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessDownEvent(args->CurrentPoint);
}

void CommandListRenderer::OnPointerReleased(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessUpEvent(args->CurrentPoint);
}

void CommandListRenderer::OnPointerMoved(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    m_gestureRecognizer->ProcessMoveEvents(args->GetIntermediatePoints());
}

void CommandListRenderer::OnPointerWheelChanged(
    _In_ CoreWindow^ window,
    _In_ PointerEventArgs^ args
    )
{
    // We are ignoring the gesture that wheel movement means panning.
    // Zooming is indicated by holding the Control key down, so we
    // override this method always to detect a zoom wheel event.
    m_gestureRecognizer->ProcessMouseWheelEvent(args->CurrentPoint, FALSE, TRUE);
}

void CommandListRenderer::OnTapped(
    _In_ GestureRecognizer^ gestureRecognizer,
    _In_ TappedEventArgs^ args
    )
{
    if (args->TapCount == 2)
    {
        HandlePointerDoubleTapped(args->Position);
    }
}

void CommandListRenderer::OnManipulationUpdated(
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
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new CommandListRenderer();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
