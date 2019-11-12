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
#include "Scenario4_SendBadge.xaml.h"

using namespace SDKSample::Tiles;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Notifications;
using namespace NotificationsExtensions::BadgeContent;

SendBadge::SendBadge()
{
    InitializeComponent();

#if WINAPI_FAMILY == WINAPI_FAMILY_PHONE_APP
    // Some glyphes are not available on WindowsPhone
    ((ComboBoxItem^)GlyphList->Items->GetAt((int)GlyphValue::Activity))->IsEnabled = false;
    ((ComboBoxItem^)GlyphList->Items->GetAt((int)GlyphValue::Available))->IsEnabled = false;
    ((ComboBoxItem^)GlyphList->Items->GetAt((int)GlyphValue::Away))->IsEnabled = false;
    ((ComboBoxItem^)GlyphList->Items->GetAt((int)GlyphValue::Busy))->IsEnabled = false;
    ((ComboBoxItem^)GlyphList->Items->GetAt((int)GlyphValue::NewMessage))->IsEnabled = false;
    ((ComboBoxItem^)GlyphList->Items->GetAt((int)GlyphValue::Paused))->IsEnabled = false;
    ((ComboBoxItem^)GlyphList->Items->GetAt((int)GlyphValue::Playing))->IsEnabled = false;
    ((ComboBoxItem^)GlyphList->Items->GetAt((int)GlyphValue::Unavailable))->IsEnabled = false;
    ((ComboBoxItem^)GlyphList->Items->GetAt((int)GlyphValue::Error))->IsEnabled = false;
    ((ComboBoxItem^)GlyphList->Items->GetAt((int)GlyphValue::Alarm))->IsEnabled = false;
#endif
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
    if ((Button^)sender == UpdateBadgeWithStringManipulation)
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
    OutputTextBlock->Text = "";
    rootPage->NotifyUser("Badge cleared", NotifyType::StatusMessage);
}

void SendBadge::UpdateBadgeWithNumber(int number)
{
    // Note: This sample contains an additional project, NotificationsExtensions.
    // NotificationsExtensions exposes an object model for creating notifications, but you can also modify the xml
    // of the notification directly. See the additional function UpdateBadgeWithNumberWithStringManipulation to see how to do it
    // by modifying strings directly.

    auto badgeContent = ref new BadgeNumericNotificationContent(number);

    // Send the notification to the application’s tile.
    BadgeUpdateManager::CreateBadgeUpdaterForApplication()->Update(badgeContent->CreateNotification());

    OutputTextBlock->Text = badgeContent->GetContent();
    rootPage->NotifyUser("Badge sent", NotifyType::StatusMessage);
}

void SendBadge::UpdateBadgeWithGlyph(int glyphIndex)
{
    // Note: This sample contains an additional project, NotificationsExtensions.
    // NotificationsExtensions exposes an object model for creating notifications, but you can also modify the xml
    // of the notification directly. See the additional function UpdateBadgeWithGlyphWithStringManipulation to see how to do it
    // by modifying strings directly.

    // Note: usually this would be created with new BadgeGlyphNotificationContent(GlyphValue.Alert) or any of the values of GlyphValue.
    auto badgeContent = ref new BadgeGlyphNotificationContent((GlyphValue)glyphIndex);

    // Send the notification to the application’s tile
    BadgeUpdateManager::CreateBadgeUpdaterForApplication()->Update(badgeContent->CreateNotification());

    OutputTextBlock->Text = badgeContent->GetContent();
    rootPage->NotifyUser("Badge sent", NotifyType::StatusMessage);
}

void SendBadge::UpdateBadgeWithNumberWithStringManipulation(int number)
{
    // Create a string with the badge template xml.
    auto badgeXmlString = "<badge value='" + number + "'/>";
    try
    {
        // Create a DOM.
        auto badgeDOM = ref new Windows::Data::Xml::Dom::XmlDocument();

        // Load the xml string into the DOM, catching any invalid xml characters.
        badgeDOM->LoadXml(badgeXmlString);

        // Create a badge notification.
        auto badge = ref new BadgeNotification(badgeDOM);

        // Send the notification to the app's application tile
        BadgeUpdateManager::CreateBadgeUpdaterForApplication()->Update(badge);

        OutputTextBlock->Text = badgeDOM->GetXml();
        rootPage->NotifyUser("Badge sent", NotifyType::StatusMessage);
    }
    catch (Platform::Exception^)
    {
        OutputTextBlock->Text = "";
        rootPage->NotifyUser("Error loading the xml, check for invalid characters in the input", NotifyType::ErrorMessage);
    }
}

void SendBadge::UpdateBadgeWithGlyphWithStringManipulation()
{
    // Create a string with the badge template xml.
    auto badgeXmlString = "<badge value='" + safe_cast<ComboBoxItem^>(GlyphList->SelectedItem)->Content->ToString() + "'/>";
    try
    {
        // Create a DOM.
        auto badgeDOM = ref new Windows::Data::Xml::Dom::XmlDocument();

        // Load the xml string into the DOM, catching any invalid xml characters.
        badgeDOM->LoadXml(badgeXmlString);

        // Create a badge notification.
        auto badge = ref new BadgeNotification(badgeDOM);

        // Send the notification to the application’s tile.
        BadgeUpdateManager::CreateBadgeUpdaterForApplication()->Update(badge);

        OutputTextBlock->Text = badgeDOM->GetXml();
        rootPage->NotifyUser("Badge sent", NotifyType::StatusMessage);
    }
    catch (Platform::Exception^)
    {
        OutputTextBlock->Text = "";
        rootPage->NotifyUser("Error loading the xml, check for invalid characters in the input", NotifyType::ErrorMessage);
    }
}
