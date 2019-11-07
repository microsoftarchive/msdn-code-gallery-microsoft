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

using namespace SDKSample::SearchContract;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    // Code for participating in the Search contract and handling the user's search queries can be found
    // in App::OnWindowCreated.  Code must be placed there so that it can receive user queries at any time.
    // You also need to add the Search declaration in the package.appxmanifest to support the Search contract.

    InitializeComponent();
}

void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    MainPage::Current->NotifyUser("Use the search pane to submit a query", NotifyType::StatusMessage);
}
