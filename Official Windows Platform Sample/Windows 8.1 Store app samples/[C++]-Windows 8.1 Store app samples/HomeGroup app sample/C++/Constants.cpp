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
using namespace Windows::UI::ViewManagement;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    { "Open the file picker at HomeGroup",    "SDKSample.HomeGroup.HomeGroupPicker" },
    { "Search HomeGroup",                     "SDKSample.HomeGroup.SearchHomeGroup" },
    { "Stream video from Homegroup",          "SDKSample.HomeGroup.HomeGroupVideoStream" },
    { "Advanced search",                      "SDKSample.HomeGroup.HomeGroupAdvancedSearch"}
};


