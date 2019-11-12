// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// LaunchUriOutput.xaml.h
// Declaration of the LaunchUriOutput class
//

#pragma once

#include "pch.h"
#include "LaunchUriOutput.g.h"
#include "MainPage.xaml.h"

namespace AssociationLaunching
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class LaunchUriOutput sealed
    {
    public:
        LaunchUriOutput();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        ~LaunchUriOutput();
        MainPage^ rootPage;
    };
}
