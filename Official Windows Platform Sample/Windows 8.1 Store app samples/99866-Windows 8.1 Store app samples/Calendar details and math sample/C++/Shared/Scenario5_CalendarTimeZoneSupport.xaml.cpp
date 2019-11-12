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
// CalendarTimeZoneSupport.xaml.cpp
// Implementation of the CalendarTimeZoneSupport class
//

#include "pch.h"
#include "Scenario5_CalendarTimeZoneSupport.xaml.h"
#include "stdio.h"

using namespace SDKSample::CalendarSample;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization;

CalendarTimeZoneSupport::CalendarTimeZoneSupport()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void CalendarTimeZoneSupport::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::CalendarSample::CalendarTimeZoneSupport::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // This scenario illustrates TimeZone support in Windows.Globalization.Calendar class

    // Displayed TimeZones (other than local timezone)
    auto timeZones = ref new Platform::Collections::Vector<String^>();
    timeZones->Append("UTC");
    timeZones->Append("America/New_York");
    timeZones->Append("Asia/Kolkata");

    // Store results here
    String^ results;
    
    // Create a calendar based on the current user.
    Calendar^ calendar = ref new Calendar();
    String^ localTimeZone = calendar->GetTimeZone();
    
    // Show current time in timezones desired to be displayed including local timezone
    String^ calItems1 = 
        "Current date and time -" + "\n" +
        GetFormattedCalendarDateTime(calendar);
    for each (String^ timezone in timeZones)
    {
        calendar->ChangeTimeZone(timezone);
        calItems1 += GetFormattedCalendarDateTime(calendar);
    }
    calendar->ChangeTimeZone(localTimeZone);

    // Show a time on 14th day of second month of next year in local, and other desired timezones
    // This will show if there were day light savings in time
    calendar->AddYears(1); calendar->Month = 2; calendar->Day = 14;
    String^ calItems2 = 
        "Same time on 14th day of second month of next year -" + "\n" +
        GetFormattedCalendarDateTime(calendar);
    for each (String^ timezone in timeZones)
    {
        calendar->ChangeTimeZone(timezone);
        calItems2 += GetFormattedCalendarDateTime(calendar);
    }
    calendar->ChangeTimeZone(localTimeZone);
    
    // Show a time on 14th day of 10th month of next year in local, and other desired timezones
    // This will show if there were day light savings in time
    calendar->AddMonths(8);
    String^ calItems3 = 
        "Same time on 14th day of tenth month of next year -" + "\n" +
        GetFormattedCalendarDateTime(calendar);
    for each (String^ timezone in timeZones)
    {
        calendar->ChangeTimeZone(timezone);
        calItems3 += GetFormattedCalendarDateTime(calendar);
    }
    calendar->ChangeTimeZone(localTimeZone);

    // Generate the results.
    results = calItems1 + "\n" + calItems2 + "\n" + calItems3;

    // Display the results
    OutputTextBlock->Text = results;
}

/// <summary>
/// This is a helper function to display calendar's date-time in presentable format
/// </summary>
/// <param name="calendar"></param>
String^ CalendarTimeZoneSupport::GetFormattedCalendarDateTime(Calendar^ calendar)
{
    String^ returnString = "In " + calendar->GetTimeZone() + " TimeZone:   " + 
                           calendar->DayOfWeekAsSoloString() + "  " +
                           calendar->MonthAsSoloString() + " " +
                           calendar->DayAsPaddedString(2) + ", " +
                           calendar->YearAsString() + "   " +
                           calendar->HourAsPaddedString(2) + ":" +
                           calendar->MinuteAsPaddedString(2) + ":" +
                           calendar->SecondAsPaddedString(2) + " " +
                           calendar->PeriodAsString() + "  " +
                           calendar->TimeZoneAsString(3) + "\n";
    return returnString;
}
