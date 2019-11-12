//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::ApplicationViews;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
    { "Use window orientation to change stacking direction of UI", "SDKSample.ApplicationViews.S1_Orientation" }, 
    { "Use edge information to position controls", "SDKSample.ApplicationViews.S2_Edges" },
}; 
