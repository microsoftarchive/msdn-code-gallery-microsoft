//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "pch.h"
#include "PaperFragment.h"

using namespace Microsoft::WRL;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;

PaperFragment::PaperFragment(
    ComPtr<ID2D1DeviceContext> d2dContext,
    ID2D1Geometry* pGeometry,
    D2D1_COLOR_F &color
    )
{
    m_d2dContext = d2dContext;
    m_geometry = pGeometry;
    m_color = color;

    m_position = D2D1::Point2F(0, 0);

    DX::ThrowIfFailed(
        m_d2dContext->CreateSolidColorBrush(
            m_color,
            &m_brush
            )
        );

    GeneratePaperBitmap();
}

void PaperFragment::GeneratePaperBitmap()
{
    ComPtr<ID2D1Device> spDevice;
    m_d2dContext->GetDevice(&spDevice);

    ComPtr<ID2D1Effect> spEdgeTurbulance;
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(
            CLSID_D2D1Turbulence,
            &spEdgeTurbulance
            )
        );

    {
        D2D1_VECTOR_2F offset = {-100000, -100000};
        DX::ThrowIfFailed(
            spEdgeTurbulance->SetValue(D2D1_TURBULENCE_PROP_OFFSET, offset)
            );

        D2D1_VECTOR_2F size = {200000, 200000};
        DX::ThrowIfFailed(
            spEdgeTurbulance->SetValue(D2D1_TURBULENCE_PROP_SIZE, size)
            );

        D2D1_VECTOR_2F baseFrequency = {0.3f, 0.3f};
        DX::ThrowIfFailed(
            spEdgeTurbulance->SetValue(D2D1_TURBULENCE_PROP_BASE_FREQUENCY, baseFrequency)
            );

        DX::ThrowIfFailed(
            spEdgeTurbulance->SetValue(D2D1_TURBULENCE_PROP_SEED, 42)
            );
    }

    ComPtr<ID2D1Effect> spEdgeTurbulanceColorMatrix;
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(
            CLSID_D2D1ColorMatrix,
            &spEdgeTurbulanceColorMatrix
            )
        );

    {
        D2D_MATRIX_5X4_F colorMatrix =
        {
            2, 0, 0, 0,
            0, 2, 0, 0,
            0, 0, 2, 0,
            0, 0, 0, 1,
            0, 0, 0, 0
        };

        DX::ThrowIfFailed(
            spEdgeTurbulanceColorMatrix->SetValue(D2D1_COLORMATRIX_PROP_COLOR_MATRIX, colorMatrix)
            );
    }

    ComPtr<ID2D1Effect> spEdgeErode;
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(
            CLSID_D2D1Morphology,
            &spEdgeErode
            )
        );

    {
        DX::ThrowIfFailed(
            spEdgeErode->SetValue(D2D1_MORPHOLOGY_PROP_WIDTH, 10)
            );

        DX::ThrowIfFailed(
            spEdgeErode->SetValue(D2D1_MORPHOLOGY_PROP_HEIGHT, 10)
            );
    }

    ComPtr<ID2D1Effect> spEdgeDisplacement;
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(
            CLSID_D2D1DisplacementMap,
            &spEdgeDisplacement
            )
        );

    {
        DX::ThrowIfFailed(
            spEdgeDisplacement->SetValue(D2D1_DISPLACEMENTMAP_PROP_SCALE, 10.0f)
            );

        DX::ThrowIfFailed(
            spEdgeDisplacement->SetValue(
                D2D1_DISPLACEMENTMAP_PROP_X_CHANNEL_SELECT,
                D2D1_CHANNEL_SELECTOR_R
                )
            );

        DX::ThrowIfFailed(
            spEdgeDisplacement->SetValue(
                D2D1_DISPLACEMENTMAP_PROP_Y_CHANNEL_SELECT,
                D2D1_CHANNEL_SELECTOR_G
                )
            );
    }

    ComPtr<ID2D1Effect> spBumpTurbulance;
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(
            CLSID_D2D1Turbulence,
            &spBumpTurbulance
            )
        );

    {
        D2D1_VECTOR_2F offset = {-100000, -100000};
        DX::ThrowIfFailed(
            spBumpTurbulance->SetValue(D2D1_TURBULENCE_PROP_OFFSET, offset)
            );

        D2D1_VECTOR_2F size = {200000, 200000};
        DX::ThrowIfFailed(
            spBumpTurbulance->SetValue(D2D1_TURBULENCE_PROP_SIZE, size)
            );

        D2D1_VECTOR_2F baseFrequency = {0.3f, 0.3f};
        DX::ThrowIfFailed(
            spBumpTurbulance->SetValue(D2D1_TURBULENCE_PROP_BASE_FREQUENCY, baseFrequency)
            );

        DX::ThrowIfFailed(
            spBumpTurbulance->SetValue(D2D1_TURBULENCE_PROP_SEED, 42)
            );
    }

    ComPtr<ID2D1Effect> spBumpDiffuse;
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(
            CLSID_D2D1DistantSpecular,
            &spBumpDiffuse
            )
        );

    {
        DX::ThrowIfFailed(
            spBumpDiffuse->SetValue(D2D1_DISTANTSPECULAR_PROP_SURFACE_SCALE, 0.25f)
            );
    }

    ComPtr<ID2D1Effect> spBumpComposite;
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(
            CLSID_D2D1ArithmeticComposite,
            &spBumpComposite
            )
        );

    {
        D2D1_VECTOR_4F coefficients = {1.3f, 0, 0, 0};
        DX::ThrowIfFailed(
            spBumpComposite->SetValue(D2D1_ARITHMETICCOMPOSITE_PROP_COEFFICIENTS, coefficients)
            );
    }

    ComPtr<ID2D1Effect> spShadow;
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(
            CLSID_D2D1Shadow,
            &spShadow
            )
        );

    {
        DX::ThrowIfFailed(
            spShadow->SetValue(D2D1_SHADOW_PROP_BLUR_STANDARD_DEVIATION, 3.0f)
            );
    }

    ComPtr<ID2D1Effect> spShadowOffset;
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(
            CLSID_D2D12DAffineTransform,
            &spShadowOffset
            )
        );

    {
        DX::ThrowIfFailed(
            spShadowOffset->SetValue(
                D2D1_2DAFFINETRANSFORM_PROP_TRANSFORM_MATRIX,
                D2D1::Matrix3x2F::Translation(3, 3)
                )
            );
    }

    ComPtr<ID2D1Effect> spShadowComposite;
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(
            CLSID_D2D1Composite,
            &spShadowComposite
            )
        );

    ComPtr<ID2D1Effect> spEdgeDisplacementColorMatrix;
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(
            CLSID_D2D1ColorMatrix,
            &spEdgeDisplacementColorMatrix
            )
        );

    {
        D2D_MATRIX_5X4_F colorMatrix =
        {
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 0,
            0, 0, 0, 1,
            1, 1, 1, 0
        };

        DX::ThrowIfFailed(
            spEdgeDisplacementColorMatrix->SetValue(
                D2D1_COLORMATRIX_PROP_COLOR_MATRIX,
                colorMatrix
                )
            );
    }

    ComPtr<ID2D1Effect> spEdgeDisplacementComposite;
    DX::ThrowIfFailed(
        m_d2dContext->CreateEffect(
            CLSID_D2D1ArithmeticComposite,
            &spEdgeDisplacementComposite
            )
        );

    ComPtr<ID2D1CommandList> spCommandList;
    DX::ThrowIfFailed(
        m_d2dContext->CreateCommandList(&spCommandList)
        );

    ComPtr<ID2D1Image> oldTarget;
    m_d2dContext->GetTarget(&oldTarget);

    m_d2dContext->SetTarget(spCommandList.Get());

    D2D1_MATRIX_3X2_F oldTransform;
    m_d2dContext->GetTransform(&oldTransform);

    m_d2dContext->SetTransform(D2D1::Matrix3x2F::Identity());

    m_d2dContext->BeginDraw();

    m_d2dContext->FillGeometry(
        m_geometry.Get(),
        m_brush.Get()
        );

    DX::ThrowIfFailed(
        m_d2dContext->EndDraw()
        );

    DX::ThrowIfFailed(
        spCommandList->Close()
        );

    spEdgeErode->SetInput(0, spCommandList.Get());

    spEdgeTurbulanceColorMatrix->SetInputEffect(0, spEdgeTurbulance.Get());

    spEdgeDisplacement->SetInputEffect(0, spEdgeErode.Get());
    spEdgeDisplacement->SetInputEffect(1, spEdgeTurbulanceColorMatrix.Get());

    spEdgeDisplacementColorMatrix->SetInputEffect(0, spEdgeDisplacement.Get());

    spShadow->SetInputEffect(0, spEdgeDisplacement.Get());

    spShadowOffset->SetInputEffect(0, spShadow.Get());

    spEdgeDisplacementComposite->SetInputEffect(0, spEdgeDisplacementColorMatrix.Get());
    spEdgeDisplacementComposite->SetInput(1, spCommandList.Get());

    spBumpDiffuse->SetInputEffect(0, spBumpTurbulance.Get());

    spBumpComposite->SetInputEffect(0, spEdgeDisplacementComposite.Get());
    spBumpComposite->SetInputEffect(1, spBumpDiffuse.Get());

    spShadowComposite->SetInputEffect(0, spShadowOffset.Get());
    spShadowComposite->SetInputEffect(1, spBumpComposite.Get());

    D2D1_RECT_F bounds;

    ComPtr<ID2D1Image> spOutputImage;
    spShadowComposite->GetOutput(&spOutputImage);

    DX::ThrowIfFailed(
        m_d2dContext->GetImageLocalBounds(
            spOutputImage.Get(),
            &bounds
            )
        );

    m_bitmapOffset = D2D1::Point2F(
        static_cast<float>(bounds.left),
        static_cast<float>(bounds.top)
        );

    DX::ThrowIfFailed(
        m_d2dContext->CreateBitmap(
            D2D1::SizeU(
                static_cast<uint32>(bounds.right - bounds.left),
                static_cast<uint32>(bounds.bottom - bounds.top)
                ),
            nullptr,    // sourceData
            0,          // pitch
            D2D1::BitmapProperties1(
                D2D1_BITMAP_OPTIONS_TARGET,
                D2D1::PixelFormat(
                    DXGI_FORMAT_B8G8R8A8_UNORM,
                    D2D1_ALPHA_MODE_PREMULTIPLIED
                    )
                ),
            &m_bitmap
            )
        );

    m_d2dContext->SetTarget(m_bitmap.Get());

    m_d2dContext->BeginDraw();

    m_d2dContext->Clear(D2D1::ColorF(0, 0, 0, 0));

    m_d2dContext->SetTransform(
        D2D1::Matrix3x2F::Translation(
            -m_bitmapOffset.x,
            -m_bitmapOffset.y
            )
        );

    m_d2dContext->DrawImage(spShadowComposite.Get());

    DX::ThrowIfFailed(
        m_d2dContext->EndDraw()
        );

    m_d2dContext->SetTarget(oldTarget.Get());

    m_d2dContext->SetTransform(oldTransform);
}

