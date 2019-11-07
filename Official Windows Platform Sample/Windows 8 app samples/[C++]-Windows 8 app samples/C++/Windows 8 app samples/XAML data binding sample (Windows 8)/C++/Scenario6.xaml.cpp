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
// Scenario6.xaml.cpp
// Implementation of the Scenario6 class
//

#include "pch.h"
#include "Scenario6.xaml.h"
#include "Team.h"

using namespace SDKSample::DataBinding;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario6::Scenario6()
{
	InitializeComponent();
	Scenario6Reset(nullptr,nullptr);
}

void SDKSample::DataBinding::Scenario6::Scenario6Reset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	auto dataSource = ref new TeamDataSource();
	groupInfoCVS->Source = dataSource->ItemGroups;
}

