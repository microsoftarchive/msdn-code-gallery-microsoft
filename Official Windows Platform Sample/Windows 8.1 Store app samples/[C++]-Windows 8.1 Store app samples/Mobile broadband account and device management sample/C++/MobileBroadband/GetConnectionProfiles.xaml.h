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
// GetConnectionProfiles.xaml.h
// Declaration of the GetConnectionProfiles class
//

#pragma once

using namespace Windows::Foundation::Collections;
using namespace Windows::Networking::Connectivity;
using namespace Platform;

#include "pch.h"
#include "GetConnectionProfiles.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace MobileBroadband
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class GetConnectionProfiles sealed
        {
        public:
            GetConnectionProfiles();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
            Windows::Foundation::Collections::IVectorView<Platform::String^>^ deviceAccountId;
            String^ connectionProfileInfo;

            String^ GetConnectionProfile(ConnectionProfile^ connectionProfile);
            String^ GetConnectionCostInfo(ConnectionCost^ connectionCost);
            String^ GetCostBasedSuggestions(ConnectionCost^ connectionCost);
            String^ GetDataPlanStatusInfo(DataPlanStatus^ dataPlan);
            String^ GetNetworkSecuritySettingsInfo(NetworkSecuritySettings^ netSecuritySettings);
            String^ GetWlanConnectionProfileDetailsInfo(WlanConnectionProfileDetails^ wlanConnectionProfileDetails);
            String^ GetWwanConnectionProfileDetailsInfo(WwanConnectionProfileDetails^ wwanConnectionProfileDetails);
            void PrintConnectionProfiles(IVectorView<ConnectionProfile^>^ connectionProfiles);
            void GetConnectionProfilesButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void PrepareScenario();
        };
    }
}