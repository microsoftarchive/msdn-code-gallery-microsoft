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
	{ "Binding modes", "SDKSample.DataBinding.Scenario1" }, 
	{ "Value converters", "SDKSample.DataBinding.Scenario2" },
	{ "Binding to a model", "SDKSample.DataBinding.Scenario3" },
	{ "Indexers", "SDKSample.DataBinding.Scenario4" },
	{ "Data templates", "SDKSample.DataBinding.Scenario5" },
	{ "CollectionViewSource", "SDKSample.DataBinding.Scenario6" },
	{ "Collection changes", "SDKSample.DataBinding.Scenario7" },
	{ "Incremental Loading", "SDKSample.DataBinding.Scenario8" }
}; 
