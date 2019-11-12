//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// DeviceEvents.xaml.h
// Declaration of the DeviceEvents class
//

#pragma once

#include "pch.h"
#include "Scenario3_DeviceEvents.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace CustomDeviceAccess
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class DeviceEvents sealed
        {
        public:
            DeviceEvents();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Windows::Foundation::EventRegistrationToken deviceClosingHandler;
    
            bool switchChangedEventsRegistered;
            Windows::Foundation::EventRegistrationToken switchChangedEventsHandler;
            Platform::Array<bool>^ previousSwitchValues;
    
            void deviceEventsGet_Click_1(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void deviceEventsRegister_Click_1(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            
            void RegisterForSwitchStateChangedEvent(bool r);
    
            void UpdateRegisterButton(void);
            void OnSwitchStateChangedEvent(Samples::Devices::Fx2::Fx2Device^ sender, Samples::Devices::Fx2::SwitchStateChangedEventArgs^ e);
    
            void ClearSwitchStateTable();
            void UpdateSwitchStateTable(Platform::Array<bool>^ SwitchStateArray);
    
            void Current_DeviceClosing(Platform::Object^ sender, Samples::Devices::Fx2::Fx2Device^ device);
        };
    }
}
