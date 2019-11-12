// Copyright (c) Microsoft Corporation. All rights reserved.

//
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once
#include "Scenario1.g.h"

namespace SDKSample
{
    namespace ControlChannelHttpClient
    {
        public enum class ConnectionStates
        {
            NotConnected = 0,
            Connecting = 1,
            Connected = 2,
        };

        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario1 sealed
        {
        public:
            Scenario1();
            void InitializeClient();

        protected:
            virtual void OnNavigatedTo(_In_ Windows::UI::Xaml::Navigation::NavigationEventArgs^ /*e*/) override;

        private:
            void ConnectButton_Click(_In_ Platform::Object^ sender, _In_ Windows::UI::Xaml::RoutedEventArgs^ /*e*/);
            void ClearButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void RegisterNetworkChangeTask();
            static ConnectionStates connectionState;

        internal:
            bool lockScreenAdded;
            HttpClientTransportHelper::CommunicationModule^ communicationModule;
        };
    }
}
