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
// SettingContainer.xaml.cpp
// Implementation of the SettingContainer class
//

#include "pch.h"
#include "SettingContainer.xaml.h"

using namespace SDKSample::ApplicationDataSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage;

#define containerName "exampleContainer"
#define settingName "exampleSetting"
#define settingValue "Hello World"

SettingContainer::SettingContainer()
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
void SettingContainer::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SettingContainer::CreateContainer_Click(Object^ sender, RoutedEventArgs^ e)
{
    ApplicationDataContainer^ container = localSettings->CreateContainer(containerName, ApplicationDataCreateDisposition::Always);

    DisplayOutput();
}

void SettingContainer::DeleteContainer_Click(Object^ sender, RoutedEventArgs^ e)
{
    localSettings->DeleteContainer(containerName);

    DisplayOutput();
}

void SettingContainer::WriteSetting_Click(Object^ sender, RoutedEventArgs^ e)
{
    if (localSettings->Containers->HasKey(containerName))
    {
        auto values = localSettings->Containers->Lookup(containerName)->Values;
        values->Insert(settingName, settingValue);
    }

    DisplayOutput();
}

void SettingContainer::DeleteSetting_Click(Object^ sender, RoutedEventArgs^ e)
{
    if (localSettings->Containers->HasKey(containerName))
    {
        auto values = localSettings->Containers->Lookup(containerName)->Values;
        values->Remove(settingName);
    }

    DisplayOutput();
}

void SettingContainer::DisplayOutput()
{
    bool hasContainer = localSettings->Containers->HasKey(containerName);
    bool hasSetting = false;

    if (hasContainer)
    {
        auto values = localSettings->Containers->Lookup(containerName)->Values;
        hasSetting = values->HasKey(settingName);
    }

    OutputTextBlock->Text = "Container Exists: " + (hasContainer ? "true" : "false") + "\n" +
                            "Setting Exists: " + (hasSetting ? "true" : "false");
}
