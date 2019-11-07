//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3.xaml.h"

using namespace SDKSample::DisplayOrientation;

using namespace Windows::Graphics::Display;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario3::Scenario3()
{
    InitializeComponent();
}

void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{
    screenOrientation->Text = DisplayProperties::CurrentOrientation.ToString();

    orientationChangedEventToken = DisplayProperties::OrientationChanged::add(
        ref new DisplayPropertiesEventHandler(this, &Scenario3::OnOrientationChanged)
        );
}

void Scenario3::OnNavigatedFrom(NavigationEventArgs^ e)
{
    DisplayProperties::OrientationChanged::remove(orientationChangedEventToken);
}


void Scenario3::OnOrientationChanged(Object^ sender)
{
    screenOrientation->Text = DisplayProperties::CurrentOrientation.ToString();
}
