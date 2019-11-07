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
    { "Stream type: BackgroundCapableMedia", "SDKSample.PlaybackManager.StreamTypeBackgroundCapableMedia" }, 
    { "Stream type: Communications", "SDKSample.PlaybackManager.StreamTypeCommunications" },
    { "Stream type: Other", "SDKSample.PlaybackManager.StreamTypeOther" },
    { "Stream type: ForegroundOnlyMedia", "SDKSample.PlaybackManager.StreamTypeForegroundOnlyMedia" },
    { "Stream type: Alert", "SDKSample.PlaybackManager.StreamTypeAlert" }
}; 
