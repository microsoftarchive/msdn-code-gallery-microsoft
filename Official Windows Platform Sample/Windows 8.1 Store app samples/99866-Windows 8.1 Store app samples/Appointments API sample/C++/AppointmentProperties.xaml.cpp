//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// AppointmentProperties.xaml.cpp
// Implementation of the AppointmentProperties class
//

#include "pch.h"
#include "AppointmentProperties.xaml.h"
#include "MainPage.xaml.h"
#include <intsafe.h>

using namespace SDKSample;
using namespace SDKSample::Appointments;

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

AppointmentProperties::AppointmentProperties()
{
    InitializeComponent();
}

void Appointments::AppointmentProperties::Create_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    bool isAppointmentValid = true;
    auto appointment = ref new Windows::ApplicationModel::Appointments::Appointment();

    // StartTime
    Windows::Globalization::Calendar^ startTimeCal = ref new Windows::Globalization::Calendar();
    startTimeCal->SetDateTime(StartTimeDatePicker->Date);

    // Convert the start time TimeSpan duration from 100-nanosecond units to minutes
    Windows::Foundation::TimeSpan minutesSpan = StartTimeTimePicker->Time;
    ULONGLONG minutesSpanNanoseconds = 0;
    HRESULT hr = ULongLongMult(minutesSpan.Duration, 100, &minutesSpanNanoseconds);
    if (SUCCEEDED(hr))
    {
        int minutes = 0;
        hr = UInt64ToInt((minutesSpanNanoseconds) / (60LL * 1000000000), &minutes);
        if (SUCCEEDED(hr))
        {
            startTimeCal->Minute = 0;
            startTimeCal->AddMinutes(minutes);
            appointment->StartTime =  startTimeCal->GetDateTime();
        }
    }

    // Subject
    appointment->Subject = SubjectTextBox->Text;

    if (appointment->Subject->Length() > 255)
    {
        isAppointmentValid = false;
        ResultTextBlock->Text = L"The subject cannot be greater than 255 characters.";
    }

    // Location
    appointment->Location = LocationTextBox->Text;

    if (appointment->Location->Length() > 32768)
    {
        isAppointmentValid = false;
        ResultTextBlock->Text = L"The location cannot be greater than 32,768 characters.";
    }

    // Details
    appointment->Details = DetailsTextBox->Text;

    if (appointment->Details->Length() > 1073741823)
    {
        isAppointmentValid = false;
        ResultTextBlock->Text = L"The details cannot be greater than 1,073,741,823 characters.";
    }

    // Duration
    if (DurationComboBox->SelectedIndex == 0)
    {
        // 30 minute duration is selected
        Windows::Foundation::TimeSpan span;
        span.Duration = (30LL * 60 * 1000000000) / 100;
        appointment->Duration = span;
    }
    else
    {
        // 1 hour duration is selected
        Windows::Foundation::TimeSpan span;
        span.Duration = (60LL * 60 * 1000000000) / 100;
        appointment->Duration = span;
    }

    // All Day
    appointment->AllDay = AllDayCheckBox->IsChecked->Value;

    // Reminder
    if (ReminderCheckBox->IsChecked->Value)
    {
        Windows::Foundation::TimeSpan span;
        switch (ReminderComboBox->SelectedIndex)
        {
            case 0:
                span.Duration = (15LL * 60 * 1000000000) / 100;
                appointment->Reminder = span;
                break;
            case 1:
                span.Duration = (60LL * 60 * 1000000000) / 100;
                appointment->Reminder = span;
                break;
            case 2:
                span.Duration = (24LL * 60 * 60 * 1000000000) / 100;
                appointment->Reminder = span;
                break;
        }
    }

    //Busy Status
    switch (BusyStatusComboBox->SelectedIndex)
    {
        case 0:
            appointment->BusyStatus = Windows::ApplicationModel::Appointments::AppointmentBusyStatus::Busy;
            break;
        case 1:
            appointment->BusyStatus = Windows::ApplicationModel::Appointments::AppointmentBusyStatus::Tentative;
            break;
        case 2:
            appointment->BusyStatus = Windows::ApplicationModel::Appointments::AppointmentBusyStatus::Free;
            break;
        case 3:
            appointment->BusyStatus = Windows::ApplicationModel::Appointments::AppointmentBusyStatus::OutOfOffice;
            break;
        case 4:
            appointment->BusyStatus = Windows::ApplicationModel::Appointments::AppointmentBusyStatus::WorkingElsewhere;
            break;
    }

    // Sensitivity
    switch (SensitivityComboBox->SelectedIndex)
    {
        case 0:
            appointment->Sensitivity = Windows::ApplicationModel::Appointments::AppointmentSensitivity::Public;
            break;
        case 1:
            appointment->Sensitivity = Windows::ApplicationModel::Appointments::AppointmentSensitivity::Private;
            break;
    }

    // Uri
    if (UriTextBox->Text->Length() > 0)
    {
        try
        {
            appointment->Uri = ref new Windows::Foundation::Uri(UriTextBox->Text);
        }
        catch (Exception^)
        {
            isAppointmentValid = false;
            ResultTextBlock->Text = L"The Uri provided is invalid.";
        }
    }

    // Organizer
    // Note: Organizer can only be set if there are no invitees added to this appointment.
    if (OrganizerRadioButton->IsChecked->Value)
    {
        auto organizer = ref new Windows::ApplicationModel::Appointments::AppointmentOrganizer();

        // Organizer Display Name
        organizer->DisplayName = OrganizerDisplayNameTextBox->Text;

        if (organizer->DisplayName->Length() > 256)
        {
            isAppointmentValid = false;
            ResultTextBlock->Text = L"The organizer display name cannot be greater than 256 characters.";
        }
        else
        {
            // Organizer Address (e.g. Email Address)
            organizer->Address = OrganizerAddressTextBox->Text;

            if (organizer->Address->Length() > 321)
            {
                isAppointmentValid = false;
                ResultTextBlock->Text = L"The organizer address cannot be greater than 321 characters.";
            }
            else if (organizer->Address->Length() == 0)
            {
                isAppointmentValid = false;
                ResultTextBlock->Text = L"The organizer address must be greater than 0 characters.";
            }
            else
            {
                appointment->Organizer = organizer;
            }
        }
    }

    // Invitees
    // Note: If the size of the Invitees list is not zero, then an Organizer cannot be set.
    if (InviteeRadioButton->IsChecked->Value)
    {
        auto invitee = ref new Windows::ApplicationModel::Appointments::AppointmentInvitee();

        // Invitee Display Name
        invitee->DisplayName = InviteeDisplayNameTextBox->Text;

        if (invitee->DisplayName->Length() > 256)
        {
            isAppointmentValid = false;
            ResultTextBlock->Text = L"The invitee display name cannot be greater than 256 characters.";
        }
        else
        {
            // Invitee Address (e.g. Email Address)
            invitee->Address = InviteeAddressTextBox->Text;

            if (invitee->Address->Length() > 321)
            {
                isAppointmentValid = false;
                ResultTextBlock->Text = L"The invitee address cannot be greater than 321 characters.";
            }
            else if (invitee->Address->Length() == 0)
            {
                isAppointmentValid = false;
                ResultTextBlock->Text = L"The invitee address must be greater than 0 characters.";
            }
            else
            {
                // Invitee Role
                switch (RoleComboBox->SelectedIndex)
                {
                    case 0:
                        invitee->Role = Windows::ApplicationModel::Appointments::AppointmentParticipantRole::RequiredAttendee;
                        break;
                    case 1:
                        invitee->Role = Windows::ApplicationModel::Appointments::AppointmentParticipantRole::OptionalAttendee;
                        break;
                    case 2:
                        invitee->Role = Windows::ApplicationModel::Appointments::AppointmentParticipantRole::Resource;
                        break;
                }

                // Invitee Response
                switch (ResponseComboBox->SelectedIndex)
                {
                    case 0:
                        invitee->Response = Windows::ApplicationModel::Appointments::AppointmentParticipantResponse::None;
                        break;
                    case 1:
                        invitee->Response = Windows::ApplicationModel::Appointments::AppointmentParticipantResponse::Tentative;
                        break;
                    case 2:
                        invitee->Response = Windows::ApplicationModel::Appointments::AppointmentParticipantResponse::Accepted;
                        break;
                    case 3:
                        invitee->Response = Windows::ApplicationModel::Appointments::AppointmentParticipantResponse::Declined;
                        break;
                    case 4:
                        invitee->Response = Windows::ApplicationModel::Appointments::AppointmentParticipantResponse::Unknown;
                        break;
                }

                appointment->Invitees->Append(invitee);
            }
        }
    }

    if (isAppointmentValid)
    {
        ResultTextBlock->Text = L"The appointment was created successfully and is valid.";
    }
}

void Appointments::AppointmentProperties::OrganizerRadioButton_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    OrganizerStackPanel->Visibility = Windows::UI::Xaml::Visibility::Visible;
    InviteeStackPanel->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
}

void Appointments::AppointmentProperties::InviteeRadioButton_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    InviteeStackPanel->Visibility = Windows::UI::Xaml::Visibility::Visible;
    OrganizerStackPanel->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
}

void Appointments::AppointmentProperties::ReminderCheckBox_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ReminderComboBox->Visibility = Windows::UI::Xaml::Visibility::Visible;
}

void Appointments::AppointmentProperties::ReminderCheckBox_UnChecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    ReminderComboBox->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
}