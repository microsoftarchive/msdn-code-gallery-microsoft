//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

#include "ImageOperation.h"

[uuid("17A76C82-5618-451F-A3D3-CEEC1A503749")]
__interface IDrawGeometryOperation : public IUnknown
{
    HRESULT __stdcall DrawToRenderTarget(__in ID2D1RenderTarget* renderTarget, D2D1_RECT_F imageRect);
    HRESULT __stdcall DiscardResources();

    HRESULT __stdcall AppendPoint(__in ID2D1RenderTarget* renderTarget, __in D2D1_POINT_2F point);
    HRESULT __stdcall SetBrushColor(__in D2D1_COLOR_F brushColor);
    HRESULT __stdcall SetStrokeSize(__in float strokeSize);
};

class DrawGeometryOperation : public IImageOperation, public IDrawGeometryOperation
{
public:
    // IDrawGeometryOperation methods
    HRESULT __stdcall DrawToRenderTarget(__in ID2D1RenderTarget* renderTarget, D2D1_RECT_F imageRect);
    HRESULT __stdcall DiscardResources()
    {
        if (m_brush)
        {
            m_brush = nullptr;
        }

        return S_OK;
    }

protected:
    DrawGeometryOperation();
    virtual ~DrawGeometryOperation();

    // Interface helper
    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IImageOperation>::CastTo(iid, this, object) ||
            CastHelper<IDrawGeometryOperation>::CastTo(iid, this, object);
    }

    // IGeometryShape
    HRESULT __stdcall AppendPoint(__in ID2D1RenderTarget* renderTarget, __in D2D1_POINT_2F point);
    HRESULT __stdcall SetBrushColor(__in D2D1_COLOR_F brushColor);
    HRESULT __stdcall SetStrokeSize(__in float strokeSize);

private:
    ComPtr<ID2D1StrokeStyle> m_strokeStyle;
    ComPtr<ID2D1Geometry> m_geometry;
    ComPtr<ID2D1SolidColorBrush> m_brush;

    D2D1_COLOR_F m_brushColor;
    float m_strokeSize;

    std::vector<D2D1_POINT_2F> m_points;

    void GetSmoothingPoints(int i, __out D2D1_POINT_2F* point1, __out D2D1_POINT_2F* point2);
    HRESULT UpdateGeometry(ID2D1RenderTarget* renderTarget);
};

