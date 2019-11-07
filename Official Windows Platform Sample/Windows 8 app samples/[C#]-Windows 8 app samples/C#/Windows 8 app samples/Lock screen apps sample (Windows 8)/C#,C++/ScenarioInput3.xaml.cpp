// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario2Input.xaml.cpp
// Implementation of the Scenario2Input class.
//

#include "pch.h"
#include "ScenarioInput3.xaml.h"
#include "MainPage.xaml.h"

using namespace LockScreenAppsCPP;

using namespace Concurrency;
using namespace NotificationsExtensions::BadgeContent;
using namespace NotificationsExtensions::TileContent;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::UI::Notifications;
using namespace Windows::UI::Popups;
using namespace Windows::UI::StartScreen;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

#define BADGE_TILE_ID "ST_BADGE"
#define TEXT_TILE_ID "ST_BADGE_AND_TEXT"

ScenarioInput3::ScenarioInput3()
{
    InitializeComponent();
    CreateBadgeTile->Click += ref new RoutedEventHandler(this, &ScenarioInput3::CreateBadgeTile_Click);
    CreateBadgeAndTextTile->Click += ref new RoutedEventHandler(this, &ScenarioInput3::CreateBadgeAndTextTile_Click);
}

ScenarioInput3::~ScenarioInput3()
{
}

void ScenarioInput3::CreateBadgeTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	 if (!SecondaryTile::Exists(BADGE_TILE_ID)) {
        auto secondTile = ref new SecondaryTile(
            BADGE_TILE_ID,
            "LockBadge",
            "LockScreen CPP - Badge only",
            "BADGE_ARGS",
            TileOptions::ShowNameOnLogo,
            ref new Uri("ms-appx:///images/squareTile-sdk.png")
        );
        secondTile->LockScreenBadgeLogo = ref new Uri("ms-appx:///images/badgelogo-sdk.png");

        create_task(secondTile->RequestCreateForSelectionAsync(GetElementRect(dynamic_cast<FrameworkElement^>(sender)), Placement::Above)).then([this] (bool isPinned) {
			if (isPinned)
			{
				auto badgeContent = ref new BadgeNumericNotificationContent(2);
				BadgeUpdateManager::CreateBadgeUpdaterForSecondaryTile(BADGE_TILE_ID)->Update(badgeContent->CreateNotification());
				rootPage->NotifyUser("Secondary tile created and badge updated. Go to PC settings to add it to the lock screen.", NotifyType::StatusMessage);
			}
			else 
			{
				rootPage->NotifyUser("Tile not created.", NotifyType::ErrorMessage);
			}
		});
                
    } else {
        rootPage->NotifyUser("Badge secondary tile already exists.", NotifyType::ErrorMessage);
    }
}


void ScenarioInput3::CreateBadgeAndTextTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    if (!SecondaryTile::Exists(TEXT_TILE_ID))
    {
        auto secondTile = ref new SecondaryTile(
            TEXT_TILE_ID,
            "LockText",
            "LockScreen CPP - Badge and tile text",
            "TEXT_ARGS",
            TileOptions::ShowNameOnLogo | TileOptions::ShowNameOnWideLogo,
            ref new Uri("ms-appx:///images/squareTile-sdk.png"),
            ref new Uri("ms-appx:///images/tile-sdk.png")
        );
        secondTile->LockScreenBadgeLogo = ref new Uri("ms-appx:///images/badgelogo-sdk.png");
        secondTile->LockScreenDisplayBadgeAndTileText = true;

        create_task(secondTile->RequestCreateForSelectionAsync(GetElementRect(dynamic_cast<FrameworkElement^>(sender)), Placement::Above)).then([this] (bool isPinned) {
			if (isPinned)
			{
				auto tileContent = TileContentFactory::CreateTileWideText03();
				tileContent->TextHeadingWrap->Text = "Text for the lock screen";
				tileContent->RequireSquareContent = false;
				TileUpdateManager::CreateTileUpdaterForSecondaryTile(TEXT_TILE_ID)->Update(tileContent->CreateNotification());
				rootPage->NotifyUser("Secondary tile created and updated. Go to PC settings to add it to the lock screen.", NotifyType::StatusMessage);
			}
			else
			{
				rootPage->NotifyUser("Tile not created.", NotifyType::ErrorMessage);
			}
		});

    }
    else
    {
        rootPage->NotifyUser("Badge and text secondary tile already exists.", NotifyType::ErrorMessage);
    }
}

Rect ScenarioInput3::GetElementRect(FrameworkElement^ element)
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

void ScenarioInput3::OnNavigatedTo(NavigationEventArgs^ e)
{
    // Get a pointer to our main page.
    rootPage = dynamic_cast<MainPage^>(e->Parameter);
}