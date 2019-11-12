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

class CarouselAnimation : public ICarouselAnimation, public IInitializable
{
public:
    // Getters
    HRESULT __stdcall GetInfo(double* rotation, double* thumbnailScale, double* thumbnailOpacity);

    // Methods
    HRESULT __stdcall SetupRotation(double targetRotation, double duration);
    HRESULT __stdcall SetupScale(double targetScale, double duration);
    HRESULT __stdcall SetupOpacity(double targetOpacity, double duration);

protected:
    // Constructor / Destructor
    CarouselAnimation();
    virtual ~CarouselAnimation();

    // Query interface helpers
    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<ICarouselAnimation>::CastTo(iid, this, object) ||
            CastHelper<IInitializable>::CastTo(iid, this, object);
    }

    // IInitializable implementation
    HRESULT __stdcall Initialize();

private:
    // Animation objects
    ComPtr<IUIAnimationManager> m_animationManager;
    ComPtr<IUIAnimationTimer> m_animationTimer;
    ComPtr<IUIAnimationTransitionLibrary> m_transitionLibrary;
    
    // Animation variables
    ComPtr<IUIAnimationVariable> m_rotation;
    ComPtr<IUIAnimationVariable> m_thumbnailScale;
    ComPtr<IUIAnimationVariable> m_thumbnailOpacity;
};
