// Copyright (c) Microsoft Corporation. All rights reserved.

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::ControlChannelHttpClient;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "ControlChannelTrigger with HttpClient", "SDKSample.ControlChannelHttpClient.Scenario1" }
}; 
