//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "SampleConfiguration.h"

using namespace SDKSample;
using namespace SDKSample::AppBarControl;

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Create an AppBar", "SDKSample.AppBarControl.Scenario1" }, 
    { "Customize AppBar color", "SDKSample.AppBarControl.Scenario2" },
    { "Customize icons", "SDKSample.AppBarControl.Scenario3" },
    { "Control the AppBar and commands", "SDKSample.AppBarControl.Scenario6" },
    { "Show contextual commands for a GridView", "SDKSample.AppBarControl.Scenario7" },
    { "Localizing AppBar commands", "SDKSample.AppBarControl.Scenario8" }
}; 
#else
Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
	// The format here is the following:
	//     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
	{ "Create an AppBar", "SDKSample.AppBarControl.Scenario1" },
	{ "Customize AppBar color", "SDKSample.AppBarControl.Scenario2" },
	{ "Customize icons", "SDKSample.AppBarControl.Scenario3" },
	{ "Using CommandBar", "SDKSample.AppBarControl.Scenario4" },
	{ "Custom content", "SDKSample.AppBarControl.Scenario5" },
	{ "Control the AppBar and commands", "SDKSample.AppBarControl.Scenario6" },
	{ "Show contextual commands for a GridView", "SDKSample.AppBarControl.Scenario7" },
	{ "Localizing AppBar commands", "SDKSample.AppBarControl.Scenario8" }
};
#endif