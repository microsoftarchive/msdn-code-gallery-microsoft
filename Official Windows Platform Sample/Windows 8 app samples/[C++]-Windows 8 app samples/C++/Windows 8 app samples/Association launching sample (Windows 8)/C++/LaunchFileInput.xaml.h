// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// LaunchFileInput.xaml.h
// Declaration of the LaunchFileInput class.
//

#pragma once

#include "pch.h"
#include "LaunchFileInput.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;

namespace AssociationLaunching
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class LaunchFileInput sealed
    {
    public:
        LaunchFileInput();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        MainPage^ rootPage;

        void LaunchFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void LaunchFileWithWarningButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void LaunchFileOpenWithButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PickAndLaunchFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        ~LaunchFileInput();

        static Windows::Foundation::Point GetOpenWithPosition(Windows::UI::Xaml::FrameworkElement^ element);
        static Platform::String^ fileToLaunch;
    };
}