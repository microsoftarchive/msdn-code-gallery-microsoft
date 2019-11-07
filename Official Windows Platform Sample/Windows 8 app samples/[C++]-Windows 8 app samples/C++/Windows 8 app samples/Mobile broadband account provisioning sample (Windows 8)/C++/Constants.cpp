//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    // The format here is the following:
    //     { "Description for the sample", "Fully quaified name for the class that implements the scenario" }
    { "Provision for mobile network operators", "SDKSample.ProvisioningAgent.ProvisionMno" },
    { "Update profile cost", "SDKSample.ProvisioningAgent.UpdateCost" },
    { "Update profile data usage", "SDKSample.ProvisioningAgent.UpdateUsage" },
    { "Provision for other operators", "SDKSample.ProvisioningAgent.ProvisionOtherOperator" }
};
