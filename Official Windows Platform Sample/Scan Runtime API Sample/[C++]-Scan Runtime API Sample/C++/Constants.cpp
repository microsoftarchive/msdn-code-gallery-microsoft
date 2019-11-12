//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
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
    { "Scanner Enumeration", "SDKSample.ScanRuntimeAPI.ScenarioScannerEnumeration" }, 
    { "Just Scan", "SDKSample.ScanRuntimeAPI.ScenarioJustScan" },
    { "Preview From Flatbed", "SDKSample.ScanRuntimeAPI.ScenarioPreviewFromFlatbed" },
    { "Device Auto-Configured Scanning", "SDKSample.ScanRuntimeAPI.ScenarioDeviceAutoConfiguredScan" },
    { "Scan From Flatbed", "SDKSample.ScanRuntimeAPI.ScenarioScanFromFlatbed" },
    { "Scan From Feeder", "SDKSample.ScanRuntimeAPI.ScenarioScanFromFeeder" },
	{ "Multiple Results With Progress", "SDKSample.ScanRuntimeAPI.ScenarioMultipleResultsWithProgress" }
}; 
