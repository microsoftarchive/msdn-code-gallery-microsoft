// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// ScenarioInput1.xaml.h
// Declaration of the ScenarioInput1 class.
//

#pragma once

#include "pch.h"
#include "ScenarioInput1.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;

namespace KeyboardEventsSampleCPP
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioInput1 sealed
    {
    public:
        ScenarioInput1();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        MainPage^ rootPage;
        Windows::Foundation::EventRegistrationToken _frameLoadedToken;

        void rootPage_OutputFrameLoaded(Object^ sender, Object^ e);
        void Scenario1Open_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        ~ScenarioInput1();
    };
}
