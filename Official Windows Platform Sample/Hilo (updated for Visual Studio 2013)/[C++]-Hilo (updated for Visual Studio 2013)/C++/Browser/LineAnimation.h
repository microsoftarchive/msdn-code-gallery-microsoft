//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
#include "MediaPaneAnimation.h"

// Use the changes of x and y coordinates to represents an animated object
struct LineAnimationVar
{
    ComPtr<IUIAnimationVariable> animationX;
    ComPtr<IUIAnimationVariable> animationY;
};

// Every thumbnail control has a corresponding animation variable
typedef std::pair<ComPtr<IThumbnail>, LineAnimationVar> LineAnimationPair;
typedef std::map<ComPtr<IThumbnail>, LineAnimationVar> LineAnimationMap;

class LineAnimation : public MediaPaneAnimation
{
protected:
    // Variables
    LineAnimationMap m_animationPoints;
    std::vector<AnimatedThumbnailCell> m_animatedThumbnailCells;

public:
    // Constructor / Destructor
    LineAnimation();
    virtual ~LineAnimation();

    // Implementation
    HRESULT __stdcall GetAnimationPosition(__in IThumbnail* const control, __out D2D1_POINT_2F* position);
    HRESULT __stdcall GetAnimatedThumbnailCells(std::vector<AnimatedThumbnailCell>& animatedCells);
};
