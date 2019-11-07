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
// BroadbandDevice.xaml.h
// Declaration of the BroadbandDevice class
//

#pragma once

#include "pch.h"
#include "BroadbandDevice.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace MobileBroadband
    {
        //
        /// This page demonstrates the use of Mobile Broadband APIs by network operators
        //
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class BroadbandDevice sealed
        {
        public:
            BroadbandDevice();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void UpdateData_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    
            int deviceSelected;
            Windows::Foundation::Collections::IVectorView<Platform::String^>^ deviceAccountIds;
            Windows::Networking::NetworkOperators::MobileBroadbandAccountWatcher^ networkAccountWatcher;
            void PrepareScenario();
            void GetCurrentDeviceInfo(Platform::String^ accountId);
            Platform::String^ NetErrorToString(uint32_t netError);
        };
    }
}
