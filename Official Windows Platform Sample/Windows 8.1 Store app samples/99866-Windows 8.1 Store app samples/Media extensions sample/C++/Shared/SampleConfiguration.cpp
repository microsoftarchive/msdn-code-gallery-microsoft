//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "SampleConfiguration.h"

using namespace SDKSample;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Local decoder", "SDKSample.MediaExtensions.LocalDecoder" }, 
    { "Local scheme handler", "SDKSample.MediaExtensions.LocalSchemeHandler" },
    { "Video Stabilization Effect", "SDKSample.MediaExtensions.VideoStabilizationEffect" },
    { "Custom Video Effect", "SDKSample.MediaExtensions.CustomEffects" }
}; 
