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
// S3_SendToDevice.xaml.h
// Declaration of the S3_SendToDevice class
//

#pragma once

#include "pch.h"
#include "S3_SendToDevice.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace PortableDeviceCPP
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class S3_SendToDevice sealed
        {
        public:
            S3_SendToDevice();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void Run_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ShowDeviceSelectorAsync();
            void DeviceList_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e);
            void SendContentToDeviceAsync(Windows::Devices::Enumeration::DeviceInformation^ deviceInfoElement);
            void TransferFileToDeviceAsync(Windows::Storage::StorageFile^ file, _In_ IPortableDevice* device);
            Microsoft::WRL::ComPtr<IPortableDeviceValues> FillInPropertiesForImage(
                Windows::Storage::StorageFile^ file, Platform::String^ parentId, ULONGLONG size);

            MainPage^ rootPage;
            // Contains the device information used for populating the device selection list
            Windows::Devices::Enumeration::DeviceInformationCollection^ _deviceInfoCollection;
        };
    }
}
