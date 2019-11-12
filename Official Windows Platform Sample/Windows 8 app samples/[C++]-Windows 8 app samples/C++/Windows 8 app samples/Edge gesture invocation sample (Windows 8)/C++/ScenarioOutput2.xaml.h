// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// ScenarioOutput2.xaml.h
// Declaration of the ScenarioOutput2 class
//

#pragma once

#include "pch.h"
#include "ScenarioOutput2.g.h"
#include "MainPage.xaml.h"

namespace EdgeGestureSample
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioOutput2 sealed
    {
    public:
        ScenarioOutput2();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        void rootPage_InputFrameLoaded(Object^ sender, Object^ e);
        ~ScenarioOutput2();

        MainPage^ rootPage;
        Windows::Foundation::EventRegistrationToken _frameLoadedToken;
    };
}
