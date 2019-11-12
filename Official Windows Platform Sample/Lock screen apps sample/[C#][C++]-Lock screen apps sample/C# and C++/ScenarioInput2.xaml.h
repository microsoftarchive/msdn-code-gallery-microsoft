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

namespace LockScreenAppsCPP
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioInput2 sealed
    {
    public:
        ScenarioInput2();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        void SendBadge_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void SendBadgeWithStringManipulation_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ClearBadge_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void SendTileWithStringManipulation_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void SendTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void ClearTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        ~ScenarioInput2();

        MainPage^ rootPage;
    };
}
