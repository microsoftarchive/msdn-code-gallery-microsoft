//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "DirectXSample.h"

ref class PaperFragment
{
internal:
    PaperFragment(
        Microsoft::WRL::ComPtr<ID2D1DeviceContext> d2dContext,
        ID2D1Geometry* pGeometry,
        D2D1_COLOR_F &color
        );

    void Draw();

    D2D1_POINT_2F GetPosition();

    void SetPosition(
        D2D1_POINT_2F position
        );

    bool HitTest(
        D2D1_POINT_2F point
        );

    bool HitTestWithGeometry(
        D2D1_POINT_2F point,
        ID2D1Geometry* pGeometry
        );

    PaperFragment^ GetIntersectionWith(
        D2D1_POINT_2F point,
        ID2D1Geometry* pGeometry
        );

    PaperFragment^ GetExclusionWith(
        D2D1_POINT_2F point,
        ID2D1Geometry* pGeometry
        );

private:
    void GeneratePaperBitmap();

    PaperFragment^ GetMergeWith(
        D2D1_POINT_2F point,
        ID2D1Geometry* pGeometry,
        D2D1_COMBINE_MODE mode
        );

    Microsoft::WRL::ComPtr<ID2D1DeviceContext>          m_d2dContext;
    Microsoft::WRL::ComPtr<ID2D1Geometry>               m_geometry;
    Microsoft::WRL::ComPtr<ID2D1SolidColorBrush>        m_brush;
    Microsoft::WRL::ComPtr<ID2D1Bitmap1>                m_bitmap;

    D2D1_COLOR_F                                        m_color;
    D2D1_POINT_2F                                       m_position;
    D2D1_POINT_2F                                       m_bitmapOffset;
};