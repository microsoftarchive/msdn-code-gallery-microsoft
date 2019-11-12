//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S1_CreateConnection.xaml.h
// Declaration of the S1_CreateConnection class
//
using namespace Windows::Networking::Connectivity;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::System;
using namespace Platform;
using namespace concurrency;
#pragma once
#include "pch.h"
#include "S1_CreateConnection.g.h"

namespace SDKSample
{
    namespace ConnectivityManager
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S1_CreateConnection sealed
        {
        public:
			S1_CreateConnection();

        private:
			void Register_NetworkStateChange();
			void Connect_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			Windows::Networking::Connectivity::NetworkStatusChangedEventHandler^ networkStatusCallback;
			Windows::Foundation::EventRegistrationToken cookie;
			void OnNetworkStatusChange(Object^ sender);
			void UnRegisterForNetworkStatusChangeNotif();
			Windows::Networking::Connectivity::CellularApnContext^ PopulateApnContextFromUI();
			Windows::Networking::Connectivity::CellularApnAuthenticationType GetAuthenticationTypeFromString(String^ authType);

			// Display Methods
			void DisplayError(String^ message);
			void DisplayWarning(String^ message);
			void DisplayInfo(String^ message);
        };
    }
}
