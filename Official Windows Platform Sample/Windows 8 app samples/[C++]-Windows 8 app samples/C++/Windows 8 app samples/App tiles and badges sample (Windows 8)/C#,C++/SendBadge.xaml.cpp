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
// SendBadge.xaml.cpp
// Implementation of the SendBadge class
//

#include "pch.h"
#include "SendBadge.xaml.h"

using namespace SDKSample::Tiles;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Notifications;
using namespace NotificationsExtensions::BadgeContent;

SendBadge::SendBadge()
{
	InitializeComponent();
}

void SendBadge::OnNavigatedTo(NavigationEventArgs^ e)
{
	rootPage = MainPage::Current;
	NumberOrGlyph->SelectedIndex = 0;
	GlyphList->SelectedIndex = 0;    
}

void SendBadge::NumberOrGlyph_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e)
{
	if (NumberOrGlyph->SelectedIndex == 0)
	{
		NumberPanel->Visibility = Windows::UI::Xaml::Visibility::Visible;
		GlyphPanel->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	}
	else
	{
		NumberPanel->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		GlyphPanel->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}
}

void SendBadge::UpdateBadge_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{ 
	bool useStrings = false;
	if (sender == UpdateBadgeWithStringManipulation)
	{
		useStrings = true;
	}

	if (NumberOrGlyph->SelectedIndex == 0)
	{
		int number = _wtoi(NumberInput->Text->Data());
		if (useStrings)
		{
			UpdateBadgeWithNumberWithStringManipulation(number);
		}
		else
		{
			UpdateBadgeWithNumber(number);
		}
	}
	else
	{		
		if (useStrings)
		{
			UpdateBadgeWithGlyphWithStringManipulation();
		}
		else
		{
			UpdateBadgeWithGlyph(GlyphList->SelectedIndex);
		}
	}
}


void SendBadge::ClearBadge_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e) 
{	
	BadgeUpdateManager::CreateBadgeUpdaterForApplication()->Clear();
	OutputTextBlock->Text = "Badge cleared";
}

void SendBadge::UpdateBadgeWithNumber(int number)
{		
	// Note: usually this would be created with new BadgeGlyphNotificationContent(GlyphValue.Alert) or any of the values of GlyphValue
	auto badgeContent = ref new BadgeNumericNotificationContent(number);

	// send the notification to the app's application tile
	BadgeUpdateManager::CreateBadgeUpdaterForApplication()->Update(badgeContent->CreateNotification());

	OutputTextBlock->Text = badgeContent->GetContent();
}

void SendBadge::UpdateBadgeWithGlyph(int glyphIndex)
{	
	auto badgeContent = ref new BadgeGlyphNotificationContent((GlyphValue)glyphIndex);

	// send the notification to the app's application tile
	BadgeUpdateManager::CreateBadgeUpdaterForApplication()->Update(badgeContent->CreateNotification());

	OutputTextBlock->Text = badgeContent->GetContent();
}

void SendBadge::UpdateBadgeWithNumberWithStringManipulation(int number)
{		
	// create a string with the badge template xml
	auto badgeXmlString = "<badge value='" + number + "'/>";
	try
	{
		// create a DOM
		auto badgeDOM = ref new Windows::Data::Xml::Dom::XmlDocument();

		// load the xml string into the DOM, catching any invalid xml characters 
		badgeDOM->LoadXml(badgeXmlString);

		// create a badge notification
		auto badge = ref new BadgeNotification(badgeDOM);

		// Send the notification to the app's application tile
		BadgeUpdateManager::CreateBadgeUpdaterForApplication()->Update(badge);

		OutputTextBlock->Text = badgeDOM->GetXml();
	}
	catch (Platform::Exception^)
	{
		OutputTextBlock->Text = "Error loading the xml, check for invalid characters in the input";
	}
}

void SendBadge::UpdateBadgeWithGlyphWithStringManipulation()
{
	// create a string with the badge template xml
	auto badgeXmlString = "<badge value='" + safe_cast<ComboBoxItem^>(GlyphList->SelectedItem)->Content->ToString() + "'/>";
	try
	{
		// create a DOM
		auto badgeDOM = ref new Windows::Data::Xml::Dom::XmlDocument();

		// load the xml string into the DOM, catching any invalid xml characters 
		badgeDOM->LoadXml(badgeXmlString);

		// create a badge notification
		auto badge = ref new BadgeNotification(badgeDOM);

		// Send the notification to the app's application tile
		BadgeUpdateManager::CreateBadgeUpdaterForApplication()->Update(badge);

		OutputTextBlock->Text = badgeDOM->GetXml();
	}
	catch (Platform::Exception^)
	{
		OutputTextBlock->Text = "Error loading the xml, check for invalid characters in the input";
	}
}
