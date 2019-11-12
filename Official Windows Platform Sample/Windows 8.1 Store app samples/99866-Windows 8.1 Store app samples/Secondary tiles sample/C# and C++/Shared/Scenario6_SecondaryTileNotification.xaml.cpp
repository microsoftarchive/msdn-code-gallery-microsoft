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
#include "Scenario6_SecondaryTileNotification.xaml.h"

using namespace SDKSample;

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
    rootPage = MainPage::Current;
    ToggleButtons(SecondaryTile::Exists(rootPage->dynamicTileId));
}

void SecondaryTileNotification::ToggleButtons(bool isEnabled)
{
    SendTileNotification->IsEnabled = isEnabled;
    SendBadgeNotification->IsEnabled = isEnabled;
    SendTileNotificationString->IsEnabled = isEnabled;
    SendBadgeNotificationString->IsEnabled = isEnabled;
}

void SDKSample::SecondaryTileNotification::PinLiveTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        // Prepare package images for use in the tile to be pinned.
        // The path provided is a relative path to the package
        auto square150x150Logo = ref new Uri("ms-appx:///Assets/square150x150Tile-sdk.png");
        auto wide310x150Logo = ref new Uri("ms-appx:///Assets/wide310x150Tile-sdk.png");

        Calendar^ cal = ref new Calendar();
        auto longtime = ref new DateTimeFormatter("longtime");
        DateTime time = cal->GetDateTime();

        // During creation of secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
        // These arguments should be meaningful to the application. In this sample, we will just pass a text as an argument.
        String^ tileActivationArguments = rootPage->dynamicTileId + " WasPinnedAt=" + longtime->Format(time);

        auto secondaryTile = ref new SecondaryTile(rootPage->dynamicTileId,
            "Live Secondary Tile", //short tile name (will be displayed on the tile)
            tileActivationArguments,
            square150x150Logo,
            TileSize::Wide310x150);

        // To have the larger tile size available the asset must be provided.
        secondaryTile->VisualElements->Wide310x150Logo = wide310x150Logo;

        // The tile background color is inherited from the parent unless a separate value is specified.
        // This operation will over-ride the default.
        secondaryTile->VisualElements->ForegroundText = ForegroundText::Light;

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

void SDKSample::SecondaryTileNotification::SendTileNotification_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        if (SecondaryTile::Exists(rootPage->dynamicTileId))
        {
            // Note: This sample contains an additional reference, NotificationsExtensions, which you can use in your apps
            auto tileContent = TileContentFactory::CreateTileWide310x150Text04();
            tileContent->TextBodyWrap->Text = "Sent to a secondary tile from NotificationsExtensions!";

            auto squareContent = TileContentFactory::CreateTileSquare150x150Text04();
            squareContent->TextBodyWrap->Text = "Sent to a secondary tile from NotificationsExtensions!";
            tileContent->Square150x150Content = squareContent;

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

void SDKSample::SecondaryTileNotification::SendBadgeNotification_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
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

void SDKSample::SecondaryTileNotification::SendBadgeNotificationWithStringManipulation_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
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


void SDKSample::SecondaryTileNotification::SendTileNotificationWithStringManipulation_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        auto tileXmlString = "<tile>"
                            + "<visual version='2'>"
                            + "<binding template='TileWide310x150Text04' fallback='TileWideText04'>"
                            + "<text id='1'>Send to a secondary tile from strings</text>"
                            + "</binding>"
                            + "<binding template='TileSquare150x150Text04' fallback='TileSquareText04'>"
                            + "<text id='1'>Send to a secondary tile from strings</text>"
                            + "</binding>"
                            + "</visual>"
                            + "</tile>";

        auto tileDOM = ref new Windows::Data::Xml::Dom::XmlDocument();
        tileDOM->LoadXml(tileXmlString);
        auto tile = ref new TileNotification(tileDOM);

        // Send the notification to the secondary tile by creating a secondary tile updater
        TileUpdateManager::CreateTileUpdaterForSecondaryTile(rootPage->dynamicTileId)->Update(tile);

        rootPage->NotifyUser("Tile Notification sent to " + rootPage->dynamicTileId + ".", NotifyType::StatusMessage);
    }
}
