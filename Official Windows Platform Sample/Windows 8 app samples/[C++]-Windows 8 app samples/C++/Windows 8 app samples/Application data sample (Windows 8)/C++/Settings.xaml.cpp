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
// Settings.xaml.cpp
// Implementation of the Settings class
//

#include "pch.h"
#include "Settings.xaml.h"

using namespace SDKSample::ApplicationDataSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage;

#define settingName "exampleSetting"
#define settingValue "Hello World"

Settings::Settings()
{
    InitializeComponent();

    localSettings = ApplicationData::Current->LocalSettings;

    DisplayOutput();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Settings::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

// Guidance for Settings.
//
// Settings are a convenient way of storing small pieces of configuration data
// for your application.
//
// Settings can be either Local or Roaming.
//
// Roaming settings will be synchronized across machines on which the user has
// signed in with a Microsoft Account.  Roaming of settings is not instant; the
// system weighs several factors when determining when to send the data.  Usage
// of roaming data should be kept below the quota (available via the 
// RoamingStorageQuota property), or else roaming of data will be suspended.
//
// User preferences for your application are a great match for roaming settings.
// User preferences are usually fixed in number and small in size.  Users will
// appreciated that your application is customized the way they prefer across
// all of their machines.
//
// Local settings are not synchronized and remain on the machine on which they
// were originally written.
//
// Care should be taken to guard against an excessive volume of data being
// stored in settings.  Settings are not intended to be used as a database.
// Large data sets will take longer to load from disk during your application's
// launch.

// This sample illustrates reading and writing from a local setting, though a
// roaming setting could be used just as easily.

void Settings::WriteSetting_Click(Object^ sender, RoutedEventArgs^ e)
{
    localSettings->Values->Insert(settingName, dynamic_cast<PropertyValue^>(PropertyValue::CreateString(settingValue)));

    DisplayOutput();
}

void Settings::DeleteSetting_Click(Object^ sender, RoutedEventArgs^ e)
{
    localSettings->Values->Remove(settingName);

    DisplayOutput();
}

void Settings::DisplayOutput()
{
    String^ value = safe_cast<String^>(localSettings->Values->Lookup(settingName));

    OutputTextBlock->Text = "Setting: " + (value == nullptr ? "<empty>" : ("\"" + value + "\""));
}
