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
// SendTextTile.xaml.cpp
// Implementation of the SendTextTile class
//

#include "pch.h"
#include "SendTextTile.xaml.h"

using namespace SDKSample::Tiles;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Notifications;
using namespace NotificationsExtensions::TileContent;

SendTextTile::SendTextTile()
{
	InitializeComponent();
}

void SendTextTile::OnNavigatedTo(NavigationEventArgs^ e)
{
	rootPage = MainPage::Current;
}

void SendTextTile::UpdateTileWithText_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{		
	// Note: This sample contains an additional project, NotificationsExtensions.
	// NotificationsExtensions exposes an object model for creating notifications, but you can also 
	// modify the strings directly. See UpdateTileWithTextWithStringManipulation_Click for an example

	// create the wide template
	auto tileContent = TileContentFactory::CreateTileWideText03();
	tileContent->TextHeadingWrap->Text = "Hello World! My very own tile notification";

	// Users can resize tiles to square or wide.
	// Apps can choose to include only square assets (meaning the app's tile can never be wide), or
	// include both wide and square assets (the user can resize the tile to square or wide).
	// Apps cannot include only wide assets.

	// Apps that support being wide should include square tile notifications since users
	// determine the size of the tile.

	// create the square template and attach it to the wide template
	auto squareContent = TileContentFactory::CreateTileSquareText04();
	squareContent->TextBodyWrap->Text = "Hello World! My very own tile notification"; 
	tileContent->SquareContent = squareContent;

	// Send the notification to the app's application tile
	TileUpdateManager::CreateTileUpdaterForApplication()->Update(tileContent->CreateNotification());

	OutputTextBlock->Text = tileContent->GetContent();
}

void SendTextTile::UpdateTileWithTextWithStringManipulation_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{		
	// create a string with the tile template xml
	auto tileXmlString = "<tile>"
		+ "<visual>"
		+ "<binding template='TileWideText03'>"
		+ "<text id='1'>Hello World! My very own tile notification</text>"
		+ "</binding>"
		+ "<binding template='TileSquareText04'>"
		+ "<text id='1'>Hello World! My very own tile notification</text>"
		+ "</binding>"
		+ "</visual>"
		+ "</tile>";

	// create a DOM
	auto tileDOM = ref new Windows::Data::Xml::Dom::XmlDocument();

	// load the xml string into the DOM, catching any invalid xml characters 
	tileDOM->LoadXml(tileXmlString);

	// create a tile notification
	auto tile = ref new TileNotification(tileDOM);

	// Send the notification to the app's application tile
	TileUpdateManager::CreateTileUpdaterForApplication()->Update(tile);

	OutputTextBlock->Text = tileDOM->GetXml();
}

void SendTextTile::ClearTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// A TileUpdater is also used to clear the tile        
	TileUpdateManager::CreateTileUpdaterForApplication()->Clear();
	OutputTextBlock->Text = "Tile cleared";
}
