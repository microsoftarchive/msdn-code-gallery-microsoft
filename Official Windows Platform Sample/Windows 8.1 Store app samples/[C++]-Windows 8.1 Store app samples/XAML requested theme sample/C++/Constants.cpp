//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::RequestedThemeCPP;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
	{ "Basic RequestedTheme use", "SDKSample.RequestedThemeCPP.Scenario1" }, 
	{ "Inheritance Properties", "SDKSample.RequestedThemeCPP.Scenario2" },
	{ "Binding RequestedTheme Property", "SDKSample.RequestedThemeCPP.Scenario3" },
	{ "Using code to change RequestedTheme Properties", "SDKSample.RequestedThemeCPP.Scenario4" },
	{ "Specifying application theme in XAML", "SDKSample.RequestedThemeCPP.Scenario5" },
	{ "Specifying application theme at startup", "SDKSample.RequestedThemeCPP.Scenario6" }
}; 

