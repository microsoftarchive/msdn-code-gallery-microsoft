//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "SampleConfiguration.h"

using namespace SDKSample;
using namespace SDKSample::AssociationLaunching;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Launching a file", "SDKSample.AssociationLaunching.LaunchFile" },
    { "Launching a URI", "SDKSample.AssociationLaunching.LaunchUri" },
    { "Receiving a file", "SDKSample.AssociationLaunching.ReceiveFile" },
    { "Receiving a URI", "SDKSample.AssociationLaunching.ReceiveUri" }
};