void PaperFragment::Draw()
{
    D2D1::Matrix3x2F originalTransform;
    m_d2dContext->GetTransform(&originalTransform);

    {
        D2D1::Matrix3x2F offsetTransform =
            D2D1::Matrix3x2F::Translation(
                m_bitmapOffset.x + m_position.x,
                m_bitmapOffset.y + m_position.y
                ) * originalTransform;

        m_d2dContext->SetTransform(&offsetTransform);

        m_d2dContext->DrawBitmap(m_bitmap.Get());
    }

    m_d2dContext->SetTransform(&originalTransform);
}

D2D1_POINT_2F PaperFragment::GetPosition()
{
    return m_position;
}

void PaperFragment::SetPosition(D2D1_POINT_2F position)
{
    m_position = position;
}

bool PaperFragment::HitTest(D2D1_POINT_2F point)
{
    BOOL contains;

    DX::ThrowIfFailed(
        m_geometry->FillContainsPoint(
            point,
            D2D1::Matrix3x2F::Translation(
                m_position.x,
                m_position.y
                ),
            &contains
            )
        );

    return (contains ? true : false);
}

bool PaperFragment::HitTestWithGeometry(D2D1_POINT_2F point, ID2D1Geometry* pGeometry)
{
    D2D1_GEOMETRY_RELATION relation;

    DX::ThrowIfFailed(
        m_geometry->CompareWithGeometry(
            pGeometry,
            D2D1::Matrix3x2F::Translation(
                point.x - m_position.x,
                point.y - m_position.y
                ),
            &relation
            )
        );

    return (relation != D2D1_GEOMETRY_RELATION_DISJOINT);
}

