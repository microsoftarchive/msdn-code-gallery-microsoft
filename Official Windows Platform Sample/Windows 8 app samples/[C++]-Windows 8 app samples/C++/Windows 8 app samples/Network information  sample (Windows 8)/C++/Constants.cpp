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
    { "Get Internet Connection Profile", "SDKSample.NetworkInformationApi.InternetConnectionProfile" },
    { "Get Local Data Usage for Internet Connection Profile", "SDKSample.NetworkInformationApi.ProfileLocalUsageData" },
    { "Get Connection Profile List", "SDKSample.NetworkInformationApi.ConnectionProfileList" },
    { "Get Lan Identifiers", "SDKSample.NetworkInformationApi.LanId" },
    { "Register for Network Status Change Notifications", "SDKSample.NetworkInformationApi.NetworkStatusChange" }
};
