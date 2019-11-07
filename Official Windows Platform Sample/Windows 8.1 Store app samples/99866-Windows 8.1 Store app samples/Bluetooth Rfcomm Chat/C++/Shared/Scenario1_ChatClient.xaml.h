//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1_ChatClient.xaml.h
// Declaration of the Scenario1_ChatClient class
//

#pragma once
#include "Scenario1_ChatClient.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace BluetoothRfcommChat
    {
        public ref class Scenario1_ChatClient sealed
        {
        public:
            Scenario1_ChatClient();

        private:
            void App_Suspending(Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ e);
            void RunButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ServiceList_Tapped(Platform::Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e);
            void SendButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void DisconnectButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            void ReceiveStringLoop(Windows::Storage::Streams::DataReader^ chatReader);
            void Disconnect();

            void NotifyUserFromAsyncThread(Platform::String^ message, SDKSample::NotifyType type);

            Windows::Devices::Enumeration::DeviceInformationCollection^ chatServiceInfoCollection;
            Windows::Devices::Bluetooth::Rfcomm::RfcommDeviceService^ chatService;
            Windows::Networking::Sockets::StreamSocket^ chatSocket;
            Windows::Storage::Streams::DataWriter^ chatWriter;
            concurrency::reader_writer_lock chatSocketLock;
        };
    }
}
