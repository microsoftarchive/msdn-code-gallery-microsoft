//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "MediaPaneAnimation.h"

using namespace Hilo::AnimationHelpers;

MediaPaneAnimation::MediaPaneAnimation()
{
}

MediaPaneAnimation::~MediaPaneAnimation()
{
}

HRESULT MediaPaneAnimation::Initialize()
{
    // Retrieve animation objects
    HRESULT hr = AnimationUtility::GetAnimationManager(&m_animationManager);

    if (SUCCEEDED(hr))
    {
        hr = AnimationUtility::GetTransitionLibrary(&m_transitionLibrary);
    }

    if (SUCCEEDED(hr))
    {
        hr = AnimationUtility::GetAnimationTimer(&m_animationTimer);
    }

    return hr;
}

HRESULT MediaPaneAnimation::Setup(const std::vector<ThumbnailCell> &prevThumbnailCells, const std::vector<ThumbnailCell> &currentThumbnailCells, D2D1_SIZE_F viewSize)
{
    HRESULT hr = CreateAnimatedThumbnailCells(prevThumbnailCells, currentThumbnailCells, viewSize);

    if (SUCCEEDED (hr))
    {
        hr = BuildStoryboard();
    }

    return hr;
}
