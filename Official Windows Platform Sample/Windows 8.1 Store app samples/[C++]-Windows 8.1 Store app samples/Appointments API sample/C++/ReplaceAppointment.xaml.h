//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ReplaceAppointment.xaml.h
// Declaration of the ReplaceAppointment class
//

#pragma once
#include "ReplaceAppointment.g.h"

namespace SDKSample
{
    namespace Appointments
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class ReplaceAppointment sealed
        {
        public:
            ReplaceAppointment();

        private:
            void Replace_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            Windows::Foundation::Rect GetElementRect(FrameworkElement^ element);
        };
    }
}
