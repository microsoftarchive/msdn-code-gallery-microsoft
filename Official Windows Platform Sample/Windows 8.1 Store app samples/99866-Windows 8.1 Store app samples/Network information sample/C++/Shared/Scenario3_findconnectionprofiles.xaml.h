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
// Scenario3_findconnectionprofiles.xaml.h
// Declaration of the ProfileLocalUsageData class
//

#pragma once

#include "pch.h"
#include "Scenario3_findconnectionprofiles.g.h"
#include "MainPage.xaml.h"

using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Networking::Connectivity;
using namespace Platform;

namespace SDKSample
{
    namespace NetworkInformationApi
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class FindConnectionProfiles sealed
        {
        public:
            FindConnectionProfiles();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            String^ connectionProfileInfo;
            void FindConnectionProfilesButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            String^ GetConnectionProfile(ConnectionProfile^ connectionProfile);
            String^ GetConnectionCostInfo(ConnectionCost^ connectionCost);
            String^ GetCostBasedSuggestions(ConnectionCost^ connectionCost);
            String^ GetDataPlanStatusInfo(DataPlanStatus^ dataPlan);
            String^ GetNetworkSecuritySettingsInfo(NetworkSecuritySettings^ netSecuritySettings);
            String^ GetWlanConnectionProfileDetailsInfo(WlanConnectionProfileDetails^ wlanConnectionProfileDetails);
            String^ GetWwanConnectionProfileDetailsInfo(WwanConnectionProfileDetails^ wwanConnectionProfileDetails);
            void PrintConnectionProfiles(IVectorView<ConnectionProfile^>^ connectionProfiles);
            String^ GetComboBoxSelectedText(Windows::UI::Xaml::Controls::ComboBox^ comboBox);
        };
    }
}
