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
// LanguageScenario.xaml.cpp
// Implementation of the LanguageScenario class
//

#include "pch.h"
#include "Language.xaml.h"

using namespace SDKSample::GlobalizationPreferencesSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::System::UserProfile;
using namespace Windows::Globalization;

LanguageScenario::LanguageScenario()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void LanguageScenario::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void LanguageScenario::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// This scenario uses the Windows.System.UserProfile.GlobalizationPreferences class to 
	// obtains the user's preferred language characteristics.
	auto userLanguage = ref new Windows::Globalization::Language(GlobalizationPreferences::Languages->GetAt(0));

	// This obtains the language characteristics by providing a BCP47 tag.
	auto exampleLanguage = ref new Windows::Globalization::Language("en-AU");

	// Generate the results
	String^ userLanguageCharacteristics = "User's Preferred Language\n" +
		"Display Name: " + userLanguage->DisplayName + "\n" +
		"Language Tag: "  + userLanguage->LanguageTag + "\n" +
		"Native Name: "  + userLanguage->NativeName + "\n" +
		"Script Code: "  + userLanguage->Script;

	String^ exampleLanguageCharacteristics = "Example Language by BCP47 tag (en-AU)\n" +
		"Display Name: " + exampleLanguage->DisplayName + "\n" +
		"Language Tag: "  + exampleLanguage->LanguageTag + "\n" +
		"Native Name: "  + exampleLanguage->NativeName + "\n" +
		"Script Code: "  + exampleLanguage->Script;

	String^ results = userLanguageCharacteristics + "\n\n" + exampleLanguageCharacteristics;

    // Display the results
	rootPage->NotifyUser(results, NotifyType::StatusMessage);
}
