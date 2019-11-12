//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
#include "animation.h"

class MediaPaneAnimation: public IMediaPaneAnimation
{
protected:
    // Constructor / Destructor
    MediaPaneAnimation();
    virtual ~MediaPaneAnimation();

    bool QueryInterfaceHelper(const IID &iid, void **object)
    {
        return CastHelper<IMediaPaneAnimation>::CastTo(iid, this, object);
    }

    // Animation objects
    ComPtr<IUIAnimationManager> m_animationManager;
    ComPtr<IUIAnimationTimer> m_animationTimer;
    ComPtr<IUIAnimationTransitionLibrary> m_transitionLibrary;

    // Initialize basic animation variables
    HRESULT Initialize();

    // Different animation create different animated thumbnail cells
    virtual HRESULT CreateAnimatedThumbnailCells(const std::vector<ThumbnailCell>& prevThumbnailCells, const std::vector<ThumbnailCell>& currentThumbnailCells, D2D1_SIZE_F viewSize) = 0;
    // Different animation builds storyboard differently
    virtual HRESULT BuildStoryboard () = 0;

public:
     // Implementation
    HRESULT __stdcall Setup(const std::vector<ThumbnailCell>& prevThumbnailCells, const std::vector<ThumbnailCell>& currentThumbnailCells, D2D1_SIZE_F viewSize);

};
