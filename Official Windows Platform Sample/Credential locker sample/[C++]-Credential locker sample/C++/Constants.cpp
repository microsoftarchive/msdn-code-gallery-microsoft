//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::PasswordVaultCPP;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Add, Read and Remove Credential", "SDKSample.PasswordVaultCPP.Scenario1" }, 
    { "Manage your credentials", "SDKSample.PasswordVaultCPP.Scenario2" }
}; 
