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
// Keyboard.xaml.cpp
// Implementation of the Keyboard class
//

#include "pch.h"
#include "Keyboard.xaml.h"

using namespace SDKSample::DeviceCaps;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Windows::Devices::Input;

Keyboard::Keyboard()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Keyboard::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::DeviceCaps::Keyboard::KeyboardGetSettings_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
		KeyboardCapabilities^ pKeyboardCapabilities = ref new KeyboardCapabilities();
        Platform::String^ Buffer;

        Buffer = "There is " + ((pKeyboardCapabilities->KeyboardPresent != 0) ? "a" : "no") + " keyboard present\n";

        KeyboardOutputTextBlock->Text = Buffer;
    }
}
