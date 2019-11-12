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
#include "SampleConfiguration.h"

using namespace SDKSample;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    // The format here is the following:
    //     { "Description for the sample", "Fully quaified name for the class that implements the scenario" }
    { "Get Internet Connection Profile", "SDKSample.NetworkInformationApi.InternetConnectionProfile" },
    { "Get Connection Profile List", "SDKSample.NetworkInformationApi.ConnectionProfileList" },
    { "Find Connection Profiles", "SDKSample.NetworkInformationApi.FindConnectionProfiles" },
    { "Get Lan Identifiers", "SDKSample.NetworkInformationApi.LanId" },
    { "Register for Network Status Change Notifications", "SDKSample.NetworkInformationApi.NetworkStatusChange" },
    { "Get Network Usage", "SDKSample.NetworkInformationApi.ProfileLocalUsageData" },
    { "Get Connectivity Intervals", "SDKSample.NetworkInformationApi.ProfileConnectivityIntervals" },
    { "Get Attributed Network Usage", "SDKSample.NetworkInformationApi.GetAttributedNetworkUsage" }
};
