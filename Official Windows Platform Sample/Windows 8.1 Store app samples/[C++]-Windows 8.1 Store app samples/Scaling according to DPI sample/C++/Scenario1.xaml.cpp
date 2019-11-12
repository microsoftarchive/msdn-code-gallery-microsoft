//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"

using namespace SDKSample::Scaling;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    InitializeComponent();
    DisplayInformation^ displayInformation = DisplayInformation::GetForCurrentView();
    token = (displayInformation->DpiChanged += ref new TypedEventHandler<DisplayInformation^, Platform::Object^>(this, &Scenario1::DisplayProperties_DpiChanged));
}

String^ Scenario1::GetMinDPI()
{
    Platform::String^ minDPI = "";
    DisplayInformation^ displayInformation = DisplayInformation::GetForCurrentView();
    switch (displayInformation->ResolutionScale)
    {
    case ResolutionScale::Invalid:
        minDPI = "Unknown";
        break;
    case ResolutionScale::Scale100Percent:
        minDPI = "No minimum DPI for this scale";
        break;
    case ResolutionScale::Scale140Percent:
        minDPI = "174 DPI";
        break;
    case ResolutionScale::Scale180Percent:
        minDPI = "240 DPI";
        break;
    }

    return minDPI;
}

String^ Scenario1::GetMinResolution()
{
    Platform::String^ minRes = "";
    DisplayInformation^ displayInformation = DisplayInformation::GetForCurrentView();
    switch (displayInformation->ResolutionScale)
    {
    case ResolutionScale::Invalid:
        minRes = "Unknown";
        break;
    case ResolutionScale::Scale100Percent:
        minRes = "1024x768 (min resolution needed to run apps)";
        break;
    case ResolutionScale::Scale140Percent:
        minRes = "1440x1080";
        break;
    case ResolutionScale::Scale180Percent:
        minRes = "1920x1440";
        break;
    }

    return minRes;
}

void Scenario1::ResetOutput()
{
    DisplayInformation^ displayInformation = DisplayInformation::GetForCurrentView();
    String^ scaleValue = static_cast<int>(displayInformation->ResolutionScale).ToString();
    ManualLoadURL->Text = "http://www.contoso.com/imageScale" + scaleValue + ".png";
    ScalingText->Text = scaleValue + "%";
    MinDPIText->Text = GetMinDPI();
    MinScreenResolutionText->Text = GetMinResolution();
    LogicalDPIText->Text = displayInformation->LogicalDpi.ToString() + " DPI";
}

void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    ResetOutput();
}

void Scenario1::OnNavigatedFrom(NavigationEventArgs^ e)
{
    DisplayInformation^ displayInformation = DisplayInformation::GetForCurrentView();
    displayInformation->DpiChanged -= token;
}

void Scenario1::DisplayProperties_DpiChanged(DisplayInformation^ sender, Platform::Object^ args)
{
    ResetOutput();
}