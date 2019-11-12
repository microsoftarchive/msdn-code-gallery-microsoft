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
// S4_GetFromDevice.xaml.h
// Declaration of the S4_GetFromDevice class
//

#pragma once

#include "pch.h"
#include "S4_GetFromDevice.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace PortableDeviceCPP
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class S4_GetFromDevice sealed
        {
        public:
            S4_GetFromDevice();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void Run_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ShowDeviceSelectorAsync();
            void DeviceList_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e);
            void GetContentFromDeviceAsync(Windows::Devices::Enumeration::DeviceInformation^ deviceInfoElement);
            void FindAndTransferFirstImageFileFromDevice(_In_ IPortableDevice* device);
            void TransferFileFromDeviceToAppLocalFolderAsync(_In_ IPortableDevice* device, Platform::String^ sourceObjectId, Platform::String^ desiredFilename);

            MainPage^ rootPage;
            // Contains the device information used for populating the device selection list
            Windows::Devices::Enumeration::DeviceInformationCollection^ _deviceInfoCollection;
        };
    }
}
