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
// NumeralSystemTranslation.xaml.cpp
// Implementation of the NumeralSystemTranslation class
//

#include "pch.h"
#include "Scenario6_NumeralSystemTranslation.xaml.h"

using namespace SDKSample::NumberFormatting;

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization::NumberFormatting;

NumeralSystemTranslation::NumeralSystemTranslation()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void NumeralSystemTranslation::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::NumberFormatting::NumeralSystemTranslation::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Text for which translation of the numeral system will be performed.  Please note that translation only happens between
    // Latin and any other supported numeral system.  Translation between numeral systems is not a supported scenario.
    String^ stringToTranslate = "These are the 10 digits of a numeral system: 0, 1, 2, 3, 4, 5, 6, 7, 8, 9";

    // Variable where we keep the results of the scenario
    String^ results = "Original string: " + stringToTranslate + "\n\n";

    // The numeral system translator is initialized based on the current application language.
    NumeralSystemTranslator^ numeralTranslator = ref new NumeralSystemTranslator();

    // Do translation
    results = results + "Using application settings (" + numeralTranslator->NumeralSystem + "): ";
    results = results + numeralTranslator->TranslateNumerals(stringToTranslate) + "\n";

    // Switch to a different numeral system
    numeralTranslator->NumeralSystem = "hanidec";

    // Do translation
    results = results + "Using numeral system via property (" + numeralTranslator->NumeralSystem + " ): ";
    results = results + numeralTranslator->TranslateNumerals(stringToTranslate) + "\n";

    // Create a converter using a language list to initialize the numeral system to an appropriate default
    auto languages = ref new Platform::Collections::Vector<String^>();
    languages->Append("ar-SA");
    languages->Append("en-US");
    numeralTranslator = ref new NumeralSystemTranslator(languages); 
    
    // Do translation
    results = results + "Using numeral system via language list (" + numeralTranslator->NumeralSystem + "): ";
    results = results + numeralTranslator->TranslateNumerals(stringToTranslate) + "\n";

    // Display the results.
    OutputTextBlock->Text = results;
}
