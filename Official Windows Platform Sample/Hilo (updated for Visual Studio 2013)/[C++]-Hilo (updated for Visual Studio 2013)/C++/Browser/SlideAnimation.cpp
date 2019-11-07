//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "SlideAnimation.h"

const double SlideAnimation::m_duration = 0.5;
const double SlideAnimation::m_accelerationRatio = 0.5;
const double SlideAnimation::m_decelerationRatio = 0.5;

using namespace Hilo::AnimationHelpers;

SlideAnimation::SlideAnimation()
{
}

SlideAnimation::~SlideAnimation()
{
}

// Create animated thumbnail cells
HRESULT SlideAnimation::CreateAnimatedThumbnailCells(
    const std::vector<ThumbnailCell>& prevThumbnailCells,
    const std::vector<ThumbnailCell>& currentThumbnailCells,
    D2D1_SIZE_F viewSize)
{
    m_animatedThumbnailCells.clear();

    for (auto currentCell = currentThumbnailCells.begin(); currentCell != currentThumbnailCells.end(); currentCell++)
    {
        D2D1_RECT_F targetPosition = currentCell->position;
        D2D1_POINT_2F endPoint = D2D1::Point2F (
            (targetPosition.right + targetPosition.left )/2,
            (targetPosition.bottom + targetPosition.top) /2);
        D2D1_POINT_2F startPoint = D2D1::Point2F (endPoint.x + viewSize.width, endPoint.y);

        m_animatedThumbnailCells.push_back(
            AnimatedThumbnailCell(*currentCell, startPoint, endPoint, D2D1::Point2F(0, 0), true));
    }

    for (auto prevCell = prevThumbnailCells.begin(); prevCell != prevThumbnailCells.end(); prevCell++)
    {
        D2D1_RECT_F startPosition = prevCell->position;
        D2D1_POINT_2F startPoint = D2D1::Point2F (
            (startPosition.right + startPosition.left )/2,
            (startPosition.bottom + startPosition.top) /2);
        D2D1_POINT_2F endPoint = D2D1::Point2F (startPoint.x - viewSize.width, startPoint.y);

        m_animatedThumbnailCells.push_back(
            AnimatedThumbnailCell(*prevCell, startPoint, endPoint, D2D1::Point2F(0, 0), false));
    }

    return S_OK;
}

//
// Build storyboard for the animation
//
HRESULT SlideAnimation::BuildStoryboard()
{
    ComPtr<IUIAnimationStoryboard> storyboard;

    // Clear all the previous animation points
    m_animationPoints.clear();

    // Initialize animation variables if necessary
    HRESULT hr = m_animationManager->CreateStoryboard(&storyboard);
    if (SUCCEEDED (hr))
    {
        // Populate all the animated thumbnail cells
        for (auto animatedCell = m_animatedThumbnailCells.begin(); animatedCell != m_animatedThumbnailCells.end(); animatedCell++)
        {
            D2D1_POINT_2F startPoint = animatedCell->start;
            D2D1_POINT_2F endPoint = animatedCell->end;
            ComPtr<IUIAnimationTransition> xTransition;
            ComPtr<IUIAnimationTransition> yTransition;

            // Create animation variables
            LineAnimationVar animationVar;
            hr = m_animationManager->CreateAnimationVariable(startPoint.x, &(animationVar.animationX));
            if (SUCCEEDED (hr))
            {
                hr = m_animationManager->CreateAnimationVariable(startPoint.y, &(animationVar.animationY));
            }

            // Add a transition for the animation variable
            if (SUCCEEDED (hr))
            {
                hr = m_transitionLibrary->CreateAccelerateDecelerateTransition(m_duration, endPoint.x, m_accelerationRatio , m_decelerationRatio , &xTransition);
            }

            if (SUCCEEDED (hr))
            {
                hr  = storyboard->AddTransition(animationVar.animationX, xTransition);
            }

            if (SUCCEEDED (hr))
            {
                hr = m_transitionLibrary->CreateConstantTransition(m_duration, &yTransition);
            }

            if (SUCCEEDED (hr))
            {
                hr = storyboard->AddTransition(animationVar.animationY, yTransition);
            }

            // Store the pair of animation variable and thumbnail control
            m_animationPoints.insert(LineAnimationPair(animatedCell->cell.control, animationVar));
        }
    }

    // Schedule storyboard
    if (SUCCEEDED (hr))
    {
        hr = AnimationUtility::ScheduleStoryboard(storyboard);
    }

    return hr;
}
