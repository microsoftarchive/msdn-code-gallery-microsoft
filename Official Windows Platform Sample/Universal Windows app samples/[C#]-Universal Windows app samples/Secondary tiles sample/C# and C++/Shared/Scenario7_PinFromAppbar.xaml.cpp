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
// PinFromAppbar.xaml.cpp
// Implementation of the PinFromAppbar class
//

#include "pch.h"
#include "Scenario7_PinFromAppbar.xaml.h"

using namespace SDKSample;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::StartScreen;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::StartScreen;
using namespace Windows::Foundation;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;
using namespace Platform;
using namespace concurrency;

PinFromAppbar::PinFromAppbar()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void PinFromAppbar::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    Init();
}

void PinFromAppbar::Init()
{
    // The appbar is open by default.
    rootPage->NotifyUser("Tap or click the Pin button to pin a tile.", NotifyType::StatusMessage);
    this->PinUnPinCommandButton->Click += ref new RoutedEventHandler(this, &PinFromAppbar::Button_Click);
    this->SecondaryTileCommandBar->Opened += ref new Windows::Foundation::EventHandler<Object^>(this, &PinFromAppbar::BottomAppBar_Opened);

    create_task(SecondaryTile::FindAllAsync("SecondaryTiles.App")).then([this](IVectorView<SecondaryTile^>^ tileList)
    {
        if (tileList->Size > 0)
        {
            bool showPinButton = true;
            std::for_each(begin(tileList), end(tileList), [this, &showPinButton](SecondaryTile^ tile)
            {
                if (tile->TileId == "SecondaryTile.AppBar")
                {
                    showPinButton = false;
                }
            });

            ToggleAppBarButton(showPinButton);
        }
    });
}

// This toggles the Pin and unpin button in the app bar
void PinFromAppbar::ToggleAppBarButton(bool showPinButton)
{
    if (showPinButton)
    {
        this->PinUnPinCommandButton->Label = "Pin";
        this->PinUnPinCommandButton->Icon = ref new SymbolIcon(Symbol::Pin);
    }
    else
    {
        this->PinUnPinCommandButton->Label = "Unpin";
        this->PinUnPinCommandButton->Icon = ref new SymbolIcon(Symbol::UnPin);
    }

    this->PinUnPinCommandButton->UpdateLayout();
}

void SDKSample::PinFromAppbar::BottomAppBar_Opened(Platform::Object^ sender, Platform::Object^ e)
{
    ToggleAppBarButton(!SecondaryTile::Exists(rootPage->appbarTileId));
}

void SDKSample::PinFromAppbar::Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        this->SecondaryTileCommandBar->IsSticky = true;
        // Let us first verify if we need to pin or unpin
        if (SecondaryTile::Exists(rootPage->appbarTileId))
        {
            // First prepare the tile to be unpinned
            auto secondaryTile = ref new SecondaryTile(rootPage->appbarTileId);
            // Now make the delete request
            auto const rect = rootPage->GetElementRect(safe_cast<FrameworkElement^>(sender));
            create_task(secondaryTile->RequestDeleteForSelectionAsync(rect, Windows::UI::Popups::Placement::Above)).then([this](bool isUnPinned)
            {
                if (isUnPinned)
                {
                    rootPage->NotifyUser("Secondary tile successfully unpinned.", NotifyType::StatusMessage);
                    ToggleAppBarButton(isUnPinned);
                }
                else
                {
                    rootPage->NotifyUser("Secondary tile not unpinned.", NotifyType::ErrorMessage);
                }

                this->SecondaryTileCommandBar->IsSticky = false;
            });
        }
        else
        {
            // Prepare package images for the medium tile size in our tile to be pinned
            auto square150x150Logo = ref new Uri("ms-appx:///Assets/square150x150Tile-sdk.png");
            Calendar^ cal = ref new Calendar();
            auto longtime = ref new DateTimeFormatter("longtime");
            DateTime time = cal->GetDateTime();
            bool isPinned = false;

            // During creation of secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
            // These arguments should be meaningful to the application. In this sample, we will just pass a text as an argument.
            String^ tileActivationArguments = rootPage->appbarTileId + " WasPinnedAt=" + longtime->Format(time);

            // Create a Secondary tile with all the required arguments.
            // Note the last argument specifies what size the Secondary tile should show up as by default in the Pin to start fly out.
            // It can be set to TileSize.Square150x150, TileSize.Wide310x150, or TileSize.Default.  
            // If set to TileSize.Wide310x150, then the asset for the wide size must be supplied as well.  
            // TileSize.Default will default to the wide size if a wide size is provided, and to the medium size otherwise.             
            auto secondaryTile = ref new SecondaryTile(rootPage->appbarTileId,
                                                       "Secondary tile pinned via AppBar",
                                                       tileActivationArguments,
                                                       square150x150Logo,
                                                       TileSize::Square150x150);

            // Whether or not the app name should be displayed on the tile can be controlled for each tile size.  The default is false.
            secondaryTile->VisualElements->ShowNameOnSquare150x150Logo = true;

            // The tile background color is inherited from the parent unless a separate value is specified.
            // This operation will over-ride the default.
            secondaryTile->VisualElements->ForegroundText = ForegroundText::Dark;

            // OK, the tile is created and we can now attempt to pin the tile.
            // Note that the status message is updated when the async operation to pin the tile completes.
            auto const rect = rootPage->GetElementRect(safe_cast<FrameworkElement^>(sender));
            create_task(secondaryTile->RequestCreateForSelectionAsync(rect, Windows::UI::Popups::Placement::Above)).then([this](bool isCreated)
            {
                if (isCreated)
                {
                    rootPage->NotifyUser("Secondary tile successfully pinned.", NotifyType::StatusMessage);
                    ToggleAppBarButton(!isCreated);
                }
                else
                {
                    rootPage->NotifyUser("Secondary tile not pinned.", NotifyType::ErrorMessage);
                }

                this->SecondaryTileCommandBar->IsSticky = false;
            });
        }
    }
}