PaperFragment^ PaperFragment::GetMergeWith(
    D2D1_POINT_2F point,
    ID2D1Geometry* pGeometry,
    D2D1_COMBINE_MODE mode
    )
{
    ComPtr<ID2D1Factory> spFactory;
    ComPtr<ID2D1TransformedGeometry> spTransformedGeometry;

    m_geometry->GetFactory(&spFactory);

    DX::ThrowIfFailed(
        spFactory->CreateTransformedGeometry(
            m_geometry.Get(),
            D2D1::Matrix3x2F::Translation(
                m_position.x,
                m_position.y
                ),
            &spTransformedGeometry
            )
        );

    ComPtr<ID2D1PathGeometry> spPathGeometry;
    ComPtr<ID2D1GeometrySink> spGeometrySink;

    DX::ThrowIfFailed(
        spFactory->CreatePathGeometry(&spPathGeometry)
        );

    DX::ThrowIfFailed(
        spPathGeometry->Open(&spGeometrySink)
        );

    DX::ThrowIfFailed(
        spTransformedGeometry->CombineWithGeometry(
            pGeometry,
            mode,
            D2D1::Matrix3x2F::Translation(
                point.x,
                point.y
                ),
            spGeometrySink.Get()
            )
        );

    DX::ThrowIfFailed(
        spGeometrySink->Close()
        );

    PaperFragment^ spNewFragment =
        ref new PaperFragment(
            m_d2dContext,
            spPathGeometry.Get(),
            m_color
            );

    spNewFragment->SetPosition(D2D1::Point2F(0, 0));

    return spNewFragment;
}

PaperFragment^ PaperFragment::GetIntersectionWith(
    D2D1_POINT_2F point,
    ID2D1Geometry* pGeometry
    )
{
    return GetMergeWith(
        point,
        pGeometry,
        D2D1_COMBINE_MODE_INTERSECT
        );
}

PaperFragment^ PaperFragment::GetExclusionWith(
    D2D1_POINT_2F point,
    ID2D1Geometry *pGeometry
    )
{
    return GetMergeWith(
        point,
        pGeometry,
        D2D1_COMBINE_MODE_EXCLUDE
        );
}
