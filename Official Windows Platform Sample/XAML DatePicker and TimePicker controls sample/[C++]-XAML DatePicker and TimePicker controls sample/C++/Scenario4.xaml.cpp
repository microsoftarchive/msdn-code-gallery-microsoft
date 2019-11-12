// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario4.xaml.cpp
// Implementation of the Scenario4 class
//

#include "pch.h"
#include "Scenario4.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::DateAndTimePickers;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Scenario4::Scenario4()
{
	InitializeComponent();
	this->toggleYear->IsChecked = true;
}

void DateAndTimePickers::Scenario4::showDayOfWeek_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// Explicitly set day format
	this->datePicker->DayFormat = "{day.integer} ({dayofweek.full})";

	MainPage::Current->NotifyUser("DatePicker with format changed to include day of week", NotifyType::StatusMessage);
}

void DateAndTimePickers::Scenario4::showMonthAsNumber_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// Explicitly set month format
	this->datePicker->MonthFormat = "{month.integer}";

	MainPage::Current->NotifyUser("DatePicker with format changed to display month as a number", NotifyType::StatusMessage);
}

void DateAndTimePickers::Scenario4::toggleYear_Update(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// Explicitly set visibility of year
	if (this->toggleYear->IsChecked->Value)
	{
		this->datePicker->YearVisible = true;
		MainPage::Current->NotifyUser("DatePicker with visible year component", NotifyType::StatusMessage);
	}
	else
	{
		this->datePicker->YearVisible = false;
		MainPage::Current->NotifyUser("DatePicker without visible year component", NotifyType::StatusMessage);
	}

}
