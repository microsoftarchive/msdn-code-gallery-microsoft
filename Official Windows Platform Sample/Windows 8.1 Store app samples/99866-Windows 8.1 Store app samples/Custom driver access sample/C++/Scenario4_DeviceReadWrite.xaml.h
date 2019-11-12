//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3.xaml.h
// Declaration of the Scenario3 class
//

#pragma once
#include "Scenario4_DeviceReadWrite.g.h"

namespace SDKSample
{
    namespace CustomDeviceAccess
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class DeviceReadWrite sealed
        {
        public:
            DeviceReadWrite();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;

            unsigned int writeCounter;
            unsigned int readCounter;

            void ReadBlock_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void WriteBlock_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void LogMessage(const std::wstringstream& stream);
        };

#define LOG_MESSAGE(...) {std::wstringstream _s;    \
                          _s << __VA_ARGS__;        \
                          LogMessage(_s);}


    }
}

