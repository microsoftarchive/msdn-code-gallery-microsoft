//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// ShowTimeFrame.xaml.h
// Declaration of the ShowTimeFrame class
//

#pragma once
#include "ShowTimeFrame.g.h"

namespace SDKSample
{
    namespace Appointments
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class ShowTimeFrame sealed
        {
        public:
            ShowTimeFrame();

        private:
            void Show_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
