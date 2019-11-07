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
// CurrencyFormatting.xaml.cpp
// Implementation of the CurrencyFormatting class
//

#include "pch.h"
#include "CurrencyFormatting.xaml.h"

using namespace SDKSample::NumberFormatting;

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization::NumberFormatting;

CurrencyFormatting::CurrencyFormatting()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void CurrencyFormatting::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::NumberFormatting::CurrencyFormatting::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // This scenario uses the Windows.Globalization.NumberFormatting.CurrencyFormatter class
    // to format a number as a currency.

    // Determine the current user's default currency.
    auto userCurrency = Windows::System::UserProfile::GlobalizationPreferences::Currencies->First()->Current;

    // Generate numbers used for formatting.
    long long wholeNumber      = 12345;
    double fractionalNumber = 12345.67;

    // Create formatter initialized using the current user's preference settings for number formatting.
    CurrencyFormatter^ userCurrencyFormat = ref new CurrencyFormatter(userCurrency);
    String^ currencyDefault = userCurrencyFormat->Format(fractionalNumber);

    // Create a formatter initialized to a specific currency, in this case it's the US Dollar, but with the default number formatting for the current user.
    CurrencyFormatter^ currencyFormatUSD = ref new CurrencyFormatter("USD");  // Specified as an ISO 4217 code.
    String^ currencyUSD = currencyFormatUSD->Format(fractionalNumber);

    // Create a formatter initialized to a specific currency, in this case it's the Euro with the default number formatting for France.  
    auto fr = ref new Platform::Collections::Vector<String^>();
    fr->Append("fr-FR");
    CurrencyFormatter^ currencyFormatEuroFR = ref new CurrencyFormatter("EUR", fr, "FR");
    String^ currencyEuroFR = currencyFormatEuroFR->Format(fractionalNumber);

    // Create a formatter initialized to a specific currency, in this case it's the Euro with the default number formatting for Ireland.  
    auto gd = ref new Platform::Collections::Vector<String^>();
    gd->Append("gd-IE");
    CurrencyFormatter^ currencyFormatEuroIE = ref new CurrencyFormatter("EUR", gd, "IE");
    String^ currencyEuroIE = currencyFormatEuroIE->Format(fractionalNumber);

    // Formatted so that fraction digits are always included.
    CurrencyFormatter^ currencyFormatUSD1 = ref new CurrencyFormatter("USD");
    currencyFormatUSD1->FractionDigits = 2;
    String^ currencyUSD1 = currencyFormatUSD1->Format(wholeNumber);

    // Formatted so that integer grouping separators are included.
    CurrencyFormatter^ currencyFormatUSD2 = ref new CurrencyFormatter("USD");
    currencyFormatUSD2->IsGrouped = 1;
    String^ currencyUSD2 = currencyFormatUSD2->Format(fractionalNumber);

    // Display the results.
    String^ results = 
        "Fixed number ("                                   + fractionalNumber + ")\n" +
        "With user's default currency: "                   + currencyDefault + "\n" +
        "Formatted US Dollar: "                            + currencyUSD     + "\n" +
        "Formatted Euro (fr-FR defaults): "                + currencyEuroFR  + "\n" +
        "Formatted Euro (gd-IE defaults): "                + currencyEuroIE  + "\n" +
        "Formatted US Dollar (with fractional digits): "   + currencyUSD1    + "\n" +
        "Formatted US Dollar (with grouping separators): " + currencyUSD2;

    rootPage->NotifyUser(results, NotifyType::StatusMessage);
}
