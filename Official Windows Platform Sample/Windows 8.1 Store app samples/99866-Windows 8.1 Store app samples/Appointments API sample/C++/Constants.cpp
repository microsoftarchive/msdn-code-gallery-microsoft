//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#include "pch.h"
#include "MainPage.xaml.h"
#include "Constants.h"

using namespace SDKSample;
using namespace SDKSample::Appointments;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    { "Create an Appointment", "SDKSample.Appointments.AppointmentProperties" },
    { "Add an Appointment", "SDKSample.Appointments.AddAppointment" },
    { "Replace an Appointment", "SDKSample.Appointments.ReplaceAppointment" },
    { "Remove an Appointment", "SDKSample.Appointments.RemoveAppointment" },
    { "Show Time Frame", "SDKSample.Appointments.ShowTimeFrame" },
    { "Recurring Appointments", "SDKSample.Appointments.Recurrence" }
};
