// Copyright (c) Microsoft Corporation. All rights reserved.

#include "pch.h"
#include "CommunicationModule.h"
#include <ppltasks.h>
#include <shcore.h>
#include "DiagnosticsHelper.h"

using namespace concurrency;
using namespace HttpClientTransportHelper;
using namespace HttpClientTransportHelper::DiagnosticsHelper;
using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::Foundation;
using namespace Windows::Networking::Sockets;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Web::Http;

CoreDispatcher^ Diag::coreDispatcher = nullptr;
TextBlock^ Diag::debugOutputTextBlock = nullptr;
TSQueue^ AppContext::messageQueue = nullptr;

CommunicationModule::CommunicationModule()
    : httpClient(nullptr),
    httpRequest(nullptr),
    inputStream(nullptr),
    sendRequestOperation(nullptr),
    readAsInputStreamOperation(nullptr),
    readOperation(nullptr),
    serverUri(nullptr)
{
}

AppContext::AppContext(_In_ CommunicationModule^ communicationInstance)
{
    CommunicationInstance = communicationInstance;
    messageQueue = ref new TSQueue();
}

CommunicationModule::~CommunicationModule(void)
{
    Reset();
}

void CommunicationModule::Reset()
{
    // Hold lock.
    critical_section::scoped_lock slock(requestLock);

    if (this->channel != nullptr)
    {
        if (CoreApplication::Properties->HasKey(channel->ControlChannelTriggerId))
        {
            CoreApplication::Properties->Remove(channel->ControlChannelTriggerId);
        }

        // Call the Dispose() method on the ControlChannelTrigger object to release any
        // OS maintained resources for this channel object.
        delete this->channel;
        this->channel = nullptr;
    }

    if (httpClient != nullptr)
    {
        delete httpClient;
        httpClient = nullptr;
    }

    Diag::DebugPrint("CommunicationModule has been reset.");
}

void CommunicationModule::ResetRequest()
{
    if (sendRequestOperation != nullptr)
    {
        sendRequestOperation->Cancel();
        sendRequestOperation = nullptr;
    }
    if (readAsInputStreamOperation != nullptr)
    {
        readAsInputStreamOperation->Cancel();
        readAsInputStreamOperation = nullptr;
    }
    if (readOperation != nullptr)
    {
        readOperation->Cancel();
        readOperation = nullptr;
    }
    if (inputStream != nullptr)
    {
        delete inputStream;
        inputStream = nullptr;
    }
    if (httpRequest != nullptr)
    {
        delete httpRequest;
        httpRequest = nullptr;
    }
}

void CommunicationModule::SetUpHttpRequestAndSendToHttpServer()
{
    try
    {
        Diag::DebugPrint("SetUpHttpRequestAndSendToHttpServer started with URI: " + serverUri->AbsoluteCanonicalUri);

        // IMPORTANT:
        // For HTTP based transports that use ControlChannelTrigger, whenever we send the next request,
        // we will abort the earlier outstanding HTTP request and start a new one.
        // For example, when the HTTP server is taking longer to reply, and the keep alive trigger is fired
        // in-between then the keep alive task will abort the outstanding HTTP request and start a new request
        // which should be finished before the next keep alive task is triggered.
        ResetRequest();

        httpRequest = ref new HttpRequestMessage(HttpMethod::Get, serverUri);

        SendHttpRequest();
    }
    catch (Exception^ ex)
    {
        Diag::DebugPrint("Connect failed with: " + ex->ToString());
        throw;
    }
}

