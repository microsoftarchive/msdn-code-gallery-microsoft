//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#include "StdAfx.h"
#include "AnimationUtility.h"

static ComPtr<IUIAnimationManager> m_animationManager;
static ComPtr<IUIAnimationTimer> m_animationTimer;
static ComPtr<IUIAnimationTransitionLibrary> m_transitionLibrary;

using namespace Hilo::AnimationHelpers;

AnimationUtility::AnimationUtility()
{
}

AnimationUtility::~AnimationUtility()
{
}

HRESULT AnimationUtility::GetAnimationTimer(IUIAnimationTimer **animationTimer)
{
    HRESULT hr = Initialize();
    if (SUCCEEDED(hr))
    {
        hr = AssignToOutputPointer(animationTimer, m_animationTimer);
    }

    return hr;
}

HRESULT AnimationUtility::GetAnimationManager(IUIAnimationManager **animationManager)
{
    HRESULT hr = Initialize();

    if (SUCCEEDED(hr))
    {
        hr = AssignToOutputPointer(animationManager, m_animationManager);
    }

    return hr;
}

HRESULT AnimationUtility::GetTransitionLibrary(IUIAnimationTransitionLibrary **transitionLibrary)
{
    HRESULT hr = Initialize();
    if (SUCCEEDED(hr))
    {
        hr = AssignToOutputPointer(transitionLibrary, m_transitionLibrary);
    }

    return hr;
}

HRESULT AnimationUtility::GetAnimationTimerTime(UI_ANIMATION_SECONDS* animationSeconds)
{
    HRESULT hr = Initialize();

    if (SUCCEEDED(hr))
    {
        hr = m_animationTimer->GetTime(animationSeconds);
    }

    return hr;
}

HRESULT AnimationUtility::UpdateAnimationManagerTime()
{
    UI_ANIMATION_SECONDS secondsNow = 0;

    HRESULT hr = Initialize();
    if (SUCCEEDED(hr))
    {
        hr = m_animationTimer->GetTime(&secondsNow);
    }

    if (SUCCEEDED(hr))
    {
        hr = m_animationManager->Update(secondsNow);
    }

    return hr;
}

HRESULT AnimationUtility::Initialize()
{
    // Create Animation Manager
    HRESULT hr = S_OK;
    
    if (!m_animationManager)
    {
        hr = CoCreateInstance(
            CLSID_UIAnimationManager,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(&m_animationManager));
    }

    if (SUCCEEDED(hr) && !m_animationTimer)
    {
        // Create Animation Timer
        hr = CoCreateInstance(
            CLSID_UIAnimationTimer,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(&m_animationTimer));
    }

    if (SUCCEEDED(hr) && !m_transitionLibrary)
    {
        // Create Animation Transition Library
        hr = CoCreateInstance(
            CLSID_UIAnimationTransitionLibrary,
            nullptr,
            CLSCTX_INPROC_SERVER,
            IID_PPV_ARGS(&m_transitionLibrary));
    }

    return hr;
}

HRESULT AnimationUtility::ScheduleStoryboard(IUIAnimationStoryboard* storyboard)
{
    UI_ANIMATION_SECONDS secondsNow = static_cast<UI_ANIMATION_SECONDS>(0);

    HRESULT hr = Initialize();
    if (SUCCEEDED(hr))
    {
        hr = m_animationTimer->GetTime(&secondsNow);
    }

    if (SUCCEEDED(hr))
    {
        hr = storyboard->Schedule(secondsNow);
    }

    return hr;
}

HRESULT AnimationUtility::IsAnimationManagerBusy(bool* isBusy)
{
    UI_ANIMATION_MANAGER_STATUS status = UI_ANIMATION_MANAGER_IDLE;

    HRESULT hr = Initialize();
    if (SUCCEEDED(hr))
    {
        hr = m_animationManager->GetStatus(&status);
    }

    if (SUCCEEDED(hr))
    {
        *isBusy = (status == UI_ANIMATION_MANAGER_BUSY);
    }

    return hr;
}
