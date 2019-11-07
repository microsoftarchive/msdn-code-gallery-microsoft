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
// DecimalFormatting.xaml.cpp
// Implementation of the DecimalFormatting class
//

#include "pch.h"
#include "DecimalFormatting.xaml.h"

using namespace SDKSample::NumberFormatting;

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization::NumberFormatting;

DecimalFormatting::DecimalFormatting()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void DecimalFormatting::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::NumberFormatting::DecimalFormatting::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // This scenario uses the Windows.Globalization.NumberFormatting.DecimalFormatter class
    // to format a number.

    // Create formatter initialized using the current user's preference settings.
    DecimalFormatter^ decimalFormat = ref new DecimalFormatter();

    // Make a random number.
    float randomNumber = (float)rand() / RAND_MAX * 100000.0f;

    // Format with the user's default preferences.
    String^ decimal = decimalFormat->Format(randomNumber);

    // Format with grouping.
    DecimalFormatter^ decimalFormat1 = ref new DecimalFormatter();
    decimalFormat1->IsGrouped = true;
    String^ decimal1 = decimalFormat1->Format(randomNumber);

    // Format with grouping using French.
    auto fr = ref new Platform::Collections::Vector<String^>();
    fr->Append("fr-FR");
    DecimalFormatter^ decimalFormatFR = ref new DecimalFormatter(fr, "ZZ");
    decimalFormatFR->IsGrouped = true;
    String^ decimalFR = decimalFormatFR->Format(randomNumber);

    // Illustrate how automatic digit substitution works
    auto ar = ref new Platform::Collections::Vector<String^>();
    ar->Append("ar");
    DecimalFormatter^ decimalFormatAR = ref new DecimalFormatter(ar, "ZZ");
    String^ decimalAR = decimalFormatAR->Format(randomNumber);

    // Get a fixed number.
    long long fixedNumber = 500;

    // Format with the user's default preferences.
    String^ decimal2 = decimalFormat->Format(fixedNumber);

    // Format always with a decimal point.
    DecimalFormatter^ decimalFormat3 = ref new DecimalFormatter();
    decimalFormat3->IsDecimalPointAlwaysDisplayed = true;
    decimalFormat3->FractionDigits = 0;
    String^ decimal3 = decimalFormat3->Format(fixedNumber);

    // Format with no fractional digits or decimal point.
    DecimalFormatter^ decimalFormat4 = ref new DecimalFormatter();
    decimalFormat4->FractionDigits = 0;
    String^ decimal4 = decimalFormat4->Format(fixedNumber);

    // Display the results.
    String^ results = 
        "Random number ("                             + randomNumber + ")\n" +
        "With current user preferences: "             + decimal   + "\n" +
        "With grouping separators: "                  + decimal1  + "\n" +
        "With grouping separators (fr-FR): "          + decimalFR + "\n" +
        "With digit substitution (ar): "              + decimalAR + "\n\n" +
        "Fixed number ("                              + fixedNumber + ")\n" +
        "With current user preferences: "             + decimal2 + "\n" +
        "Always with a decimal point: "               + decimal3 + "\n" +
        "With no fraction digits or decimal points: " + decimal4;
    rootPage->NotifyUser(results, NotifyType::StatusMessage);
}
