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
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once

#include "pch.h"
#include "Scenario2.g.h"
#include "MainPage.xaml.h"

using namespace Platform;
using namespace Windows::Storage::Streams;
using namespace Windows::Networking::Sockets;

namespace DatagramSocketSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class Scenario2 sealed
    {
    public:
        Scenario2();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    private:
        MainPage^ rootPage;
        void ConnectSocket_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class SocketContext sealed
    {
    public:
        SocketContext(MainPage^ rootPage, DatagramSocket^ socket);
        void OnMessage(DatagramSocket^ socket, DatagramSocketMessageReceivedEventArgs^ eventArguments);
        DataWriter^ GetWriter();
        boolean IsConnected();
        void SetConnected();

    private:
        ~SocketContext();
        MainPage^ rootPage;
        DatagramSocket^ socket;
        DataWriter^ writer;
        boolean connected;

        void NotifyUserFromAsyncThread(String^ message, NotifyType type);
    };
}