void CommunicationModule::SendHttpRequest()
{
    // Tie the transport method to the ControlChannelTrigger object to push enable it.
    // Note that if the transport's TCP connection is broken at a later point of time,
    // the ControlChannelTrigger object can be reused to plug in a new transport by
    // calling UsingTransport again.
    Diag::DebugPrint("Calling UsingTransport() ...");
    channel->UsingTransport(httpRequest);

    // Call the SendRequestAsync function to kick start the TCP connection establishment
    // process for this HTTP request.
    Diag::DebugPrint("Calling SendRequestAsync() ...");
    sendRequestOperation = httpClient->SendRequestAsync(httpRequest, HttpCompletionOption::ResponseHeadersRead);

    sendRequestOperation->Completed = ref new AsyncOperationWithProgressCompletedHandler<HttpResponseMessage^, HttpProgress>(
        this,
        &CommunicationModule::OnSendRequestCompleted);

    // Call WaitForPushEnabled API to make sure the TCP connection has been established, 
    // which will mean that the OS will have allocated any hardware or software slot for this TCP connection.

    ControlChannelTriggerStatus status = channel->WaitForPushEnabled();
    Diag::DebugPrint("WaitForPushEnabled() completed with status: " + status.ToString());
    if (status != ControlChannelTriggerStatus::HardwareSlotAllocated
        && status != ControlChannelTriggerStatus::SoftwareSlotAllocated)
    {
        throw ref new Exception(E_FAIL, "Hardware/Software slot not allocated");
    }

    Diag::DebugPrint("Transport is ready to read response from server.");
}

void CommunicationModule::ReadMore()
{
    IBuffer^ buffer = ref new Buffer(1024);
    readOperation = inputStream->ReadAsync(buffer, buffer->Capacity, InputStreamOptions::Partial);
    readOperation->Completed = ref new AsyncOperationWithProgressCompletedHandler<IBuffer^, unsigned int>(
        this,
        &CommunicationModule::OnReadCompleted);
}

void CommunicationModule::OnSendRequestCompleted(
    _In_ IAsyncOperationWithProgress<HttpResponseMessage^, HttpProgress>^ asyncInfo,
    _In_ AsyncStatus asyncStatus)
{
    try
    {
        if (asyncStatus == AsyncStatus::Canceled)
        {
            Diag::DebugPrint("HttpRequestMessage.SendRequestAsync was canceled.");
            return;
        }

        if (asyncStatus == AsyncStatus::Error)
        {
            Diag::DebugPrint("HttpRequestMessage.SendRequestAsync failed: " + asyncInfo->ErrorCode.Value);
            return;
        }

        Diag::DebugPrint("HttpRequestMessage.SendRequestAsync succeeded.");

        // Once the headers are received, get the input stream from the response content.
        HttpResponseMessage^ response = asyncInfo->GetResults();
        readAsInputStreamOperation = response->Content->ReadAsInputStreamAsync();
        readAsInputStreamOperation->Completed = ref new AsyncOperationWithProgressCompletedHandler<IInputStream^, unsigned long long>(
            this,
            &CommunicationModule::OnReadAsInputStreamCompleted);
    }
    catch (Exception^ ex)
    {
        Diag::DebugPrint("Error in OnSendRequestCompleted: " + ex->ToString());
    }
}

void CommunicationModule::OnReadAsInputStreamCompleted(
    _In_ IAsyncOperationWithProgress<Windows::Storage::Streams::IInputStream^, unsigned long long>^ asyncInfo,
    _In_ AsyncStatus asyncStatus)
{
    try
    {
        if (asyncStatus == AsyncStatus::Canceled)
        {
            Diag::DebugPrint("IHttpContent.ReadAsInputStreamAsync was canceled.");
            return;
        }

        if (asyncStatus == AsyncStatus::Error)
        {
            Diag::DebugPrint("IHttpContent.ReadAsInputStreamAsync failed: " + asyncInfo->ErrorCode.Value);
            return;
        }

        Diag::DebugPrint("IHttpContent.ReadAsInputStreamAsync succeeded.");

        inputStream = asyncInfo->GetResults();
        ReadMore();
    }
    catch (Exception^ ex)
    {
        Diag::DebugPrint("Error in OnReadAsInputStreamCompleted: " + ex->ToString());
    }
}

