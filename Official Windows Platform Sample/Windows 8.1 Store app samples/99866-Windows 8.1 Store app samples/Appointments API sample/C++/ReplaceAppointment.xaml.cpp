//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// AddAppointment.xaml.cpp
// Implementation of the AddAppointment class
//

#include "pch.h"
#include "ReplaceAppointment.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Appointments;

using namespace concurrency;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

ReplaceAppointment::ReplaceAppointment()
{
    InitializeComponent();
}

void Appointments::ReplaceAppointment::Replace_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // The appointment id argument for ReplaceAppointmentAsync is typically retrieved from AddAppointmentAsync and stored in app data.
    String^ appointmentIdOfAppointmentToReplace = AppointmentIdTextBox->Text;

    if (appointmentIdOfAppointmentToReplace->IsEmpty())
    {
        ResultTextBlock->Text = "The appointment id cannot be empty";
    }
    else
    {
        // The Appointment argument for ReplaceAppointmentAsync should contain all of the Appointment's properties including those that may have changed.
        auto appointment = ref new Windows::ApplicationModel::Appointments::Appointment();

        // Get the selection rect of the button pressed to replace this appointment
        auto const rect = GetElementRect(safe_cast<FrameworkElement^>(sender));

        // ReplaceAppointmentAsync returns an updated appointment id when the appointment was successfully replaced.
        // The updated id may or may not be the same as the original one retrieved from AddAppointmentAsync.
        // An optional instance start time can be provided to indicate that a specific instance on that date should be replaced
        // in the case of a recurring appointment.
        // If the appointment id returned is an empty string, that indicates that the appointment was not replaced.
        if (InstanceStartDateCheckBox->IsChecked->Value)
        {
            // Replace a specific instance starting on the date provided.
            auto instanceStartDate = InstanceStartDateDatePicker->Date;
            create_task(Windows::ApplicationModel::Appointments::AppointmentManager::ShowReplaceAppointmentAsync(appointmentIdOfAppointmentToReplace, appointment, rect, Windows::UI::Popups::Placement::Default, instanceStartDate))
                .then([=](String^ updatedAppointmentId)
                {
                    if (!updatedAppointmentId->IsEmpty())
                    {
                        ResultTextBlock->Text = L"Updated Appointment Id: " + updatedAppointmentId;
                    }
                    else
                    {
                        ResultTextBlock->Text = L"Appointment not replaced";
                    }
                });
        }
        else
        {
            // Replace an appointment that occurs only once or in the case of a recurring appointment, replace the entire series.
            create_task(Windows::ApplicationModel::Appointments::AppointmentManager::ShowReplaceAppointmentAsync(appointmentIdOfAppointmentToReplace, appointment, rect, Windows::UI::Popups::Placement::Default))
                .then([=](String^ updatedAppointmentId)
                {
                    if (!updatedAppointmentId->IsEmpty())
                    {
                        ResultTextBlock->Text = L"Updated Appointment Id: " + updatedAppointmentId;
                    }
                    else
                    {
                        ResultTextBlock->Text = L"Appointment not replaced";
                    }
                });
        }
    }
}

Windows::Foundation::Rect Appointments::ReplaceAppointment::GetElementRect(FrameworkElement^ element)
{
    Windows::UI::Xaml::Media::GeneralTransform^ buttonTransform = element->TransformToVisual(nullptr);
    const Windows::Foundation::Point pointOrig(0, 0);
    const Windows::Foundation::Point pointTransformed = buttonTransform->TransformPoint(pointOrig);
    const Windows::Foundation::Rect rect(pointTransformed.X,
                    pointTransformed.Y,
                    safe_cast<float>(element->ActualWidth),
                    safe_cast<float>(element->ActualHeight));
    return rect;
}
