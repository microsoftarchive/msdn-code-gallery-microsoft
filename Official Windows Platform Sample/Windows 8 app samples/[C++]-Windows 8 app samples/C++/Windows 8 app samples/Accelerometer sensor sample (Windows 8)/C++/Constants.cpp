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
    { "Accelerometer data events", "SDKSample.AccelerometerCPP.Scenario1" }, 
    { "Accelerometer shake events", "SDKSample.AccelerometerCPP.Scenario2" },
    { "Poll accelerometer readings", "SDKSample.AccelerometerCPP.Scenario3" }
}; 
