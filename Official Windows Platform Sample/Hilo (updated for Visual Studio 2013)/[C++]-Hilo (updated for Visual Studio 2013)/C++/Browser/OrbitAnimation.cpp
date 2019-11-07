//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "stdafx.h"
#include "OrbitAnimation.h"

using namespace Hilo::AnimationHelpers;

OrbitAnimation::OrbitAnimation() : m_initialOrbit(D2D1::Ellipse(D2D1::Point2F(0, 0), 10, 10))
{
}

OrbitAnimation::OrbitAnimation(D2D1_ELLIPSE initialOrbit) : m_initialOrbit(initialOrbit)
{
}

OrbitAnimation::~OrbitAnimation()
{
}

HRESULT OrbitAnimation::GetEllipse(D2D1_ELLIPSE* ellipse, double* opacity)
{
    HRESULT hr = S_OK;

    double centerX = 0;
    double centerY = 0;
    double radiusX = 100;
    double radiusY = 100;

    if (nullptr != ellipse)
    {
        hr = m_centerX->GetValue(&centerX);

        if (SUCCEEDED(hr))
        {
            hr = m_centerY->GetValue(&centerY);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_radiusX->GetValue(&radiusX);
        }

        if (SUCCEEDED(hr))
        {
            hr = m_radiusY->GetValue(&radiusY);
        }

        *ellipse = D2D1::Ellipse(
            D2D1::Point2F(static_cast<float>(centerX), static_cast<float>(centerY)),
            static_cast<float>(radiusX),
            static_cast<float>(radiusY));
    }

    if (SUCCEEDED(hr))
    {
        if (nullptr != opacity)
        {
            hr = m_opacity->GetValue(opacity);
        }
    }

    return hr;
}

HRESULT OrbitAnimation::Initialize()
{
    ComPtr<IUIAnimationManager> animationManager;

    HRESULT hr = AnimationUtility::GetAnimationManager(&animationManager);
    if (SUCCEEDED(hr))
    {
        hr = animationManager->CreateAnimationVariable(m_initialOrbit.point.x, &m_centerX);
    }

    if (SUCCEEDED(hr))
    {
        hr = animationManager->CreateAnimationVariable(m_initialOrbit.point.y, &m_centerY);
    }

    if (SUCCEEDED(hr))
    {
        hr = animationManager->CreateAnimationVariable(m_initialOrbit.radiusX, &m_radiusX);
    }

    if (SUCCEEDED(hr))
    {
        hr = animationManager->CreateAnimationVariable(m_initialOrbit.radiusY, &m_radiusY);
    }

    if (SUCCEEDED(hr))
    {
        hr = animationManager->CreateAnimationVariable(1, &m_opacity);
    }

    return hr;
}

HRESULT OrbitAnimation::Setup(D2D1_ELLIPSE targetEllipse, double targetOpacity, double duration)
{
    // Animation objects
    ComPtr<IUIAnimationManager> animationManager;
    ComPtr<IUIAnimationTimer> animationTimer;
    ComPtr<IUIAnimationTransitionLibrary> transitionLibrary;
    ComPtr<IUIAnimationStoryboard> storyboard;

    // Transition objects
    ComPtr<IUIAnimationTransition> centerXTransition;
    ComPtr<IUIAnimationTransition> centerYTransition;
    ComPtr<IUIAnimationTransition> radiusXTransition;
    ComPtr<IUIAnimationTransition> radiusYTransition;
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
            hr = transitionLibrary->CreateLinearTransition(duration, targetEllipse.point.x, &centerXTransition);

            if (SUCCEEDED(hr))
            {
                hr = storyboard->AddTransition(m_centerX, centerXTransition);
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = transitionLibrary->CreateLinearTransition(duration, targetEllipse.point.y, &centerYTransition);

            if (SUCCEEDED(hr))
            {
                hr = storyboard->AddTransition(m_centerY, centerYTransition);
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = transitionLibrary->CreateLinearTransition(duration, targetEllipse.radiusX, &radiusXTransition);

            if (SUCCEEDED(hr))
            {
                hr = storyboard->AddTransition(m_radiusX, radiusXTransition);
            }
        }

        if (SUCCEEDED(hr))
        {
            hr = transitionLibrary->CreateLinearTransition(duration, targetEllipse.radiusY, &radiusYTransition);

            if (SUCCEEDED(hr))
            {
                hr = storyboard->AddTransition(m_radiusY, radiusYTransition);
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
