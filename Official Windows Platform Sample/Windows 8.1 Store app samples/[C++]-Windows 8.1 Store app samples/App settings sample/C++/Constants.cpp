//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::ApplicationSettings;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
	// The format here is the following:
	//     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
	{ "Default Behavior with No Settings Integration", "SDKSample.ApplicationSettings.Scenario1" }, 
	{ "Add Settings Commands to the Settings Charm", "SDKSample.ApplicationSettings.Scenario2" },
	{ "Add a SettingsFlyout", "SDKSample.ApplicationSettings.Scenario3" },
	{ "Programmatically Show a SettingsFlyout", "SDKSample.ApplicationSettings.Scenario4" },
	{ "Navigating Content", "SDKSample.ApplicationSettings.Scenario5" },
}; 
