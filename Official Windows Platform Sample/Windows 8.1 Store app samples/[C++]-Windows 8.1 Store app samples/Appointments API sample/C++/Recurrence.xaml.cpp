//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Recurrence.xaml.cpp
// Implementation of the Recurrence class
//

#include "pch.h"
#include "Recurrence.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Appointments;

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

Recurrence::Recurrence()
{
    InitializeComponent();
}

void Appointments::Recurrence::Create_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    bool isRecurrenceValid = true;
    auto recurrence = ref new Windows::ApplicationModel::Appointments::AppointmentRecurrence();

    // Unit
    switch (UnitComboBox->SelectedIndex)
    {
        case 0:
            recurrence->Unit = Windows::ApplicationModel::Appointments::AppointmentRecurrenceUnit::Daily;
            break;
        case 1:
            recurrence->Unit = Windows::ApplicationModel::Appointments::AppointmentRecurrenceUnit::Weekly;
            break;
        case 2:
            recurrence->Unit = Windows::ApplicationModel::Appointments::AppointmentRecurrenceUnit::Monthly;
            break;
        case 3:
            recurrence->Unit = Windows::ApplicationModel::Appointments::AppointmentRecurrenceUnit::MonthlyOnDay;
            break;
        case 4:
            recurrence->Unit = Windows::ApplicationModel::Appointments::AppointmentRecurrenceUnit::Yearly;
            break;
        case 5:
            recurrence->Unit = Windows::ApplicationModel::Appointments::AppointmentRecurrenceUnit::YearlyOnDay;
            break;
    }

    // Occurrences
    // Note: Occurrences and Until properties are mutually exclusive.
    if (OccurrencesRadioButton->IsChecked->Value)
    {
        recurrence->Occurrences = static_cast<unsigned int>(OccurrencesSlider->Value);
    }

    // Until
    // Note: Until and Occurrences properties are mutually exclusive.
    if (UntilRadioButton->IsChecked->Value)
    {
        recurrence->Until = UntilDatePicker->Date;
    }

    // Interval
    recurrence->Interval = static_cast<unsigned int>(IntervalSlider->Value);

    // Week of the month
    switch (WeekOfMonthComboBox->SelectedIndex)
    {
        case 0:
            recurrence->WeekOfMonth = Windows::ApplicationModel::Appointments::AppointmentWeekOfMonth::First;
            break;
        case 1:
            recurrence->WeekOfMonth = Windows::ApplicationModel::Appointments::AppointmentWeekOfMonth::Second;
            break;
        case 2:
            recurrence->WeekOfMonth = Windows::ApplicationModel::Appointments::AppointmentWeekOfMonth::Third;
            break;
        case 3:
            recurrence->WeekOfMonth = Windows::ApplicationModel::Appointments::AppointmentWeekOfMonth::Fourth;
            break;
        case 4:
            recurrence->WeekOfMonth = Windows::ApplicationModel::Appointments::AppointmentWeekOfMonth::Last;
            break;
    }

    // Days of the Week
    // Note: For Weekly, MonthlyOnDay or YearlyOnDay recurrence unit values, at least one day must be specified.
    if (SundayCheckBox->IsChecked->Value) { recurrence->DaysOfWeek = (recurrence->DaysOfWeek | Windows::ApplicationModel::Appointments::AppointmentDaysOfWeek::Sunday); }
    if (MondayCheckBox->IsChecked->Value) { recurrence->DaysOfWeek = (recurrence->DaysOfWeek | Windows::ApplicationModel::Appointments::AppointmentDaysOfWeek::Monday); }
    if (TuesdayCheckBox->IsChecked->Value) { recurrence->DaysOfWeek = (recurrence->DaysOfWeek | Windows::ApplicationModel::Appointments::AppointmentDaysOfWeek::Tuesday); }
    if (WednesdayCheckBox->IsChecked->Value) { recurrence->DaysOfWeek = (recurrence->DaysOfWeek | Windows::ApplicationModel::Appointments::AppointmentDaysOfWeek::Wednesday); }
    if (ThursdayCheckBox->IsChecked->Value) { recurrence->DaysOfWeek = (recurrence->DaysOfWeek | Windows::ApplicationModel::Appointments::AppointmentDaysOfWeek::Thursday); }
    if (FridayCheckBox->IsChecked->Value) { recurrence->DaysOfWeek = (recurrence->DaysOfWeek | Windows::ApplicationModel::Appointments::AppointmentDaysOfWeek::Friday); }
    if (SaturdayCheckBox->IsChecked->Value) { recurrence->DaysOfWeek = (recurrence->DaysOfWeek | Windows::ApplicationModel::Appointments::AppointmentDaysOfWeek::Saturday); }

    if (((recurrence->Unit == Windows::ApplicationModel::Appointments::AppointmentRecurrenceUnit::Weekly) ||
         (recurrence->Unit == Windows::ApplicationModel::Appointments::AppointmentRecurrenceUnit::MonthlyOnDay) ||
         (recurrence->Unit == Windows::ApplicationModel::Appointments::AppointmentRecurrenceUnit::YearlyOnDay)) &&
        (recurrence->DaysOfWeek == Windows::ApplicationModel::Appointments::AppointmentDaysOfWeek::None))
    {
        isRecurrenceValid = false;
        ResultTextBlock->Text = L"The recurrence specified is invalid. For Weekly, MonthlyOnDay or YearlyOnDay recurrence unit values, at least one day must be specified.";
    }

    // Month of the year
    recurrence->Month = static_cast<unsigned int>(MonthSlider->Value);

    // Day of the month
    recurrence->Day = static_cast<unsigned int>(DaySlider->Value);

    if (isRecurrenceValid)
    {
        ResultTextBlock->Text = L"The recurrence specified was created successfully and is valid.";
    }
}
