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
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"

using namespace SDKSample::XAMLPopup;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario1::Scenario1()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    rootPage = MainPage::Current;
}

// Handles the Click event on the Button within the simple Popup control and simply opens it.
void SDKSample::XAMLPopup::Scenario1::ShowPopupOffsetClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // open the Popup if it isn't open already
	if (!StandardPopup->IsOpen) { StandardPopup->IsOpen = true; }
}

// Handles the Click event on the Button within the simple Popup control and simply closes it.
void SDKSample::XAMLPopup::Scenario1::ClosePopupClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // open the Popup if it isn't open already
	if (StandardPopup->IsOpen) { StandardPopup->IsOpen = false; }
}
