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
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"

using namespace SDKSample::XAMLPopup;

using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Navigation;

Scenario2::Scenario2()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

// Handles the Click event of the Button demonstrating a parented Popup with input content
void SDKSample::XAMLPopup::Scenario2::ShowPopupWithParentClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if (!ParentedPopup->IsOpen) { ParentedPopup->IsOpen = true; }
}

// Handles the Click event of the Button demonstrating a non-parented Popup with input content
void SDKSample::XAMLPopup::Scenario2::ShowPopupWithoutParentClicked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// if we already have one showing, don't create another one
    if (nonParentPopup == nullptr)
    {
        // create the Popup in code
        nonParentPopup = ref new Popup();

        // we are creating this in code and need to handle multiple instances
        // so we are attaching to the Popup.Closed event to remove our reference
		nonParentPopup->Closed += ref new EventHandler<Platform::Object^>(this, &Scenario2::OnPopupClosed);
        nonParentPopup->HorizontalOffset = 200;
		auto windowBounds = Window::Current->Bounds;
		nonParentPopup->VerticalOffset = windowBounds.Height - 200;

        // set the content to our UserControl
        nonParentPopup->Child = ref new PopupInputContent();

        // open the Popup
        nonParentPopup->IsOpen = true;
    }
}

// Handles the closing of the popup created in code
void SDKSample::XAMLPopup::Scenario2::OnPopupClosed(Platform::Object^ sender, Platform::Object^ args)
{
	nonParentPopup = nullptr;
}