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
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Retrieve", "SDKSample.AtomPub.Scenario1" }, 
    { "Create", "SDKSample.AtomPub.Scenario2" }, 
    { "Delete", "SDKSample.AtomPub.Scenario3" },
    { "Update", "SDKSample.AtomPub.Scenario4" }
}; 

// The default values for the WordPress site.
Platform::String^ CommonData::baseUri = "http://<YourWordPressSite>.wordpress.com/";
Platform::String^ CommonData::user;
Platform::String^ CommonData::password;

// The default Service Document and Edit 'URIs' for WordPress.
Platform::String^ CommonData::editUri = "./wp-app.php/posts";
Platform::String^ CommonData::serviceDocUri = "./wp-app.php/service";
Platform::String^ CommonData::feedUri = "./?feed=atom";
