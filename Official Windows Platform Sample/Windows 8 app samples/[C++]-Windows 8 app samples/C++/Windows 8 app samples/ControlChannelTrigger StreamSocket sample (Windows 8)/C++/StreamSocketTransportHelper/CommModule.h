#pragma once
#include <Windows.h>
#include <collection.h>
#include <queue>
#include <concrt.h>
using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Networking::Sockets;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml::Controls;

#pragma once

namespace StreamSocketTransportHelper
{
    public enum class AppRole
    {
        ClientRole,
        ServerRole
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class CommModule sealed
    {
    private:
        const int TIMEOUT;
        const int MAX_BUFFER_LENGTH;
        AppRole appRole;
        
        // Below data members are used for client role.
        StreamSocket^ socket;
        DataReader^ readPacket;
        DataWriter^ writePacket;
        
        ControlChannelTrigger^ channel_;
        String^ serverName_;
        String^ serverPort_;
        
        // Used for server role.
        StreamSocket^ serverSocket;
        StreamSocketListener^ serverListener;
        
        // Used for providing Mutual-exclusion lock.
        concurrency::critical_section lock;
    public:
        CommModule(AppRole appRole);
        bool SetupTransport(String^ serverHostName, String^ serverPort);
        bool RegisterWithCCT(String^ serverHostName, String^ serverPort);
        void PostSocketRead(int length);
        void OnDataReadCompletion(unsigned int bytesRead, DataReader^ readPacket);
        bool AcceptConnection(String^ serviceName);
        void WriteToSocket(StreamSocket^ socket, String^ message);
        void SendMessage(String^ message);
        bool SendKAMessage(String^ message);

        property ControlChannelTrigger^ channel
        {
            ControlChannelTrigger^ get() {return channel_;}
            void set(ControlChannelTrigger^ value) {channel_ = value;}
        }

        property String^ serverName
        {
            String^ get() {return serverName_;}
            void set(String^ value) {serverName_ = value;}
        }

        property String^ serverPort
        {
            String^ get() {return serverPort_;}
            void set(String^ value) {serverPort_ = value;}
        }
        void Reset();
    };

    public ref class TSQueue sealed
    {
        ~TSQueue();
        std::queue<String^,std::deque<String^,std::allocator<String^>>>* queue;
        concurrency::critical_section lock;
    public:
        TSQueue();
        void Enqueue(String^ data);
        String^ Dequeue();
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class AppContext sealed
    {
        CommModule^ CommInstance_;
        String^ ChannelId_;
        ControlChannelTrigger^ Channel_;
        StreamSocket^ SocketHandle_;
        static TSQueue^ messageQueue_;
    public:
        AppContext(CommModule^ commInstance, StreamSocket^ socket, ControlChannelTrigger^ channel, String^ id);
        property TSQueue^ messageQueue
        {
            TSQueue^ get() {return messageQueue_;}
            void set(TSQueue^ value) { messageQueue_ = value;}
        }
        property StreamSocket^ SocketHandle
        {
            StreamSocket^ get() {return SocketHandle_;}
            void set(StreamSocket^ value) {SocketHandle_ = value;}
        }
        property ControlChannelTrigger^ Channel
        {
            ControlChannelTrigger^ get() { return Channel_;}
            void set(ControlChannelTrigger^ value) {Channel_ = value;}
        }
        property String^ ChannelId
        {
            String^ get() {return ChannelId_;}
            void set(String^ value) {ChannelId_ = value;}
        }
        property CommModule^ CommInstance
        {
            CommModule^ get() { return CommInstance_;}
            void set(CommModule^ value) {CommInstance_ = value;}
        }
    };
}

