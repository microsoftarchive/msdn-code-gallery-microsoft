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
#include "AddAppointment.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::Appointments;

using namespace concurrency;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

AddAppointment::AddAppointment()
{
    InitializeComponent();
}

void Appointments::AddAppointment::Add_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Create an Appointment that should be added the user's appointments provider app.
    auto appointment = ref new Windows::ApplicationModel::Appointments::Appointment();

    // Get the selection rect of the button pressed to add this appointment
    auto const rect = GetElementRect(safe_cast<FrameworkElement^>(sender));

    // ShowAddAppointmentAsync returns an appointment id if the appointment given was added to the user's calendar.
    // This value should be stored in app data and roamed so that the appointment can be replaced or removed in the future.
    // An empty string return value indicates that the user canceled the operation before the appointment was added.
    create_task(Windows::ApplicationModel::Appointments::AppointmentManager::ShowAddAppointmentAsync(appointment, rect, Windows::UI::Popups::Placement::Default))
        .then([=](String^ appointmentId)
        {
            if (!appointmentId->IsEmpty())
            {
                ResultTextBlock->Text = L"Appointment Id: " + appointmentId;
            }
            else
            {
                ResultTextBlock->Text = L"Appointment not added";
            }
        });
}

Windows::Foundation::Rect Appointments::AddAppointment::GetElementRect(FrameworkElement^ element)
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
