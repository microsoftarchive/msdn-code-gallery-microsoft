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
    { "Navigate to a URL", "SDKSample.WebViewControl.Scenario1" }, 
    { "Load Html from a string", "SDKSample.WebViewControl.Scenario2" },
    { "Navigate to files", "SDKSample.WebViewControl.Scenario3" },
	{ "Navigate using a custom resolver", "SDKSample.WebViewControl.Scenario4" },
	{ "Using InvokeScript", "SDKSample.WebViewControl.Scenario5" },
	{ "Using ScriptNotify", "SDKSample.WebViewControl.Scenario6" },
	{ "Using CaptureBitmap", "SDKSample.WebViewControl.Scenario8" }

}; 
#else
Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
	// The format here is the following:
	//     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
	{ "Navigate to a URL", "SDKSample.WebViewControl.Scenario1" },
	{ "Load Html from a string", "SDKSample.WebViewControl.Scenario2" },
	{ "Navigate to files", "SDKSample.WebViewControl.Scenario3" },
	{ "Navigate using a custom resolver", "SDKSample.WebViewControl.Scenario4" },
	{ "Using InvokeScript", "SDKSample.WebViewControl.Scenario5" },
	{ "Using ScriptNotify", "SDKSample.WebViewControl.Scenario6" },
	{ "Supporting the Share contract", "SDKSample.WebViewControl.Scenario7" },
	{ "Using CaptureBitmap", "SDKSample.WebViewControl.Scenario8" }

};
#endif
