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
// Touch.xaml.cpp
// Implementation of the Touch class
//

#include "pch.h"
#include "Touch.xaml.h"

using namespace SDKSample::DeviceCaps;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Windows::Devices::Input;

Touch::Touch()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Touch::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::DeviceCaps::Touch::TouchGetSettings_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        TouchCapabilities^ pTouchCapabilities = ref new TouchCapabilities();
        Platform::String^ Buffer;

        Buffer = "There is " + (pTouchCapabilities->TouchPresent != 0 ? "a" : "no") + " digitizer present\n";
        Buffer += "The digitizer supports " + pTouchCapabilities->Contacts.ToString() + " contacts\n";

        TouchOutputTextBlock->Text = Buffer;
    }
}
