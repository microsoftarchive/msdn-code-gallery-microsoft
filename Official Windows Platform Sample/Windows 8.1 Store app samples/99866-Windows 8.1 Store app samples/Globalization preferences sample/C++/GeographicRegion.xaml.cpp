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
// GeographicRegionScenario.xaml.cpp
// Implementation of the GeographicRegionScenario class
//

#include "pch.h"
#include "GeographicRegion.xaml.h"
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

GeographicRegionScenario::GeographicRegionScenario()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void GeographicRegionScenario::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::GlobalizationPreferencesSample::GeographicRegionScenario::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// This scenario uses the Windows.System.UserProfile.GlobalizationPreferences class to 
	// obtain the user's preferred geographic region characteristics.
	auto userGeoRegion = ref new GeographicRegion();

	// This obtains the geographic region characteristics by providing a country or region code.
	auto exampleGeoRegion = ref new GeographicRegion("AU");

	// Generate the results
	String^ userGeoRegionCharacteristics = 
        "User's Preferred Geographic Region\n" +
		"Display Name: "      + userGeoRegion->DisplayName + "\n" +
		"Native Name: "       + userGeoRegion->NativeName + "\n" + 
        "Currencies in use: " + Utilities::VectorViewToString(userGeoRegion->CurrenciesInUse) + "\n" + 
		"Codes: "             + userGeoRegion->CodeTwoLetter + ", " + userGeoRegion->CodeThreeLetter + ", " + userGeoRegion->CodeThreeDigit;

	String^ exampleGeoRegionCharacteristics = 
        "Example Geographic Region by region/country code (AU)\n" +
		"Display Name: "      + exampleGeoRegion->DisplayName + "\n" +
		"Native Name: "       + exampleGeoRegion->NativeName + "\n" +
		"Currencies in use: " + Utilities::VectorViewToString(exampleGeoRegion->CurrenciesInUse) + "\n" +
		"Codes: "             + exampleGeoRegion->CodeTwoLetter + ", " + exampleGeoRegion->CodeThreeLetter + " , " + exampleGeoRegion->CodeThreeDigit;

	String^ results = userGeoRegionCharacteristics + "\n\n" + exampleGeoRegionCharacteristics;

    // Display the results
    rootPage->NotifyUser(results, NotifyType::StatusMessage);
}
