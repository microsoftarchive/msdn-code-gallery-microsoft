//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario6.xaml.cpp
// Implementation of the Scenario6 class
//

#include "pch.h"
#include "Scenario6.xaml.h"

using namespace SDKSample;
using namespace SDKSample::RequestedThemeCPP;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

Scenario6::Scenario6()
{
	InitializeComponent();
}


void SDKSample::RequestedThemeCPP::Scenario6::lightButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	SaveTheme(true);
}

void SDKSample::RequestedThemeCPP::Scenario6::darkButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	SaveTheme(false);
}

void SDKSample::RequestedThemeCPP::Scenario6::SaveTheme(bool isLightTheme)
{
	Windows::Storage::ApplicationDataContainer^ localSettings = Windows::Storage::ApplicationData::Current->LocalSettings;
	auto values = localSettings->Values;
	values->Insert("IsLightTheme", dynamic_cast<PropertyValue^>(PropertyValue::CreateBoolean(isLightTheme)));

	// Changing the theme requires app restart. Notify user.
	Windows::UI::Popups::MessageDialog^ md = ref new Windows::UI::Popups::MessageDialog("Please restart the sample to see this theme applied to the output area below");
	md->ShowAsync();
}
