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
// UsingUnicodeExtensions.xaml.cpp
// Implementation of the UsingUnicodeExtensions class
//

#include "pch.h"
#include "Scenario5_UsingUnicodeExtensions.xaml.h"

using namespace SDKSample::DateTimeFormatting;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;

UsingUnicodeExtensions::UsingUnicodeExtensions()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void UsingUnicodeExtensions::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::DateTimeFormatting::UsingUnicodeExtensions::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // This scenario uses the Windows.Globalization.DateTimeFormatting.DateTimeFormatter class
    // to format a date/time by using a formatter that uses unicode extenstion in the specified
    // language name

    // Get the current application context language
    String^ currentLanguage = ApplicationLanguages::Languages->GetAt(0);

    // Keep the results here
    String^ results = "";
    results = results + "Current application context language: " + currentLanguage + "\n\n";
    
    // Array languages used by the test
    auto teluguLanguageGregorianCalendarLatnNumerals = ref new Platform::Collections::Vector<String^>();
    teluguLanguageGregorianCalendarLatnNumerals->Append("te-in-u-ca-gregory-nu-latn");
    teluguLanguageGregorianCalendarLatnNumerals->Append("en-US");

    auto hebrewLanguageArabNumerals = ref new Platform::Collections::Vector<String^>();
    hebrewLanguageArabNumerals->Append("he-IL-u-nu-arab");
    hebrewLanguageArabNumerals->Append("en-US");

    auto hebrewLanguageHebrewCalendarCollationPhonebook = ref new Platform::Collections::Vector<String^>();
    hebrewLanguageHebrewCalendarCollationPhonebook->Append("he-IL-u-ca-hebrew-co-phonebk");
    hebrewLanguageHebrewCalendarCollationPhonebook->Append("en-US");

    // Formatters for dates and times
    Array<DateTimeFormatter^> ^dateFormatters = {
        // Default application context  
        ref new DateTimeFormatter("longdate longtime"),
        // Telugu language, Gregorian Calendar and Latin Numeral System
        ref new DateTimeFormatter("longdate longtime", teluguLanguageGregorianCalendarLatnNumerals),
        // Hebrew language and Arabic Numeral System - calendar NOT specified in constructor
        ref new DateTimeFormatter(YearFormat::Default, 
            MonthFormat::Default, 
            DayFormat::Default, 
            DayOfWeekFormat::Default,
            HourFormat::Default,
            MinuteFormat::Default,
            SecondFormat::Default,
            hebrewLanguageArabNumerals),             
        // Hebrew language and calendar - calendar specified in constructor
        // also, which overrides the one specified in Unicode extension
        ref new DateTimeFormatter(YearFormat::Default, 
            MonthFormat::Default, 
            DayFormat::Default, 
            DayOfWeekFormat::Default,
            HourFormat::Default,
            MinuteFormat::Default,
            SecondFormat::Default,
            hebrewLanguageHebrewCalendarCollationPhonebook,
            "US",
            CalendarIdentifiers::Gregorian,
            ClockIdentifiers::TwentyFourHour), 
    };

    // Obtain the date that will be formatted.
    Windows::Globalization::Calendar^ cal = ref new Windows::Globalization::Calendar();
    Windows::Foundation::DateTime dateToFormat = cal->GetDateTime();

    // Format and display date/time along with other relevant properites
    for (unsigned int i = 0; i < dateFormatters->Length; i++)
    {
        // Perform the actual formatting. 
        results += "Using DateTimeFormatter with Language List:  ";

        bool hasPrecedingLanguage = false;
        for (auto language : dateFormatters[i]->Languages)
        {
            if (hasPrecedingLanguage)
            {
                results += ", ";
            }

            hasPrecedingLanguage = true;
            results += language;
        }

        results += "\n";
        results += "\t Template:   " + dateFormatters[i]->Template + "\n";
        results += "\t Resolved Language:   " + dateFormatters[i]->ResolvedLanguage + "\n";
        results += "\t Calendar System:   " + dateFormatters[i]->Calendar + "\n";
        results += "\t Numeral System:   " + dateFormatters[i]->NumeralSystem + "\n";
        results += "Formatted DateTime:   " + dateFormatters[i]->Format(dateToFormat) + "\n";
        results += "\n";
    }

    // Display the results.
    OutputTextBlock->Text = results;
}
