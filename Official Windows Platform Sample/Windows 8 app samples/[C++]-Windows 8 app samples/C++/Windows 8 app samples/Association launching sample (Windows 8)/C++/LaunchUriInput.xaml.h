// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// LaunchUriInput.xaml.h
// Declaration of the LaunchUriInput class.
//

#pragma once

#include "pch.h"
#include "LaunchUriInput.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;

namespace AssociationLaunching
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class LaunchUriInput sealed
    {
    public:
        LaunchUriInput();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        MainPage^ rootPage;

        void LaunchUriButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void LaunchUriWithWarningButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void LaunchUriOpenWithButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        ~LaunchUriInput();

        static Windows::Foundation::Point GetOpenWithPosition(Windows::UI::Xaml::FrameworkElement^ element);
        static Platform::String^ uriToLaunch;
    };
}