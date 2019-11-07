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
// SetVersion.xaml.cpp
// Implementation of the SetVersion class
//

#include "pch.h"
#include "SetVersion.xaml.h"

using namespace SDKSample::ApplicationDataSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage;
using namespace concurrency;

#define settingName   "SetVersionSetting"
#define settingValue0 "Data.v0"
#define settingValue1 "Data.v1"

SetVersion::SetVersion()
{
    InitializeComponent();

    appData = ApplicationData::Current;

    DisplayOutput();
}

void SetVersion::SetVersionHandler0(SetVersionRequest^ request)
{
    SetVersionDeferral^ deferral = request->GetDeferral();

    unsigned int version = appData->Version;

    switch (version)
    {
        case 0:
            // Version is already 0.  Nothing to do.
            break;

        case 1:
            // Need to convert data from v1 to v0.

            // This sample simulates that conversion by writing a version-specific value.
            appData->LocalSettings->Values->Insert(settingName, dynamic_cast<PropertyValue^>(PropertyValue::CreateString(settingValue0)));

            break;

        default:
            throw ref new Exception(E_FAIL, "Unexpected ApplicationData Version: " + version.ToString());
    }

    deferral->Complete();
}

void SetVersion::SetVersionHandler1(SetVersionRequest^ request)
{
    SetVersionDeferral^ deferral = request->GetDeferral();

    unsigned int version = appData->Version;

    switch (version)
    {
        case 0:
            // Need to convert data from v0 to v1.

            // This sample simulates that conversion by writing a version-specific value.
            appData->LocalSettings->Values->Insert(settingName, dynamic_cast<PropertyValue^>(PropertyValue::CreateString(settingValue1)));
            break;

        case 1:
            // Version is already 1.  Nothing to do.
            break;

        default:
            throw ref new Exception(E_FAIL, "Unexpected ApplicationData Version: " + version.ToString());
    }

    deferral->Complete();
}

void SetVersion::SetVersion0_Click(Object^ sender, RoutedEventArgs^ e)
{
    task<void>(appData->SetVersionAsync(0, ref new ApplicationDataSetVersionHandler(this, &SetVersion::SetVersionHandler0))).then([=]
    {
        DisplayOutput();
    });
}

void SetVersion::SetVersion1_Click(Object^ sender, RoutedEventArgs^ e)
{
    task<void>(appData->SetVersionAsync(1, ref new ApplicationDataSetVersionHandler(this, &SetVersion::SetVersionHandler1))).then([=]
    {
        DisplayOutput();
    });
}

void SetVersion::DisplayOutput()
{
    OutputTextBlock->Text = "Version: " + appData->Version.ToString();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void SetVersion::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}
