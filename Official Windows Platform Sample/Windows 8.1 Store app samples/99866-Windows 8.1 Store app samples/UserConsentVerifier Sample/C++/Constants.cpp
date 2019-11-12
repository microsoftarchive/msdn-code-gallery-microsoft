//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::UserConsentVerifierCPP;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    { "Check Consent Availability", "SDKSample.UserConsentVerifierCPP.Scenario1" }, 
    { "Request Consent", "SDKSample.UserConsentVerifierCPP.Scenario2" }
}; 
