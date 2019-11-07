//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "FlyerAnimation.h"

using namespace Hilo::AnimationHelpers;
using namespace Hilo::Direct2DHelpers;

const double FlyerAnimation::m_acceleration = 8000;

FlyerAnimation::FlyerAnimation()
{
}

FlyerAnimation::~FlyerAnimation()
{
}

// Create thumbnail cells
HRESULT FlyerAnimation::CreateAnimatedThumbnailCells(const std::vector<ThumbnailCell> &prevThumbnailCells, const std::vector<ThumbnailCell> &currentThumbnailCells, D2D1_SIZE_F viewSize)
{
    m_animatedThumbnailCells.clear();

    // Set entry point
    D2D1_POINT_2F entryPoint = D2D1::Point2F(10.0f, -(viewSize.height * 0.5f + 1.0f));
    D2D1_POINT_2F entryCenterPoint = Point2F(10.0f + viewSize.width * 0.5f, -(viewSize.height * 0.5f + 1.0f));

    // Construct the animated thumbnail cells for flying-in thumbnail controls
    for (auto currentCell = currentThumbnailCells.begin(); currentCell != currentThumbnailCells.end(); currentCell ++)
    {
        D2D1_RECT_F targetPosition = currentCell->position;
        D2D1_POINT_2F targetPoint = D2D1::Point2F ((targetPosition.right + targetPosition.left )/2, 
            (targetPosition.bottom + targetPosition.top )/2);

        m_animatedThumbnailCells.push_back(AnimatedThumbnailCell(*currentCell, entryPoint, targetPoint, entryCenterPoint, true));
    }

    // Set exit point
    D2D1_POINT_2F exitPoint = D2D1::Point2F(viewSize.width + 200.0f, (viewSize.height * 0.5f + 1.0f));
    D2D1_POINT_2F exitCenterPoint = Point2F(10.0f + viewSize.width * 0.5f, -(viewSize.height * 0.5f + 1.0f));

    // Construct the animated thumbnail cells for flying-out thumbnail controls
    for (auto prevCell = prevThumbnailCells.begin(); prevCell != prevThumbnailCells.end(); prevCell++)
    {
        D2D1_RECT_F startPosition = prevCell->position;
        D2D1_POINT_2F startPoint = D2D1::Point2F (
            (startPosition.right + startPosition.left) /2, 
            (startPosition.bottom + startPosition.top) /2);

        m_animatedThumbnailCells.push_back(AnimatedThumbnailCell(*prevCell, startPoint, exitPoint, exitCenterPoint, false));
    }

    return S_OK;
}

// Build storyboard for the animation
HRESULT FlyerAnimation::BuildStoryboard()
{
    HRESULT hr = S_OK;

    ComPtr<IUIAnimationStoryboard> storyboard;

    if (nullptr == m_animationManager)
    {
        hr = Initialize();
    }

    if (SUCCEEDED(hr))
    {
        hr = m_animationManager->CreateStoryboard(&storyboard);
        m_geometryAnimationVariables.clear();
    }

    if (SUCCEEDED (hr))
    {
        hr = CreatePath();
    }

    if (SUCCEEDED (hr))
    {
        for (auto iter = m_geometryAnimationVariables.begin(); iter!= m_geometryAnimationVariables.end(); iter++)
        {
            // Update the animation variable for the length
            float length=0;
            iter->second.geometry->ComputeLength(nullptr, &length);
            hr = m_animationManager->CreateAnimationVariable(0, &(iter->second.animationLength));
            if (SUCCEEDED (hr))
            {
                hr = iter->second.animationLength->SetLowerBound(0);
            }
            if (SUCCEEDED (hr))
            {
                hr  = iter->second.animationLength->SetUpperBound(length);
            }
            if (SUCCEEDED (hr))
            {
                // Add transitions
                hr = AddTransition (storyboard, iter->second.animationLength, length);
            }
        }
    }

    if (SUCCEEDED (hr))
    {
        hr = AnimationUtility::ScheduleStoryboard(storyboard);
    }

    return hr;
}

