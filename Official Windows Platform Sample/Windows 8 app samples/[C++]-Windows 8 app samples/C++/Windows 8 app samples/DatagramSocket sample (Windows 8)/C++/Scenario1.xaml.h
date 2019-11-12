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
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once

#include "pch.h"
#include "Scenario1.g.h"
#include "MainPage.xaml.h"

using namespace Platform;
using namespace Windows::Networking;
using namespace Windows::Networking::Sockets;
using namespace Windows::Storage::Streams;

namespace DatagramSocketSample
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class Scenario1 sealed
    {
    public:
        Scenario1();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    private:
        MainPage^ rootPage;
        void StartListener_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ListenerContext sealed
    {
    public:
        ListenerContext(MainPage^ rootPage, DatagramSocket^ listener);
        
        void OnMessage(DatagramSocket^ socket, DatagramSocketMessageReceivedEventArgs^ eventArguments);
        bool IsMatching(HostName^ hostName, String^ port);

    private:
        ~ListenerContext();
        CRITICAL_SECTION lock;
        MainPage^ rootPage;
        DatagramSocket^ listener;
        IOutputStream^ outputStream;
        HostName^ hostName;
        String^ port;
        void NotifyUserFromAsyncThread(String^ message, NotifyType type);

        void EchoMessage(DatagramSocketMessageReceivedEventArgs^ eventArguments);
    };
}
