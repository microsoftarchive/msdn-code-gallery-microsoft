//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::CustomDeviceAccess;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Connecting to the Fx2 Device",          "SDKSample.CustomDeviceAccess.DeviceConnect" },
    { "Sending IOCTLs to and from the device", "SDKSample.CustomDeviceAccess.DeviceIO" },
    { "Handling asynchronous device events",   "SDKSample.CustomDeviceAccess.DeviceEvents" },
    { "Read and Write operations",             "SDKSample.CustomDeviceAccess.DeviceReadWrite" }
}; 
