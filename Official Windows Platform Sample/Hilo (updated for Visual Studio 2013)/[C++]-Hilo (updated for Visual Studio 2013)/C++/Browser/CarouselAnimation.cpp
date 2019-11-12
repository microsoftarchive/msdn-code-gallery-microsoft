//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "stdafx.h"
#include "CarouselAnimation.h"

using namespace Hilo::AnimationHelpers;

//
// Constructor
//
CarouselAnimation::CarouselAnimation()
{
}

//
// Destructor
//
CarouselAnimation::~CarouselAnimation()
{
}

//
// Retrieve the current values for this animation
//
HRESULT CarouselAnimation::GetInfo(double* rotation, double* thumbnailScale, double* thumbnailOpacity)
{
    HRESULT hr = S_OK;

    double rotationValue = 0;
    double scale = 1;
    double opacity = 1;

    if (rotation)
    {
        hr = m_rotation->GetValue(&rotationValue);

        *rotation = rotationValue;
    }

    if (SUCCEEDED(hr) && thumbnailScale)
    {
        hr = m_thumbnailScale->GetValue(&scale);

        *thumbnailScale = scale;
    }

    if (SUCCEEDED(hr) && thumbnailOpacity)
    {
        hr = m_thumbnailOpacity->GetValue(&opacity);

        *thumbnailOpacity = opacity;
    }

    return hr;
}

//
// Initializes a new CarouselAnimation object. Called via SharedObject<>::Create
//
HRESULT CarouselAnimation::Initialize()
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

    if (SUCCEEDED(hr))
    {
        hr = m_animationManager->CreateAnimationVariable(0, &m_rotation);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_animationManager->CreateAnimationVariable(1, &m_thumbnailScale);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_animationManager->CreateAnimationVariable(1, &m_thumbnailOpacity);
    }

    return hr;
}

//
// Setup a rotation animation. Rotation is specified in radians
//
HRESULT CarouselAnimation::SetupRotation(double rotation, double duration)
{
    // Animation objects
    ComPtr<IUIAnimationStoryboard> storyboard;
    ComPtr<IUIAnimationTransition> transition;

    // Initialize storyboard
    HRESULT hr = m_animationManager->CreateStoryboard(&storyboard);

    if (SUCCEEDED(hr))
    {
        // Create rotation transition
        hr = m_transitionLibrary->CreateAccelerateDecelerateTransition(
            duration,
            rotation,
            0.0,
            1.0,
            &transition);

        if (SUCCEEDED(hr))
        {
            hr = storyboard->AddTransition(m_rotation, transition);
        }

        if (SUCCEEDED(hr))
        {
            hr = AnimationUtility::ScheduleStoryboard(storyboard);
        }
    }

    return hr;
}

//
// Setup a scaling animation
//
HRESULT CarouselAnimation::SetupScale(double thumbnailScale, double duration)
{
    // Animation objects
    ComPtr<IUIAnimationStoryboard> storyboard;
    ComPtr<IUIAnimationTransition> transition;

    // Initialize storyboard
    HRESULT hr = m_animationManager->CreateStoryboard(&storyboard);

    if (SUCCEEDED(hr))
    {
        // Create scale transition
        hr = m_transitionLibrary->CreateLinearTransition(
            duration,
            thumbnailScale,
            &transition);

        if (SUCCEEDED(hr))
        {
            hr = storyboard->AddTransition(m_thumbnailScale, transition);
        }

        if (SUCCEEDED(hr))
        {
            hr = AnimationUtility::ScheduleStoryboard(storyboard);
        }
    }

    return hr;
}

//
// Setup an animation for change in opacity
//
HRESULT CarouselAnimation::SetupOpacity(double thumbnailOpacity, double duration)
{
    // Animation objects
    ComPtr<IUIAnimationStoryboard> storyboard;
    ComPtr<IUIAnimationTransition> transition;

    // Initialize storyboard
    HRESULT hr = m_animationManager->CreateStoryboard(&storyboard);

    if (SUCCEEDED(hr))
    {
        // Create rotation transition
        hr = m_transitionLibrary->CreateLinearTransition(
            duration,
            thumbnailOpacity,
            &transition);

        if (SUCCEEDED(hr))
        {
            hr = storyboard->AddTransition(m_thumbnailOpacity, transition);
        }

        if (SUCCEEDED(hr))
        {
            hr = AnimationUtility::ScheduleStoryboard(storyboard);
        }
    }

    return hr;
}
