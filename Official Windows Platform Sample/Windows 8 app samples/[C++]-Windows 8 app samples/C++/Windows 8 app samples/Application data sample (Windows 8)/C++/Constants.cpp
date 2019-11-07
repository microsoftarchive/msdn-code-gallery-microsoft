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
    { "Files",                  "SDKSample.ApplicationDataSample.Files" },
    { "Settings",               "SDKSample.ApplicationDataSample.Settings" },
    { "Setting Containers",     "SDKSample.ApplicationDataSample.SettingContainer" },
    { "Composite Settings",     "SDKSample.ApplicationDataSample.CompositeSettings" },
    { "DataChanged Event",      "SDKSample.ApplicationDataSample.DataChangedEvent" },
    { "Roaming: HighPriority",  "SDKSample.ApplicationDataSample.HighPriority" },
    { "ms-appdata:// Protocol", "SDKSample.ApplicationDataSample.Msappdata" },
    { "Clear",                  "SDKSample.ApplicationDataSample.ClearScenario" },
    { "SetVersion",             "SDKSample.ApplicationDataSample.SetVersion" },
}; 
