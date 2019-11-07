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
// S2_SendToStorage.xaml.h
// Declaration of the S2_SendToStorage class
//

#pragma once

#include "pch.h"
#include "S2_SendToStorage.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace RemovableStorageCPP
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class S2_SendToStorage sealed
        {
        public:
            S2_SendToStorage();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            void SendImage_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ShowDeviceSelectorAsync();
            void DeviceList_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e);
            void SendImageFileToStorageAsync(Windows::Devices::Enumeration::DeviceInformation^ deviceInfoElement);
            void CopyFileToFolderOnStorageAsync(Windows::Storage::StorageFile^ sourceFile, Windows::Storage::StorageFolder^ storage);

            MainPage^ rootPage;
            // Contains the device information used for populating the device selection list
            Windows::Devices::Enumeration::DeviceInformationCollection^ _deviceInfoCollection;
        };
    }
}
