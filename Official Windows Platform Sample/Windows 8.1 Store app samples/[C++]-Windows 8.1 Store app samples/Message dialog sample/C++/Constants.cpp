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
    { "Use custom commands", "SDKSample.MessageDialogSample.CustomCommand" },
    { "Use default close command", "SDKSample.MessageDialogSample.DefaultCloseCommand" },
    { "Use completed callback", "SDKSample.MessageDialogSample.CompletedCallback" },
    { "Use cancel command", "SDKSample.MessageDialogSample.CancelCommand" }
};
