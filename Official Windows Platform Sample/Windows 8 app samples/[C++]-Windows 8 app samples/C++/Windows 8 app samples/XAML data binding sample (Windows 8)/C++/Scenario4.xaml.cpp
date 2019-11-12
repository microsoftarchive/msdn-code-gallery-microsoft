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
// Scenario4.xaml.cpp
// Implementation of the Scenario4 class
//

#include "pch.h"
#include "Scenario4.xaml.h"
#include "Team.h"

using namespace SDKSample::DataBinding;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario4::Scenario4()
{
	InitializeComponent();
	Scenario4Reset(nullptr, nullptr);
}

void SDKSample::DataBinding::Scenario4::Scenario4Reset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Output->DataContext =ref new Teams();
}

