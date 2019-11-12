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
// Mouse.xaml.cpp
// Implementation of the Mouse class
//

#include "pch.h"
#include "Mouse.xaml.h"

using namespace SDKSample::DeviceCaps;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Windows::Devices::Input;

Mouse::Mouse()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Mouse::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::DeviceCaps::Mouse::MouseGetSettings_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        MouseCapabilities^ pMouseCapabilities = ref new MouseCapabilities();
        Platform::String^ Buffer;

        Buffer = "There is " + (pMouseCapabilities->MousePresent != 0 ? "a" : "no") + " mouse present\n";
        Buffer += "There is " + (pMouseCapabilities->VerticalWheelPresent != 0 ? "a" : "no") + " vertical mouse wheel present\n";
        Buffer += "There is " + (pMouseCapabilities->HorizontalWheelPresent != 0 ? "a" : "no") + " horizontal mouse wheel present\n";
        Buffer += "The user has " + (pMouseCapabilities->SwapButtons != 0 ? "" : "not ") + "opted to swap the mouse buttons\n";
        Buffer += "The mouse has " + pMouseCapabilities->NumberOfButtons.ToString() + " button(s)\n";

        MouseOutputTextBlock->Text = Buffer;
    }
}
