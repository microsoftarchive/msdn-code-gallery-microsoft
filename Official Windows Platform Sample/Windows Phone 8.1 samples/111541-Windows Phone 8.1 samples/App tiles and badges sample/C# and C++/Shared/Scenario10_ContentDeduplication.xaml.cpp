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
#include "Scenario10_ContentDeduplication.xaml.h"

using namespace SDKSample::Tiles;

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Notifications;
using namespace NotificationsExtensions::TileContent;

ContentDeduplication::ContentDeduplication()
{
    InitializeComponent();
}

void ContentDeduplication::OnNavigatedTo(NavigationEventArgs^ e)
{
    rootPage = MainPage::Current;
}

void ContentDeduplication::EnableNotificationQueue_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Enable the notification queue - this only needs to be called once in the lifetime of your app.
    // Note that the default is false.
    TileUpdateManager::CreateTileUpdaterForApplication()->EnableNotificationQueue(true);
    rootPage->NotifyUser("Notification cycling enabled for all tile sizes.", NotifyType::StatusMessage);
}

void ContentDeduplication::ClearTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    TileUpdateManager::CreateTileUpdaterForApplication()->Clear();
    rootPage->NotifyUser("Tile cleared.", NotifyType::StatusMessage);
}

void ContentDeduplication::SendNotifications_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Create a notification for the first set of stories with bindings for all 3 tile sizes
    auto square310x310TileContent1 = TileContentFactory::CreateTileSquare310x310Text09();
    square310x310TileContent1->TextHeadingWrap->Text = "Main Story";

    auto wide310x150TileContent1 = TileContentFactory::CreateTileWide310x150Text03();
    wide310x150TileContent1->TextHeadingWrap->Text = "Main Story";

    auto square150x150TileContent1 = TileContentFactory::CreateTileSquare150x150Text04();
    square150x150TileContent1->TextBodyWrap->Text = "Main Story";

    wide310x150TileContent1->Square150x150Content = square150x150TileContent1;
    square310x310TileContent1->Wide310x150Content = wide310x150TileContent1;

    // Set the contentId on the Square310x310 tile
    square310x310TileContent1->ContentId = "Main_1";

    // Tag the notification and send it to the tile
    auto tileNotification = square310x310TileContent1->CreateNotification();
    tileNotification->Tag = "1";
    TileUpdateManager::CreateTileUpdaterForApplication()->Update(tileNotification);

    // Create the first notification for the second set of stories with binding for all 3 tiles sizes
    auto square310x310TileContent2 = TileContentFactory::CreateTileSquare310x310TextList03();
    square310x310TileContent2->TextHeading1->Text = "Additional Story 1";
    square310x310TileContent2->TextHeading2->Text = "Additional Story 2";
    square310x310TileContent2->TextHeading3->Text = "Additional Story 3";

    auto wide310x150TileContent2 = TileContentFactory::CreateTileWide310x150Text03();
    wide310x150TileContent2->TextHeadingWrap->Text = "Additional Story 1";

    auto square150x150TileContent2 = TileContentFactory::CreateTileSquare150x150Text04();
    square150x150TileContent2->TextBodyWrap->Text = "Additional Story 1";

    wide310x150TileContent2->Square150x150Content = square150x150TileContent2;
    square310x310TileContent2->Wide310x150Content = wide310x150TileContent2;

    // Set the contentId on the Square310x310 tile
    square310x310TileContent2->ContentId = "Additional_1";

    // Tag the notification and send it to the tile
    tileNotification = square310x310TileContent2->CreateNotification();
    tileNotification->Tag = "2";
    TileUpdateManager::CreateTileUpdaterForApplication()->Update(tileNotification);

    // Create the second notification for the second set of stories with binding for all 3 tiles sizes
    // Notice that we only replace the Wide310x150 and Square150x150 binding elements,
    // and keep the Square310x310 content the same - this will cause the Square310x310 to be ignored for this notification,
    // since the contentId for this size is the same as in the first notification of the second set of stories.
    auto wide310x150TileContent3 = TileContentFactory::CreateTileWide310x150Text03();
    wide310x150TileContent3->TextHeadingWrap->Text = "Additional Story 2";

    auto square150x150TileContent3 = TileContentFactory::CreateTileSquare150x150Text04();
    square150x150TileContent3->TextBodyWrap->Text = "Additional Story 2";

    wide310x150TileContent3->Square150x150Content = square150x150TileContent3;
    square310x310TileContent2->Wide310x150Content = wide310x150TileContent3;

    // Tag the notification and send it to the tile
    tileNotification = square310x310TileContent2->CreateNotification();
    tileNotification->Tag = "3";
    TileUpdateManager::CreateTileUpdaterForApplication()->Update(tileNotification);

    // Create the third notification for the second set of stories with binding for all 3 tiles sizes
    // Notice that we only replace the Wide310x150 and Square150x150 binding elements,
    // and keep the Square310x310 content the same again - this will cause the Square310x310 to be ignored for this notification,
    // since the contentId for this size is the same as in the first notification of the second set of stories.
    auto wide310x150TileContent4 = TileContentFactory::CreateTileWide310x150Text03();
    wide310x150TileContent4->TextHeadingWrap->Text = "Additional Story 3";

    auto square150x150TileContent4 = TileContentFactory::CreateTileSquare150x150Text04();
    square150x150TileContent4->TextBodyWrap->Text = "Additional Story 3";

    wide310x150TileContent4->Square150x150Content = square150x150TileContent4;
    square310x310TileContent2->Wide310x150Content = wide310x150TileContent4;

    // Tag the notification and send it to the tile
    tileNotification = square310x310TileContent2->CreateNotification();
    tileNotification->Tag = "4";
    TileUpdateManager::CreateTileUpdaterForApplication()->Update(tileNotification);

    rootPage->NotifyUser("Four notifications sent.", NotifyType::StatusMessage);
}