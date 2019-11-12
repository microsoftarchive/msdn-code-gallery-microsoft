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
    //     { "Description for the sample", "Fully quaified name for the class that implements the scenario" }

    { "String Resources In XAML", "SDKSample.ApplicationResources.Scenario1" }, 
    { "File Resources In XAML", "SDKSample.ApplicationResources.Scenario2" },
    { "String Resources In Code", "SDKSample.ApplicationResources.Scenario3" },
	{ "Resources in the AppX manifest", "SDKSample.ApplicationResources.Scenario4" },
	{ "Additional Resource Files", "SDKSample.ApplicationResources.Scenario5" },
	{ "Class Library Resources", "SDKSample.ApplicationResources.Scenario6" },
	{ "Runtime Changes/Events", "SDKSample.ApplicationResources.Scenario7" },
	{ "Application Languages", "SDKSample.ApplicationResources.Scenario8" },
	{ "Override Languages", "SDKSample.ApplicationResources.Scenario9" },
	{ "Multi-dimensional fallback", "SDKSample.ApplicationResources.Scenario10" },
	{ "Working with webservices", "SDKSample.ApplicationResources.Scenario11" }
}; 
