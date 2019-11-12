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
// OverrideSettings.xaml.cpp
// Implementation of the OverrideSettings class
//

#include "pch.h"
#include "OverrideSettings.xaml.h"

using namespace SDKSample::DateTimeFormatting;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;

OverrideSettings::OverrideSettings()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void OverrideSettings::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void OverrideSettings::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// This scenario uses the Windows.Globalization.DateTimeFormatting.DateTimeFormatter class
	// to format a date/time by using a formatter that provides specific languages, 
	// geographic region, calendar and clock

	//Get the current default application context language
	String^ currentLanguage = ApplicationLanguages::Languages->GetAt(0);

    // Array languages used by the test
    auto ja = ref new Platform::Collections::Vector<String^>();
    ja->Append("ja-JP");
    auto fr = ref new Platform::Collections::Vector<String^>();
    fr->Append("fr-FR");
    auto de = ref new Platform::Collections::Vector<String^>();
    de->Append("de-DE");

    // Formatters for dates and times
    Array<DateTimeFormatter^> ^dateFormatters = {
		ref new DateTimeFormatter("longdate", ja, "JP", CalendarIdentifiers::Japanese, ClockIdentifiers::TwelveHour),

        ref new DateTimeFormatter("month day", fr, "FR", CalendarIdentifiers::Gregorian, ClockIdentifiers::TwentyFourHour),
		
        ref new DateTimeFormatter(
			YearFormat::Abbreviated, 
			MonthFormat::Abbreviated, 
			DayFormat::Default, 
			DayOfWeekFormat::None, 
			HourFormat::None, 
			MinuteFormat::None, 
			SecondFormat::None, 
			de, "DE", CalendarIdentifiers::Gregorian, ClockIdentifiers::TwelveHour),

		ref new DateTimeFormatter("longtime", ja, "JP", CalendarIdentifiers::Japanese, ClockIdentifiers::TwelveHour),

        ref new DateTimeFormatter("hour minute", fr, "FR", CalendarIdentifiers::Gregorian, ClockIdentifiers::TwentyFourHour),
		
        ref new DateTimeFormatter(
			YearFormat::None, 
			MonthFormat::None, 
			DayFormat::None, 
			DayOfWeekFormat::None,
			HourFormat::Default, 
			MinuteFormat::Default, 
			SecondFormat::None, 
			de, "DE", CalendarIdentifiers::Gregorian, ClockIdentifiers::TwelveHour)
    };

	// Obtain the date that will be formatted.
	Windows::Globalization::Calendar^ cal = ref new Windows::Globalization::Calendar();
    Windows::Foundation::DateTime dateToFormat = cal->GetDateTime();
    
    // Keep the results here
    String^ results = "";
	results = results + "Current default application context language: " + currentLanguage + "\n\n";

	// Generate the results.
    for (unsigned int i = 0; i < dateFormatters->Length; i++)
    {
        // Perform the actual formatting. 
        try 
        { 
			results = results + dateFormatters[i]->Template + ": (" + dateFormatters[i]->ResolvedLanguage + ") " + dateFormatters[i]->Format(dateToFormat) + "\n";
        }
        catch(Platform::Exception^)
        {
            // Retrieve and display formatter properties. 
            results = results + "Unable to format using formatter with template: " +
                dateFormatters[i]->Template + ", language: " + 
                dateFormatters[i]->Languages->GetAt(0) + ", region: " + 
                dateFormatters[i]->GeographicRegion + ", calendar: " +
                dateFormatters[i]->Calendar + ", clock:" + 
                dateFormatters[i]->Clock + "\n";
        }
    }

	// Display the results.
    rootPage->NotifyUser(results, NotifyType::StatusMessage);
}
