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
// CalendarEnumerationAndMath.xaml.cpp
// Implementation of the CalendarEnumerationAndMath class
//

#include "pch.h"
#include "CalendarEnumerationAndMath.xaml.h"

using namespace SDKSample::CalendarSample;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;

CalendarEnumerationAndMath::CalendarEnumerationAndMath()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void CalendarEnumerationAndMath::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::CalendarSample::CalendarEnumerationAndMath::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // This scenario uses the Windows.Globalization.Calendar class to enumerate through a calendar and
    // perform calendar math
    
    // Store results here
    String^ results;

    results = "The number of years in each era of the Japanese era calendar is not regular. It is determined by the length of the given imperial era:\n\n";

    // Array languages used by the test
    auto en = ref new Platform::Collections::Vector<String^>();
	en->Append("en-US");

    // Create Japanese calendar->
    Calendar^ calendar = ref new Calendar(en, CalendarIdentifiers::Japanese, ClockIdentifiers::TwentyFourHour);

    // Enumerate all supported years in all supported Japanese eras.
    for (calendar->Era = calendar->FirstEra; true; calendar->AddYears(1))
    {
        // Process current era.
        results = results + "Era " + calendar->EraAsString() + " contains " + calendar->NumberOfYearsInThisEra + " year(s)\n";

        // Enumerate all years in this era.
        for (calendar->Year = calendar->FirstYearInThisEra; true; calendar->AddYears(1))
        {
            // Begin sample processing of current year.

            // Move to first day of year. Change of month can affect day so order of assignments is important.
            calendar->Month = calendar->FirstMonthInThisYear;
            calendar->Day = calendar->FirstDayInThisMonth;

            // Set time to midnight (local time).
            calendar->Period = calendar->FirstPeriodInThisDay;    // All days have 1 or 2 periods depending on clock type
            calendar->Hour = calendar->FirstHourInThisPeriod;     // Hours start from 12 or 0 depending on clock type
            calendar->Minute = 0;
            calendar->Second = 0;
            calendar->Nanosecond = 0;

            results = results + ".";

            // End sample processing of current year.

            // Break after processing last year.
            if (calendar->Year == calendar->LastYearInThisEra)
                break;
        }
        results = results + "\n";

        // Break after processing last era.
        if (calendar->Era == calendar->LastEra)
            break;
    }

    // This section shows enumeration through the hours in a day to demonstrate that the number of time units in a given period (hours in a day, minutes in an hour, etc.)
    // should not be regarded as fixed. With Daylight Saving Time and other local calendar adjustments, a given day may have not have 24 hours, and
    // a given hour may not have 60 minutes, etc.
    results = results + "\nThe number of hours in a day is not invariable. The US calendar transitions from DST to standard time on 4 November 2012. Set your system time zone to a US time zone to see the effect on the number of hours in the day:\n\n";

    // Create a DateTimeFormatter to display dates
    DateTimeFormatter^ displayDate = ref new Windows::Globalization::DateTimeFormatting::DateTimeFormatter("longdate");

    // Create a gregorian calendar for the US with 12-hour clock format
    Calendar^ currentCal = ref new Windows::Globalization::Calendar(en, CalendarIdentifiers::Gregorian, ClockIdentifiers::TwentyFourHour);

    // Set the calendar to a the date of the Daylight Saving Time-to-Standard Time transition for the US in 2012.
    // DST ends in the US at 02:00 on 4 November 2012
    currentCal->Year = 2012;
	currentCal->Month = 11;
	currentCal->Day = 4;
	currentCal->Hour = 0;
	currentCal->Minute = 0;
	currentCal->Second = 0;

    // Set the current calendar to one day before DST change. Create a second calendar for comparision and set it to one day after DST change.
    Calendar^ endDate = currentCal->Clone();
    currentCal->AddDays(-1);
    endDate->AddDays(1);

    // Enumerate the day before, the day of, and the day after the 2012 DST-to-Standard time transition
    while (currentCal->Day <= endDate->Day) 
	{
        // Process current day.
        DateTime date = currentCal->GetDateTime();
        results = results + displayDate->Format(date) + " contains " + currentCal->NumberOfHoursInThisPeriod + " hour(s)\n";

        // Enumerate all hours in this day.
        // Create a calendar to represent the following day.
        Calendar^ nextDay = currentCal->Clone();
        nextDay->AddDays(1);
        for (currentCal->Hour = currentCal->FirstHourInThisPeriod; true; currentCal->AddHours(1)) 
        {
            // Display the hour for each hour in the day.             
            results = results + currentCal->HourAsPaddedString(2) + " ";

            // Break upon reaching the next period (i.e. the first period in the following day).
            if (currentCal->Day == nextDay->Day && currentCal->Period == nextDay->Period) 
            {
                break;
            }
        }

        results = results + "\n";
    }


    // Display the results
    rootPage->NotifyUser(results, NotifyType::StatusMessage);
}
