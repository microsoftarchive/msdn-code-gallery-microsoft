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
// ClearScenario.xaml.cpp
// Implementation of the ClearScenario class
//

#include "pch.h"
#include "ClearScenario.xaml.h"

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

ClearScenario::ClearScenario()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ClearScenario::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void ClearScenario::Clear_Click(Object^ sender, RoutedEventArgs^ e)
{
    concurrency::create_task(Windows::Storage::ApplicationData::Current->ClearAsync())
        .then([this](concurrency::task<void> result)
    {
        try
        {
            result.get();
            OutputTextBlock->Text = "ApplicationData has been cleared.  Visit the other scenarios to see that their data has been cleared.";
        }
        catch (Exception ^ex)
        {
            OutputTextBlock->Text = "Unable to clear settings, make sure all files are closed.";
        }
    });
}
