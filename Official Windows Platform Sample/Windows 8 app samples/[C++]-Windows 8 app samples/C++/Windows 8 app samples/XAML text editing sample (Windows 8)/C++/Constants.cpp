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
    { "Windows 8 text selection", "SDKSample.TextEditing.Scenario1" }, 
    { "Accessing text selection", "SDKSample.TextEditing.Scenario2" },
    { "Spell checking", "SDKSample.TextEditing.Scenario3" },
	{ "Text prediction", "SDKSample.TextEditing.Scenario4" },
	{ "Input scopes", "SDKSample.TextEditing.Scenario5" },
	{ "RichEditBox and WinRT TOM", "SDKSample.TextEditing.Scenario6" }, 
	{ "PasswordBox", "SDKSample.TextEditing.Scenario7" }
}; 
