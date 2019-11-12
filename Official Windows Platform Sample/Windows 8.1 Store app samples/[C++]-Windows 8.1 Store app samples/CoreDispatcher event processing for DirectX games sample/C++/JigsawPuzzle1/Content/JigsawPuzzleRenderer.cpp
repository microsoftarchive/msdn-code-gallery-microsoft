//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "Common\DirectXHelper.h"
#include "Content\JigsawPuzzleRenderer.h"

using namespace JigsawPuzzle;

using namespace DirectX;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;

// Loads resources needed for rendering.
JigsawPuzzleRenderer::JigsawPuzzleRenderer(const std::shared_ptr<DX::DeviceResources>& deviceResources, const std::shared_ptr<GameState>& gameState) :
    m_deviceResources(deviceResources),
    m_state(gameState),
    m_framesRendered(0)
{
    for (UINT n = 0; n < Constants::PuzzlePieceCount; n++)
    {
        m_pieceRenderers[n].Initialize(m_state->GetPuzzlePiece(n), m_deviceResources);
        m_targetRenderers[n].Initialize(m_state->GetDropTarget(n), m_deviceResources);
    }

    CreatePuzzlePieceGeometry();

    DX::ThrowIfFailed(
        m_deviceResources->GetDWriteFactory()->CreateTextFormat(
            L"Segoe UI",
            nullptr,
            DWRITE_FONT_WEIGHT_NORMAL,
            DWRITE_FONT_STYLE_NORMAL,
            DWRITE_FONT_STRETCH_NORMAL,
            24.0f,
            L"en-us",
            &m_textFormat
            )
        );

    m_textFormat->SetTextAlignment(DWRITE_TEXT_ALIGNMENT_CENTER);

    CreateDeviceDependentResources();
    CreateWindowSizeDependentResources();
}

// Renders the current state of the game.
void JigsawPuzzleRenderer::Render()
{
    m_framesRendered++;
    auto context = m_deviceResources->GetD2DDeviceContext();
    D2D1_COLOR_F backgroundColor = D2D1::ColorF(D2D1::ColorF::CornflowerBlue);

#ifdef MEASURE_LATENCY
    if (m_state->BackgroundToggled())
    {
        backgroundColor = D2D1::ColorF(D2D1::ColorF::YellowGreen);
        m_textBrush->SetColor(D2D1::ColorF(D2D1::ColorF::White));
    }
    else
    {
        backgroundColor = D2D1::ColorF(D2D1::ColorF::White);
        m_textBrush->SetColor(D2D1::ColorF(D2D1::ColorF::Black));
    }
#endif

    context->BeginDraw();
    context->Clear(backgroundColor);
    context->SetTransform(m_deviceResources->GetOrientationTransform2D());

    std::wstring text = L"Frames Rendered: ";
    text += m_framesRendered.ToString()->Data();
    context->DrawText(
        text.c_str(),
        static_cast<UINT>(text.length()),
        m_textFormat.Get(),
        D2D1::RectF(Constants::FrameCounterLeft, Constants::FrameCounterTop, Constants::FrameCounterRight, Constants::FrameCounterBottom),
        m_textBrush.Get()
        );

    for (UINT n = 0; n < Constants::PuzzlePieceCount; n++)
    {
        m_targetRenderers[n].Render();
    }

    int movingPiece = -1;
    for (UINT n = 0; n < Constants::PuzzlePieceCount; n++)
    {
        if (m_state->GetPuzzlePiece(n)->IsMoving())
        {
            // Wait and render this piece last so that it shows up on top of the others.
            movingPiece = n;
        }
        else
        {
            m_pieceRenderers[n].Render();
        }
    }

    if (movingPiece >= 0)
    {
        m_pieceRenderers[movingPiece].Render();
    }

    context->EndDraw();
}

