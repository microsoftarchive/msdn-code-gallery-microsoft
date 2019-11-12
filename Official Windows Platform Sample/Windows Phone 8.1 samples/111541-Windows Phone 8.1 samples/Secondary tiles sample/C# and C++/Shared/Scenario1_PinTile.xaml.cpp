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
#include "Scenario1_PinTile.xaml.h"

using namespace SDKSample;

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
    rootPage = MainPage::Current;
}


void SDKSample::PinTile::PinButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        // Prepare package images for all four tile sizes in our tile to be pinned as well as for the square30x30 logo used in the Apps view.
        auto square150x150Logo = ref new Uri("ms-appx:///Assets/square150x150Tile-sdk.png");
        auto wide310x150Logo = ref new Uri("ms-appx:///Assets/wide310x150Tile-sdk.png");
        auto square310x310Logo = ref new Uri("ms-appx:///Assets/square310x310Tile-sdk.png");
        auto square30x30Logo = ref new Uri("ms-appx:///Assets/square30x30Tile-sdk.png");
        Calendar^ cal = ref new Calendar();
        auto longtime = ref new DateTimeFormatter("longtime");
        DateTime time = cal->GetDateTime();

        // During creation of the secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
        // These arguments should be meaningful to the application. In this sample, we will just pass a text as an argument.
        String^ tileActivationArguments = rootPage->tileId + " WasPinnedAt=" + longtime->Format(time);

        // Create a Secondary tile with all the required arguments.
        // Note the last argument specifies what size the Secondary tile should show up as by default in the Pin to start fly out.
        // It can be set to TileSize.Square150x150, TileSize.Wide310x150, or TileSize.Default.  
        // If set to TileSize.Wide310x150, then the asset for the wide size must be supplied as well.
        // TileSize.Default will default to the wide size if a wide size is provided, and to the medium size otherwise. 
        auto secondaryTile = ref new SecondaryTile(rootPage->tileId,
            "Title text shown on the tile",
            tileActivationArguments,
            square150x150Logo,
            TileSize::Square150x150);

        // Only support of the small and medium tile sizes is mandatory.
        // To have the larger tile sizes available the assets must be provided.
        secondaryTile->VisualElements->Wide310x150Logo = wide310x150Logo;
        secondaryTile->VisualElements->Square310x310Logo = square310x310Logo;

        // Like the background color, the square30x30 logo is inherited from the parent application tile by default.
        // Let's override it, just to see how that's done.
        secondaryTile->VisualElements->Square30x30Logo = square30x30Logo;

        // The display of the secondary tile name can be controlled for each tile size.
        // The default is false.
        secondaryTile->VisualElements->ShowNameOnSquare150x150Logo = true;
        secondaryTile->VisualElements->ShowNameOnWide310x150Logo = true;
        secondaryTile->VisualElements->ShowNameOnSquare310x310Logo = true;

        // The tile background color is inherited from the parent unless a separate value is specified.
        // This operation will over-ride the default.
        secondaryTile->VisualElements->ForegroundText = ForegroundText::Dark;

        // Set this to false if roaming doesn't make sense for the secondary tile.
        // The default is true.
        secondaryTile->RoamingEnabled = false;

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
