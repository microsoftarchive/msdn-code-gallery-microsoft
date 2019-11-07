//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Multi-select storefront", "SDKSample.ListViewInteraction.Scenario1" }, 
    { "Creating a master-detail ListView", "SDKSample.ListViewInteraction.Scenario2" },
    { "Creating a static ListView", "SDKSample.ListViewInteraction.Scenario3" },
	{ "Creating a 'picker' ListView", "SDKSample.ListViewInteraction.Scenario4" }
}; 
