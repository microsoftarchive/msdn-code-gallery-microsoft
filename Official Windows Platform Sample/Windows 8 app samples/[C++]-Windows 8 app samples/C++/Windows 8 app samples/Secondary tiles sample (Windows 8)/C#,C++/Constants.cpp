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

using namespace SecondaryTiles;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    { "Pin Tile",                           "SecondaryTiles.PinTile" },
    { "Unpin Tile",                         "SecondaryTiles.UnpinTile" },
    { "Enumerate Tiles",                    "SecondaryTiles.EnumerateTiles" },
    { "Is Tile Pinned?",                    "SecondaryTiles.TilePinned" },
    { "Show Activation Arguments",          "SecondaryTiles.LaunchedFromSecondaryTile" },
    { "Secondary Tile Notifications",       "SecondaryTiles.SecondaryTileNotification" },
    { "Pin/Unpin Through Appbar",           "SecondaryTiles.PinFromAppbar" },
    { "Update Secondary Tile Default Logo", "SecondaryTiles.UpdateAsync" }
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
