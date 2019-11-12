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
    { "Opening a notification channel", "SDKSample.SDKTemplate.Scenario1" }, 
	{ "Renewing channels", "SDKSample.SDKTemplate.Scenario2" },
    { "Listening for push notification events", "SDKSample.SDKTemplate.Scenario3" },
    { "Periodic tile updates", "SDKSample.SDKTemplate.Scenario4" },
	{ "Periodic badge updates", "SDKSample.SDKTemplate.Scenario5" }
}; 
