//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// RemoveAppointment.xaml.cpp
// Implementation of the RemoveAppointment class
//

#include "pch.h"
#include "RemoveAppointment.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Appointments;

using namespace concurrency;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

RemoveAppointment::RemoveAppointment()
{
    InitializeComponent();
}

void Appointments::RemoveAppointment::Remove_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // The appointment id argument for ShowRemoveAppointmentAsync is typically retrieved from AddAppointmentAsync and stored in app data.
    String^ appointmentId = AppointmentIdTextBox->Text;

    // The appointment id cannot be empty.
    if (appointmentId->IsEmpty())
    {
        ResultTextBlock->Text = "The appointment id cannot be empty";
    }
    else
    {
        // Get the selection rect of the button pressed to remove this appointment
        auto const rect = GetElementRect(safe_cast<FrameworkElement^>(sender));

        // ShowRemoveAppointmentAsync returns a boolean indicating whether or not the appointment related to the appointment id given was removed.
        // An optional instance start time can be provided to indicate that a specific instance on that date should be removed
        // in the case of a recurring appointment.
        if (InstanceStartDateCheckBox->IsChecked->Value)
        {
            // Remove a specific instance starting on the date provided.
            auto instanceStartDate = InstanceStartDateDatePicker->Date;
            create_task(Windows::ApplicationModel::Appointments::AppointmentManager::ShowRemoveAppointmentAsync(appointmentId, rect, Windows::UI::Popups::Placement::Default, instanceStartDate))
                .then([=](bool removed)
                {
                    if (removed)
                    {
                        ResultTextBlock->Text = "Appointment removed";
                    }
                    else
                    {
                        ResultTextBlock->Text = "Appointment not removed";
                    }
                });
        }
        else
        {
            // Remove an appointment that occurs only once or in the case of a recurring appointment, replace the entire series.
            create_task(Windows::ApplicationModel::Appointments::AppointmentManager::ShowRemoveAppointmentAsync(appointmentId, rect, Windows::UI::Popups::Placement::Default))
                .then([=](bool removed)
                {
                    if (removed)
                    {
                        ResultTextBlock->Text = "Appointment removed";
                    }
                    else
                    {
                        ResultTextBlock->Text = "Appointment not removed";
                    }
                });
        }
    }
}

Windows::Foundation::Rect Appointments::RemoveAppointment::GetElementRect(FrameworkElement^ element)
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
