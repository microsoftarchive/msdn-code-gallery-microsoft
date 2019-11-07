//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once
#include "S1_EdgeGestureEvents.g.h"

namespace SDKSample
{
    namespace EdgeGesture
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S1_EdgeGestureEvents sealed
        {
        public:
            S1_EdgeGestureEvents();

        private:
            Windows::Foundation::EventRegistrationToken EdgeGestureStartingEventToken;
            Windows::Foundation::EventRegistrationToken EdgeGestureCompletedEventToken;
            Windows::Foundation::EventRegistrationToken EdgeGestureCanceledEventToken;
            Windows::Foundation::EventRegistrationToken RightTappedEventToken;

            ~S1_EdgeGestureEvents();
            void InitializeEventHandlers();
            void UninitializeEventHandlers();
            void OnStarting(Windows::UI::Input::EdgeGesture^ sender, Windows::UI::Input::EdgeGestureEventArgs^ e);
            void OnCompleted(Windows::UI::Input::EdgeGesture^ sender, Windows::UI::Input::EdgeGestureEventArgs^ e);
            void OnCanceled(Windows::UI::Input::EdgeGesture^ sender, Windows::UI::Input::EdgeGestureEventArgs^ e);
            void OnRightTapped(Object^ sender, Windows::UI::Xaml::Input::RightTappedRoutedEventArgs^ e);
        };
    }
}
