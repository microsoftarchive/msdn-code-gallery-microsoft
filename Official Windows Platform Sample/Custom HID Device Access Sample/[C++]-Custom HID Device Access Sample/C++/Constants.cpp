//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::CustomHidDeviceAccess;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Connecting To Device", "SDKSample.CustomHidDeviceAccess.DeviceConnect" }, 
    { "Feature Reports", "SDKSample.CustomHidDeviceAccess.FeatureReports" },
    { "Input Report Events", "SDKSample.CustomHidDeviceAccess.InputReportEvents" },
    { "Input and Output Reports", "SDKSample.CustomHidDeviceAccess.InputOutputReports" }
}; 
