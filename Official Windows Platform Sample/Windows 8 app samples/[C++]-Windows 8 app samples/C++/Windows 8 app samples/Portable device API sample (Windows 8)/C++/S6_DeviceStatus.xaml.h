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
// S6_DeviceStatus.xaml.h
// Declaration of the S6_DeviceStatus class
//

#pragma once

#include "pch.h"
#include "S6_DeviceStatus.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace PortableDeviceCPP
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class S6_DeviceStatus sealed
        {
        public:
            S6_DeviceStatus();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void Run_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ShowDeviceSelectorAsync();
            void DeviceList_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e);
            void DisplayDeviceStatusAsync(Windows::Devices::Enumeration::DeviceInformation^ deviceInfoElement);
            void DisplayDeviceStatusServiceProperties(_In_ IPortableDeviceService* service);

            MainPage^ rootPage;
            // Contains the device information used for populating the device selection list
            Windows::Devices::Enumeration::DeviceInformationCollection^ _deviceInfoCollection;
        };

        // Delegate for IPortableDeviceServiceOpenCallback::OnComplete
        [uuid("bced49c8-8efe-41ed-960b-61313abd47a9")]
        delegate void OnServiceOpenComplete(HRESULT result);
    }
}
