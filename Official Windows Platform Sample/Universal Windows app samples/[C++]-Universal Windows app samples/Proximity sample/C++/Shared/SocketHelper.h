//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

#pragma once

#include "pch.h"
#include "Scenario1_PeerFinder.g.h"

namespace SDKSample
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class BufferConverter sealed : Windows::UI::Xaml::Data::IValueConverter 
    {
    public:
        virtual Platform::Object^ Convert(Platform::Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, 
            Platform::Object^ parameter, Platform::String^ language)
        {
            Platform::String^ discoveryData = ref new Platform::String();
            if (value != nullptr)
            {
                try
                {
                    Windows::Storage::Streams::Buffer^ buffer = safe_cast<Windows::Storage::Streams::Buffer^>(value);
                    Windows::Storage::Streams::DataReader^ discoveryDataReader = Windows::Storage::Streams::DataReader::FromBuffer(buffer);
                    discoveryData = discoveryDataReader->ReadString(buffer->Length);
                }
                catch(Platform::InvalidCastException^)
                {
                }
            }
            return discoveryData;
        }

        // No need to implement converting back on a one-way binding 
        virtual Platform::Object^ ConvertBack(Platform::Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, 
            Platform::Object^ parameter, Platform::String^ language)
        {
            throw ref new Platform::NotImplementedException();
        }
    };

    public ref class ConnectedPeer sealed
    {
    public:
        Windows::Networking::Sockets::StreamSocket^ GetSocket() { return m_socket; }
        void SetSocket(Windows::Networking::Sockets::StreamSocket^ socket) { m_socket = socket; }
        Windows::Storage::Streams::DataWriter^ GetWriter() { return m_dataWriter; }
        void SetWriter(Windows::Storage::Streams::DataWriter^ dataWriter) { m_dataWriter = dataWriter; }
        bool IsSocketClosed() { return m_socketClosed; }
        void SetSocketClosedState(bool socketClosed) { m_socketClosed = socketClosed; }

    private:
        Windows::Networking::Sockets::StreamSocket^ m_socket;
        bool m_socketClosed;
        Windows::Storage::Streams::DataWriter^ m_dataWriter;
    };

    [Windows::UI::Xaml::Data::Bindable] // in c++, adding this attribute to ref classes enables data binding for more info search for 'Bindable' on the page http://go.microsoft.com/fwlink/?LinkId=254639
    [Windows::Foundation::Metadata::WebHostHiddenAttribute]
    public ref class PeerInfoWrapper sealed
    {
        Windows::Networking::Proximity::PeerInformation^ m_PeerInfo;

    public:
        PeerInfoWrapper(Windows::Networking::Proximity::PeerInformation^ peerInfo)
        {
            m_PeerInfo = peerInfo;
        };

        Windows::Networking::Proximity::PeerInformation^ GetInnerObject()
        {
            return m_PeerInfo;
        }

        // DisplayName
        property Platform::String^ DisplayName
        {
            Platform::String^ get()
            {
                return m_PeerInfo->DisplayName;
            }
        }

        // Id
        property Platform::String^ Id
        {
            Platform::String^ get()
            {
                return m_PeerInfo->Id;
            }
        }

        // DiscoveryData
        property Windows::Storage::Streams::IBuffer^ DiscoveryData
        {
            Windows::Storage::Streams::IBuffer^ get()
            {
                return m_PeerInfo->DiscoveryData;
            }
        }
    };


    [Windows::Foundation::Metadata::WebHostHiddenAttribute]
    public ref class Peers sealed
    { 
    public: 
        Peers() :
            m_items(ref new Platform::Collections::Vector<PeerInfoWrapper^>())
        {}

        property Windows::UI::Xaml::Interop::IBindableObservableVector^ Items
        { 
            Windows::UI::Xaml::Interop::IBindableObservableVector^ get()
            {
                return m_items;
            }
        }

    private:
        Windows::UI::Xaml::Interop::IBindableObservableVector ^ m_items;
    };

    ref class SocketHelper;
    public delegate void SocketErrorEventHandler(SocketHelper^ sender, Platform::String^ s);
    public delegate void MessageEventHandler(SocketHelper^ sender, Platform::String^ s);

    public ref class SocketHelper sealed
    {
    public:
        event SocketErrorEventHandler^ SocketError;
        event MessageEventHandler^ MessageSent;

        SocketHelper() :
            m_connectedPeers(ref new Platform::Collections::Vector<ConnectedPeer ^>())
        {}

        property Windows::Foundation::Collections::IVectorView<ConnectedPeer^>^ ConnectedPeers
        {
            Windows::Foundation::Collections::IVectorView<ConnectedPeer^>^ get() { return m_connectedPeers->GetView(); }
        }

        void Add(ConnectedPeer^ p)
        {
            m_connectedPeers->Append(p);
        }


        void SendMessageToPeer(Platform::String^ message, ConnectedPeer^ connectedPeer);
        void StartReader(Windows::Storage::Streams::DataReader^ socketReader, bool fSocketClosed);
        void CloseSocket();
    private:
        Platform::Collections::Vector<ConnectedPeer ^>^ m_connectedPeers;
    };
}