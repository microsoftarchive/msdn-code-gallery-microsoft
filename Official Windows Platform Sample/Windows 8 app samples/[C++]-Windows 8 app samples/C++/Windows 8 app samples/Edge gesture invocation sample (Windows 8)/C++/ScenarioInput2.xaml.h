// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// ScenarioInput2.xaml.h
// Declaration of the ScenarioInput2 class.
//

#pragma once

#include "pch.h"
#include "ScenarioInput2.g.h"
#include "MainPage.xaml.h"

namespace EdgeGestureSample
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioInput2 sealed
    {
    public:
        ScenarioInput2();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        MainPage^ rootPage;
        TextBlock^ outputText;
        ListBox^ RightClickRegion;
        Windows::Foundation::EventRegistrationToken RightClickEventToken;
        Windows::Foundation::EventRegistrationToken RightClickRegionEventToken;
        Windows::Foundation::EventRegistrationToken EdgeGestureStartingEventToken;
        Windows::Foundation::EventRegistrationToken EdgeGestureCompletedEventToken;
        Windows::Foundation::EventRegistrationToken EdgeGestureCanceledEventToken;

        void rootPage_OutputFrameLoaded(Object^ sender, Object^ e);

        void OnStarting(EdgeGesture^ sender, EdgeGestureEventArgs^ e);
        void OnCompleted(EdgeGesture^ sender, EdgeGestureEventArgs^ e);
        void OnCanceled(EdgeGesture^ sender, EdgeGestureEventArgs^ e);
        void OnContextMenu(Object^ sender, RightTappedRoutedEventArgs^ e);
        void RightClickOverride(Object^ sender, RightTappedRoutedEventArgs^ e);
        void InitializeEdgeGestureHandlers();
        ~ScenarioInput2();

        Windows::Foundation::EventRegistrationToken _layoutHandlerToken;
        Windows::Foundation::EventRegistrationToken _frameLoadedToken;
    };
}
