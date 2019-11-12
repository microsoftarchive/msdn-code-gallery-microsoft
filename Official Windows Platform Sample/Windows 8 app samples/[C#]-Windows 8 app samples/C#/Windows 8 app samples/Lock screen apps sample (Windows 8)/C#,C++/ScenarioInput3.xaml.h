// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "pch.h"
#include "ScenarioInput3.g.h"
#include "MainPage.g.h"

namespace LockScreenAppsCPP
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioInput3 sealed
    {
    public:
        ScenarioInput3();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        void CreateBadgeTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void CreateBadgeAndTextTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        ~ScenarioInput3();

		Windows::Foundation::Rect GetElementRect(Windows::UI::Xaml::FrameworkElement^ element);
        MainPage^ rootPage;
    };
}
