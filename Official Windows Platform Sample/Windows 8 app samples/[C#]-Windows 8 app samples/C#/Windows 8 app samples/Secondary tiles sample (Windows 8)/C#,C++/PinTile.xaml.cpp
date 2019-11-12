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
// PinTile.xaml.cpp
// Implementation of the PinTile class
//

#include "pch.h"
#include "PinTile.xaml.h"

using namespace SecondaryTiles;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::StartScreen;
using namespace Windows::Foundation;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;
using namespace Platform;
using namespace concurrency;

PinTile::PinTile()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void PinTile::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::current;
    // Preserve the app bar
    appBar = rootPage->BottomAppBar;
    // Now null it so we don't show the appbar in this scenario
    rootPage->BottomAppBar = nullptr;
}

/// <summary>
/// Invoked when this page is about to be navigated out in a Frame
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void PinTile::OnNavigatingFrom(NavigatingCancelEventArgs^ e)
{
    // Restore the app bar
    rootPage->BottomAppBar = appBar;
}

void SecondaryTiles::PinTile::PinButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        // Prepare package images for use as the Tile Logo and small logo in the tile to be pinned.
        // The path provided is a relative path to the package
        auto logo = ref new Uri("ms-appx:///Assets/squareTile-sdk.png");
        auto smallLogo = ref new Uri("ms-appx:///Assets/smallTile-sdk.png");
        Calendar^ cal = ref new Calendar();
        auto longtime = ref new DateTimeFormatter("longtime");
        DateTime time = cal->GetDateTime();

        // During creation of secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
        // These arguments should be meaningful to the application. In this sample, we will just pass a text as an argument.
        String^ tileActivationArguments = rootPage->tileId + " WasPinnedAt=" + longtime->Format(time);

        auto secondaryTile = ref new SecondaryTile(rootPage->tileId,
            "Title text shown on the tile",
            "Name of the tile the user sees when searching for the tile",
            tileActivationArguments,
            Windows::UI::StartScreen::TileOptions::ShowNameOnLogo,
            logo);

        // The tile background color is inherited from the parent unless a separate value is specified.
        // This operation will over-ride the default.
        secondaryTile->ForegroundText = ForegroundText::Dark;

        // Like the background color, the small tile logo is inherited from the parent application tile by default.
        // This operation will over-ride the default.
        secondaryTile->SmallLogo = smallLogo;

        // OK, the tile is created and we can now attempt to pin the tile.
        // Note that the status message is updated when the async operation to pin the tile completes.
        auto const rect = rootPage->GetElementRect(safe_cast<FrameworkElement^>(sender));
        create_task(secondaryTile->RequestCreateForSelectionAsync(rect, Windows::UI::Popups::Placement::Below)).then([this](bool isCreated)
        {
            if (isCreated)
            {
                rootPage->NotifyUser("Secondary tile successfully pinned.", NotifyType::StatusMessage);
            }
            else
            {
                rootPage->NotifyUser("Secondary tile not pinned.", NotifyType::ErrorMessage);
            }
        });
    }
}
