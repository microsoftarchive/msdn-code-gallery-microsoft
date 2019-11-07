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
// AccountWatcher.xaml.h
// Declaration of the AccountWatcher class
//

#pragma once

#include "pch.h"
#include "AccountWatcher.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace MobileBroadband
    {
        //
        /// This page demonstrates the use of the mobile broadband network account watcher APIs
        //
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class AccountWatcher sealed
        {
        public:
            AccountWatcher();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void StartMonitoring_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void StopMonitoring_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    
            Windows::UI::Core::CoreDispatcher^ sampleDispatcher;
            Windows::Networking::NetworkOperators::MobileBroadbandAccountWatcher^ networkAccountWatcher;
            Platform::String^ DumpPropertyData(Platform::String^ networkAccountId, bool hasDeviceInformationChanged, bool hasNetworkChanged);
            void PrepareWatcher();
            void DisplayWatcherOutputFromCallback(Platform::String^ value);
            Platform::String^ NetErrorToString(uint32_t netError);
            Platform::String^ DumpAccountDeviceInformation(Windows::Networking::NetworkOperators::MobileBroadbandDeviceInformation^ deviceInformation);
            Platform::String^ DumpAccountNetwork(Windows::Networking::NetworkOperators::MobileBroadbandNetwork^ network);
        };
    }
}
