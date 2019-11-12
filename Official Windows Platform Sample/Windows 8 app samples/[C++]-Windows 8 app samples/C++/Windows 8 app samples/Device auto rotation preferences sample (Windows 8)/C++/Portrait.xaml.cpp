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
// Portrait.xaml.cpp
// Implementation of the Portrait class
//

#include "pch.h"
#include "Portrait.xaml.h"

using namespace SDKSample::Rotation;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Portrait::Portrait()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Portrait::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::Rotation::Portrait::SetPortrait_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        Windows::Graphics::Display::DisplayProperties::AutoRotationPreferences = Windows::Graphics::Display::DisplayOrientations::Portrait;
        
        if (Windows::Graphics::Display::DisplayProperties::AutoRotationPreferences == Windows::Graphics::Display::DisplayOrientations::Portrait)
        {
            rootPage->NotifyUser("Succeeded: Preference set to Portrait.", NotifyType::StatusMessage);
        }
        else
        {
            rootPage->NotifyUser("Error: Failed to set the preference.", NotifyType::StatusMessage);
        }
    }
}
