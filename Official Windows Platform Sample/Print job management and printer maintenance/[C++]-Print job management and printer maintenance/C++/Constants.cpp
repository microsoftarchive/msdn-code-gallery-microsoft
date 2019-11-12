//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::DeviceAppForPrinters2;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Printer device maintenance scenario", "SDKSample.DeviceAppForPrinters2.DeviceMaintenance" }, 
    { "Print job management scenario", "SDKSample.DeviceAppForPrinters2.PrintJobManagement" },
}; 
                