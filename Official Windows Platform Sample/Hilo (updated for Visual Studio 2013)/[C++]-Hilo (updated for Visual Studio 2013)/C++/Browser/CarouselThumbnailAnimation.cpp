//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "stdafx.h"
#include "CarouselThumbnailAnimation.h"

using namespace Hilo::AnimationHelpers;

CarouselThumbnailAnimation::CarouselThumbnailAnimation() :
    m_initialPoint(D2D1::Point2F(0, 0)),
    m_initialOpacity(1)
{
}

CarouselThumbnailAnimation::CarouselThumbnailAnimation(D2D1_POINT_2F initialPoint, double initialOpacity) :
    m_initialPoint(initialPoint),
    m_initialOpacity(initialOpacity)
{
}

CarouselThumbnailAnimation::~CarouselThumbnailAnimation()
{
}

HRESULT CarouselThumbnailAnimation::GetInfo(D2D1_POINT_2F* point, double* opacity)
{
    HRESULT hr;

    double x = 0;
    double y = 0;
    double opacityValue = 1;

    hr = m_pointX->GetValue(&x);

    if (SUCCEEDED(hr))
    {
        hr = m_pointY->GetValue(&y);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_opacity->GetValue(&opacityValue);
    }

    *point = D2D1::Point2F(static_cast<float>(x), static_cast<float>(y));
    *opacity = opacityValue;

    return hr;
}

HRESULT CarouselThumbnailAnimation::Initialize()
{
    ComPtr<IUIAnimationManager> animationManager;

    HRESULT hr = AnimationUtility::GetAnimationManager(&animationManager);
    if (SUCCEEDED(hr))
    {
        hr = animationManager->CreateAnimationVariable(m_initialPoint.x, &m_pointX);
    }

    if (SUCCEEDED(hr))
    {
        hr = animationManager->CreateAnimationVariable(m_initialPoint.y, &m_pointY);
    }

    if (SUCCEEDED(hr))
    {
        hr = animationManager->CreateAnimationVariable(m_initialOpacity, &m_opacity);
    }

    return hr;
}

HRESULT CarouselThumbnailAnimation::Setup(D2D1_POINT_2F targetPoint, double targetOpacity, double duration)
{
    // Animation objects
    ComPtr<IUIAnimationManager> animationManager;
    ComPtr<IUIAnimationTimer> animationTimer;
    ComPtr<IUIAnimationTransitionLibrary> transitionLibrary;
    ComPtr<IUIAnimationStoryboard> storyboard;

    // Transition objects
    ComPtr<IUIAnimationTransition> pointXTransition;
    ComPtr<IUIAnimationTransition> pointYTransition;
    ComPtr<IUIAnimationTransition> opacityTransition;

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

        // Create one transition for each orbit variable
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
            hr = transitionLibrary->CreateLinearTransition(duration, targetOpacity, &opacityTransition);

            if (SUCCEEDED(hr))
            {
                hr = storyboard->AddTransition(m_opacity, opacityTransition);
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = AnimationUtility::ScheduleStoryboard(storyboard);
        }
    }

    return hr;
}

HRESULT CarouselThumbnailAnimation::Setup(D2D1_POINT_2F keyFramePoint, D2D1_POINT_2F targetPoint, double duration)
{
    // Animation objects
    ComPtr<IUIAnimationManager> animationManager;
    ComPtr<IUIAnimationTimer> animationTimer;
    ComPtr<IUIAnimationTransitionLibrary> transitionLibrary;
    ComPtr<IUIAnimationStoryboard> storyboard;

    // Transition objects
    ComPtr<IUIAnimationTransition> transitionX1;
    ComPtr<IUIAnimationTransition> transitionY1;
    ComPtr<IUIAnimationTransition> transitionX2;
    ComPtr<IUIAnimationTransition> transitionY2;

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

        // Add keyframe to increase the time of the first transition
        UI_ANIMATION_SECONDS keyFrameStartTime = duration * 0.2;
        UI_ANIMATION_KEYFRAME keyframe1;

        hr = storyboard->AddKeyframeAtOffset(
            UI_ANIMATION_KEYFRAME_STORYBOARD_START,
            keyFrameStartTime,
            &keyframe1);

        // Use initial point to determine whether or not to do first transition
        double initialPointY;
        hr = m_pointY->GetValue(&initialPointY);

        if (initialPointY > keyFramePoint.y)
        {
            if (SUCCEEDED(hr))
            {
                hr = transitionLibrary->CreateAccelerateDecelerateTransition(
                    keyFrameStartTime,
                    keyFramePoint.x,
                    0.0,
                    1.0,
                    &transitionX1);
            }

            if (SUCCEEDED(hr))
            {
                hr = storyboard->AddTransition(m_pointX, transitionX1);
            }

            if (SUCCEEDED(hr))
            {
                hr = transitionLibrary->CreateAccelerateDecelerateTransition(
                    keyFrameStartTime,
                    keyFramePoint.y,
                    1.0,
                    0.0,
                    &transitionY1);

                if (SUCCEEDED(hr))
                {
                    hr = storyboard->AddTransition(m_pointY, transitionY1);
                }
            }
        }
        else
        {
            keyFrameStartTime = 0.0;
        }

        if (SUCCEEDED(hr))
        {
            hr = transitionLibrary->CreateAccelerateDecelerateTransition(
                duration - keyFrameStartTime,
                targetPoint.x,
                1.0,
                0.0,
                &transitionX2);

            if (SUCCEEDED(hr))
            {
                if (initialPointY > keyFramePoint.y)
                {
                    hr = storyboard->AddTransitionAtKeyframe(m_pointX, transitionX2, keyframe1);
                }
                else
                {
                    hr = storyboard->AddTransition(m_pointX, transitionX2);
                }
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = transitionLibrary->CreateAccelerateDecelerateTransition(
                duration - keyFrameStartTime,
                targetPoint.y,
                0.0,
                1.0,
                &transitionY2);

            if (SUCCEEDED(hr))
            {
                if (initialPointY > keyFramePoint.y)
                {
                    hr = storyboard->AddTransitionAtKeyframe(m_pointY, transitionY2, keyframe1);
                }
                else
                {
                    hr = storyboard->AddTransition(m_pointY, transitionY2);
                }
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = AnimationUtility::ScheduleStoryboard(storyboard);
        }
    }

    return hr;
}
