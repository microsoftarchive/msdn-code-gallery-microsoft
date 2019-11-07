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
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"

using namespace SDKSample::DataBinding;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
	InitializeComponent();
}

void SDKSample::DataBinding::Scenario1::Scenario1Reset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	sliderOneWayDataSource->Value = 10;
	sliderTwoWayDataSource->Value = 50;
	sliderOneTimeDataSource->Value = 100;

}

