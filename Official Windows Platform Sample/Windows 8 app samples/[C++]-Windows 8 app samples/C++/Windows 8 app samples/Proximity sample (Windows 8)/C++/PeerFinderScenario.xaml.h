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
// PeerFinderScenario.xaml.h
// Declaration of the PeerFinderScenario class
//

#pragma once

#include "pch.h"
#include "PeerFinderScenario.g.h"
#include "MainPage.xaml.h"

namespace ProximityCPP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class PeerFinderScenario sealed
    {
    public:
        PeerFinderScenario();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs ^ e) override;
    private:

        void TriggeredConnectionStateChangedEventHandler(Object^ sender, Windows::Networking::Proximity::TriggeredConnectionStateChangedEventArgs^ e);
        void ConnectionRequestedEventHandler(Object^ sender, Windows::Networking::Proximity::ConnectionRequestedEventArgs^ e);
        void PeerFinder_StartFindingPeers(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PeerFinder_BrowsePeers(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PeerFinder_Accept(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PeerFinder_Connect(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void SocketError(Platform::String^ errMessage);
        void CloseSocket();
        void PeerFinder_Send(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        void PeerFinderInit();
        void PeerFinderReset();
        void Scenario2Reset();
        void PeerFinder_StartSendReceive(Windows::Networking::Sockets::StreamSocket^ socket);
        void PeerFinder_StartReader(Windows::Storage::Streams::DataReader^ socketReader);

        //members
        MainPage^ m_rootPage;
        Windows::UI::Core::CoreDispatcher^ m_coreDispatcher;
        bool m_peerFinderStarted;
        Windows::Foundation::Collections::IVectorView<Windows::Networking::Proximity::PeerInformation^>^ m_peerInformationList;
        Windows::Networking::Proximity::PeerInformation^ m_requestingPeer;
        Windows::Networking::Sockets::StreamSocket^ m_socket;
        Windows::Storage::Streams::DataWriter^ m_dataWriter;
        bool m_triggeredConnectSupported;
        bool m_browseConnectSupported;
        bool m_startPeerFinderImmediately;
        bool m_socketClosed;
        Windows::Foundation::EventRegistrationToken m_triggerToken;
        Windows::Foundation::EventRegistrationToken m_connectionRequestedToken;

        //connection states
        Platform::Collections::Vector<Platform::String^>^ m_ConnectStateArray;
    };
}