void CommunicationModule::OnReadCompleted(
    _In_ IAsyncOperationWithProgress<Windows::Storage::Streams::IBuffer^, unsigned int>^ asyncInfo,
    _In_ AsyncStatus asyncStatus)
{
    try
    {
        if (asyncStatus == AsyncStatus::Canceled)
        {
            Diag::DebugPrint("IInputStream.ReadAsync was canceled.");
            return;
        }

        if (asyncStatus == AsyncStatus::Error)
        {
            Diag::DebugPrint("IInputStream.ReadAsync failed: " + asyncInfo->ErrorCode.Value);
            return;
        }

        IBuffer^ buffer = asyncInfo->GetResults();

        Diag::DebugPrint("IInputStream.ReadAsync succeeded. " + buffer->Length + " bytes read.");

        if (buffer->Length == 0)
        {
            // The response is complete. Dispose channel so no more KATasks are executed.
            ResetRequest();
            return;
        }

        DataReader^ dataReader = DataReader::FromBuffer(buffer);
        String^ responseString = dataReader->ReadString(buffer->Length);

        // Enqueue the message received to a queue that the push notify task will pick up.
        AppContext^ appContext = dynamic_cast<AppContext^>(CoreApplication::Properties->Lookup("channelOne"));
        appContext->MessageQueue->Enqueue(responseString);

        ReadMore();
    }
    catch (Exception^ ex)
    {
        Diag::DebugPrint("Error in OnReadCompleted: " + ex->ToString());
    }
}

void CommunicationModule::AppendOriginToUri()
{
    // The origin allows to track where does the current request is being originated, i.e.: keep alive task,
    // network change task or click event.
    String^ uriString = serverUri->AbsoluteCanonicalUri;

    // Remove previous query string.
    std::wstring::size_type queryStringIndex = LastIndexOf(uriString, '?');
    if (queryStringIndex != std::wstring::npos)
    {
        uriString = Substring(uriString, 0, queryStringIndex);
    }

    serverUri = ref new Uri(uriString + "?origin=" + origin);
}

std::wstring::size_type CommunicationModule::LastIndexOf(_In_ String^ s, _In_ const wchar_t value)
{
    std::wstring sourceString(s->Data());
    return sourceString.find_last_of(value);
}

String^ CommunicationModule::Substring(
    _In_ String^ s,
    _In_ std::wstring::size_type startIndex,
    _In_ std::wstring::size_type length)
{
    std::wstring sourceString(s->Data());
    std::wstring substring = sourceString.substr(startIndex, length);
    return ref new String(substring.data());
}

bool CommunicationModule::RegisterWithCct()
{
    // Make sure the objects are created in a system thread that is guaranteed
    // to run in an MTA. Any objects that are required for use within background
    // tasks must not be affinitized to the ASTA.
    bool registerResult = false;
    auto registerTask = create_task([this, &registerResult]()
    {
        registerResult = RegisterWithCctHelper();
    });
    registerTask.wait();
    return registerResult;
}

bool CommunicationModule::RegisterWithCctHelper()
{
    bool result = false;

    httpClient = ref new HttpClient();

    // Specify the keepalive interval expected by the server for this app
    // in order of minutes.
    const int serverKeepAliveInterval = 15;

    // Specify the channelId string to differentiate this
    // channel instance from any other channel instance.
    // When the background task runs, the channel object is provided
    // as context and the channel id can be used to adapt the behavior
    // of the app as required.
    String^ channelId = "channelOne";

    // Try creating the ControlChannelTrigger if this has not been already created and stored
    // in the property bag.
    Diag::DebugPrint("RegisterWithCctHelper Starting...");

    Diag::DebugPrint("Create ControlChannelTrigger ...");

    // Create the ControlChannelTrigger object and request a hardware slot for this app.
    // If the app is not on lock screen, then the ControlChannelTrigger constructor will
    // fail right away.
    try
    {
        channel = ref new ControlChannelTrigger(
            channelId,
            serverKeepAliveInterval,
            ControlChannelTriggerResourceType::RequestHardwareSlot);
    }
    catch (Exception^ ex)
    {
        Diag::DebugPrint("Error while creating ControlChannelTrigger: " + ex->Message + " Please add the app to the lock screen.");
        return result;
    }

    // Register the apps background task with the trigger for keepalive.
    auto keepAliveBuilder = ref new BackgroundTaskBuilder();
    keepAliveBuilder->Name = "KeepaliveTaskForChannelOne";
    keepAliveBuilder->TaskEntryPoint = "BackgroundTaskHelper.KATask";
    keepAliveBuilder->SetTrigger(channel->KeepAliveTrigger);
    keepAliveBuilder->Register();

    // Register the apps background task with the trigger for push notification.
    auto pushNotifyBuilder = ref new BackgroundTaskBuilder();
    pushNotifyBuilder->Name = "PushNotificationTaskForChannelOne";
    pushNotifyBuilder->TaskEntryPoint = "BackgroundTaskHelper.PushNotifyTask";
    pushNotifyBuilder->SetTrigger(channel->PushNotificationTrigger);
    pushNotifyBuilder->Register();

    // Store the objects created in the property bag for later use.
    // NOTE: make sure these objects are free threaded. STA/Both objects can
    // cause deadlocks when foreground threads are suspended.
    if(CoreApplication::Properties->HasKey(channel->ControlChannelTriggerId))
    {
        CoreApplication::Properties->Remove(channel->ControlChannelTriggerId);
    }

    AppContext^ appContext = ref new AppContext(this);
    CoreApplication::Properties->Insert(channel->ControlChannelTriggerId, appContext);
    result = true;

    // Tie the transport method to the ControlChannelTrigger object to push enable it.
    // Note that if the transport's TCP connection is broken at a later point of time,
    // the ControlChannelTrigger object can be reused to plug in a new transport by
    // calling UsingTransport API again.
    try
    {
        // Send HTTP request
        SetUpHttpRequestAndSendToHttpServer();
        result = true;

        Diag::DebugPrint("RegisterWithCCTHelper Completed.");
    }
    catch (Exception^ ex)
    {
        Diag::DebugPrint("RegisterWithCCTHelper Task failed with: " + ex->Message);

        // Exceptions may be thrown for example if the application has not
        // registered the background task class id for using ControlChannelTrigger
        // in the package appx manifest.
        result = false;
    }
    return result;
}

