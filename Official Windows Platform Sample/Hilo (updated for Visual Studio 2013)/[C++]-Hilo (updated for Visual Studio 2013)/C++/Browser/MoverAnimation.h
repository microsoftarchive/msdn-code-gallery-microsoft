//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
#include "LineAnimation.h"

class MoverAnimation : public LineAnimation
{
private:
    // Private constant params
    static const double m_duration;

    HRESULT AddTransition(IUIAnimationStoryboard* storyboard, IUIAnimationVariable* primaryVariable, double primaryValue, IUIAnimationVariable* secondVariable, double secondValue);

protected:
    MoverAnimation();
    virtual ~MoverAnimation();

    HRESULT BuildStoryboard();
    HRESULT CreateAnimatedThumbnailCells(const std::vector<ThumbnailCell>& prevThumbnailCells, const std::vector<ThumbnailCell>& currentThumbnailCells, D2D1_SIZE_F viewSize);
};
