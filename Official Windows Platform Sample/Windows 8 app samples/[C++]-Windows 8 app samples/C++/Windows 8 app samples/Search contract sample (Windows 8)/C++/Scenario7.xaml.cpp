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
// Scenario7.xaml.cpp
// Implementation of the Scenario7 class
//

#include "pch.h"
#include "Scenario7.xaml.h"

using namespace SDKSample::SearchContract;

using namespace Windows::ApplicationModel::Search;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario7::Scenario7()
{
    InitializeComponent();
}

void Scenario7::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Turn on type to search.
    SearchPane::GetForCurrentView()->ShowOnKeyboardInput = true;
}

void Scenario7::OnNavigatedFrom(NavigationEventArgs^ e)
{
    // Turn off type to search.
    SearchPane::GetForCurrentView()->ShowOnKeyboardInput = false;
}
