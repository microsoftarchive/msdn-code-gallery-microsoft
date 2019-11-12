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
// Scenario5.xaml.cpp
// Implementation of the Scenario5 class
//

#include "pch.h"
#include "Scenario5.xaml.h"
#include "Team.h"

using namespace SDKSample::DataBinding;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario5::Scenario5()
{
    InitializeComponent();
	Scenario5Reset(nullptr,nullptr);
}

void SDKSample::DataBinding::Scenario5::Scenario5Reset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    auto teams = ref new Teams();
	lvDataTemplates->ItemsSource = teams->Items;
}



