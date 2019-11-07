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
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "GET Text With Cache Control", "SDKSample.HttpClientSample.Scenario1" },
    { "GET Stream", "SDKSample.HttpClientSample.Scenario2" },
    { "GET List", "SDKSample.HttpClientSample.Scenario3" },
    { "POST Text", "SDKSample.HttpClientSample.Scenario4" },
    { "POST Stream", "SDKSample.HttpClientSample.Scenario5" },
    { "POST Multipart", "SDKSample.HttpClientSample.Scenario6" },
    { "POST Stream With Progress", "SDKSample.HttpClientSample.Scenario7" },
    { "POST Custom Content", "SDKSample.HttpClientSample.Scenario8" },
    { "Get Cookies", "SDKSample.HttpClientSample.Scenario9" },
    { "Set Cookie", "SDKSample.HttpClientSample.Scenario10" },
    { "Delete Cookie", "SDKSample.HttpClientSample.Scenario11" },
    { "Metered Connection Filter", "SDKSample.HttpClientSample.Scenario12" },
    { "Retry Filter", "SDKSample.HttpClientSample.Scenario13" },
};
