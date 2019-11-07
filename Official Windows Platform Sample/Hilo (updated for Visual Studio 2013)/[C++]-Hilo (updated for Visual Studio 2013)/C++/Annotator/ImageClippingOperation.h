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

[uuid("46D742E6-A59B-406D-9BBD-2C77391A5F60")]
__interface IImageClippingOperation : public IUnknown
{
    HRESULT _stdcall GetClippingRect(__out D2D1_RECT_F* clippingRect);
    HRESULT _stdcall SetClippingRect(__in const D2D1_RECT_F& clippingRect);

    HRESULT _stdcall GetDrawingRect(__out D2D1_RECT_F* clippingRect);
    HRESULT _stdcall SetDrawingRect(__in const D2D1_RECT_F& clippingRect);

};

class ImageClippingOperation : public IImageOperation, public IImageClippingOperation
{
public:
    // IImageClippingOperation
    HRESULT _stdcall GetClippingRect(__out D2D1_RECT_F* clippingRect);
    HRESULT _stdcall SetClippingRect(__in const D2D1_RECT_F& clippingRect);
    HRESULT _stdcall GetDrawingRect(__out D2D1_RECT_F* clippingRect);
    HRESULT _stdcall SetDrawingRect(__in const D2D1_RECT_F& clippingRect);

protected:
    ImageClippingOperation(const D2D1_RECT_F& clippingRect);
    virtual ~ImageClippingOperation();

    // Interface helper
    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IImageOperation>::CastTo(iid, this, object) ||
            CastHelper<IImageClippingOperation>::CastTo(iid, this, object);
    }

private:
    D2D1_RECT_F m_clippingRect; // Relative to the full image size 
    D2D1_RECT_F m_drawingRect; // Absolute position within the window
};

