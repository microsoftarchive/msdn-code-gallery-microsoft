// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "pch.h"
#include "Scenario1_PeerFinder.g.h"
#include "SocketHelper.h"
#include "MainPage.xaml.h"


namespace SDKSample
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
        void PeerFinder_StartAdvertising(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PeerFinder_StopAdvertising(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PeerFinder_BrowsePeers(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        
        void PeerFinder_Accept(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PeerFinder_Connect(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PeerFinder_Send(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        void SocketErrorHandler(SocketHelper^ sender, Platform::String^ errMessage);
        void MessageHandler(SocketHelper^ sender, Platform::String^ message);

        void PeerFinderInit();
        void PeerFinderReset();
        void Scenario2Reset();
        void PeerFinder_StartSendReceive(Windows::Networking::Sockets::StreamSocket^ socket, Windows::Networking::Proximity::PeerInformation^ peerInformation);

        void OnLoaded(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void Current_SizeChanged(Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e);
        void ChangeVisualState(double width);

        void HideAllControls();
        void ToggleAdvertiseControls(bool show);
        void ShowStartAdvertiseControls();
        void ShowPostBrowseControls(bool found);
        void ToggleConnectedControls(bool show);

        //members
        MainPage^ m_rootPage;
        Windows::UI::Core::CoreDispatcher^ m_coreDispatcher;
        bool m_peerFinderStarted;

        Windows::Foundation::Collections::IVectorView<Windows::Networking::Proximity::PeerInformation^>^ m_peerInformationList;
        Windows::Networking::Proximity::PeerInformation^ m_requestingPeer;
        
        bool m_triggeredConnectSupported;
        bool m_browseConnectSupported;
        bool m_startPeerFinderImmediately;
        bool m_fLaunchByTap;

        Windows::Foundation::EventRegistrationToken m_triggerToken;
        Windows::Foundation::EventRegistrationToken m_connectionRequestedToken;

        SocketHelper^ m_socketHelper;
    };
}
