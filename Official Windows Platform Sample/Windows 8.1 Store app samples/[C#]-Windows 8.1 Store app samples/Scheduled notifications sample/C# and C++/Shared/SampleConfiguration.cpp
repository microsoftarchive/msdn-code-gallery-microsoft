// Copyright (c) Microsoft. All rights reserved.

#include "pch.h"
#include "MainPage.xaml.h"
#include "SampleConfiguration.h"

using namespace SDKSample;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    { "Schedule Notifications", "SDKSample.Scenario1_Schedule" },
    { "Manage Scheduled Notifications", "SDKSample.Scenario2_Manage" },
};