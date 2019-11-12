//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "D2DBasicAnimation.h"

using namespace Microsoft::WRL;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

static const float AnimationDuration = 20.0f; // Defines how long it takes the triangle to traverse the path.

D2DBasicAnimation::D2DBasicAnimation() :
    m_windowClosed(false),
    m_windowVisible(true),
    m_pathLength(0.0f),
    m_elapsedTime(0.0f)
{
}

void D2DBasicAnimation::CreateDeviceIndependentResources()
{
    DirectXBase::CreateDeviceIndependentResources();
}

void D2DBasicAnimation::CreateSpiralPathAndTriangle()
{
    DX::ThrowIfFailed(
        m_d2dFactory->CreatePathGeometry(&m_pathGeometry)
        );

    {
        ComPtr<ID2D1GeometrySink> geometrySink;

        // Write to the path geometry using the geometry sink. We are going to create a
        // spiral.
        DX::ThrowIfFailed(
            m_pathGeometry->Open(&geometrySink)
            );

        D2D1_POINT_2F currentLocation = {0, 0};

        geometrySink->BeginFigure(currentLocation, D2D1_FIGURE_BEGIN_FILLED);

        D2D1_POINT_2F locDelta = {2, 2};
        float radius = 3;

        for (UINT i = 0; i < 30; ++i)
        {
            currentLocation.x += radius * locDelta.x;
            currentLocation.y += radius * locDelta.y;

            geometrySink->AddArc(
                D2D1::ArcSegment(
                    currentLocation,
                    D2D1::SizeF(2*radius, 2*radius),    // radius x/y
                    0.0f,                               // rotation angle
                    D2D1_SWEEP_DIRECTION_CLOCKWISE,
                    D2D1_ARC_SIZE_SMALL
                    )
                );

            locDelta = D2D1::Point2F(-locDelta.y, locDelta.x);

            radius += 3;
        }

        geometrySink->EndFigure(D2D1_FIGURE_END_OPEN);

        DX::ThrowIfFailed(
            geometrySink->Close()
            );
    }

    DX::ThrowIfFailed(
        m_d2dFactory->CreatePathGeometry(&m_objectGeometry)
        );

    {
        ComPtr<ID2D1GeometrySink> geometrySink;

        // Create a simple triangle by writing the object geometry using the geometry sink.
        DX::ThrowIfFailed(
            m_objectGeometry->Open(&geometrySink)
            );

        geometrySink->BeginFigure(
            D2D1::Point2F(0.0f, 0.0f),
            D2D1_FIGURE_BEGIN_FILLED
            );

        const D2D1_POINT_2F triangle[] = {{-10.0f, -10.0f}, {-10.0f, 10.0f}, {0.0f, 0.0f}};
        geometrySink->AddLines(triangle, 3);

        geometrySink->EndFigure(D2D1_FIGURE_END_OPEN);

        DX::ThrowIfFailed(
            geometrySink->Close()
            );
    }

    DX::ThrowIfFailed(
        m_pathGeometry->ComputeLength(
            nullptr,            // Apply no transform.
            &m_pathLength
            )
        );
}

float D2DBasicAnimation::ComputeTriangleLocation(float startPoint, float endPoint, float duration, float elapsedTime)
{
    float time = min(max(elapsedTime, 0), duration);
    return startPoint + ((endPoint - startPoint) * (time / duration));
}

void D2DBasicAnimation::CreateDeviceResources()
{
    DirectXBase::CreateDeviceResources();

    this->CreateSpiralPathAndTriangle();

    m_sampleOverlay = ref new SampleOverlay();

    m_sampleOverlay->Initialize(
        m_d2dDevice.Get(),
        m_d2dContext.Get(),
        m_wicFactory.Get(),
        m_dwriteFactory.Get(),
        "Direct2D animation sample"
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Black),
            &m_blackBrush
            )
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::White),
            &m_whiteBrush
            )
        );
}


