#pragma once
#include<Windows.h>
#include<collection.h>
#include<queue>
#include "pch.h"

using namespace Windows::Networking::Sockets;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml::Controls;
using namespace Platform;
using namespace Platform::Collections;
using namespace Microsoft::WRL;

#pragma once

namespace XHRTransportHelper
{
    private enum class CanaryRequestStatus
    {
        CanaryNotStarted,
        CanaryInProcess,
        CanarySucceed
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class CommModule sealed
    {
    private:
        ~CommModule(void);

        const static int TIMEOUT = 30000;
        const static int MAX_BUFFER_LENGTH = 100;

        CanaryRequestStatus m_canaryRequestStatus;

        String^ m_url;
        String^ m_postData;
        
        // Below data members are used for client role.
        ControlChannelTrigger^ channel_;

        IXMLHTTPRequest2* m_xhr;
        IInspectable* m_xhrInspectable;

        // Mutual-exclusion lock for m_xhr
        concurrency::critical_section m_XhrLock;

        // Used for providing Mutual-exclusion lock.
        concurrency::critical_section CSection;
    public:
        CommModule(String^ url, String ^postData);
        bool SetupTransport();
        bool RegisterWithCCT();
        bool RegisterWithCCTHelper();
        void OnDataReadCompletion(String^ message);
        bool SendKAMessage(String^ Message);
        bool PrepareXHRPost();
        bool DoXHRPost();
        bool SendCanaryRequestHelper(String^ url);
        bool SendCanaryRequest();
        void OnCanarySuccess();
        void OnCanaryFailure();
        void OnRequestSuccess();
        void OnRequestFailure();

        property ControlChannelTrigger^ channel
        {
            ControlChannelTrigger^ get() {return channel_;}
            void set(ControlChannelTrigger^ value) {channel_=value;}
        }

        void Reset();
    };

    public ref class TSQueue sealed
    {
        ~TSQueue();
        std::queue<String^,std::deque<String^,std::allocator<String^>>> *queue;
        concurrency::critical_section CSection;

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
        static TSQueue^ messageQueue_;
    public:
        AppContext(CommModule^ commInstance, ControlChannelTrigger^ channel, String^ id);
        property TSQueue^ messageQueue
        {
            TSQueue^ get() {return messageQueue_;}
            void set(TSQueue^ value) { messageQueue_=value;}
        }
        property ControlChannelTrigger^ Channel
        {
            ControlChannelTrigger^ get() { return Channel_;}
            void set(ControlChannelTrigger^ value) {Channel_=value;}
        }
        property String^ ChannelId
        {
            String^ get() {return ChannelId_;}
            void set(String^ value) {ChannelId_=value;}
        }
        property CommModule^ CommInstance
        {
            CommModule^ get() { return CommInstance_;}
            void set(CommModule^ value) {CommInstance_=value;}
        }
    };
}
