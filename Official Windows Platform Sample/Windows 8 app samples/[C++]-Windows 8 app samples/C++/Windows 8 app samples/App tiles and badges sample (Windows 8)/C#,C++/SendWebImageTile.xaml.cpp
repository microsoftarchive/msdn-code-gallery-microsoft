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
// SendWebImageTile.xaml.cpp
// Implementation of the SendWebImageTile class
//

#include "pch.h"
#include "SendWebImageTile.xaml.h"

using namespace SDKSample::Tiles;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Notifications;
using namespace NotificationsExtensions::TileContent;

SendWebImageTile::SendWebImageTile()
{
    InitializeComponent();
}

void SendWebImageTile::OnNavigatedTo(NavigationEventArgs^ e)
{
    rootPage = MainPage::Current;
}

void SendWebImageTile::UpdateTileWithWebImage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{ 
	// Note: This sample contains an additional project, NotificationsExtensions.
	// NotificationsExtensions exposes an object model for creating notifications, but you can also 
	// modify the strings directly. See UpdateTileWithWebImageWithStringManipulation_Click for an example

	// Create notification content based on a visual template.
	auto tileContent = TileContentFactory::CreateTileWideImageAndText01();
	tileContent->TextCaptionWrap->Text = "This tile notification uses web images";

	// !Important!
	// The Internet (Client) capability must be checked in the manifest in the Capabilities tab
	// to display web images in tiles (either the http:// or https:// protocols)

	tileContent->Image->Src = ImageUrl->Text;
	tileContent->Image->Alt = "Web image";

	// Users can resize tiles to square or wide.
	// Apps can choose to include only square assets (meaning the app's tile can never be wide), or
	// include both wide and square assets (the user can resize the tile to square or wide).
	// Apps cannot include only wide assets.

	// Apps that support being wide should include square tile notifications since users
	// determine the size of the tile.

	// Create square notification content based on a visual template.
	auto squareContent = TileContentFactory::CreateTileSquareImage();

	squareContent->Image->Src = ImageUrl->Text;
	squareContent->Image->Alt = "Web image";

	// include the square template.
	tileContent->SquareContent = squareContent;

	// Send the notification to the app's application tile
	TileUpdateManager::CreateTileUpdaterForApplication()->Update(tileContent->CreateNotification());

	OutputTextBlock->Text = tileContent->GetContent();
}

void SendWebImageTile::UpdateTileWithWebImageWithStringManipulation_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{ 
	// create a string with the tile template xml
	auto tileXmlString = "<tile>"
		+ "<visual>"
		+ "<binding template='TileWideImageAndText01'>"
		+ "<text id='1'>This tile notification uses web images</text>"
		+ "<image id='1' src='" + ImageUrl->Text + "'/>"
		+ "</binding>"
		+ "<binding template='TileSquareImage'>"
		+ "<image id='1' src='" + ImageUrl->Text + "'/>"
		+ "</binding>"
		+ "</visual>"
		+ "</tile>";

	try
	{
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
	catch (Platform::Exception^)
	{
		OutputTextBlock->Text = "Error loading the xml, check for invalid characters in the input";
	}
}

void SendWebImageTile::ClearTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// A TileUpdater is also used to clear the tile        
	TileUpdateManager::CreateTileUpdaterForApplication()->Clear();
	OutputTextBlock->Text = "Tile cleared";
}
