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

class OrbitAnimation : public IOrbitAnimation, public IInitializable
{
public:
    HRESULT __stdcall GetEllipse(__out D2D1_ELLIPSE* ellipse, __out_opt double* opacity);
    HRESULT __stdcall Setup(D2D1_ELLIPSE targetEllipse, double targetOpacity, double duration);

protected:
    // Constructor / Destructor
    OrbitAnimation();
    OrbitAnimation(D2D1_ELLIPSE initialOrbit);
    virtual ~OrbitAnimation();

    // QueryInterface helper
    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IOrbitAnimation>::CastTo(iid, this, object) ||
            CastHelper<IInitializable>::CastTo(iid, this, object);
    }

    // IInitializable implementation
    HRESULT __stdcall Initialize();

private:
    // Animation variables
    ComPtr<IUIAnimationVariable> m_centerX;
    ComPtr<IUIAnimationVariable> m_centerY;
    ComPtr<IUIAnimationVariable> m_radiusX;
    ComPtr<IUIAnimationVariable> m_radiusY;
    ComPtr<IUIAnimationVariable> m_opacity;

    // Initial values
    D2D1_ELLIPSE m_initialOrbit;
};
