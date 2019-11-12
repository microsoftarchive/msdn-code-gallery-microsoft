// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario2Input.xaml.h
// Declaration of the Scenario2Input class.
//

#pragma once

#include "pch.h"
#include "ScenarioInput2.g.h"
#include "MainPage.g.h"

namespace CredentialPickerCPP
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
        void rootPage_OutputFrameLoaded(Object^ sender, Object^ e);
        void Launch_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        Windows::Foundation::EventRegistrationToken _frameLoadedToken;
        void SetError(Platform::String^ errorText);
        void SetResult(Windows::Security::Credentials::UI::CredentialPickerResults^ res);
        ~ScenarioInput2();
    };
}
