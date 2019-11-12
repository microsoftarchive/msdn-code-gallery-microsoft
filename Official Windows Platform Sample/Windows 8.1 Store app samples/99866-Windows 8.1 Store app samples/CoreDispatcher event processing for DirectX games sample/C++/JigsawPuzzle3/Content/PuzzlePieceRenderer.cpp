//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "Common\DirectXHelper.h"
#include "Content\PuzzlePieceRenderer.h"

using namespace JigsawPuzzle;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;

PuzzlePieceRenderer::PuzzlePieceRenderer()
{
}

void PuzzlePieceRenderer::Initialize(const std::shared_ptr<PuzzlePiece>& piece, const std::shared_ptr<DX::DeviceResources>& deviceResources)
{
    m_piece = piece;
    m_deviceResources = deviceResources;
}

void PuzzlePieceRenderer::SetResources(ID2D1Geometry* puzzleShape, ID2D1Brush* brush)
{
    float rotation = 0.0f;
    switch (m_piece->GetId())
    {
    case TopLeft:
        m_geometryBrushTransform = D2D1::Matrix3x2F::Identity();
        break;

    case TopRight:
        rotation = 90.0f;
        m_geometryBrushTransform = D2D1::Matrix3x2F::Translation(-Constants::PuzzlePieceSize, 0.0f);
        break;

    case BottomLeft:
        rotation = 270.0f;
        m_geometryBrushTransform = D2D1::Matrix3x2F::Translation(0.0f, -Constants::PuzzlePieceSize);
        break;

    case BottomRight:
        rotation = 180.0f;
        m_geometryBrushTransform = D2D1::Matrix3x2F::Translation(-Constants::PuzzlePieceSize, -Constants::PuzzlePieceSize);
        break;
    }

    ComPtr<ID2D1Factory> factory;
    puzzleShape->GetFactory(&factory);

    DX::ThrowIfFailed(
        factory->CreateTransformedGeometry(
            puzzleShape,
            D2D1::Matrix3x2F::Rotation(
                rotation,
                D2D1::Point2F(Constants::PuzzlePieceSize / 2.0f, Constants::PuzzlePieceSize / 2.0f)
                ),
            &m_geometry
            )
        );

    DX::ThrowIfFailed(
        m_geometry->GetBounds(nullptr, &m_selectionBounds)
        );

    m_selectionBounds = D2D1::RectF(
        m_selectionBounds.left - 5.0f,
        m_selectionBounds.top - 5.0f,
        m_selectionBounds.right + 5.0f,
        m_selectionBounds.bottom + 5.0f
        );

    DX::ThrowIfFailed(
        m_deviceResources->GetD2DDeviceContext()->CreateSolidColorBrush(D2D1::ColorF(1.0f, 0.5f, 0.0f, 0.75f), &m_selectionBrush)
        );

    m_geometryBrush = brush;
}

void PuzzlePieceRenderer::ReleaseResources()
{
    m_geometry.Reset();
    m_geometryBrush.Reset();
    m_selectionBrush.Reset();
}

void PuzzlePieceRenderer::Render()
{
    auto context = m_deviceResources->GetD2DDeviceContext();
    auto offset = m_piece->GetPosition();
    D2D1_MATRIX_3X2_F geometryTransform = D2D1::Matrix3x2F::Translation(offset.X, offset.Y);

    m_geometryBrush->SetTransform(m_geometryBrushTransform);
    context->SetTransform(geometryTransform * m_deviceResources->GetOrientationTransform2D());

    if (m_piece->IsSnapped())
    {
        // Prevent the user from seeing seams in the geometry when puzzle pieces are snapped together.
        // All of the pixels are guaranteed to be rendered fully opaque if the pieces are rendered aliased.
        // Antialiased borders are rendered beneath the puzzle pieces, so this technique will not be
        // readily noticeable.

        context->SetAntialiasMode(D2D1_ANTIALIAS_MODE_ALIASED);
        context->FillGeometry(m_geometry.Get(), m_geometryBrush.Get());
        context->SetAntialiasMode(D2D1_ANTIALIAS_MODE_PER_PRIMITIVE);
    }
    else
    {
        context->FillGeometry(m_geometry.Get(), m_geometryBrush.Get());

        if (m_piece->IsSelected() && !m_piece->IsMoving())
        {
            context->DrawRectangle(m_selectionBounds, m_selectionBrush.Get(), 2.0f);
        }
    }
}
