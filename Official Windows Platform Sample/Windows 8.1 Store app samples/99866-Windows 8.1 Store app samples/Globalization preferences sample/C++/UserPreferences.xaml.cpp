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
// UserPreferences.xaml.cpp
// Implementation of the UserPreferences class
//

#include "pch.h"
#include "UserPreferences.xaml.h"
#include "Utilities.h"

using namespace SDKSample::GlobalizationPreferencesSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::System::UserProfile;
using namespace Windows::Globalization;

UserPreferences::UserPreferences()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void UserPreferences::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void UserPreferences::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // This scenario uses the Windows.System.UserProfile.GlobalizationPreferences class to
    // obtain the user's globalization preferences.

    // Generate the list of languages


    // Generate the results by obtaining the user preferences.
    String^ results = 
        "Languages: "                + Utilities::VectorViewToString(GlobalizationPreferences::Languages) + "\n" +
		"Home Region: "              + GlobalizationPreferences::HomeGeographicRegion + "\n" + 
		"Calendar System: "          + Utilities::VectorViewToString(GlobalizationPreferences::Calendars) + "\n"+
		"Clock: "                    + Utilities::VectorViewToString(GlobalizationPreferences::Clocks) + "\n" + 
        "First Day of the Week: "    + ((int)GlobalizationPreferences::WeekStartsOn).ToString();

    // Display the results
    rootPage->NotifyUser(results, NotifyType::StatusMessage);
}