bool CommunicationModule::SetUpTransport(_In_ String^ origin)
{
    bool result = false;

    this->origin = origin;

    try
    {
        // Limit lock scope.
        {
            // Hold lock.
            critical_section::scoped_lock slock(requestLock);

            // Set up the CCT with HttpClient.
            result = RegisterWithCct();
        }

        if (result == false)
        {
            Diag::DebugPrint("Failed to sign on and connect");
            ResetRequest();
        }
    }
    catch (Exception^ ex)
    {
        Diag::DebugPrint("Failed to sign on and connect. Exception: " + ex->ToString());
        ResetRequest();
    }

    return result;
}

bool CommunicationModule::SendKAMessage(_In_ String^ origin)
{
    // Hold lock.
    critical_section::scoped_lock slock(requestLock);

    this->origin = origin;

    // Here the keepalive task will abort earlier HTTP request and send a new HTTP request
    // as the same HTTP request cannot be sent twice.
    // We will try to get the URI from the earlier HTTP request, if it's not null and then use the same URI
    // to send the HTTP request from the keepalive task.
    if (httpClient == nullptr)
    {
        Diag::DebugPrint("HttpClient does not exist. Create another CCT enabled transport.");
        return false;
    }

    bool sendResult = false;
    auto sendTask = create_task([this, &sendResult]()
    {
        try
        {
            AppendOriginToUri();
            Diag::DebugPrint("Sending HTTP request from keepalive task with URI: " + serverUri->AbsoluteCanonicalUri);
            SetUpHttpRequestAndSendToHttpServer();
            sendResult = true;
        }
        catch (Exception^ ex)
        {
            Diag::DebugPrint("HttpClient write failed with error:  " + ex->ToString());
        }
    });
    sendTask.wait();
    return sendResult;
}

TSQueue::TSQueue()
{
    queue = new std::queue<String^,std::deque<String^,std::allocator<String^>>>();
}

TSQueue::~TSQueue()
{
    delete queue;
    queue = nullptr;
}

void TSQueue::Enqueue(_In_ String^ data)
{
    // Hold lock.
    critical_section::scoped_lock slock(queueLock);
    queue->push(data);
}

String^ TSQueue::Dequeue()
{
    String^ outdata;

    // Hold lock.
    critical_section::scoped_lock slock(queueLock);

    if(queue->empty())
    {
        outdata = nullptr;
    }
    else
    {
        outdata = queue->front();
        queue->pop();
    }

    return outdata;
}
