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
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3.xaml.h"

using namespace SDKSample::XAMLPopup;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario3::Scenario3()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario3::OnNavigatedTo(NavigationEventArgs^ e)
{
    rootPage = MainPage::Current;
}

// handles the Click event of the Button for showing the light dismiss behavior
void SDKSample::XAMLPopup::Scenario3::ShowPopupLightDismissClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if (!LightDismissSimplePopup->IsOpen) { LightDismissSimplePopup->IsOpen = true; }
}

// handles the Click event of the Button for showing the light dismiss with animations behavior
void SDKSample::XAMLPopup::Scenario3::ShowPopupAnimationClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if (!LightDismissAnimatedPopup->IsOpen) { LightDismissAnimatedPopup->IsOpen = true; }
}

// handles the Click event of the Button for showing the light dismiss with settings behavior
void SDKSample::XAMLPopup::Scenario3::ShowPopupSettingsClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if (!SettingsAnimatedPopup->IsOpen) 
    {
		/* The UI guidelines for a proper 'Settings' flyout are such that it should fill the height of the 
		 current Window and be either narrow (346px) or wide (646px)
		 Using the measurements of the Window::Curent::Bounds will help you position correctly.
		 This sample here shows a simple *example* of this using the Width to get the HorizontalOffset but
		 the app developer will have to perform these measurements depending on the structure of the app's 
		 views in their code*/
        RootPopupBorder->Width = 646;
		auto windowBounds = Window::Current->Bounds;
		SettingsAnimatedPopup->HorizontalOffset = windowBounds.Width- 646;

        SettingsAnimatedPopup->IsOpen = true; 
    }
}

// Handles the Click event on the Button within the simple Popup control and simply closes it.
void SDKSample::XAMLPopup::Scenario3::ClosePopupClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// if the Popup is open, then close it
	if (LightDismissSimplePopup->IsOpen) { LightDismissSimplePopup->IsOpen = false; }
}

// Handles the Click event on the Button within the simple Popup control and simply closes it.
void SDKSample::XAMLPopup::Scenario3::CloseAnimatedPopupClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if (LightDismissAnimatedPopup->IsOpen) { LightDismissAnimatedPopup->IsOpen = false; }
}

// Handles the Click event on the Button within the simple Popup control and simply closes it.
void SDKSample::XAMLPopup::Scenario3::CloseSettingsPopupClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if (SettingsAnimatedPopup->IsOpen) { SettingsAnimatedPopup->IsOpen = false; }
}
