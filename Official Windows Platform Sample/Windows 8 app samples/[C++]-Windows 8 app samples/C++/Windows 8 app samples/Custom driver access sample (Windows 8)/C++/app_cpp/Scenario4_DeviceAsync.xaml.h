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
// DeviceAsync.xaml.h
// Declaration of the DeviceAsync class
//

#pragma once

#include "pch.h"
#include "Scenario4_DeviceAsync.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace CustomDeviceAccess
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class DeviceAsync sealed
        {
        public:
            DeviceAsync();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void deviceAsyncSet_Click_1(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void deviceAsyncGet_Click_1(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ClearBarGraphTable(void);
            void UpdateBarGraphTable(Platform::Array<bool>^ BarGraphArray);
        };
    }
}
