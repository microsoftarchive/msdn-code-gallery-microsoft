//===================================================================================
// Copyright (c) Microsoft Corporation.  All rights reserved.                        
//                                                                                   
// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY                    
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT                       
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND                          
// FITNESS FOR A PARTICULAR PURPOSE.                                                 
//===================================================================================


#pragma once

#include "UIAnimation.h"

namespace Hilo
{
    namespace AnimationHelpers
    {
        //
        // This utility class provides several utility functions
        // to work with Window Animation manager, including the
        // Animation manager, timer and transitions libraty.
        //
        class AnimationUtility
        {
        private:
            AnimationUtility();
            virtual ~AnimationUtility();
            static HRESULT Initialize();

        public:
            static HRESULT GetAnimationManager(__out IUIAnimationManager **animationManager);
            static HRESULT GetAnimationTimer(__out IUIAnimationTimer **animationTimer);
            static HRESULT GetTransitionLibrary(__out IUIAnimationTransitionLibrary **transitionLibrary);
            static HRESULT GetAnimationTimerTime(__out UI_ANIMATION_SECONDS* animationSeconds);
            static HRESULT UpdateAnimationManagerTime();
            static HRESULT ScheduleStoryboard(IUIAnimationStoryboard* storyboard);
            static HRESULT IsAnimationManagerBusy(__out bool* isBusy);
        };
    }
}
