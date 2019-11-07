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

namespace MediaExtensionsCPP
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
        MediaElement^ outputVideo;

        void rootPage_OutputFrameLoaded(Object^ sender, Object^ e);
        void OpenGrayscale_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void OpenFisheye_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void OpenPinch_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void OpenWarp_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void OpenInvert_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void Stop_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        Windows::Foundation::EventRegistrationToken _frameLoadedToken;

        void OpenVideoWithPolarEffect(String^ effectName);
    };
}
