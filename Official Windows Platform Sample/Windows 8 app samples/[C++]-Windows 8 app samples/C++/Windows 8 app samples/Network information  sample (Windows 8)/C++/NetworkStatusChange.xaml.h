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
// NetworkStatusChange.xaml.h
// Declaration of the NetworkStatusChange class
//

#pragma once

#include "pch.h"
#include "NetworkStatusChange.g.h"
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
        public ref class NetworkStatusChange sealed
        {
        public:
            NetworkStatusChange();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            void NetworkStatusChange_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            bool registeredNetworkStatusNotif;
            Windows::Foundation::EventRegistrationToken cookie;
            String^ internetProfileInfo;
            Windows::Networking::Connectivity::NetworkInformation^ networkInfo;
            Windows::Networking::Connectivity::NetworkStatusChangedEventHandler^ networkStatusCallback;
    
            String^ GetConnectionProfile(Windows::Networking::Connectivity::ConnectionProfile^ connectionProfile);
            String^ GetConnectionCostInfo(Windows::Networking::Connectivity::ConnectionCost^ connectionCost);
            String^ GetCostBasedSuggestions(Windows::Networking::Connectivity::ConnectionCost^ connectionCost);
            String^ GetDataPlanStatusInfo(Windows::Networking::Connectivity::DataPlanStatus^ dataPlan);
            String^ GetNetworkSecuritySettingsInfo(Windows::Networking::Connectivity::NetworkSecuritySettings^ netSecuritySettings);
            void OnNetworkStatusChange(Object^ sender);
            void UnRegisterForNetworkStatusChangeNotif ();
    
        };
    }
}
