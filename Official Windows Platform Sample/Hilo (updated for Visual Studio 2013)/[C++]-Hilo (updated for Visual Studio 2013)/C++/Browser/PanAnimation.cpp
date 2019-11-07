//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "stdafx.h"
#include "PanAnimation.h"

using namespace Hilo::AnimationHelpers;

PanAnimation::PanAnimation()
{
}

PanAnimation::~PanAnimation()
{
}

HRESULT PanAnimation::GetValue(double *currentPosition)
{
    return m_panValue->GetValue(currentPosition);
}

//
// Initializes the animation variable when this animation is created via SharedObject::Create.
//
HRESULT PanAnimation::Initialize()
{
    ComPtr<IUIAnimationManager> animationManager;

    HRESULT hr = AnimationUtility::GetAnimationManager(&animationManager);
    if (SUCCEEDED(hr))
    {
        hr = animationManager->CreateAnimationVariable(0, &m_panValue);
    }

    return hr;
}

//
// Sets up a new animation using the specified distance and duration
//
HRESULT PanAnimation::Setup(double startPosition, double targetPosition, double duration)
{
    // Animation objects
    ComPtr<IUIAnimationManager> animationManager;
    ComPtr<IUIAnimationTimer> animationTimer;
    ComPtr<IUIAnimationTransitionLibrary> transitionLibrary;
    ComPtr<IUIAnimationStoryboard> storyboard;

    // Transition objects
    ComPtr<IUIAnimationTransition> instantTransition;
    ComPtr<IUIAnimationTransition> panTransition;

    // Retrieve animation objects
    HRESULT hr = AnimationUtility::GetAnimationManager(&animationManager);
    if (SUCCEEDED(hr))
    {
        hr = AnimationUtility::GetTransitionLibrary(&transitionLibrary);
    }

    if (SUCCEEDED(hr))
    {
        hr = AnimationUtility::GetAnimationTimer(&animationTimer);
    }

    // Initialize storyboard
    if (SUCCEEDED(hr))
    {
        hr = animationManager->CreateStoryboard(&storyboard);

        if (SUCCEEDED(hr))
        {
            hr = transitionLibrary->CreateInstantaneousTransition(startPosition, &instantTransition);

            if (SUCCEEDED(hr))
            {
                hr = storyboard->AddTransition(m_panValue, instantTransition);
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = transitionLibrary->CreateAccelerateDecelerateTransition(
                duration,
                targetPosition,
                0.25,
                0.25,
                &panTransition);

            if (SUCCEEDED(hr))
            {
                hr = storyboard->AddTransition(m_panValue, panTransition);
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = AnimationUtility::ScheduleStoryboard(storyboard);
        }
    }

    return hr;
}
