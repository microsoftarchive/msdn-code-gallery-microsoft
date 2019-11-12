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
#include "SampleConfiguration.h"

using namespace SDKSample;

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Instantiating a GridView", "SDKSample.ListViewSimple.Scenario1" }, 
    { "Responding to click events", "SDKSample.ListViewSimple.Scenario2" },
    { "Instantiating a ListView", "SDKSample.ListViewSimple.Scenario3" }
}; 
#else
Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
	// The format here is the following:
	//     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
	{ "Instantiating a GridView", "SDKSample.ListViewSimple.Scenario1" },
	{ "Responding to click events", "SDKSample.ListViewSimple.Scenario2" },
	{ "Instantiating a ListView", "SDKSample.ListViewSimple.Scenario3" },
	{ "Retemplating GridViewItems", "SDKSample.ListViewSimple.Scenario4" },
	{ "Retemplating ListViewItems", "SDKSample.ListViewSimple.Scenario5" },
	{ "Custom item container template structure", "SDKSample.ListViewSimple.Scenario6" }
};
#endif