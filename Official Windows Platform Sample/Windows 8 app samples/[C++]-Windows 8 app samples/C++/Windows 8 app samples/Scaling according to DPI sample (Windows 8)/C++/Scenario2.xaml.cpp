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
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"

using namespace SDKSample::Scaling;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Text;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();
    DisplayProperties::LogicalDpiChanged += ref new DisplayPropertiesEventHandler(this, &Scenario2::DisplayProperties_LogicalDpiChanged);

    defaultFontFamily = ref new Windows::UI::Xaml::Media::FontFamily("Segoe UI");
    overrideFontFamily = ref new Windows::UI::Xaml::Media::FontFamily("Segoe Script");

    DefaultLayoutText->FontSize = PxFromPt(20);  // xaml fontsize is in pixels.
    DefaultLayoutText->FontFamily = defaultFontFamily;
}

double Scenario2::PtFromPx(double pixel)
{
    return pixel * 72 / 96;
}

double Scenario2::PxFromPt(double pt)
{
    return pt * 96 / 72;
}

void Scenario2::SetOverrideRectSize(double sizeInPhysicalPx, double scaleFactor)
{
    // Set the size of OverrideLayoutRect based on the desired size in physical pixels and the scale factor.
    // The code here is to demonstrate how to override default scaling behavior to keep the physical pixel size of a control.
    double sizeInRelativePx = sizeInPhysicalPx / scaleFactor;
    OverrideLayoutRect->Width = sizeInRelativePx;
    OverrideLayoutRect->Height = sizeInRelativePx;
}

void Scenario2::SetOverrideTextFont(double size, Windows::UI::Xaml::Media::FontFamily^ fontFamily)
{
    OverrideLayoutText->FontSize = PxFromPt(size);  // xaml fontsize is in pixels.
    OverrideLayoutText->FontFamily = fontFamily;
}

String^ Scenario2::StringFromDouble(double x)
{
    // Round to the nearest tenth for display.
    return (((int)(x * 10 + 0.5)) / 10.0).ToString();
}

void Scenario2::OutputSettings(double scaleFactor, FrameworkElement^ rectangle, TextBlock^ relativePxText, TextBlock^ physicalPxText, TextBlock^ fontTextBlock)
{
    // Get the size of the rectangle in relative pixels and calulate the size in physical pixels.
    double sizeInRelativePx = rectangle->Width;
    double sizeInPhysicalPx = sizeInRelativePx * scaleFactor;

    relativePxText->Text = StringFromDouble(sizeInRelativePx) + " relative px";
    physicalPxText->Text = StringFromDouble(sizeInPhysicalPx) + " physical px";

    double fontSize = PtFromPx(fontTextBlock->FontSize);
    fontTextBlock->Text = StringFromDouble(fontSize) + "pt " + fontTextBlock->FontFamily->Source;
}

void Scenario2::ResetOutput()
{
    ResolutionTextBlock->Text = StringFromDouble(Window::Current->Bounds.Width) + "x" + StringFromDouble(Window::Current->Bounds.Height);

    double scaleFactor;
    double fontSize;
    Windows::UI::Xaml::Media::FontFamily^ fontFamily;
    switch (DisplayProperties::ResolutionScale)
    {
    case ResolutionScale::Invalid:
    case ResolutionScale::Scale100Percent:
    default:
        scaleFactor = 1.0;
        fontSize = 20;
        fontFamily = defaultFontFamily;
        break;

    case ResolutionScale::Scale140Percent:
        scaleFactor = 1.4;
        fontSize = 11;
        fontFamily = overrideFontFamily;
        break;

    case ResolutionScale::Scale180Percent:
        scaleFactor = 1.8;
        fontSize = 9;
        fontFamily = overrideFontFamily;
        break;
    }

    // Set the override rectangle size and override text font.
    const double rectSizeInPhysicalPx = 100;
    SetOverrideRectSize(rectSizeInPhysicalPx, scaleFactor);
    SetOverrideTextFont(fontSize, fontFamily);

    // Output settings for controls with default scaling behavior.
    OutputSettings(scaleFactor, DefaultLayoutRect, DefaultRelativePx, DefaultPhysicalPx, DefaultLayoutText);
    // Output settings for override controls.
    OutputSettings(scaleFactor, OverrideLayoutRect, OverrideRelativePx, OverridePhysicalPx, OverrideLayoutText);
}

void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    ResetOutput();
}

void Scenario2::DisplayProperties_LogicalDpiChanged(Object^ sender)
{
    ResetOutput();
}
