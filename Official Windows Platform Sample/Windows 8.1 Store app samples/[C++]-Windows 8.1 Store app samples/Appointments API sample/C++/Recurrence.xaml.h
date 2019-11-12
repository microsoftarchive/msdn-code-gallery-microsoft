//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Recurrence.xaml.h
// Declaration of the Recurrence class
//

#pragma once
#include "Recurrence.g.h"

namespace SDKSample
{
    namespace Appointments
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Recurrence sealed
        {
        public:
            Recurrence();

        private:
            void Create_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
