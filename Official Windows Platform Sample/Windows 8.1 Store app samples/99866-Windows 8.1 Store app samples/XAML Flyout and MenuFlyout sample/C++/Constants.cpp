//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::Flyouts;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
	// The format here is the following:
	//     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
	{ "Basic Flyout and MenuFlyout Usage", "SDKSample.Flyouts.Scenario1" }, 
	{ "Sharing a Flyout", "SDKSample.Flyouts.Scenario2" },
	{ "Non-Button Flyouts", "SDKSample.Flyouts.Scenario3" },
	{ "Dynamic Content", "SDKSample.Flyouts.Scenario4" }
}; 