// Get all the animated thumbnail cells
HRESULT FlyerAnimation::GetAnimatedThumbnailCells(std::vector<AnimatedThumbnailCell>& animatedCells)
{
    animatedCells.assign(m_animatedThumbnailCells.begin(), m_animatedThumbnailCells.end());
    return S_OK;
}

// Read the animation position for the specific control
HRESULT FlyerAnimation::GetAnimationPosition(IThumbnail* const control, D2D1_POINT_2F *position)
{
    HRESULT hr = E_FAIL;

    if (nullptr == control || nullptr == position)
    {
        return E_POINTER;
    }

    FlyerAnimationMap :: const_iterator iter;
    iter = m_geometryAnimationVariables.find(control);
    // Every input control is expected to have an animation variable
    if (iter != m_geometryAnimationVariables.end())
    {
        double length;
        hr = iter->second.animationLength->GetValue(&length);
        if (SUCCEEDED(hr))
        {
            D2D1_POINT_2F tangent;
            hr = iter->second.geometry->ComputePointAtLength(static_cast<float>(length), nullptr, position, &tangent);
        }
    }

    return hr;
}

// Add transition for an animation variable
HRESULT FlyerAnimation::AddTransition(IUIAnimationStoryboard *storyboard, IUIAnimationVariable *primaryVariable, double primaryValue)
{
    if (nullptr == storyboard || nullptr == primaryVariable)
    {
        return E_POINTER;
    }
    ComPtr<IUIAnimationTransition> lengthTransition, opacityTransition;
    HRESULT hr = S_OK;

    if (nullptr == m_transitionLibrary)
    {
        hr = Initialize();
    }
    if (SUCCEEDED (hr))
    {
        hr = m_transitionLibrary->CreateParabolicTransitionFromAcceleration(primaryValue, 0.0, m_acceleration, &lengthTransition );
    }
    if (SUCCEEDED (hr))
    {
        hr = storyboard->AddTransition(primaryVariable, lengthTransition);
    }
    return hr;
}

// Create path geometry for every animated thumbnail cell
HRESULT FlyerAnimation::CreatePath()
{
    HRESULT hr = S_OK;

    if (nullptr == m_d2dFactory)
    {
        hr = Direct2DUtility::GetD2DFactory(&m_d2dFactory);
    }

    if (FAILED(hr))
    {
        return hr;
    }

    m_geometryAnimationVariables.clear();
    for (auto animatedCell = m_animatedThumbnailCells.begin(); animatedCell != m_animatedThumbnailCells.end(); animatedCell ++)
    {
        ComPtr<ID2D1GeometrySink> sink;
        FlyerAnimationVar animationVar;

        hr = m_d2dFactory->CreatePathGeometry(&(animationVar.geometry));
        if (SUCCEEDED(hr))
        {
            hr = animationVar.geometry->Open(&sink);
        }
        if (SUCCEEDED(hr))
        {
            sink->SetFillMode(D2D1_FILL_MODE_WINDING);
            // Set an arc for the flying-in thumbnail control
            if (animatedCell->isCurrent) 
            {
                sink->BeginFigure(animatedCell->start, D2D1_FIGURE_BEGIN_FILLED);
                sink->AddArc(
                    D2D1::ArcSegment(
                    animatedCell->end,
                    D2D1::SizeF(150.0f, 100.f), 
                    0.0f,
                    D2D1_SWEEP_DIRECTION_COUNTER_CLOCKWISE,
                    D2D1_ARC_SIZE_SMALL));
                sink->EndFigure(D2D1_FIGURE_END_OPEN);
                sink->Close();
            }
            // Set an arc for the flying-out thumbnail control
            else
            {
                sink->BeginFigure(animatedCell->start, D2D1_FIGURE_BEGIN_FILLED);
                sink->AddArc(
                    D2D1::ArcSegment(
                    animatedCell->end,
                    D2D1::SizeF(100.0f, 100.f), 
                    0.0f,
                    D2D1_SWEEP_DIRECTION_COUNTER_CLOCKWISE,
                    D2D1_ARC_SIZE_SMALL));
                sink->EndFigure(D2D1_FIGURE_END_OPEN);
                sink->Close();
            }
            m_geometryAnimationVariables.insert(FlyerAnimationPair(animatedCell->cell.control, animationVar));
        }
    }

    return hr;
}
