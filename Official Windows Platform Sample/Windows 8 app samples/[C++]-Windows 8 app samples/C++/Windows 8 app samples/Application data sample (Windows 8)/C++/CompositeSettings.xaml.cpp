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
// CompositeSettings.xaml.cpp
// Implementation of the CompositeSettings class
//

#include "pch.h"
#include "CompositeSettings.xaml.h"

using namespace SDKSample::ApplicationDataSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage;

#define settingName   "exampleCompositeSetting"
#define settingName1  "one"
#define settingValue1 1
#define settingName2  "hello"
#define settingValue2 "world"

CompositeSettings::CompositeSettings()
{
    InitializeComponent();

    roamingSettings = ApplicationData::Current->RoamingSettings;

    DisplayOutput();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void CompositeSettings::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void CompositeSettings::WriteCompositeSetting_Click(Object^ sender, RoutedEventArgs^ e)
{
    ApplicationDataCompositeValue^ composite = ref new ApplicationDataCompositeValue();
    composite->Insert(settingName1, dynamic_cast<PropertyValue^>(PropertyValue::CreateInt32(settingValue1)));
    composite->Insert(settingName2, dynamic_cast<PropertyValue^>(PropertyValue::CreateString(settingValue2)));

    roamingSettings->Values->Insert(settingName, composite);

    DisplayOutput();
}

void CompositeSettings::DeleteCompositeSetting_Click(Object^ sender, RoutedEventArgs^ e)
{
    roamingSettings->Values->Remove(settingName);

    DisplayOutput();
}

void CompositeSettings::DisplayOutput()
{
    ApplicationDataCompositeValue^ composite = safe_cast<ApplicationDataCompositeValue^>(roamingSettings->Values->Lookup(settingName));

    String^ output;
    if (composite == nullptr)
    {
        output = "Composite Setting: <empty>";
    }
    else
    {
        int one = safe_cast<IPropertyValue^>(composite->Lookup(settingName1))->GetInt32();
        String^ hello = safe_cast<String^>(composite->Lookup(settingName2));

        output = "Composite Setting: {"
            + settingName1
            +" = "
            + one.ToString()
            + ", "
            + settingName2
            + " = \""
            + hello
            + "\"}";
    }

    OutputTextBlock->Text = output;
}
