//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once
#include "S2_HandlingRightClick.g.h"

namespace SDKSample
{
    namespace EdgeGesture
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S2_HandlingRightClick sealed
        {
        public:
            S2_HandlingRightClick();

        private:
            Windows::Foundation::EventRegistrationToken EdgeGestureStartingEventToken;
            Windows::Foundation::EventRegistrationToken EdgeGestureCompletedEventToken;
            Windows::Foundation::EventRegistrationToken EdgeGestureCanceledEventToken;
            Windows::Foundation::EventRegistrationToken RightTappedEventToken;
            Windows::Foundation::EventRegistrationToken RightTappedOverrideEventToken;

            ~S2_HandlingRightClick();
            void InitializeEventHandlers();
            void UninitializeEventHandlers();
            void OnStarting(Windows::UI::Input::EdgeGesture^ sender, Windows::UI::Input::EdgeGestureEventArgs^ e);
            void OnCompleted(Windows::UI::Input::EdgeGesture^ sender, Windows::UI::Input::EdgeGestureEventArgs^ e);
            void OnCanceled(Windows::UI::Input::EdgeGesture^ sender, Windows::UI::Input::EdgeGestureEventArgs^ e);
            void OnRightTapped(Object^ sender, Windows::UI::Xaml::Input::RightTappedRoutedEventArgs^ e);
            void OnRightTappedOverride(Object^ sender, Windows::UI::Xaml::Input::RightTappedRoutedEventArgs^ e);
        };
    }
}
