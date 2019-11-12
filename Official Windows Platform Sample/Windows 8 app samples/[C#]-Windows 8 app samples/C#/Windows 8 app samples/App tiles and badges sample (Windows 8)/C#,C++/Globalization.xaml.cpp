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
// Globalization.xaml.cpp
// Implementation of the Globalization class
//

#include "pch.h"
#include "Globalization.xaml.h"

using namespace SDKSample::Tiles;

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Notifications;
using namespace NotificationsExtensions::TileContent;
using namespace Windows::ApplicationModel::Resources::Core;

Globalization::Globalization()
{
    InitializeComponent();
}

void Globalization::OnNavigatedTo(NavigationEventArgs^ e)
{
    rootPage = MainPage::Current;
}

void Globalization::ViewCurrentResources_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{	
	String^ language = ResourceManager::Current->DefaultContext->QualifierValues->Lookup("Language");

	String^ scale = ResourceManager::Current->DefaultContext->QualifierValues->Lookup("Scale");

	String^ contrast = ResourceManager::Current->DefaultContext->QualifierValues->Lookup("Contrast");

	OutputTextBlock->Text = "Your system is currently set to the following values: Application Language: " + language + ", Scale: " + scale + ", Contrast: " + contrast + ". If using web images and AddImageQuery, the following query string would be appened to the URL: ?ms-lang=" + language + "&ms-scale=" + scale + "&ms-contrast=" + contrast;
}

void Globalization::SendTileNotificationWithQueryStrings_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{	
	auto tileContent = TileContentFactory::CreateTileWideImageAndText01();
	tileContent->TextCaptionWrap->Text = "This tile notification uses web images";

	tileContent->Image->Src = ImageUrl->Text;
	tileContent->Image->Alt = "Web image";

	// enable AddImageQuery on the notification
    tileContent->AddImageQuery = true;

	auto squareContent = TileContentFactory::CreateTileSquareImage();

	squareContent->Image->Src = ImageUrl->Text;
	squareContent->Image->Alt = "Web image";

	// include the square template.
	tileContent->SquareContent = squareContent;

	// Send the notification to the app's application tile
	TileUpdateManager::CreateTileUpdaterForApplication()->Update(tileContent->CreateNotification());

	OutputTextBlock->Text = tileContent->GetContent();
}

void Globalization::SendTileNotificationImage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{	
	String^ scale = ResourceManager::Current->DefaultContext->QualifierValues->Lookup("Scale");

	auto tileContent = TileContentFactory::CreateTileWideSmallImageAndText03(); 

	tileContent->TextBodyWrap->Text = "graySquare.png in the xml is actually graySquare.scale-" + scale + ".png";
	tileContent->Image->Src = "ms-appx:///graySquare.png";
	tileContent->Image->Alt = "Gray square";

	auto squaretileContent = TileContentFactory::CreateTileSquareImage();
	squaretileContent->Image->Src = "ms-appx:///graySquare.png";
	squaretileContent->Image->Alt = "Gray square";
	tileContent->SquareContent = squaretileContent;

	TileUpdateManager::CreateTileUpdaterForApplication()->Update(tileContent->CreateNotification());

	OutputTextBlock->Text = tileContent->GetContent();
}

void Globalization::SendTileNotificationText_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{	
	auto tileContent = TileContentFactory::CreateTileWideText03();
	// check out /en-US/resources.resw to understand where this string will come from
	tileContent->TextHeadingWrap->Text = "ms-resource:greeting";

	auto squareContent = TileContentFactory::CreateTileSquareText04();
	// check out /en-US/resources.resw to understand where this string will come from 
	squareContent->TextBodyWrap->Text = "ms-resource:greeting";
	tileContent->SquareContent = squareContent;

	TileUpdateManager::CreateTileUpdaterForApplication()->Update(tileContent->CreateNotification());

	OutputTextBlock->Text = tileContent->GetContent();
}
