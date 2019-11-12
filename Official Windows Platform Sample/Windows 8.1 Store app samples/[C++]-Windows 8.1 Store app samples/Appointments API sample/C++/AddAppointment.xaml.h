//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// AddAppointment.xaml.h
// Declaration of the AddAppointment class
//

#pragma once
#include "AddAppointment.g.h"

namespace SDKSample
{
    namespace Appointments
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class AddAppointment sealed
        {
        public:
            AddAppointment();

        private:
            void Add_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            Windows::Foundation::Rect GetElementRect(FrameworkElement^ element);
        };
    }
}
