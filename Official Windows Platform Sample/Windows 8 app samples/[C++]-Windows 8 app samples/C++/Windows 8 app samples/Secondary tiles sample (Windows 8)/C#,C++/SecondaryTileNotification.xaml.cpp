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
// SecondaryTileNotification.xaml.cpp
// Implementation of the SecondaryTileNotification class
//

#include "pch.h"
#include "SecondaryTileNotification.xaml.h"

using namespace SecondaryTiles;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::StartScreen;
using namespace Windows::Foundation;
using namespace Platform;
using namespace concurrency;
using namespace Windows::UI::Notifications;
using namespace NotificationsExtensions::TileContent;
using namespace NotificationsExtensions::BadgeContent;
using namespace Windows::Data::Xml::Dom;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;

SecondaryTileNotification::SecondaryTileNotification()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void SecondaryTileNotification::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::current;
    ToggleButtons(SecondaryTile::Exists(rootPage->dynamicTileId));
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
void SecondaryTileNotification::OnNavigatingFrom(NavigatingCancelEventArgs^ e)
{
    // Restore the app bar
    rootPage->BottomAppBar = appBar;
}

void SecondaryTileNotification::ToggleButtons(bool isEnabled)
{
    SendTileNotification->IsEnabled = isEnabled;
    SendBadgeNotification->IsEnabled = isEnabled;
    SendTileNotificationString->IsEnabled = isEnabled;
    SendBadgeNotificationString->IsEnabled = isEnabled;
}

void SecondaryTiles::SecondaryTileNotification::PinLiveTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        // Prepare package images for use as the Tile Logo and small logo in the tile to be pinned.
        // The path provided is a relative path to the package
        auto logo = ref new Uri("ms-appx:///Assets/squareTile-sdk.png");
        auto wideLogo = ref new Uri("ms-appx:///Assets/tile-sdk.png");

        Calendar^ cal = ref new Calendar();
        auto longtime = ref new DateTimeFormatter("longtime");
        DateTime time = cal->GetDateTime();

        // During creation of secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
        // These arguments should be meaningful to the application. In this sample, we will just pass a text as an argument.
        String^ tileActivationArguments = rootPage->dynamicTileId + " WasPinnedAt=" + longtime->Format(time);

        auto secondaryTile = ref new SecondaryTile(rootPage->dynamicTileId,
            "Live Secondary Tile", //short tile name (will be displayed on the tile)
            "SDK Sample Live Secondary Tile", // Long tile name (for search, tooltip)
            tileActivationArguments,
            Windows::UI::StartScreen::TileOptions::None,
            logo,
            wideLogo);

        // The tile background color is inherited from the parent unless a separate value is specified.
        // This operation will over-ride the default.
        secondaryTile->ForegroundText = ForegroundText::Light;

            // OK, the tile is created and we can now attempt to pin the tile.
            // Note that the status message is updated when the async operation to pin the tile completes.
            auto const rect = rootPage->GetElementRect(safe_cast<FrameworkElement^>(sender));
            create_task(secondaryTile->RequestCreateForSelectionAsync(rect, Windows::UI::Popups::Placement::Below)).then([this](bool isCreated)
            {
                if (isCreated)
                {
                    rootPage->NotifyUser("Secondary tile successfully pinned.", NotifyType::StatusMessage);
                    ToggleButtons(true);
                }
                else
                {
                    rootPage->NotifyUser("Secondary tile not pinned.", NotifyType::ErrorMessage);
                }
            });
        }
}

void SecondaryTiles::SecondaryTileNotification::SendTileNotification_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        if (SecondaryTile::Exists(rootPage->dynamicTileId))
        {
            // Note: This sample contains an additional reference, NotificationsExtensions, which you can use in your apps
            auto tileContent = TileContentFactory::CreateTileWideText04();
            tileContent->TextBodyWrap->Text = "Sent to a secondary tile from NotificationsExtensions!";

            auto squareContent = TileContentFactory::CreateTileSquareText04();
        squareContent->TextBodyWrap->Text = "Sent to a secondary tile from NotificationsExtensions!";
        tileContent->SquareContent = squareContent;

            // Send the notification to the secondary tile by creating a secondary tile updater
            TileUpdateManager::CreateTileUpdaterForSecondaryTile(rootPage->dynamicTileId)->Update(tileContent->CreateNotification());

            rootPage->NotifyUser("Tile Notification sent to " + rootPage->dynamicTileId + ".", NotifyType::StatusMessage);
        }
        else
        {
            ToggleButtons(false);
            rootPage->NotifyUser(rootPage->dynamicTileId + " not pinned.", NotifyType::ErrorMessage);
        }
    }
}

void SecondaryTiles::SecondaryTileNotification::SendBadgeNotification_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        if (SecondaryTile::Exists(rootPage->dynamicTileId))
        {
            auto badgeContent = ref new BadgeNumericNotificationContent(6);

            // Send the notification to the secondary tile
            BadgeUpdateManager::CreateBadgeUpdaterForSecondaryTile(rootPage->dynamicTileId)->Update(badgeContent->CreateNotification());
            rootPage->NotifyUser("BadgeNotification sent to " + rootPage->dynamicTileId + ".", NotifyType::StatusMessage);
        }
        else
        {
            ToggleButtons(false);
            rootPage->NotifyUser(rootPage->dynamicTileId + " not pinned.", NotifyType::ErrorMessage);
        }
    }
}

void SecondaryTiles::SecondaryTileNotification::SendBadgeNotificationWithStringManipulation_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        Platform::String^ badgeXmlString = "<badge value='9'/>";
        auto badgeDOM = ref new Windows::Data::Xml::Dom::XmlDocument();
        badgeDOM->LoadXml(badgeXmlString);
        auto badge = ref new BadgeNotification(badgeDOM);

        // Send the notification to the secondary tile
        BadgeUpdateManager::CreateBadgeUpdaterForSecondaryTile(rootPage->dynamicTileId)->Update(badge);

        rootPage->NotifyUser("BadgeNotification sent to " + rootPage->dynamicTileId + ".", NotifyType::StatusMessage);
    }
}


void SecondaryTiles::SecondaryTileNotification::SendTileNotificationWithStringManipulation_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        auto tileXmlString = "<tile>"
            +  "<visual>"
            +   "<binding template='TileWideText04'>"
            +    "<text id='1'>Send to a secondary tile from strings</text>"
            +   "</binding>"
            +   "<binding template='TileSquareText04'>"
            +    "<text id='1'>Send to a secondary tile from strings</text>"
            +   "</binding>"
            +  "</visual>"
            + "</tile>";

        auto tileDOM = ref new Windows::Data::Xml::Dom::XmlDocument();
        tileDOM->LoadXml(tileXmlString);
        auto tile = ref new TileNotification(tileDOM);

        // Send the notification to the secondary tile by creating a secondary tile updater
        TileUpdateManager::CreateTileUpdaterForSecondaryTile(rootPage->dynamicTileId)->Update(tile);

        rootPage->NotifyUser("Tile Notification sent to " + rootPage->dynamicTileId + ".", NotifyType::StatusMessage);
    }
}
