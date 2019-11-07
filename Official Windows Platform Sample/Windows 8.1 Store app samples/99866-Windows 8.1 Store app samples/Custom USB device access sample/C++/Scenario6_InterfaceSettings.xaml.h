//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario6_InterfaceSettings.xaml.h
// Declaration of the Scenario6_InterfaceSettings class
//

#pragma once
#include "Scenario6_InterfaceSettings.g.h"

namespace SDKSample
{
    namespace CustomUsbDeviceAccess
    {
        /// <summary>
        /// Scenario will demonstrate how to change interface settings (also known as alternate interface setting).
        /// 
        /// This scenario can work for any device as long as it is "added" into the sample. For more information on how to add a 
        /// device to make it compatible with this scenario, please see Scenario1_DeviceConnect.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class InterfaceSettings sealed
        {
        public:
            InterfaceSettings(void);
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ eventArgs) override;
        private:
            void SetSuperMuttInterfaceSetting_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void GetInterfaceSetting_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            
            void SetInterfaceSetting(uint8 settingNumber);
            void GetInterfaceSetting(void);
        };
    }
}
