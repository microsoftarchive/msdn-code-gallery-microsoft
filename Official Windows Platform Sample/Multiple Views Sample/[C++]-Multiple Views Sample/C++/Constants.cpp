//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::MultipleViews;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    { "Creating and showing multiple views", "SDKSample.MultipleViews.Scenario1" }, 
    { "Responding to activation", "SDKSample.MultipleViews.Scenario2" },
    { "Using animations when switching", "SDKSample.MultipleViews.Scenario3" }
}; 
