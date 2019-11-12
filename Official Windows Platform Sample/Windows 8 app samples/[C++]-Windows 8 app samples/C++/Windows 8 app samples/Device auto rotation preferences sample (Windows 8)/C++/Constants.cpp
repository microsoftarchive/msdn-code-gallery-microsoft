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
    { "Clear Preferences", "SDKSample.Rotation.None" }, 
    { "Landscape Only", "SDKSample.Rotation.Landscape" },
    { "Portrait Only", "SDKSample.Rotation.Portrait" },
    { "Landscape Flipped Only", "SDKSample.Rotation.LandscapeFlipped" },
    { "Portrait Flipped Only", "SDKSample.Rotation.PortraitFlipped" }
}; 
