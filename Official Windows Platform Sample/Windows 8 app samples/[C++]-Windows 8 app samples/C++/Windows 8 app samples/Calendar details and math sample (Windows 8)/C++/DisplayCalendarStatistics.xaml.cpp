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
// DisplayCalendarStatistics.xaml.cpp
// Implementation of the DisplayCalendarStatistics class
//

#include "pch.h"
#include "DisplayCalendarStatistics.xaml.h"

using namespace SDKSample::CalendarSample;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization;

DisplayCalendarStatistics::DisplayCalendarStatistics()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void DisplayCalendarStatistics::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::CalendarSample::DisplayCalendarStatistics::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // This scenario uses the Windows.Globalization.Calendar class to display the calendar
    // system statistics.

    // Store results here
    String^ results;

    // Array languages used by the test
    auto ja = ref new Platform::Collections::Vector<String^>();
	ja->Append("ja-JP");
	auto he = ref new Platform::Collections::Vector<String^>();
	he->Append("he-IL");

    // Calendar based on the current user.
	Calendar^ cal1 = ref new Calendar();

	// Calendar using Japanese language and Japanese calendar.
	Calendar^ cal2 = ref new Calendar(ja, CalendarIdentifiers::Japanese, ClockIdentifiers::TwelveHour);

	// Calendar using Hebrew language and Hebrew calendar.
	Calendar^ cal3 = ref new Calendar(he, CalendarIdentifiers::Hebrew, ClockIdentifiers::TwentyFourHour);

	String^ calStats1 = 
        "User's default calendar: " + cal1->GetCalendarSystem() + "\n" +
		"Months in this Year: " + cal1->NumberOfMonthsInThisYear + "\n" +
		"Days in this Month: " + cal1->NumberOfDaysInThisMonth + "\n" +
		"Hours in this Period: " + cal1->NumberOfHoursInThisPeriod + "\n"
		"Era: " + cal1->EraAsString();

	String^ calStats2 = "Calendar system: " + cal2->GetCalendarSystem() + "\n" +
		"Months in this Year: " + cal2->NumberOfMonthsInThisYear + "\n" +
		"Days in this Month: " + cal2->NumberOfDaysInThisMonth + "\n" +
		"Hours in this Period: " + cal2->NumberOfHoursInThisPeriod + "\n"
		"Era: " + cal2->EraAsString();

	String^ calStats3 = 
        "Calendar system: " + cal3->GetCalendarSystem() + "\n" +
		"Months in this Year: " + cal3->NumberOfMonthsInThisYear + "\n" +
		"Days in this Month: " + cal3->NumberOfDaysInThisMonth + "\n" +
		"Hours in this Period: " + cal3->NumberOfHoursInThisPeriod + "\n"
		"Era: " + cal3->EraAsString();

	// Generate the result.
	results = calStats1 + "\n\n" + calStats2 + "\n\n" + calStats3;

    // Display the results
    rootPage->NotifyUser(results, NotifyType::StatusMessage);
}
