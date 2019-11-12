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
// CurrentInputLanguage.xaml.cpp
// Implementation of the CurrentInputLanguage class
//

#include "pch.h"
#include "CurrentInputLanguage.xaml.h"

using namespace SDKSample::GlobalizationPreferencesSample;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::System::UserProfile;
using namespace Windows::Globalization;

CurrentInputLanguage::CurrentInputLanguage()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void CurrentInputLanguage::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void CurrentInputLanguage::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // This scenario uses the Windows.Globalization.Language class to obtain the user's current 
    // input language.  The language tag returned reflects the current input language specified 
    // by the user.

    // Get the user's input language
    String^ userInputLanguage = Windows::Globalization::Language::CurrentInputMethodLanguageTag;

    // Generate the results
    String^ results = "User's current input language: " + userInputLanguage;

    // Display the results
    rootPage->NotifyUser(results, NotifyType::StatusMessage);
}
