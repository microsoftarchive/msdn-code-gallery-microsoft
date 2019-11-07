// Copyright (c) Microsoft Corporation. All rights reserved.

#pragma once
#include<Windows.h>
#include<collection.h>
#include<queue>
#include "pch.h"

namespace HttpClientTransportHelper
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class CommunicationModule sealed
    {
    public:
        CommunicationModule();
        bool SetUpTransport(_In_ Platform::String^ origin);
        bool SendKAMessage(_In_ Platform::String^ Message);
        void Reset();

        property Windows::Foundation::Uri^ ServerUri
        {
            Windows::Foundation::Uri^ get()
            {
                return serverUri;
            }
            void set(_In_ Windows::Foundation::Uri^ value)
            {
                serverUri = value;
            }
        }

        property Windows::Networking::Sockets::ControlChannelTrigger^ Channel
        {
            Windows::Networking::Sockets::ControlChannelTrigger^ get()
            {
                return channel;
            }
            void set(Windows::Networking::Sockets::ControlChannelTrigger^ value)
            {
                channel = value;
            }
        }

    private:
        ~CommunicationModule(void);
        bool RegisterWithCct();
        bool RegisterWithCctHelper();
        void ResetRequest();
        void SetUpHttpRequestAndSendToHttpServer();
        void SendHttpRequest();
        void AppendOriginToUri();
        void ReadMore();
        void OnSendRequestCompleted(
            _In_ Windows::Foundation::IAsyncOperationWithProgress<Windows::Web::Http::HttpResponseMessage^, Windows::Web::Http::HttpProgress>^ asyncInfo,
            _In_ Windows::Foundation::AsyncStatus asyncStatus);
        void OnReadAsInputStreamCompleted(
            _In_ Windows::Foundation::IAsyncOperationWithProgress<Windows::Storage::Streams::IInputStream^, unsigned long long>^ asyncInfo,
            _In_ Windows::Foundation::AsyncStatus asyncStatus);
        void OnReadCompleted(
            _In_ Windows::Foundation::IAsyncOperationWithProgress<Windows::Storage::Streams::IBuffer^, unsigned int>^ asyncInfo,
            _In_ Windows::Foundation::AsyncStatus asyncStatus);
        std::wstring::size_type LastIndexOf(_In_ Platform::String^ s, _In_ const wchar_t value);
        Platform::String^ Substring(
            _In_ Platform::String^ s,
            _In_ std::wstring::size_type startIndex,
            _In_ std::wstring::size_type length);

        // Lock to protect variables that can be accessed by multiple threads: channel,
        // httpClient and httpRequest.
        concurrency::critical_section requestLock;

        Platform::String^ origin;
        Windows::Foundation::Uri^ serverUri;
        Windows::Networking::Sockets::ControlChannelTrigger^ channel;
        Windows::Web::Http::HttpClient^ httpClient;
        Windows::Web::Http::HttpRequestMessage^ httpRequest;
        Windows::Storage::Streams::IInputStream^ inputStream;
        Windows::Foundation::IAsyncOperationWithProgress<
            Windows::Web::Http::HttpResponseMessage^,
            Windows::Web::Http::HttpProgress>^ sendRequestOperation;
        Windows::Foundation::IAsyncOperationWithProgress<
            Windows::Storage::Streams::IInputStream^,
            unsigned long long>^ readAsInputStreamOperation;
        Windows::Foundation::IAsyncOperationWithProgress<
            Windows::Storage::Streams::IBuffer^,
            unsigned int>^ readOperation;
        };

    public ref class TSQueue sealed
    {
    public:
        TSQueue();
        void Enqueue(_In_ Platform::String^ data);
        Platform::String^ Dequeue();

    private:
        ~TSQueue();
        std::queue <Platform::String^, std::deque <Platform::String^, std::allocator<Platform::String^>>>* queue;
        concurrency::critical_section queueLock;
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class AppContext sealed
    {
    public:
        AppContext(_In_ CommunicationModule^ communicationInstance);

        property TSQueue^ MessageQueue
        {
            TSQueue^ get()
            { 
                return messageQueue;
            }
            void set(_In_ TSQueue^ value)
            {
                messageQueue = value;
            }
        }

        property CommunicationModule^ CommunicationInstance
        {
            CommunicationModule^ get()
            {
                return communicationInstance;
            }
            void set(_In_ CommunicationModule^ value)
            {
                communicationInstance = value;
            }
        }

    private:
        CommunicationModule^ communicationInstance;
        static TSQueue^ messageQueue;
    };
}
