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
#include "SampleConfiguration.h"

using namespace SDKSample;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    { "Pin Tile",                           "SDKSample.PinTile" },
    { "Unpin Tile",                         "SDKSample.UnpinTile" },
    { "Enumerate Tiles",                    "SDKSample.EnumerateTiles" },
    { "Is Tile Pinned?",                    "SDKSample.TilePinned" },
    { "Show Activation Arguments",          "SDKSample.LaunchedFromSecondaryTile" },
    { "Secondary Tile Notifications",       "SDKSample.SecondaryTileNotification" },
    { "Pin/Unpin Through Appbar",           "SDKSample.PinFromAppbar" },
    { "Update Secondary Tile Default Logo", "SDKSample.UpdateAsync" },
    { "Pin Tile Alternate Visual Elements", "SDKSample.PinTileAlternateVisualElements" },
    { "Pin Tile Alternate Visual Elements Async", "SDKSample.PinTileAlternateVisualElementsAsync" }
};

Rect MainPage::GetElementRect(FrameworkElement^ element)
{
    Windows::UI::Xaml::Media::GeneralTransform^ buttonTransform = element->TransformToVisual(nullptr);
    const Point pointOrig(0, 0);
    const Point pointTransformed = buttonTransform->TransformPoint(pointOrig);
    const Rect rect(pointTransformed.X,
                    pointTransformed.Y,
                    safe_cast<float>(element->ActualWidth),
                    safe_cast<float>(element->ActualHeight));

    return rect;
}

int MainPage::GetScenarioIdForLaunch(Platform::String^ launchParam)
{
    // this will only be called when we are launched by our secondary tiles.
    return 4;
}
