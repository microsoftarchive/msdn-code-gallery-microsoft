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
// S5_Events.xaml.h
// Declaration of the S5_Events class
//

#pragma once

#include "pch.h"
#include "S5_Events.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace PortableDeviceCPP
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class S5_Events sealed
        {
        public:
            S5_Events();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void Run_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Trigger_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ShowDeviceSelectorAsync();
            void DeviceList_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e);
            void RegisterForDeviceEvents(Windows::Devices::Enumeration::DeviceInformation^ deviceInfo);
            void UnregisterForDeviceEvents();
            bool IsRegisteredForDeviceEvents()
            {
                return ((_deviceEventSource != nullptr) && (_deviceEventCookie != nullptr));
            }
            void OnDeviceEventReceived(Platform::String^ senderDeviceId, Platform::String^ eventData);
            void CreateAndThenDeleteFolderOnDeviceAsync();
            Microsoft::WRL::ComPtr<IPortableDeviceValues> FillInPropertiesForFolder(Platform::String^ parentId);

            MainPage^ rootPage;
            // Contains the device information used for populating the device selection list
            Windows::Devices::Enumeration::DeviceInformationCollection^ _deviceInfoCollection;
            // Contains the device event registration cookie
            Platform::String^ _deviceEventCookie;
            // Contains the device event source
            Microsoft::WRL::ComPtr<IPortableDevice> _deviceEventSource;
        };
    }
}