void D2DBasicAnimation::Render()
{
    // Retrieve the size of the render target.
    D2D1_SIZE_F renderTargetSize = m_d2dContext->GetSize();

    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(D2D1::ColorF::CornflowerBlue));
    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    float minWidthHeightScale = min(renderTargetSize.width, renderTargetSize.height) / 512;

    D2D1::Matrix3x2F scale = D2D1::Matrix3x2F::Scale(
        minWidthHeightScale,
        minWidthHeightScale
        );

    D2D1::Matrix3x2F translation = D2D1::Matrix3x2F::Translation(
        renderTargetSize.width / 2.0f,
        renderTargetSize.height / 2.0f
        );

    // Center the path.
    m_d2dContext->SetTransform(scale * translation);

    // Draw the path in black.
    m_d2dContext->DrawGeometry(m_pathGeometry.Get(), m_blackBrush.Get());

    float length = ComputeTriangleLocation(0.0f, m_pathLength, AnimationDuration, m_elapsedTime);

    // Ask the geometry to give us the point that corresponds with the
    // length at the current time.
    D2D1_POINT_2F point;
    D2D1_POINT_2F tangent;
    DX::ThrowIfFailed(
        m_pathGeometry->ComputePointAtLength(
            length,
            nullptr,
            &point,
            &tangent
            )
        );

    // Reorient the triangle so that it follows the
    // direction of the path.
    D2D1_MATRIX_3X2_F triangleMatrix;
    triangleMatrix = D2D1::Matrix3x2F(
        tangent.x, tangent.y,
        -tangent.y, tangent.x,
        point.x, point.y
        );

    m_d2dContext->SetTransform(triangleMatrix * scale * translation);

    // Draw the white triangle.
    m_d2dContext->FillGeometry(m_objectGeometry.Get(), m_whiteBrush.Get());

    // When we reach the end of the animation, loop back to the beginning.
    if (m_elapsedTime >= AnimationDuration)
    {
        m_elapsedTime = 0.0f;
    }
    else
    {
        m_elapsedTime += 0.0166f; // This controls the number of time units that time is incremented by on every render call.
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

void D2DBasicAnimation::Initialize(
    _In_ CoreApplicationView^ applicationView
    )
{
    applicationView->Activated +=
        ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &D2DBasicAnimation::OnActivated);

    CoreApplication::Suspending +=
        ref new EventHandler<SuspendingEventArgs^>(this, &D2DBasicAnimation::OnSuspending);

    CoreApplication::Resuming +=
        ref new EventHandler<Platform::Object^>(this, &D2DBasicAnimation::OnResuming);
}

void D2DBasicAnimation::SetWindow(
    _In_ CoreWindow^ window
    )
{
    window->PointerCursor = ref new CoreCursor(CoreCursorType::Arrow, 0);

    window->SizeChanged +=
        ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &D2DBasicAnimation::OnWindowSizeChanged);

    window->VisibilityChanged +=
        ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &D2DBasicAnimation::OnVisibilityChanged);

    window->Closed +=
        ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &D2DBasicAnimation::OnWindowClosed);

    DisplayProperties::LogicalDpiChanged +=
        ref new DisplayPropertiesEventHandler(this, &D2DBasicAnimation::OnLogicalDpiChanged);

    DirectXBase::Initialize(window, DisplayProperties::LogicalDpi);
}

void D2DBasicAnimation::Load(
    _In_ Platform::String^ entryPoint
    )
{
}

void D2DBasicAnimation::Run()
{
    while (!m_windowClosed)
    {
        if (m_windowVisible)
        {
            m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);
            Render();
            Present();
        }
        else
        {
            m_window->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessOneAndAllPending);
        }
    }
}

void D2DBasicAnimation::Uninitialize()
{
}

void D2DBasicAnimation::OnWindowSizeChanged(
    _In_ CoreWindow^ sender,
    _In_ WindowSizeChangedEventArgs^ args
    )
{
    UpdateForWindowSizeChange();
    m_sampleOverlay->UpdateForWindowSizeChange();
}

void D2DBasicAnimation::OnVisibilityChanged(
    _In_ CoreWindow^ sender,
    _In_ VisibilityChangedEventArgs^ args
    )
{
    m_windowVisible = args->Visible;
}

void D2DBasicAnimation::OnWindowClosed(
    _In_ CoreWindow^ sender,
    _In_ CoreWindowEventArgs^ args
    )
{
    m_windowClosed = true;
}

void D2DBasicAnimation::OnLogicalDpiChanged(
    _In_ Platform::Object^ sender
    )
{
    SetDpi(DisplayProperties::LogicalDpi);
}

void D2DBasicAnimation::OnActivated(
    _In_ CoreApplicationView^ applicationView,
    _In_ IActivatedEventArgs^ args
    )
{
    m_window->Activate();
}

void D2DBasicAnimation::OnSuspending(
    _In_ Platform::Object^ sender,
    _In_ SuspendingEventArgs^ args
    )
{
}

void D2DBasicAnimation::OnResuming(
    _In_ Platform::Object^ sender,
    _In_ Platform::Object^ args
    )
{
}

IFrameworkView^ DirectXAppSource::CreateView()
{
    return ref new D2DBasicAnimation();
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
    auto directXAppSource = ref new DirectXAppSource();
    CoreApplication::Run(directXAppSource);
    return 0;
}
