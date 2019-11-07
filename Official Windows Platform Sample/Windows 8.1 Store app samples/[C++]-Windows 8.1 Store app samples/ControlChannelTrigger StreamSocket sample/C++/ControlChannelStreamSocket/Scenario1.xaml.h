//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once
#include "pch.h"
#include "Scenario1.g.h"
#include "MainPage.xaml.h"
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace StreamSocketTransportHelper;
using namespace Windows::UI::Core;
using namespace Platform;

namespace SDKSample
{
    namespace ControlChannelStreamSocket
    {
        public enum class appRoles
        {
            clientRole,
            serverRole
        };
        public enum class connectionStates
        {
            notConnected = 0,
            connecting = 1,
            connected = 2,
        };
        public enum class listeningStates
        {
            notListening = 0,
            listening = 1,
        };

        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario1 sealed
        {
        public:
            Scenario1();
            String^ GetServerName();
            String^ GetServerPort();
            void ClientInit();
            void ServerInit();

        private:
            void ServerRole_Click(Object^ sender, RoutedEventArgs^ e);
            void ClientRole_Click(Object^ sender, RoutedEventArgs^ e);
            void ConnectButton_Click(Object^ sender, RoutedEventArgs^ e);
            void ListenButton_Click(Object^ sender, RoutedEventArgs^ e);
            void SendButton_Click(Object^ sender, RoutedEventArgs^ e);
            void RegisterNetworkChangeTask();
            static connectionStates connectionState;
            static listeningStates listenState;

        internal:
            bool lockScreenAdded;
            appRoles appRole;
            CommModule^ commModule;

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        };
    }
}
