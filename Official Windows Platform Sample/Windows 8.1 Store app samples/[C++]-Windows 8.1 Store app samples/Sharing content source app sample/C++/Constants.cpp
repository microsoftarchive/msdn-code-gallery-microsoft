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

using namespace Windows::UI::ViewManagement;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    { "Share text",                 "SDKSample.ShareSource.ShareText" },
    { "Share web link",             "SDKSample.ShareSource.ShareWebLink" },
    { "Share application link",     "SDKSample.ShareSource.ShareApplicationLink" },
    { "Share image",                "SDKSample.ShareSource.ShareImage" },
    { "Share files",                "SDKSample.ShareSource.ShareFiles" },
    { "Share delay rendered files", "SDKSample.ShareSource.ShareDelayRenderedFiles" },
    { "Share HTML content",         "SDKSample.ShareSource.ShareHtml" },
    { "Share custom data",          "SDKSample.ShareSource.ShareCustomData" },
    { "Fail with display text",     "SDKSample.ShareSource.SetErrorMessage" }
};

