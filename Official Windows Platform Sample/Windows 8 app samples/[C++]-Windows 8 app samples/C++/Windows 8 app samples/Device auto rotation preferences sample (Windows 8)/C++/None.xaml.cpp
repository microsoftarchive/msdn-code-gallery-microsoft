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
// None.xaml.cpp
// Implementation of the None class
//

#include "pch.h"
#include "None.xaml.h"

using namespace SDKSample::Rotation;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

None::None()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void None::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::Rotation::None::ClearPreference_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        Windows::Graphics::Display::DisplayProperties::AutoRotationPreferences = Windows::Graphics::Display::DisplayOrientations::None;
        
        if (Windows::Graphics::Display::DisplayProperties::AutoRotationPreferences == Windows::Graphics::Display::DisplayOrientations::None)
        {
            rootPage->NotifyUser("Succeeded: All preferences cleared.", NotifyType::StatusMessage);
        }
        else
        {
            rootPage->NotifyUser("Error: Failed to set the preference.", NotifyType::StatusMessage);
        }
    }
}
