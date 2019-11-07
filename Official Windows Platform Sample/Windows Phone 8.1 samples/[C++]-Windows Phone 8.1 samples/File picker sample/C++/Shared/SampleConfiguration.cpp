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

using namespace Platform;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml::Controls;

Array<Scenario>^ MainPage::scenariosInner = ref new Array<Scenario>
{
    { "Pick a single photo",   "SDKSample.FilePicker.Scenario1" },
    { "Pick multiple files",   "SDKSample.FilePicker.Scenario2" },
    { "Pick a folder",         "SDKSample.FilePicker.Scenario3" },
    { "Save a file",           "SDKSample.FilePicker.Scenario4" },
    { "Open a cached file",    "SDKSample.FilePicker.Scenario5" },
    { "Update a cached file",  "SDKSample.FilePicker.Scenario6" },
};
