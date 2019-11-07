//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once
#include "lineanimation.h"

class SlideAnimation : public LineAnimation
{
private:
    // Private onstant params
    static const double m_duration;
    static const double m_accelerationRatio;
    static const double m_decelerationRatio;

protected:
    SlideAnimation();
    virtual ~SlideAnimation();

    HRESULT BuildStoryboard();
    HRESULT CreateAnimatedThumbnailCells(const std::vector<ThumbnailCell>& prevThumbnailCells, const std::vector<ThumbnailCell>& currentThumbnailCells, D2D1_SIZE_F viewSize);
};
