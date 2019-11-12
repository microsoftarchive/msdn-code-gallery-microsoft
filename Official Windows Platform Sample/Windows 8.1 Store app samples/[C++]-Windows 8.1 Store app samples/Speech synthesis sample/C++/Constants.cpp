//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::VoiceSynthesisCPP;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    { "Speak Text", "SDKSample.VoiceSynthesisCPP.Scenario1" }, 
    { "Speak SSML", "SDKSample.VoiceSynthesisCPP.Scenario2" }
}; 
