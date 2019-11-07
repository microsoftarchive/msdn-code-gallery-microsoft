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
    { "Percent and Permille Formatting", "SDKSample.NumberFormatting.PercentPermilleFormatting" }, 
    { "Decimal Formatting", "SDKSample.NumberFormatting.DecimalFormatting" },
    { "Currency Formatting", "SDKSample.NumberFormatting.CurrencyFormatting" },
    { "Number Parsing", "SDKSample.NumberFormatting.NumberParsing" }
}; 
