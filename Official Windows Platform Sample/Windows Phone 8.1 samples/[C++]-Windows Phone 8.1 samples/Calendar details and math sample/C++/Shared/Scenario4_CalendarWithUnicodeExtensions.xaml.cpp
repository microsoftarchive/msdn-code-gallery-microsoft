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
// CalendarWithUnicodeExtensions.xaml.cpp
// Implementation of the CalendarWithUnicodeExtensions class
//

#include "pch.h"
#include "Scenario4_CalendarWithUnicodeExtensions.xaml.h"
#include "stdio.h"

using namespace SDKSample::CalendarSample;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization;

CalendarWithUnicodeExtensions::CalendarWithUnicodeExtensions()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void CalendarWithUnicodeExtensions::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::CalendarSample::CalendarWithUnicodeExtensions::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // This scenario uses the Windows.Globalization.Calendar class to display the parts of a date.
    
    // Store results here
    String^ results;

    // Array of languages used by the test
    // NOTE: Calendar (ca) and numeral system (nu) are the only supported extensions with any others being ignored 
    // (note that collation (co) extension is ignored in the last example).
    auto arabicLanguageGregorianCalendarLatinNumerals = ref new Platform::Collections::Vector<String^>();
    arabicLanguageGregorianCalendarLatinNumerals->Append("ar-SA-u-ca-gregory-nu-Latn");
    auto hebrewLanguageArabNumerals = ref new Platform::Collections::Vector<String^>();
    hebrewLanguageArabNumerals->Append("he-IL-u-nu-arab");
    auto hebrewLanguageHebrewCalendarCollationPhonebook = ref new Platform::Collections::Vector<String^>();
    hebrewLanguageHebrewCalendarCollationPhonebook->Append("he-IL-u-ca-hebrew-co-phonebk");

    // Create Calendar objects using different Unicode extensions for different languages.
    Calendar^ cal1 = ref new Calendar();
    Calendar^ cal2 = ref new Calendar(arabicLanguageGregorianCalendarLatinNumerals);
    Calendar^ cal3 = ref new Calendar(hebrewLanguageArabNumerals);
    Calendar^ cal4 = ref new Calendar(hebrewLanguageHebrewCalendarCollationPhonebook);

    // Obtain the relevant properties and date parts for the calendar objects
    String^ calItems1 = "User's default calendar object:\n" + 
                        GetCalendarProperties(cal1);

    String^ calItems2 = "Calendar object with Arabic language, Gregorian Calendar and Latin Numeral System (ar-SA-ca-gregory-nu-Latn) :\n" +  
                        GetCalendarProperties(cal2);

    String^ calItems3 = "Calendar object with Hebrew language, Default Calendar for that language and Arab Numeral System (he-IL-u-nu-arab) :\n" +
                        GetCalendarProperties(cal3);
    
    String^ calItems4 = "Calendar object with Hebrew language, Hebrew Calendar, Default Numeral System for that language and Phonebook collation (he-IL-u-ca-hebrew-co-phonebk) :\n" +
                        GetCalendarProperties(cal4);

    // Generate the results.
    results = calItems1 + "\n\n" + calItems2 + "\n\n" + calItems3 + "\n\n" + calItems4;

    // Display the results
    OutputTextBlock->Text = results;
}

/// <summary>
/// This is a helper function to display calendar's properties in presentable format
/// </summary>
/// <param name="calendar"></param>
String^ CalendarWithUnicodeExtensions::GetCalendarProperties(Calendar^ calendar)
{
    String^ returnString = "Calendar System: " + calendar->GetCalendarSystem() + "\n" +
        "Numeral System: " + calendar->NumeralSystem + "\n" +
        "Resolved Language: " + calendar->ResolvedLanguage + "\n" +
        "Name of Month: " + calendar->MonthAsSoloString() + "\n" +
        "Day of Month: " + calendar->DayAsPaddedString(2) + "\n" +
        "Day of Week: " + calendar->DayOfWeekAsString() + "\n" +
        "Year: " + calendar->YearAsString();
    return returnString;
}
