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
#include "ScenarioInput2.xaml.h"
#include "MainPage.xaml.h"

using namespace LockScreenAppsCPP;

using namespace NotificationsExtensions::BadgeContent;
using namespace NotificationsExtensions::TileContent;
using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::UI::Notifications;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;


ScenarioInput2::ScenarioInput2()
{
	InitializeComponent();
}

ScenarioInput2::~ScenarioInput2()
{
}

void ScenarioInput2::SendBadge_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	BadgeNumericNotificationContent^ badgeContent = ref new BadgeNumericNotificationContent(6);
	BadgeUpdateManager::CreateBadgeUpdaterForApplication()->Update(badgeContent->CreateNotification());

	rootPage->NotifyUser("Badge notification sent", NotifyType::StatusMessage);
}

void ScenarioInput2::SendBadgeWithStringManipulation_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Platform::String^ badgeXmlString = "<badge value='6'/>";
	auto badgeDOM = ref new Windows::Data::Xml::Dom::XmlDocument();
	badgeDOM->LoadXml(badgeXmlString);
	auto badge = ref new BadgeNotification(badgeDOM);
	BadgeUpdateManager::CreateBadgeUpdaterForApplication()->Update(badge);
	rootPage->NotifyUser("Badge notification sent", NotifyType::StatusMessage);
}

void ScenarioInput2::ClearBadge_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	BadgeUpdateManager::CreateBadgeUpdaterForApplication()->Clear();
	rootPage->NotifyUser("Badge notification cleared", NotifyType::StatusMessage);
}

void ScenarioInput2::SendTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{   
	ITileWideSmallImageAndText03^ tileContent = TileContentFactory::CreateTileWideSmallImageAndText03();
	tileContent->TextBodyWrap->Text = "This tile notification has an image, but it won't be displayed on the lock screen";
	tileContent->Image->Src = "ms-appx:///images/tile-sdk.png";
	tileContent->RequireSquareContent = false;

	TileUpdateManager::CreateTileUpdaterForApplication()->Update(tileContent->CreateNotification());

	rootPage->NotifyUser("Tile notification sent", NotifyType::StatusMessage);
}

void ScenarioInput2::SendTileWithStringManipulation_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{   
	auto tileXmlString = "<tile>"
		+  "<visual>"
		+   "<binding template='TileWideSmallImageAndText03'>"
		+    "<image id='1' src='ms-appx:///images/tile-sdk.png'/>"
		+    "<text id='1'>This tile notification has an image, but it won't be displayed on the lock screen</text>"
		+   "</binding>"
		+  "</visual>"
		+ "</tile>";

	auto tileDOM = ref new Windows::Data::Xml::Dom::XmlDocument();
	tileDOM->LoadXml(tileXmlString);
	auto tile = ref new TileNotification(tileDOM);
	TileUpdateManager::CreateTileUpdaterForApplication()->Update(tile);
	rootPage->NotifyUser("Tile notification sent", NotifyType::StatusMessage);
}

void ScenarioInput2::ClearTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	TileUpdateManager::CreateTileUpdaterForApplication()->Clear();
	rootPage->NotifyUser("Tile notification cleared", NotifyType::StatusMessage);
}

void ScenarioInput2::OnNavigatedTo(NavigationEventArgs^ e)
{
	// Get a pointer to our main page.
	rootPage = dynamic_cast<MainPage^>(e->Parameter);
}