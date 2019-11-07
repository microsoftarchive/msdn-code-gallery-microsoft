//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>  
{
	{ "Basic Usage", "SDKSample.Navigation.Scenario1" },
	{ "Passing information between pages", "SDKSample.Navigation.Scenario2" },
	{ "Cancel Navigation", "SDKSample.Navigation.Scenario3" },
	{ "Caching a Page", "SDKSample.Navigation.Scenario4" }
}; 
