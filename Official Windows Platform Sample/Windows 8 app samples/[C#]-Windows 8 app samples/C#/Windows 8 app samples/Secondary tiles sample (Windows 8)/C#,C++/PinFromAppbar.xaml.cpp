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
#include "PinFromAppbar.xaml.h"

using namespace SecondaryTiles;
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
    rootPage = MainPage::current;
    Init();
}

void PinFromAppbar::OnNavigatingFrom(NavigatingCancelEventArgs^ e)
{
    if (rightPanel != nullptr)
    {
        rootPage->BottomAppBar->Opened -= appBarOpenedToken;
        pinToAppBar->Click -= buttonClickToken;
        rightPanel->Children->Clear();
        rightPanel = nullptr;
    }
}

void PinFromAppbar::Init()
{
    rootPage->NotifyUser("To show the bar swipe up from the bottom of the screen, right-click, or press Windows Logo + z. To dismiss the bar, swipe, right-click, or press Windows Logo + z again.", NotifyType::StatusMessage);
    rootPage->BottomAppBar->IsOpen = true;
    rightPanel = safe_cast<StackPanel^>(rootPage->BottomAppBar->FindName("RightPanel"));
    rootPage->BottomAppBar->IsOpen = false;

    if (rightPanel != nullptr)
    {
        pinToAppBar = ref new Button();
        ToggleAppBarButton(!SecondaryTile::Exists(rootPage->appbarTileId));
        appBarOpenedToken = rootPage->BottomAppBar->Opened += ref new Windows::Foundation::EventHandler<Object^>(this, &PinFromAppbar::BottomAppBar_Opened);
        buttonClickToken = pinToAppBar->Click += ref new RoutedEventHandler(this, &PinFromAppbar::Button_Click);
        rightPanel->Children->Append(pinToAppBar);
    }
}

// This toggles the Pin and unpin button in the app bar
void PinFromAppbar::ToggleAppBarButton(bool showPinButton)
{
    if (pinToAppBar != nullptr)
    {
        pinToAppBar->Style = (showPinButton) ? (safe_cast<Windows::UI::Xaml::Style^>(Resources->Lookup("PinAppBarButtonStyle"))) : (safe_cast<Windows::UI::Xaml::Style^>(Resources->Lookup("UnpinAppBarButtonStyle")));
    }
}

void SecondaryTiles::PinFromAppbar::BottomAppBar_Opened(Platform::Object^ sender, Platform::Object^ e)
{
    ToggleAppBarButton(!SecondaryTile::Exists(rootPage->appbarTileId));
}

void SecondaryTiles::PinFromAppbar::Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        rootPage->BottomAppBar->IsSticky = true;
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
                rootPage->BottomAppBar->IsSticky = false;
            });
        }
        else
        {
            // Prepare package images for use as the Tile Logo and small logo in the tile to be pinned.
            // The path provided is a relative path to the package
            auto logo = ref new Uri("ms-appx:///Assets/squareTile-sdk.png");
            Calendar^ cal = ref new Calendar();
            auto longtime = ref new DateTimeFormatter("longtime");
            DateTime time = cal->GetDateTime();
            bool isPinned = false;

            // During creation of secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
            // These arguments should be meaningful to the application. In this sample, we will just pass a text as an argument.
            String^ tileActivationArguments = rootPage->appbarTileId + " WasPinnedAt=" + longtime->Format(time);

            auto secondaryTile = ref new SecondaryTile(rootPage->appbarTileId,
                                                       "Secondary tile pinned via AppBar",
                                                       "SDK Sample Secondary Tile pinned from AppBar",
                                                       tileActivationArguments,
                                                       Windows::UI::StartScreen::TileOptions::ShowNameOnLogo,
                                                       logo);

            // The tile background color is inherited from the parent unless a separate value is specified.
            // This operation will over-ride the default.
            secondaryTile->ForegroundText = ForegroundText::Dark;

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
                rootPage->BottomAppBar->IsSticky = false;
            });
        }
    }
}
