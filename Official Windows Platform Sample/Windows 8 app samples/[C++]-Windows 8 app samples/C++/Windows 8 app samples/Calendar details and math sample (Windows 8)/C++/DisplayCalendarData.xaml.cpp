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
// DisplayCalendarData.xaml.cpp
// Implementation of the DisplayCalendarData class
//

#include "pch.h"
#include "DisplayCalendarData.xaml.h"

using namespace SDKSample::CalendarSample;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Globalization;

DisplayCalendarData::DisplayCalendarData()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void DisplayCalendarData::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::CalendarSample::DisplayCalendarData::Display_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // This scenario uses the Windows.Globalization.Calendar class to display the parts of a date.
	
    // Store results here
    String^ results;

    // Array languages used by the test
    auto ja = ref new Platform::Collections::Vector<String^>();
	ja->Append("ja-JP");
	auto he = ref new Platform::Collections::Vector<String^>();
	he->Append("he-IL");

	// Create a calendar based on the current user.
	Calendar^ cal1 = ref new Calendar();

	// Create a calendar using Japanese language and Japanese calendar.
	Calendar^ cal2 = ref new Calendar(ja, CalendarIdentifiers::Japanese, ClockIdentifiers::TwelveHour);

	// Create a calendar using Hebrew language and Hebrew calendar.
	Calendar^ cal3 = ref new Calendar(he, CalendarIdentifiers::Hebrew, ClockIdentifiers::TwentyFourHour);

	// Obtain the date parts using the default calendar system.
	String^ calItems1 = 
        "User's default calendar: " + cal1->GetCalendarSystem() + "\n" +
		"Name of Month: " + cal1->MonthAsSoloString() + "\n" +
		"Day of Month: " + cal1->DayAsPaddedString(2) + "\n" +
		"Day of Week: " + cal1->DayOfWeekAsString() + "\n" +
		"Year: " + cal1->YearAsString();

	// Obtain the date parts using the Japanese calendar system.
	String^ calItems2 = 
        "Calendar system: " + cal2->GetCalendarSystem() + "\n" +
		"Name of Month: " + cal2->MonthAsSoloString() + "\n" +
		"Day of Month: " + cal2->DayAsPaddedString(2) + "\n" +
		"Day of Week: " + cal2->DayOfWeekAsString() + "\n" +
		"Year: " + cal2->YearAsString();

	// Obtain the date parts using the Hebrew calendar system.
	String^ calItems3 = 
        "Calendar system: " + cal3->GetCalendarSystem() + "\n" +
		"Name of Month: " + cal3->MonthAsSoloString() + "\n" +
		"Day of Month: " + cal3->DayAsPaddedString(2) + "\n" +
		"Day of Week: " + cal3->DayOfWeekAsString() + "\n" +
		"Year: " + cal3->YearAsString();

	// Generate the results.
	results = calItems1 + "\n\n" + calItems2 + "\n\n" + calItems3;

    // Display the results
    rootPage->NotifyUser(results, NotifyType::StatusMessage);
}
