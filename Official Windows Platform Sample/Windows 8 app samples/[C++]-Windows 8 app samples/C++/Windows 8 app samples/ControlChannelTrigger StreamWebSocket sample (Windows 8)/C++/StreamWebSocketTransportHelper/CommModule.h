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

namespace StreamWebSocketTransportHelper
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
        
        // Below data members are used for client role.
        StreamWebSocket^ socket;
        DataReader^ readPacket;    
        ControlChannelTrigger^ channel_;
        String^ ServerUri_;
        DataWriter^ writePacket;
        
        // Used for providing Mutual-exclusion lock.
        concurrency::critical_section lock;
    public:
        CommModule(AppRole appRole);
        bool SetupTransport(String^ serverUri);
        bool RegisterWithCCT(String^ serverUri);
        void PostSocketRead(int length);
        void OnDataReadCompletion(unsigned int bytesRead, DataReader^ readPacket);
        void SendMessage(String^ message);

        property ControlChannelTrigger^ channel
        {
            ControlChannelTrigger^ get() {return channel_;}
            void set(ControlChannelTrigger^ value) {channel_ = value;}
        }

        property String^ ServerUri
        {
            String^ get() {return ServerUri_;}
            void set(String^ value) {ServerUri_ = value;}
        }

        void Reset();
    };

    public ref class TSQueue sealed
    {
        std::queue<String^,std::deque<String^,std::allocator<String^>>> *queue;
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
        StreamWebSocket^ SocketHandle_;
        static TSQueue^ messageQueue_;
    public:
        AppContext(CommModule^ commInstance, StreamWebSocket^ socket, ControlChannelTrigger^ channel, String^ id);
        property TSQueue^ messageQueue
        {
            TSQueue^ get() {return messageQueue_;}
            void set(TSQueue^ value) { messageQueue_ = value;}
        }
        property StreamWebSocket^ SocketHandle
        {
            StreamWebSocket^ get() {return SocketHandle_;}
            void set(StreamWebSocket^ value) {SocketHandle_ = value;}
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
