// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "MainPage.xaml.h"
#include "SampleConfiguration.h"

using namespace SDKSample;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
	{ "Registering a notification channel", "SDKSample.SDKTemplate.Scenario1" },
	{ "Renewing channels", "SDKSample.SDKTemplate.Scenario2" },
	{ "Listening for push notifications", "SDKSample.SDKTemplate.Scenario3" },
	{ "Polling for tile updates", "SDKSample.SDKTemplate.Scenario4" },
	{ "Polling for badge updates", "SDKSample.SDKTemplate.Scenario5" }
};