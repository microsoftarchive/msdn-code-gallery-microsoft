// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// ScenarioInput6.xaml.h
// Declaration of the ScenarioInput6 class.
//

#pragma once

#include "pch.h"
#include "ScenarioInput6.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;

namespace ToastsSampleCPP
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioInput6 sealed
    {
    public:
        ScenarioInput6();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        ~ScenarioInput6();

		MainPage^ rootPage;
        void OutputText(Platform::String^ text);
        void Scenario6DisplayLongToast(bool loopAudio);
        void Scenario6DisplayLongToastWithStringManipulation(bool loopAudio);

        Windows::UI::Xaml::Controls::TextBlock^ outputText;
    };
}
