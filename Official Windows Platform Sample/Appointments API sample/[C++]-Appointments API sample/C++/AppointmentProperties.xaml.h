//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// AppointmentProperties.xaml.h
// Declaration of the AppointmentProperties class
//

#pragma once
#include "AppointmentProperties.g.h"

namespace SDKSample
{
    namespace Appointments
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class AppointmentProperties sealed
        {
        public:
            AppointmentProperties();

        private:
            void Create_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void OrganizerRadioButton_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void InviteeRadioButton_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ReminderCheckBox_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ReminderCheckBox_UnChecked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
