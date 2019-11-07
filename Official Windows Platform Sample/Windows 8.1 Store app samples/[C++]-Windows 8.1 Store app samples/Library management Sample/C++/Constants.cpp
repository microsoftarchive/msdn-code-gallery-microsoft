//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::LibraryManagement;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Adding a folder to the Pictures library",     "SDKSample.LibraryManagement.Scenario1" },
    { "Listing folders in the Pictures library",     "SDKSample.LibraryManagement.Scenario2" },
    { "Removing a folder from the Pictures library", "SDKSample.LibraryManagement.Scenario3" }
};