void JigsawPuzzleRenderer::CreatePuzzlePieceGeometry()
{
    // Create the geometry outline for the jigsaw puzzle piece.
    ComPtr<ID2D1PathGeometry> path;
    ComPtr<ID2D1GeometrySink> sink;

    DX::ThrowIfFailed(m_deviceResources->GetD2DFactory()->CreatePathGeometry(&path));
    DX::ThrowIfFailed(path->Open(&sink));

    // The following math calculates the information needed to determine where the arcs for the puzzle piece connector begin and end.
    // The inner radius is the radius of the arc that defines the stem of the connector. The outer radius is the radius of the arc
    // that defines the connector.

    float edgeLength = Constants::PuzzlePieceSize;
    float innerRadius = Constants::PuzzlePieceConnectorInnerRadius;
    float outerRadius = Constants::PuzzlePieceConnectorOuterRadius;
    float stemPosition = (edgeLength / 2.0f) - (innerRadius * 2.0f);
    float connectorParallelOffset = (2.0f * pow(innerRadius, 2.0f)) / (innerRadius + outerRadius);
    float connectorPerpendicularOffset = sqrt(pow(innerRadius, 2.0f) - pow(connectorParallelOffset, 2.0f)) + innerRadius;

    sink->BeginFigure(D2D1::Point2F(0, 0), D2D1_FIGURE_BEGIN_FILLED);
    sink->AddLine(D2D1::Point2F(edgeLength, 0));
    sink->AddLine(D2D1::Point2F(edgeLength, stemPosition));
    sink->AddArc(
        D2D1::ArcSegment(
            D2D1::Point2F(edgeLength - connectorPerpendicularOffset, stemPosition + connectorParallelOffset),
            D2D1::SizeF(innerRadius, innerRadius),
            0.0f,
            D2D1_SWEEP_DIRECTION_CLOCKWISE,
            D2D1_ARC_SIZE_SMALL
            )
        );
    sink->AddArc(
        D2D1::ArcSegment(
            D2D1::Point2F(edgeLength - connectorPerpendicularOffset, edgeLength - stemPosition - connectorParallelOffset),
            D2D1::SizeF(outerRadius, outerRadius),
            0.0f,
            D2D1_SWEEP_DIRECTION_COUNTER_CLOCKWISE,
            D2D1_ARC_SIZE_LARGE
            )
        );
    sink->AddArc(
        D2D1::ArcSegment(
            D2D1::Point2F(edgeLength, edgeLength - stemPosition),
            D2D1::SizeF(innerRadius, innerRadius),
            0.0f,
            D2D1_SWEEP_DIRECTION_CLOCKWISE,
            D2D1_ARC_SIZE_SMALL
            )
        );
    sink->AddLine(D2D1::Point2F(edgeLength, edgeLength));
    sink->AddLine(D2D1::Point2F(edgeLength - stemPosition, edgeLength));
    sink->AddArc(
        D2D1::ArcSegment(
            D2D1::Point2F(edgeLength - stemPosition - connectorParallelOffset, edgeLength + connectorPerpendicularOffset),
            D2D1::SizeF(innerRadius, innerRadius),
            0.0f,
            D2D1_SWEEP_DIRECTION_COUNTER_CLOCKWISE,
            D2D1_ARC_SIZE_SMALL
            )
        );
    sink->AddArc(
        D2D1::ArcSegment(
            D2D1::Point2F(stemPosition + connectorParallelOffset, edgeLength + connectorPerpendicularOffset),
            D2D1::SizeF(outerRadius, outerRadius),
            0.0f,
            D2D1_SWEEP_DIRECTION_CLOCKWISE,
            D2D1_ARC_SIZE_LARGE
            )
        );
    sink->AddArc(
        D2D1::ArcSegment(
            D2D1::Point2F(stemPosition, edgeLength),
            D2D1::SizeF(innerRadius, innerRadius),
            0.0f,
            D2D1_SWEEP_DIRECTION_COUNTER_CLOCKWISE,
            D2D1_ARC_SIZE_SMALL
            )
        );
    sink->AddLine(D2D1::Point2F(0, edgeLength));
    sink->EndFigure(D2D1_FIGURE_END_CLOSED);

    DX::ThrowIfFailed(sink->Close());
    m_geometry = path;
}

void JigsawPuzzleRenderer::CreateDeviceDependentResources()
{
    auto context = m_deviceResources->GetD2DDeviceContext();
    ComPtr<ID2D1SolidColorBrush> solidBrush;
    ComPtr<ID2D1RadialGradientBrush> gradientBrush;
    ComPtr<ID2D1GradientStopCollection> gradientStops;
    D2D1_GRADIENT_STOP stops[2] = {
        D2D1::GradientStop(0.0f, D2D1::ColorF(1.0f, 0.0f, 0.0f)),
        D2D1::GradientStop(1.0f, D2D1::ColorF(0.0f, 0.0f, 1.0f))
    };

    DX::ThrowIfFailed(
        context->CreateSolidColorBrush(D2D1::ColorF(0.0f, 0.0f, 0.0f), &m_textBrush)
        );

    DX::ThrowIfFailed(
        context->CreateSolidColorBrush(D2D1::ColorF(0.5f, 0.5f, 0.5f, 0.5f), &solidBrush)
        );

    DX::ThrowIfFailed(
        context->CreateGradientStopCollection(stops, ARRAYSIZE(stops), &gradientStops)
        );

    DX::ThrowIfFailed(
        context->CreateRadialGradientBrush(
            D2D1::RadialGradientBrushProperties(
                D2D1::Point2F(Constants::PuzzlePieceSize, Constants::PuzzlePieceSize),
                D2D1::Point2F(0.0f, 0.0f),
                Constants::PuzzlePieceSize,
                Constants::PuzzlePieceSize
                ),
            gradientStops.Get(),
            &gradientBrush
            )
        );

    for (UINT n = 0; n < Constants::PuzzlePieceCount; n++)
    {
        m_pieceRenderers[n].SetResources(m_geometry.Get(), gradientBrush.Get());
        m_targetRenderers[n].SetResources(m_geometry.Get(), solidBrush.Get());
    }
}

void JigsawPuzzleRenderer::CreateWindowSizeDependentResources()
{
    Size outputSize = m_deviceResources->GetOutputSize();
}

void JigsawPuzzleRenderer::ReleaseDeviceDependentResources()
{
    for (UINT n = 0; n < Constants::PuzzlePieceCount; n++)
    {
        m_pieceRenderers[n].ReleaseResources();
        m_targetRenderers[n].ReleaseResources();
    }
    m_textBrush.Reset();
}
