// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario3Output.xaml.h
// Declaration of the Scenario3Output class
//

#pragma once

#include "pch.h"
#include "ScenarioOutput3.g.h"
#include "MainPage.g.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::ViewManagement;

namespace ToastsSampleCPP
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioOutput3 sealed
    {
    public:
        ScenarioOutput3();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    private:
        ~ScenarioOutput3();

		MainPage^ rootPage;
        void Page_Loaded(Object^ sender, RoutedEventArgs^ e);
        void rootPage_InputFrameLoaded(Object^ sender, Object^ e);
        void Page_SizeChanged(Platform::Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e);
        void DisplayProperties_LogicalDpiChanged(Object^ sender);
        Windows::Foundation::EventRegistrationToken _frameLoadedToken;
        void CheckResolutionAndViewState();
    };
}
