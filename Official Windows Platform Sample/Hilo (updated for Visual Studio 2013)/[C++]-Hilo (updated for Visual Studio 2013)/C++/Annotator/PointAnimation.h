//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#pragma once

#include "Animation.h"

class PointAnimation : public IInitializable, public IPointAnimation
{
public:
    HRESULT __stdcall GetCurrentPoint(__out D2D1_POINT_2F* point);
    HRESULT __stdcall Setup(D2D1_POINT_2F targetPoint, double duration);

protected:
    // Constructor / destructor
    PointAnimation(D2D1_POINT_2F initialPoint);
    virtual ~PointAnimation();

    // Interface helper
    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IInitializable>::CastTo(iid, this, object) ||
            PointAnimation::QueryInterfaceHelper(iid, object);;
    }

    HRESULT __stdcall Initialize();

private:
    // Initial point. Used during SharedObject<PointAnimation>::Create method
    D2D1_POINT_2F m_initialPoint;

    // Animation variables
    ComPtr<IUIAnimationVariable> m_pointX;
    ComPtr<IUIAnimationVariable> m_pointY;
};