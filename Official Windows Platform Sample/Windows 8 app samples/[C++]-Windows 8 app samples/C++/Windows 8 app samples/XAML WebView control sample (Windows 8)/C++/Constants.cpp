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
    { "Navigate to a URL", "SDKSample.WebViewControl.Scenario1" }, 
    { "Load HTML", "SDKSample.WebViewControl.Scenario2" },
    { "Ineract with script", "SDKSample.WebViewControl.Scenario3" },
	{ "Using ScriptNotify", "SDKSample.WebViewControl.Scenario4" },
	{ "Accessing the DOM", "SDKSample.WebViewControl.Scenario5" },
	{ "Using the WebViewBrush", "SDKSample.WebViewControl.Scenario6" },
	{ "Supporting the Share contract", "SDKSample.WebViewControl.Scenario7" },
	{ "Co-existing with the AppBar", "SDKSample.WebViewControl.Scenario8" }

}; 
