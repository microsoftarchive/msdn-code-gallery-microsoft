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

[uuid("AF20381E-E9D5-4FD9-8CDB-6572BDC54785")]
__interface IImageTransformationOperation : public IUnknown
{
    HRESULT __stdcall GetTransformationMatrix(__in D2D1_POINT_2F point, __out D2D1_MATRIX_3X2_F* transform);
    HRESULT __stdcall GetInverseTransformationMatrix(__in D2D1_POINT_2F point, __out D2D1_MATRIX_3X2_F* transform);
    HRESULT __stdcall GetTransformationType(__out ImageOperationType* transformationType);
};

class ImageTransformationOperation : public IImageOperation, public IImageTransformationOperation
{
public:
    // IImageTransformationOperation methods
    HRESULT __stdcall GetTransformationMatrix(__in D2D1_POINT_2F point, __out D2D1_MATRIX_3X2_F* transform);
    HRESULT __stdcall GetInverseTransformationMatrix(__in D2D1_POINT_2F point, __out D2D1_MATRIX_3X2_F* transform);
    HRESULT __stdcall GetTransformationType(__out ImageOperationType* transformationType);

protected:
    ImageTransformationOperation(ImageOperationType);
    virtual ~ImageTransformationOperation();

    // Interface helper
    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IImageOperation>::CastTo(iid, this, object) || CastHelper<IImageTransformationOperation>::CastTo(iid, this, object);
    }

private:
    ImageOperationType m_transformationType;
};

