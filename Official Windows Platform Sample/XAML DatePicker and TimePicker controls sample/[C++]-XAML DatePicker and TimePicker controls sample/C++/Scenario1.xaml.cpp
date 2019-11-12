// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::DateAndTimePickers;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;

Scenario1::Scenario1()
{
	InitializeComponent();

	MainPage::Current->NotifyUser("No selection changes made", NotifyType::StatusMessage);
}

void DateAndTimePickers::Scenario1::datePicker_DateChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::DatePickerValueChangedEventArgs^ e)
{
	// The DateTimeFormatter class formats dates and times with the user's default settings
	DateTimeFormatter^ dateFormatter = ref new DateTimeFormatter("shortdate");

	MainPage::Current->NotifyUser("Date changed to " + dateFormatter->Format(e->NewDate), NotifyType::StatusMessage);
}
void DateAndTimePickers::Scenario1::timePicker_TimeChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::TimePickerValueChangedEventArgs^ e)
{
	// TimeSpan.Duration is represented in 100-nanosecond units
	// We convert to hours and minutes
	int64 selectedTimeInSeconds = e->NewTime.Duration/10000000;
	int64 hours = selectedTimeInSeconds/3600;
	int64 minutes = (selectedTimeInSeconds % 3600)/60;

	// Prepare output string with leading zeroes
	Platform::String^ minutesAsString = "";
	if (minutes < 10)
	{
		minutesAsString += "0" + minutes;
	}
	else
	{
		minutesAsString += minutes;
	}

	MainPage::Current->NotifyUser("Time changed to " + hours + ":" + minutesAsString, NotifyType::StatusMessage);
}
