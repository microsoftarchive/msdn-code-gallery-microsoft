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
// NotificationExpiration.xaml.cpp
// Implementation of the NotificationExpiration class
//

#include "pch.h"
#include "Scenario7_NotificationExpiration.xaml.h"

using namespace SDKSample::Tiles;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Notifications;
using namespace NotificationsExtensions::TileContent;

NotificationExpiration::NotificationExpiration()
{
    InitializeComponent();
}

void NotificationExpiration::OnNavigatedTo(NavigationEventArgs^ e)
{
    rootPage = MainPage::Current;
}

void NotificationExpiration::UpdateTileExpiring_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    int seconds = _wtoi(Time->Text->Data());
    if (seconds == 0)
    {
        seconds = 10;
    }

    Windows::Globalization::Calendar^ cal = ref new Windows::Globalization::Calendar();
    cal->SetToNow();
    cal->AddSeconds(seconds);

    auto longTime = ref new Windows::Globalization::DateTimeFormatting::DateTimeFormatter("longtime");
    DateTime time = cal->GetDateTime();
    Platform::String^ result = longTime->Format(time);

    // Create a notification for the Square310x310 tile using one of the available templates for the size.
    auto square310x310TileContent = TileContentFactory::CreateTileSquare310x310Text09();
    square310x310TileContent->TextHeadingWrap->Text = "This notification will expire at " + result;

    // Create a notification for the Wide310x150 tile using one of the available templates for the size.
    auto wide310x150TileContent = TileContentFactory::CreateTileWide310x150Text04();
    wide310x150TileContent->TextBodyWrap->Text = "This notification will expire at " + result;

    // Create a notification for the Square150x150 tile using one of the available templates for the size.
    auto square150x150TileContent = TileContentFactory::CreateTileSquare150x150Text04();
    square150x150TileContent->TextBodyWrap->Text = "This notification will expire at " + result;

    // Attach the Square150x150 template to the Wide310x150 template.
    wide310x150TileContent->Square150x150Content = square150x150TileContent;

    // Attach the Wide310x150 template to the Square310x310 template.
    square310x310TileContent->Wide310x150Content = wide310x150TileContent;

    auto tileNotification = square310x310TileContent->CreateNotification();

    // Set the expiration time.
    tileNotification->ExpirationTime = dynamic_cast<Platform::IBox<DateTime>^>(PropertyValue::CreateDateTime(time));

    TileUpdateManager::CreateTileUpdaterForApplication()->Update(tileNotification);
    rootPage->NotifyUser("Tile notification sent. It will expire at " + result, NotifyType::StatusMessage);
}