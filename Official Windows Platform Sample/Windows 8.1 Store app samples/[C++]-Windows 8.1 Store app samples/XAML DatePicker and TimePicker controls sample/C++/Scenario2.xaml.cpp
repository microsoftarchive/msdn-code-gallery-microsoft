// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::DateAndTimePickers;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;

Scenario2::Scenario2()
{
	InitializeComponent();
}

void DateAndTimePickers::Scenario2::changeDate_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// The Calendar class is used to explicitly set date components of a DateTime
	Calendar^ calendar = ref new Calendar();

	// The DateTimeFormatter class formats dates and times with the user's default settings
	DateTimeFormatter^ dateFormatter = ref new DateTimeFormatter("shortdate");

	calendar->Year = 2013;
	calendar->Month = 1;
	calendar->Day = 31;
	datePicker->Date = calendar->GetDateTime();

	MainPage::Current->NotifyUser("DatePicker date set to " + dateFormatter->Format(datePicker->Date), NotifyType::StatusMessage);
}
void DateAndTimePickers::Scenario2::changeYearRange_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// The Calendar class is used to explicitly set date components of a DateTime
	Calendar^ calendar = ref new Calendar();
	calendar->Year = 2000;

	// MinYear and MaxYear are type DateTime. We set the month to 2 to avoid time zone issues with January 1. 
	calendar->Month = 2;
	datePicker->MinYear = calendar->GetDateTime();

	calendar->Year = 2020;
	datePicker->MaxYear = calendar->GetDateTime();

	MainPage::Current->NotifyUser("DatePicker year range set from 2000 to 2020", NotifyType::StatusMessage);
}