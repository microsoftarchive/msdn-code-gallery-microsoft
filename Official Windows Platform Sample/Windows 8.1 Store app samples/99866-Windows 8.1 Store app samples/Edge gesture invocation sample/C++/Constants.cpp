//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::EdgeGesture;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Listening for EdgeGesture Events", "SDKSample.EdgeGesture.S1_EdgeGestureEvents" }, 
    { "Handling Right-Click", "SDKSample.EdgeGesture.S2_HandlingRightClick" }
}; 
