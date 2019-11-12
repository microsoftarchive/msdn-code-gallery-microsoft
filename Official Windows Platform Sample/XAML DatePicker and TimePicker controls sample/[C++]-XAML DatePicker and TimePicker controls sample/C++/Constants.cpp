// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::DateAndTimePickers;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
	// The format here is the following:
	//     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
	{ "Basics", "SDKSample.DateAndTimePickers.Scenario1" }, 
	{ "Setting Initial Values", "SDKSample.DateAndTimePickers.Scenario2" },
	{ "Combining DatePicker and TimePicker Values", "SDKSample.DateAndTimePickers.Scenario3" },
	{ "DatePicker Formatting", "SDKSample.DateAndTimePickers.Scenario4" }
}; 
