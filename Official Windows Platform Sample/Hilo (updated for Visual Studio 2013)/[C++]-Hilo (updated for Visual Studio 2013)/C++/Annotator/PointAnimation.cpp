//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================

#include "stdafx.h"
#include "PointAnimation.h"

using namespace Hilo::AnimationHelpers;
using namespace Hilo::Direct2DHelpers;

PointAnimation::PointAnimation(D2D1_POINT_2F initialPoint) : m_initialPoint(initialPoint)
{
}

PointAnimation::~PointAnimation()
{
}

//
// Retrieve the current animation values for x and y
//
HRESULT PointAnimation::GetCurrentPoint(D2D1_POINT_2F* point)
{
    HRESULT hr = S_OK;

    double pointX = 0;
    double pointY = 0;

    if (nullptr != point)
    {
        hr = m_pointX->GetValue(&pointX);

        if (SUCCEEDED(hr))
        {
            hr = m_pointY->GetValue(&pointY);
        }

        *point = D2D1::Point2F(static_cast<float>(pointX), static_cast<float>(pointY));
    }

    return hr;
}

//
// Initialize image animation with specificed point
//
HRESULT PointAnimation::Initialize()
{
    ComPtr<IUIAnimationManager> animationManager;
    HRESULT hr = AnimationUtility::GetAnimationManager(&animationManager);

    if (SUCCEEDED(hr) && nullptr == m_pointX)
    {
        hr = animationManager->CreateAnimationVariable(m_initialPoint.x, &m_pointX);
    }

    if (SUCCEEDED(hr) && nullptr == m_pointY)
    {
        hr = animationManager->CreateAnimationVariable(m_initialPoint.y, &m_pointY);
    }
    
    return S_OK;
}

//
// Setup a new animation that animation from the current point to the specified target point
//
HRESULT PointAnimation::Setup(D2D1_POINT_2F targetPoint, double duration)
{
    // Animation objects
    ComPtr<IUIAnimationManager> animationManager;
    ComPtr<IUIAnimationTransitionLibrary> transitionLibrary;
    ComPtr<IUIAnimationStoryboard> storyboard;

    // Transition objects
    ComPtr<IUIAnimationTransition> pointXTransition;
    ComPtr<IUIAnimationTransition> pointYTransition;

    // Retrieve animation objects
    HRESULT hr = AnimationUtility::GetAnimationManager(&animationManager);
    if (SUCCEEDED(hr))
    {
        hr = AnimationUtility::GetTransitionLibrary(&transitionLibrary);
    }

    // Initialize storyboard
    if (SUCCEEDED(hr))
    {
        hr = animationManager->CreateStoryboard(&storyboard);
        
        // Create one transition each coordinate
        if (SUCCEEDED(hr))
        {
            hr = transitionLibrary->CreateLinearTransition(duration, targetPoint.x, &pointXTransition);
            if (SUCCEEDED(hr))
            {
                hr = storyboard->AddTransition(m_pointX, pointXTransition);
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = transitionLibrary->CreateLinearTransition(duration, targetPoint.y, &pointYTransition);
            if (SUCCEEDED(hr))
            {
                hr = storyboard->AddTransition(m_pointY, pointYTransition);
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = AnimationUtility::ScheduleStoryboard(storyboard);
        }
    }

    return hr;
}