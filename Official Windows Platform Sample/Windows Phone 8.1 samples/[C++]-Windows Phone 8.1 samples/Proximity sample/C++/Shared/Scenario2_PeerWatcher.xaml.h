// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "pch.h"
#include <mutex> 
#include "Scenario2_PeerWatcher.g.h"
#include "SocketHelper.h"
#include "MainPage.xaml.h"


namespace SDKSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class PeerWatcherScenario sealed
    {
    public:
        PeerWatcherScenario();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs ^ e) override;
    private:

        void TriggeredConnectionStateChangedEventHandler(Object^ sender, Windows::Networking::Proximity::TriggeredConnectionStateChangedEventArgs^ e);
        void ConnectionRequestedEventHandler(Object^ sender, Windows::Networking::Proximity::ConnectionRequestedEventArgs^ e);
        void PeerFinder_StartAdvertising(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PeerFinder_StopAdvertising(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PeerFinder_StartPeerWatcher(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PeerFinder_StopPeerWatcher(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        
        void PeerWatcher_Added(Windows::Networking::Proximity::PeerWatcher^ sender, Windows::Networking::Proximity::PeerInformation^ peerInfo);
        void PeerWatcher_Removed(Windows::Networking::Proximity::PeerWatcher^ sender, Windows::Networking::Proximity::PeerInformation^ peerInfo);
        void PeerWatcher_Updated(Windows::Networking::Proximity::PeerWatcher^ sender, Windows::Networking::Proximity::PeerInformation^ peerInfo);
        void PeerWatcher_EnumerationCompleted(Windows::Networking::Proximity::PeerWatcher^ sender, Object^ o);
        void PeerWatcher_Stopped(Windows::Networking::Proximity::PeerWatcher^ sender, Object^ o);
        
        void PeerFinder_Accept(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PeerFinder_Connect(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PeerFinder_Send(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        void SocketErrorHandler(SocketHelper^ sender, Platform::String^ errMessage);
        void MessageHandler(SocketHelper^ sender, Platform::String^ message);

        void PeerFinderInit();
        void PeerFinderReset();
        void Scenario2Reset();
        void PeerFinder_StartSendReceive(Windows::Networking::Sockets::StreamSocket^ socket, Windows::Networking::Proximity::PeerInformation^ peerInformation);
        Platform::String^ GetTruncatedPeerId(Platform::String^ id);

        void OnLoaded(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void Current_SizeChanged(Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e);
        void ChangeVisualState(double width);

        void HideAllControls();
        void HideAllControlGroups();
        void ShowAdvertiseControls();
        void ToggleWatcherControls(bool show);
        void ShowPeerAddedControls();
        void ShowStartPeerWatcherControls();
        void TogglePeerWatcherStartControls(bool running);
        void ShowSendOrAcceptControls(bool send);

        //members
        MainPage^ m_rootPage;
        Windows::UI::Core::CoreDispatcher^ m_coreDispatcher;
        bool m_peerFinderStarted;
        bool m_peerWatcherIsRunning;

        std::mutex m_peerListMutex;
        Peers^ m_discoveredPeers;
        Windows::Networking::Proximity::PeerInformation^ m_requestingPeer;
        
        bool m_triggeredConnectSupported;
        bool m_browseConnectSupported;
        bool m_startPeerFinderImmediately;
        
        Windows::Foundation::EventRegistrationToken m_triggerToken;
        Windows::Foundation::EventRegistrationToken m_connectionRequestedToken;
        Windows::Foundation::EventRegistrationToken m_peerAddedToken;
        Windows::Foundation::EventRegistrationToken m_peerRemovedToken;
        Windows::Foundation::EventRegistrationToken m_peerUpdatedToken;
        Windows::Foundation::EventRegistrationToken m_peerWatcherEnumerationCompletedToken;
        Windows::Foundation::EventRegistrationToken m_peerWatcherStoppedToken;

        Windows::Networking::Proximity::PeerWatcher^ m_peerWatcher;

        SocketHelper^ m_socketHelper;
    };
}
