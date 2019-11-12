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
// EnableNotificationQueue.xaml.cpp
// Implementation of the EnableNotificationQueue class
//

#include "pch.h"
#include "EnableNotificationQueue.xaml.h"

using namespace SDKSample::Tiles;

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Notifications;
using namespace NotificationsExtensions::TileContent;

EnableNotificationQueue::EnableNotificationQueue()
{
    InitializeComponent();
}

void EnableNotificationQueue::OnNavigatedTo(NavigationEventArgs^ e)
{
    rootPage = MainPage::Current;
}

void EnableNotificationQueue::EnableNotificationQueue_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{		
	TileUpdateManager::CreateTileUpdaterForApplication()->EnableNotificationQueue(true);
	OutputTextBlock->Text = "Notification queue enabled";
}

void EnableNotificationQueue::DisableNotificationQueue_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{	
	TileUpdateManager::CreateTileUpdaterForApplication()->EnableNotificationQueue(false);
	OutputTextBlock->Text = "Notification queue disabled";
}

void EnableNotificationQueue::UpdateTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	String^ text = "TestTag01";
	if( TextContent->Text->IsEmpty())
	{
		text = TextContent->Text;
	}

	auto tileContent = TileContentFactory::CreateTileWideText03();
	tileContent->TextHeadingWrap->Text = text;
	auto squareContent = TileContentFactory::CreateTileSquareText04();
	squareContent->TextBodyWrap->Text = text;
	tileContent->SquareContent = squareContent;

	auto tile = tileContent->CreateNotification();

	String^ tag = "TestTag01";
	if ( !IdBox->Text->IsEmpty())
	{
		tag = IdBox->Text;
	}
		
	// set the tag on the notification	
	tile->Tag = tag;
	TileUpdateManager::CreateTileUpdaterForApplication()->Update(tile); 

	OutputTextBlock->Text = "Tile notification sent. It is tagged with " + tag;
}

void EnableNotificationQueue::ClearTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	TileUpdateManager::CreateTileUpdaterForApplication()->Clear();
	OutputTextBlock->Text = "Tile cleared";
}
