// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario3.xaml.cpp
// Implementation of the Scenario3 class
//

#include "pch.h"
#include "Scenario3.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::DateAndTimePickers;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;

Scenario3::Scenario3()
{
	InitializeComponent();
}

void DateAndTimePickers::Scenario3::combine_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Calendar^ calendar = ref new Calendar();

	// The DateTimeFormatter class formats dates and times with the user's default settings
	DateTimeFormatter^ dateFormatter = ref new DateTimeFormatter("shortdate");
	DateTimeFormatter^ timeFormatter = ref new DateTimeFormatter("shorttime");

	// We use a calendar to determine daylight savings time transition days
	calendar->SetDateTime(datePicker->Date);

	// Resets the calendar's time to midnight, then adds the TimePicker value
	calendar->ChangeClock("24HourClock");
	calendar->Hour = 0;
	calendar->Minute = 0;
	calendar->AddSeconds((int)(timePicker->Time.Duration/10000000));

	// If the day does not have 24 hours, then the user has selected a day in which a Daylight Savings Time transition occurs.
	//    It is the app developer's responsibility for validating the combination of the date and time values.
	if (calendar->NumberOfHoursInThisPeriod != 24)
	{
		MainPage::Current->NotifyUser("You selected a DST transition date", NotifyType::StatusMessage);
	}
	else
	{
		MainPage::Current->NotifyUser("Combined value: " + dateFormatter->Format(calendar->GetDateTime()) + " " + timeFormatter->Format(calendar->GetDateTime()), NotifyType::StatusMessage);
	}
}
