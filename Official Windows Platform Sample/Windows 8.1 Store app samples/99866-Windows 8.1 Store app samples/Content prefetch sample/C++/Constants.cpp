//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::SDKTemplate;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Adding direct content URI", "SDKSample.SDKTemplate.DirectContentUriScenario" }, 
    { "Setting the indirect content uri", "SDKSample.SDKTemplate.IndirectContentUriScenario" },
    { "Getting the last prefetch time", "SDKSample.SDKTemplate.LastPrefetchTimeScenario" }
}; 