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
    { "Enumerate files and folders in the Pictures library",         "SDKSample.FolderEnumeration.Scenario1" },
    { "Enumerate files in the Pictures library, by groups",          "SDKSample.FolderEnumeration.Scenario2" },
    { "Enumerate files in the Pictures library with prefetch APIs",  "SDKSample.FolderEnumeration.Scenario3" },
};
