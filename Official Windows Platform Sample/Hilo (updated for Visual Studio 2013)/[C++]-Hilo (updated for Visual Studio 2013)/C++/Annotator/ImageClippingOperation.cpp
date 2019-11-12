//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#include "StdAfx.h"
#include "ImageClippingOperation.h"

ImageClippingOperation::ImageClippingOperation(const D2D1_RECT_F& clippingRect) : m_clippingRect(clippingRect)
{
}

ImageClippingOperation::~ImageClippingOperation()
{
}

// IImageClippingOperation
HRESULT ImageClippingOperation::GetClippingRect(D2D1_RECT_F* clippingRect)
{
    assert(clippingRect);

    *clippingRect = m_clippingRect;
    return S_OK;
}

HRESULT ImageClippingOperation::SetClippingRect(const D2D1_RECT_F& clippingRect)
{
    m_clippingRect = clippingRect;
    return S_OK;
}

// IImageClippingOperation
HRESULT ImageClippingOperation::GetDrawingRect(D2D1_RECT_F* drawingRect)
{
    assert(drawingRect);

    *drawingRect = m_drawingRect;
    return S_OK;
}

HRESULT ImageClippingOperation::SetDrawingRect(const D2D1_RECT_F& drawingRect)
{
    m_drawingRect = drawingRect;
    return S_OK;
}