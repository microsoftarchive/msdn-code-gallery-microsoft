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
// ScenarioInput1.xaml.h
// Declaration of the ScenarioInput1 class
//

#pragma once

#include "pch.h"
#include "ScenarioInput1.g.h"
#include "MainPage.xaml.h"
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml::Controls;
using namespace StreamSocketTransportHelper;
using namespace Windows::UI::Core;

namespace ControlChannelTrigger
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
    public ref class ScenarioInput1 sealed
    {
    public:
        ScenarioInput1();
        void rootPage_OutputFrameLoaded(Object^ sender, Object^ e);
        String^ GetServerName();
        String^ GetServerPort();
        void ClientInit();
        void ServerInit();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
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
        MainPage^ rootPage;
        bool lockScreenAdded;
        Page outputFrame;
        EventRegistrationToken outputFrameERToken;
        appRoles appRole;
        CommModule^ commModule;
    };
}
