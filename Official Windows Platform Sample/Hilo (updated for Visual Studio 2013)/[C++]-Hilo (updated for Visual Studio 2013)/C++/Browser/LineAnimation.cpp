//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "LineAnimation.h"

LineAnimation::LineAnimation()
{
}

LineAnimation::~LineAnimation()
{
}

// Read animation position for the specific control
HRESULT LineAnimation::GetAnimationPosition(IThumbnail *control, D2D1_POINT_2F *point)
{
    if (nullptr == control || nullptr == point)
    {
        return E_POINTER;
    }

    LineAnimationMap::const_iterator iter = m_animationPoints.find(control);

    // Every input control is expected to have an animation variable
    HRESULT hr = S_OK;
    if (iter != m_animationPoints.end())
    {
        double pointX, pointY;
        hr = iter->second.animationX->GetValue(&pointX);
        if (SUCCEEDED(hr))
        {
            point->x = static_cast<float>(pointX);
            hr = iter->second.animationY->GetValue(&pointY);

            if (SUCCEEDED(hr))
            {
                point->y = static_cast<float>(pointY);
            }
        }
    }

    return hr;
}

// Get all the animated thumbnail cells
HRESULT LineAnimation::GetAnimatedThumbnailCells(std::vector<AnimatedThumbnailCell>& animatedCells)
{
    animatedCells.assign(m_animatedThumbnailCells.begin(), m_animatedThumbnailCells.end());
    return S_OK;
}
