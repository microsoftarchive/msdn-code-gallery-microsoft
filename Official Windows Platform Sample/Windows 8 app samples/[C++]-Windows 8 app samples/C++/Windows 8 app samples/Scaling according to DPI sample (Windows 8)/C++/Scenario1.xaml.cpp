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
    DisplayProperties::LogicalDpiChanged += ref new DisplayPropertiesEventHandler(this, &Scenario1::DisplayProperties_LogicalDpiChanged);

    String^ baseUri = ManualLoadImage->BaseUri->AbsoluteUri;
    image100 = ref new BitmapImage(ref new Uri(baseUri, "Assets/projector.scale-100.png"));
    image140 = ref new BitmapImage(ref new Uri(baseUri, "Assets/projector.scale-140.png"));
    image180 = ref new BitmapImage(ref new Uri(baseUri, "Assets/projector.scale-180.png"));
}

String^ Scenario1::GetMinDPI()
{
    Platform::String^ minDPI = "";
    switch (DisplayProperties::ResolutionScale)
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
    switch (DisplayProperties::ResolutionScale)
    {
    case ResolutionScale::Invalid:
        minRes = "Unknown";
        break;
    case ResolutionScale::Scale100Percent:
        minRes = "1024x768 (min resolution needed to run apps)";
        break;
    case ResolutionScale::Scale140Percent:
        minRes = "1920x1080";
        break;
    case ResolutionScale::Scale180Percent:
        minRes = "2560x1440";
        break;
    }

    return minRes;
}

void Scenario1::SetManualLoadImage()
{
    switch (DisplayProperties::ResolutionScale)
    {
    default:
    case ResolutionScale::Scale100Percent:
        ManualLoadImage->Source = image100;
        break;
    case ResolutionScale::Scale140Percent:
        ManualLoadImage->Source = image140;
        break;
    case ResolutionScale::Scale180Percent:
        ManualLoadImage->Source = image180;
        break;
    }
}

void Scenario1::ResetOutput()
{
    ScalingText->Text = (DisplayProperties::LogicalDpi * PERCENT / DEFAULT_LOGICALPPI).ToString() + "%";
    MinDPIText->Text = GetMinDPI();
    MinScreenResolutionText->Text = GetMinResolution();
    LogicalDPIText->Text = DisplayProperties::LogicalDpi.ToString() + " DPI";
    SetManualLoadImage();
}

void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    ResetOutput();
}

void Scenario1::DisplayProperties_LogicalDpiChanged(Object^ sender)
{
    ResetOutput();
}
