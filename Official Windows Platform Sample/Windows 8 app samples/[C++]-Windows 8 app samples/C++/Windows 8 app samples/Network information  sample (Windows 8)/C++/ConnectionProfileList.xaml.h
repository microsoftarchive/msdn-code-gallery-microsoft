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
// ConnectionProfileList.xaml.h
// Declaration of the ConnectionProfileList class
//

#pragma once

#include "pch.h"
#include "ConnectionProfileList.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Platform;
using namespace Windows::Networking;
using namespace Windows::Networking::Connectivity;

namespace SDKSample
{
    namespace NetworkInformationApi
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ConnectionProfileList sealed
        {
        public:
            ConnectionProfileList();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void ConnectionProfileList_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    
            Windows::Networking::Connectivity::NetworkInformation^ networkInfo;
            String ^connectionProfileInfo;
            String^ GetConnectionProfile(Windows::Networking::Connectivity::ConnectionProfile^ connectionProfile);
            String^ GetConnectionCostInfo(Windows::Networking::Connectivity::ConnectionCost^ connectionCost);
            String^ GetCostBasedSuggestions(Windows::Networking::Connectivity::ConnectionCost^ connectionCost);
            String^ GetDataPlanStatusInfo(Windows::Networking::Connectivity::DataPlanStatus^ dataPlan);
            String^ GetNetworkSecuritySettingsInfo(Windows::Networking::Connectivity::NetworkSecuritySettings^ netSecuritySettings);
        };
    }
}
