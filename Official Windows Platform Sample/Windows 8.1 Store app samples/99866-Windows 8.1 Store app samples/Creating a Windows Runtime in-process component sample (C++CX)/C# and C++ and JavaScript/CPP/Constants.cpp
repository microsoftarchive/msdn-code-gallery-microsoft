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
    // The format here is the following:
    //     { "Description for the sample", "Fully qualified name for the class that implements the scenario" }
    { "Using Custom Components",                                     "SDKSample.ProxyStubsForWinRTComponents.OvenClient" },
    { "Using Custom Components (Implemented using WRL)",             "SDKSample.ProxyStubsForWinRTComponents.OvenClientWRL" },
    { "Handling Windows Runtime Exceptions",                         "SDKSample.ProxyStubsForWinRTComponents.CustomException" },
    { "Handling Windows Runtime Exceptions (Implemented using WRL)", "SDKSample.ProxyStubsForWinRTComponents.CustomExceptionWRL" }
}; 
