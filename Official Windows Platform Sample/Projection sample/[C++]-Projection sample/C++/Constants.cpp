//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::Projection;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    { "Creating and projecting a view", "SDKSample.Projection.Scenario1" }, 
    { "Second screen availability", "SDKSample.Projection.Scenario2" }
}; 
