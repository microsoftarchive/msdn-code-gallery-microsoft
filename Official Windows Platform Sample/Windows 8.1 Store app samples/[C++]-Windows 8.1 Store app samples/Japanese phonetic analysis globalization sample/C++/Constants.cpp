//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::JapanesePhoneticAnalysis;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Get words from Japanese text", "SDKSample.JapanesePhoneticAnalysis.Scenario1" }, 
    { "Get words from Japanese text with MonoRuby option", "SDKSample.JapanesePhoneticAnalysis.Scenario2" },
}; 
