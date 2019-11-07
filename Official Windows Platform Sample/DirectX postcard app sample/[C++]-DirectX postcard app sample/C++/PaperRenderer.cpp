//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "PaperRenderer.h"

using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml::Input;
using namespace Platform::Collections;

PaperRenderer::PaperRenderer(void) :
    m_paperMode(PaperMode::Moving)
{
}

void PaperRenderer::CreateDeviceIndependentResources(
    ComPtr<ID2D1Factory1> d2dFactory
    )
{
    // Save shaerd resource.
    m_d2dFactory = d2dFactory;
}

void PaperRenderer::CreateDeviceResources(
    ComPtr<ID2D1DeviceContext> d2dContext
    )
{
    // Save shared resource.
    m_d2dContext = d2dContext;

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            D2D1::ColorF(D2D1::ColorF::Black),
            &m_blackBrush
            )
        );

    // Create a star-shaped geometry for stamping.

    ComPtr<ID2D1PathGeometry> starStampGeometry;
    DX::ThrowIfFailed(
        m_d2dFactory->CreatePathGeometry(&starStampGeometry)
        );

    {
        UINT points = 5;
        float innerRadius = 50;
        float outerRadius = 100;

        ComPtr<ID2D1GeometrySink> geometrySink;

        DX::ThrowIfFailed(
            starStampGeometry->Open(&geometrySink)
            );

        geometrySink->BeginFigure(
            D2D1::Point2F(0, -outerRadius),
            D2D1_FIGURE_BEGIN_FILLED
            );

        for (UINT i = 0; i < (2 * points) - 1; ++i)
        {
            float radius = (i % 2 == 0) ? innerRadius : outerRadius;
            float angle = 2 * PI_F * (i + 1) / (2 * points);

            geometrySink->AddLine(
                D2D1::Point2F(
                    radius * sin(angle),
                    -radius * cos(angle)
                    )
                );
        }

        geometrySink->EndFigure(
            D2D1_FIGURE_END_CLOSED
            );

        DX::ThrowIfFailed(geometrySink->Close());
    }

    m_starStampGeometry = starStampGeometry;
}

void PaperRenderer::CreateWindowSizeDependentResources(float dpi)
{
    // Save shared DPI.
    m_dpi = dpi;
}

void PaperRenderer::DrawPaper()
{
    D2D1_SIZE_F size = m_d2dContext->GetSize();

    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    for (PaperFragment^ fragment : m_paperFragmentList)
    {
        fragment->Draw();
    }
}

void PaperRenderer::AddPaper()
{
    m_paperMode = PaperMode::Moving;

    D2D1_SIZE_F size = m_d2dContext->GetSize();

    // Create a rectangular geometry for the piece of paper.

    ComPtr<ID2D1RectangleGeometry> paperGeometry;

    D2D1_RECT_F paperRect = D2D1::RectF(-300, -225, 300, 225);

    DX::ThrowIfFailed(
        m_d2dFactory->CreateRectangleGeometry(
            &paperRect,
            &paperGeometry
            )
        );

    // Create a rotated version of the geometry.

    ComPtr<ID2D1TransformedGeometry> rotatedPaperGeometry;

    D2D_MATRIX_3X2_F translation = D2D1::Matrix3x2F::Translation(
        (size.width / 4) + (RandomFloat() * (size.width / 2)),
        (size.height / 4) + (RandomFloat() * (size.height / 2))
        );

    D2D_MATRIX_3X2_F rotation = D2D1::Matrix3x2F::Rotation(
        RandomFloat() * 30.0f - 15.0f
        );

    DX::ThrowIfFailed(
        m_d2dFactory->CreateTransformedGeometry(
            paperGeometry.Get(),
            &(rotation * translation),
            &rotatedPaperGeometry
            )
        );

    // Create a new paper fragment from the rotated geometry.

    PaperFragment^ paperFragment =
        ref new PaperFragment(
            m_d2dContext,
            rotatedPaperGeometry.Get(),
            D2D1::ColorF(
                0.5f + 0.5f*RandomFloat(),
                0.5f + 0.5f*RandomFloat(),
                0.5f + 0.5f*RandomFloat(),
                1.0f
                )
            );

    paperFragment->SetPosition(D2D1::Point2F(0, 0));

    m_paperFragmentList.push_back(paperFragment);
}

void PaperRenderer::RemovePaper()
{
    m_paperMode = PaperMode::Deleting;
}

void PaperRenderer::MovePaper()
{
    m_paperMode = PaperMode::Moving;
}

void PaperRenderer::StampPaper()
{
    m_paperMode = PaperMode::Stamping;
}

void PaperRenderer::Reset()
{
    m_paperMode = PaperMode::Moving;
    m_paperFragmentList.clear();
    m_selectedFragment = nullptr;
}

void PaperRenderer::OnManipulationStarted(ManipulationStartedRoutedEventArgs^ args)
{
    D2D1_POINT_2F d2dPosition = D2D1::Point2F(args->Position.X, args->Position.Y);

    if (m_paperMode == PaperMode::Moving)
    {
        PaperFragment^ pressedFragment = HitTestFragmentsWithPosition(d2dPosition);
        if (pressedFragment != nullptr)
        {
            m_selectedFragment = pressedFragment;
        }
    }
}

void PaperRenderer::OnManipulationCompleted(ManipulationCompletedRoutedEventArgs^ args)
{
    m_selectedFragment = nullptr;
    OnTapped(args->Position);
}

void PaperRenderer::OnManipulationDelta(ManipulationDeltaRoutedEventArgs^ args)
{
    if (m_paperMode == PaperMode::Moving)
    {
        if (m_selectedFragment != nullptr)
        {
            D2D1_POINT_2F point = m_selectedFragment->GetPosition();
            point.x += args->Delta.Translation.X;
            point.y += args->Delta.Translation.Y;
            m_selectedFragment->SetPosition(point);
        }
    }
}

void PaperRenderer::OnTapped(Point position)
{
    D2D1_POINT_2F d2dPosition = D2D1::Point2F(position.X, position.Y);

    PaperFragment^ tappedFragment = HitTestFragmentsWithPosition(d2dPosition);
    if (tappedFragment != nullptr)
    {
        if (m_paperMode == PaperMode::Deleting)
        {
            // Delete the tapped fragment.
            m_paperFragmentList.remove(tappedFragment);
        }
        else if (m_paperMode == PaperMode::Stamping)
        {
            // Delete the tapped fragment, and add the intersection and exclusion
            // of the fragment with the stamp geometry.
            PaperFragment^ intersectFrag = tappedFragment->GetIntersectionWith(
                d2dPosition,
                m_starStampGeometry.Get()
                );

            PaperFragment^ excludeFrag = tappedFragment->GetExclusionWith(
                d2dPosition,
                m_starStampGeometry.Get()
                );

            m_paperFragmentList.remove(tappedFragment);
            m_paperFragmentList.push_back(intersectFrag);
            m_paperFragmentList.push_back(excludeFrag);
        }
    }
}

PaperFragment^ PaperRenderer::HitTestFragmentsWithPosition(D2D1_POINT_2F position)
{
    PaperFragment^ chosenFragment = nullptr;
    for (PaperFragment^ fragment : m_paperFragmentList)
    {
        if (fragment->HitTest(position))
        {
            chosenFragment = fragment;
        }
    };
    return chosenFragment;
}

float PaperRenderer::RandomFloat()
{
    return static_cast<float>(rand()) / static_cast<float>(RAND_MAX);
}