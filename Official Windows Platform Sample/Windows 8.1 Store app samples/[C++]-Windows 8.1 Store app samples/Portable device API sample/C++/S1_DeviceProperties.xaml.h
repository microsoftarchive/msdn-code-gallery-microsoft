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
// S1_DeviceProperties.xaml.h
// Declaration of the S1_DeviceProperties class
//

#pragma once

#include "pch.h"
#include "S1_DeviceProperties.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace PortableDeviceCPP
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class S1_DeviceProperties sealed
        {
        public:
            S1_DeviceProperties();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void Run_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ShowDeviceSelectorAsync();
            void DeviceList_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e);
            void DisplayDeviceProperties(Windows::Devices::Enumeration::DeviceInformation^ deviceInfoElement);

            MainPage^ rootPage;
            // Contains the device information used for populating the device selection list
            Windows::Devices::Enumeration::DeviceInformationCollection^ _deviceInfoCollection;
        };
    }
}
