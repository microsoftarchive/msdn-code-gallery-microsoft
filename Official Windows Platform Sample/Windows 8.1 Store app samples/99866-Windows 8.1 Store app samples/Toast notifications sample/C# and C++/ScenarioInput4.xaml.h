// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// ScenarioInput4.xaml.h
// Declaration of the ScenarioInput4 class.
//

#pragma once

#include "pch.h"
#include "ScenarioInput4.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;

namespace ToastsSampleCPP
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioInput4 sealed
    {
    public:
        ScenarioInput4();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        ~ScenarioInput4();

		MainPage^ rootPage;
        void OutputText(Platform::String^ text);
        void Scenario4DisplayToastWithSound(NotificationsExtensions::ToastContent::ToastAudioContent audioContent, Platform::String^ audioSrc);
        void Scenario4DisplayToastWithSoundWithStringManipulation(NotificationsExtensions::ToastContent::ToastAudioContent audioContent, Platform::String^ audioSrc);

        Windows::UI::Xaml::Controls::TextBlock^ outputText;
    };
}
