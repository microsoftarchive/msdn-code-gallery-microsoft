// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario3Input.xaml.h
// Declaration of the Scenario3Input class
//

#pragma once

#include "pch.h"
#include "ScenarioInput3.g.h"
#include "MainPage.g.h"

namespace ToastsSampleCPP
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioInput3 sealed
    {
    public:
        ScenarioInput3();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        ~ScenarioInput3();

		MainPage^ rootPage;
        void OutputText(Platform::String^ text);
        void Scenario3DisplayToast(Windows::UI::Notifications::ToastTemplateType templateType);
        void Scenario3DisplayToastWithStringManipulation(Windows::UI::Notifications::ToastTemplateType templateType);

        Windows::UI::Xaml::Controls::TextBlock^ outputText;
    };
}
