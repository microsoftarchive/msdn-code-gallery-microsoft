//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// RemoveAppointment.xaml.h
// Declaration of the RemoveAppointment class
//

#pragma once
#include "RemoveAppointment.g.h"

namespace SDKSample
{
    namespace Appointments
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class RemoveAppointment sealed
        {
        public:
            RemoveAppointment();

        private:
            void Remove_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            Windows::Foundation::Rect GetElementRect(FrameworkElement^ element);
        };
    }
}
