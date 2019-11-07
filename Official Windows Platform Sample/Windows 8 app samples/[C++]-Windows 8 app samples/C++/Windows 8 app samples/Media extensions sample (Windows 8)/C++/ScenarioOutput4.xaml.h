// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// ScenarioOutput4.xaml.h
// Declaration of the ScenarioOutput4 class
//

#pragma once

#include "pch.h"
#include "ScenarioOutput4.g.h"
#include "MainPage.xaml.h"

namespace MediaExtensionsCPP
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioOutput4 sealed
    {
    public:
        ScenarioOutput4();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        ~ScenarioOutput4();
        MainPage^ rootPage;
        Windows::Foundation::EventRegistrationToken _mediaFailedToken;
    };
}
