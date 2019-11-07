//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::ContactActions;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    { "Handling an activation to make a call", "SDKSample.ContactActions.CallScenario" }, 
    { "Handling an activation to send a message", "SDKSample.ContactActions.SendMessageScenario" },
    { "Handling an activation to map an address", "SDKSample.ContactActions.MapAddressScenario" }
}; 
