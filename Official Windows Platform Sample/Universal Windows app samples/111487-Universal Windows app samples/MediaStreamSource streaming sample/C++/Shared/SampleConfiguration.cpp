//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "SampleConfiguration.h"

using namespace SDKSample;
using namespace SDKSample::MediaStreamSource;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Stream MP3 Audio", "SDKSample.MediaStreamSource.Scenario1_LoadMP3FileForAudioStream" },
    { "Stream DirectX3D Video", "SDKSample.MediaStreamSource.Scenario2_UseDirectXForVideoStream" }
}; 
