//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "Common\DirectXHelper.h"
#include "Content\DropTargetRenderer.h"

using namespace JigsawPuzzle;
using namespace Microsoft::WRL;
using namespace Windows::Foundation;

DropTargetRenderer::DropTargetRenderer()
{
}

void DropTargetRenderer::Initialize(const std::shared_ptr<DropTarget>& target, const std::shared_ptr<DX::DeviceResources>& deviceResources)
{
    m_target = target;
    m_deviceResources = deviceResources;
}

void DropTargetRenderer::SetResources(ID2D1Geometry* puzzleShape, ID2D1SolidColorBrush* brush)
{
    float rotation = 0.0f;
    switch (m_target->GetId())
    {
    case TopLeft:
        break;

    case TopRight:
        rotation = 90.0f;
        break;

    case BottomLeft:
        rotation = 270.0f;
        break;

    case BottomRight:
        rotation = 180.0f;
        break;
    }

    ComPtr<ID2D1Factory> factory;
    puzzleShape->GetFactory(&factory);

    factory->CreateTransformedGeometry(
        puzzleShape,
        D2D1::Matrix3x2F::Rotation(
            rotation,
            D2D1::Point2F(Constants::PuzzlePieceSize / 2.0f, Constants::PuzzlePieceSize / 2.0f)
            ),
        &m_geometry
        );

    m_brush = brush;
}

void DropTargetRenderer::ReleaseResources()
{
    m_geometry.Reset();
    m_brush.Reset();
}

void DropTargetRenderer::Render()
{
    auto context = m_deviceResources->GetD2DDeviceContext();
    auto offset = m_target->GetPosition();
    D2D1_MATRIX_3X2_F geometryTransform = D2D1::Matrix3x2F::Translation(offset.X, offset.Y);

    context->SetTransform(geometryTransform * m_deviceResources->GetOrientationTransform2D());
    m_brush->SetColor(D2D1::ColorF(0.5f, 0.0f, 0.5f, 0.4f));
    context->DrawGeometry(m_geometry.Get(), m_brush.Get(), 2.0f);

    if (m_target->IsHovering())
    {
        m_brush->SetColor(D2D1::ColorF(1.0f, 0.5f, 0.0f, 0.5f));
        context->FillGeometry(m_geometry.Get(), m_brush.Get());
    }
}
