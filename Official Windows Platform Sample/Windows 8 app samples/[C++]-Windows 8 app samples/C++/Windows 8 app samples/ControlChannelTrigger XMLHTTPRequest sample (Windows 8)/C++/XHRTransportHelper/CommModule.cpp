// WinRTComponent.cpp
#include "pch.h"
#include "CommModule.h"
#include <ppltasks.h>
#include <shcore.h>
#include "DiagnosticsHelper.h"


using namespace concurrency;
using namespace Platform;
using namespace Platform::Collections::Details::WFC;
using namespace Microsoft::WRL;
using namespace Microsoft::WRL::Details;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::Foundation;
using namespace Windows::Networking;
using namespace Windows::Storage::Streams;
using namespace XHRTransportHelper;
using namespace XHRTransportHelper::DiagnosticsHelper;



CoreDispatcher^ Diag::coreDispatcher=nullptr;
TextBlock^ Diag::debugOutputTextBlock=nullptr;
TSQueue^ AppContext::messageQueue_=nullptr;

CommModule::CommModule(String^ url, String ^postData)
    : m_canaryRequestStatus(CanaryRequestStatus::CanaryNotStarted),
    m_url(url),
    m_postData(postData),
    m_xhr(nullptr),
    m_xhrInspectable(nullptr)
{
}

AppContext::AppContext(CommModule^ commInstance, ControlChannelTrigger^ channel, String^ id)
{
    Channel = channel;
    ChannelId = id;
    CommInstance = commInstance;
    messageQueue = ref new TSQueue();
}

CommModule::~CommModule(void)
{
    if (m_xhr != nullptr)
    {
        m_xhr->Abort();
        m_xhr->Release();
        m_xhr = nullptr;
    }
}

void CommModule::Reset()
{
    CSection.lock();

    m_canaryRequestStatus = CanaryRequestStatus::CanaryNotStarted;

    if (this->channel != nullptr)
    {
        if (CoreApplication::Properties->HasKey(channel->ControlChannelTriggerId))
        {
            CoreApplication::Properties->Remove(channel->ControlChannelTriggerId);
        }
        // Call the Dispose() method on the controlchanneltrigger object to release any
        // OS maintained resources for this channel object.
        delete this->channel_;
        this->channel_ = nullptr;
    }
    Diag::DebugPrint("CommModule has been reset.");
    CSection.unlock();
}

bool CommModule::RegisterWithCCT()
{
    // Make sure the objects are created in a system thread that are guaranteed
    // to run in an MTA. Any objects that are required for use within background
    // tasks must not be affinitized to the ASTA.
    bool registerResult=false;
    auto registerTask = create_task([this, &registerResult]()
    {
        registerResult = RegisterWithCCTHelper();
    });
    registerTask.wait();
    return registerResult;
}


bool CommModule::RegisterWithCCTHelper()
{
    bool result = false;

    // Specify the keepalive interval expected by the server for this app
    // in order of minutes.
    const int serverKeepAliveInterval = 30;

    // Specify the channelId string to differentiate this
    // channel instance from any other channel instance.
    // When background task fires, the channel object is provided
    // as context and the channel id can be used to adapt the behavior
    // of the app as required.
    String^ channelId = "channelOne";

    // Try creating the controlchanneltrigger if this has not been already created and stored
    // in the property bag.
    Diag::DebugPrint("RegisterCCT Starting...");

    ControlChannelTriggerStatus status;
    Diag::DebugPrint("Create ControlChannelTrigger ...");

    // Create the ControlChannelTrigger object and request a hardware slot for this app.
    // If the app is not on LockScreen, then the ControlChannelTrigger constructor will
    // fail right away.
    try
    {
        this->channel =
            ref new ControlChannelTrigger(channelId, serverKeepAliveInterval, ControlChannelTriggerResourceType::RequestHardwareSlot);
    }
    catch (Exception^ e)
    {
        Diag::DebugPrint("Error while creating controlchanneltrigger" + e->Message + " Is the app on lockscreen?");
        return result;
    }
    // Register the apps background task with the trigger for keepalive.
    auto keepAliveBuilder = ref new BackgroundTaskBuilder();
    keepAliveBuilder->Name = "KeepaliveTaskForChannelOne";
    keepAliveBuilder->TaskEntryPoint = "Background.KATask";
    keepAliveBuilder->SetTrigger(channel->KeepAliveTrigger);
    keepAliveBuilder->Register();

    // Register the apps background task with the trigger for push notification task.
    auto pushNotifyBuilder = ref new BackgroundTaskBuilder();
    pushNotifyBuilder->Name = "PushNotificationTaskForChannelOne";
    pushNotifyBuilder->TaskEntryPoint = "Background.PushNotifyTask";
    pushNotifyBuilder->SetTrigger(channel->PushNotificationTrigger);
    pushNotifyBuilder->Register();

    // Tie the transport method to the ControlChannelTrigger object to push enable it.
    // Note that if the transport's TCP connection is broken at a later point of time,
    // the ControlChannelTrigger object can be reused to plugin a new transport by
    // calling UsingTransport API again.
    try
    {
        if (!PrepareXHRPost())
        {
            Diag::DebugPrint("PrepareXHRPost failed.");
            return false;
        }

        Diag::DebugPrint("Calling UsingTransport() ...");
        channel->UsingTransport(reinterpret_cast<Object^>(m_xhrInspectable));
        m_xhrInspectable->Release();
        m_xhrInspectable = nullptr;

        if (!DoXHRPost())
        {
            Diag::DebugPrint("DoXHRPost failed.");
            return false;
        }

        // Call WaitForPushEnabled API to make sure the TCP connection has
        // been established, which will mean that the OS will have allocated
        // any hardware slot for this TCP connection.
        //
        // In this sample, the ControlChannelTrigger object was created by
        // explicitly requesting a hardware slot.
        //
        // On Non-AOAC systems, if app requests hardware slot as above,
        // the system will fallback to a software slot automatically.
        //
        // On AOAC systems, if no hardware slot is available, then app
        // can request a software slot [by re-creating the ControlChannelTrigger object].
        status = channel->WaitForPushEnabled();
        Diag::DebugPrint("WaitForPushEnabled() completed with status: " + status.ToString());
        if (status != ControlChannelTriggerStatus::HardwareSlotAllocated
            && status != ControlChannelTriggerStatus::SoftwareSlotAllocated)
        {
            Diag::DebugPrint("Neither hardware nor software slot was allocated.");
            return false;
        }

        // Store the objects created in the property bag for later use.
        // NOTE: make sure these objects are free threaded. STA/Both objects can
        // cause deadlocks when foreground threads are suspended.
        if(CoreApplication::Properties->HasKey(channel->ControlChannelTriggerId))
        {
            CoreApplication::Properties->Remove(channel->ControlChannelTriggerId);
        }
        AppContext^ appContext = ref new AppContext(this, channel, channel->ControlChannelTriggerId);
        CoreApplication::Properties->Insert(channel->ControlChannelTriggerId, appContext);
        result = true;
        Diag::DebugPrint("RegisterCCT Completed.");
    }
    catch (Exception^ exp)
    {
        Diag::DebugPrint("RegisterCCT Task failed with: " + exp->Message);

        // Exceptions may be thrown for example if the application has not
        // registered the background task class id for using ControlChannelTrigger
        // in the package appx manifest.
    }
    return result;
}

bool CommModule::SetupTransport()
{
    bool result = false;
    bool XhrLocked = false;

    m_XhrLock.lock();
    XhrLocked = true;

    if (m_xhr != nullptr ||
        m_canaryRequestStatus != CanaryRequestStatus::CanarySucceed)
    {
        // You must not set up another transport when the previous one is not
        // terminated properly or canary request is not succeed.
        Diag::DebugPrint("System is not ready for a new SetupTransport.");
        goto Exit;
    }

    m_XhrLock.unlock();
    XhrLocked = false;

    CSection.lock();

    result = RegisterWithCCT();

    CSection.unlock();

Exit:

    if (XhrLocked)
    {
        m_XhrLock.unlock();
        XhrLocked = false;
    }

    return result;
}

void CommModule::OnDataReadCompletion(String^ message)
{
    Diag::DebugPrint("OnDataReadCompletion Entry");

    // Perform minimal processing in the completion
    Diag::DebugPrint("Received Buffer : " + message);

    // Enqueue the message received to a queue that the push notify
    // task will pick up.
    auto appContext = dynamic_cast<AppContext^>(CoreApplication::Properties->Lookup("channelOne"));
    appContext->messageQueue->Enqueue(message);

    // Post another receive to ensure future push notifications.
    Diag::DebugPrint("OnDataReadCompletion Exit");
}

bool CommModule::SendKAMessage(String^ Message)
{
    // There're no keep alive messages in HTTP.
    return TRUE;
}

void CommModule::OnCanarySuccess()
{
    IXMLHTTPRequest2* xhr = nullptr;

    m_XhrLock.lock();

    xhr = m_xhr;
    m_xhr = nullptr;
    m_canaryRequestStatus = CanaryRequestStatus::CanarySucceed;

    m_XhrLock.unlock();

    if (xhr != nullptr)
    {
        xhr->Release();
        xhr = nullptr;
    }
}

void CommModule::OnCanaryFailure()
{
    IXMLHTTPRequest2* xhr = nullptr;

    m_XhrLock.lock();

    xhr = m_xhr;
    m_xhr = nullptr;
    m_canaryRequestStatus = CanaryRequestStatus::CanaryNotStarted;

    m_XhrLock.unlock();

    if (xhr != nullptr)
    {
        xhr->Release();
        xhr = nullptr;
    }
}

void CommModule::OnRequestSuccess()
{
    IXMLHTTPRequest2* xhr = nullptr;

    m_XhrLock.lock();

    xhr = m_xhr;
    m_xhr = nullptr;

    m_XhrLock.unlock();

    if (xhr != nullptr)
    {
        xhr->Release();
        xhr = nullptr;
    }

    try
    {
        if (this->channel_ != nullptr)
        {
            OnDataReadCompletion("POST to " + m_url + " succeeded");
        }
        else
        {
            Diag::DebugPrint("Channel was reset");
        }
    }
    catch (Exception^ exp)
    {
        Diag::DebugPrint("OnDataReadCompletion Failed. Error was  " + exp->Message);
    }
}

void CommModule::OnRequestFailure()
{
    IXMLHTTPRequest2* xhr = nullptr;

    m_XhrLock.lock();

    xhr = m_xhr;
    m_xhr = nullptr;

    m_XhrLock.unlock();

    if (xhr != nullptr)
    {
        xhr->Release();
        xhr = nullptr;
    }

    Diag::DebugPrint("Failed to post " + m_url);
}

bool CommModule::PrepareXHRPost()
{
    bool Result = false;
    ComPtr<IXMLHTTPRequest2> xhr;
    ComPtr<XMLHTTPRequest2Callback> myCallback;
    ComPtr<IXMLHTTPRequest2Callback>  xhrCallback;
    ComPtr<IInspectable> xhrInspectable;
    MULTI_QI mutliQi = {&IID_IXMLHTTPRequest2, nullptr, S_OK};
    HRESULT hr = S_FALSE;

    // IXMLHTTPRequest2 is not a managed object, thus we need to call
    // CoCreateInstanceFromApp to create an instance. XHR instance is one-time
    // only for each http transaction, that is, one http request and its
    // response.
    hr = CoCreateInstanceFromApp(CLSID_FreeThreadedXMLHTTP60,
        nullptr,
        CLSCTX_INPROC,
        nullptr,
        1,
        &mutliQi);
    if (FAILED(mutliQi.hr))
    {
        hr = mutliQi.hr;
        goto Exit;
    }

    if (FAILED(hr))
    {
        goto Exit;
    }

    xhr.Attach(static_cast<IXMLHTTPRequest2*>(mutliQi.pItf));
    mutliQi.pItf = NULL;

    try
    {
        hr = MakeAndInitialize<XMLHTTPRequest2Callback>(&myCallback,
            ref new RequestSucceedHandler(this, &CommModule::OnRequestSuccess),
            ref new RequestFailHandler(this, &CommModule::OnRequestFailure));
        if (FAILED(hr))
        {
            goto Exit;
        }
    }
    catch (Exception^)
    {
        hr = E_OUTOFMEMORY;
        goto Exit;
    }

    hr = myCallback.As(&xhrCallback);
    if (FAILED(hr))
    {
        goto Exit;
    }

    hr = xhr->Open(L"POST",             // Method
        m_url->Data(),     // Url
        xhrCallback.Get(), // Callback
        L"",               // Username
        L"",               // Password
        L"",               // Proxy username
        L"");              // Proxy password

    if (FAILED(hr))
    {
        goto Exit;
    }

    hr  = xhr.As(&xhrInspectable);

    if (FAILED(hr))
    {
        goto Exit;
    }

    m_XhrLock.lock();

    m_xhr = xhr.Detach();
    m_xhrInspectable = xhrInspectable.Detach();
    xhr = nullptr;
    xhrInspectable = nullptr;

    m_XhrLock.unlock();

    Result = true;

Exit:

    if (xhr != nullptr)
    {
        xhr->Abort();
    }

    return Result;
}

bool CommModule::DoXHRPost()
{
    bool Result = false;
    ULONG BytesWritten = 0;
    ComPtr<IStream> postStream;
    IXMLHTTPRequest2* xhr = nullptr;
    InMemoryRandomAccessStream^ randomAccessStream = nullptr;
    HRESULT hr = S_FALSE;

    m_XhrLock.lock();

    xhr = m_xhr;

    m_XhrLock.unlock();

    if (xhr == nullptr)
    {
        goto Exit;
    }

    try
    {
        randomAccessStream = ref new InMemoryRandomAccessStream();
    }
    catch (Exception^)
    {
        hr = E_OUTOFMEMORY;
        goto Exit;
    }

    hr = CreateStreamOverRandomAccessStream(randomAccessStream,
                                            IID_PPV_ARGS(&postStream));
    if (FAILED(hr))
    {
        goto Exit;
    }

    // When posting data to a real server, the application must encode the post
    // data properly as required by the server.
    hr = postStream->Write(m_postData->Data(),
                           m_postData->Length(),
                           &BytesWritten);
    if (FAILED(hr) ||
        BytesWritten == 0)
    {
        goto Exit;
    }

    // XHR will hold references to the stream object when send returns S_OK.
    // Hence it's safe to release our references when exit this function.
    hr = xhr->Send(postStream.Get(), BytesWritten);
    if (FAILED(hr))
    {
        goto Exit;
    }

    Result = true;

Exit:

    if (!Result)
    {
        xhr->Abort();
        xhr = nullptr;
    }

    return Result;
}

bool CommModule::SendCanaryRequestHelper(String^ url)
{
    ComPtr<IXMLHTTPRequest2> xhr;
    ComPtr<XMLHTTPRequest2Callback> myCallback;
    ComPtr<IXMLHTTPRequest2Callback>  xhrCallback;
    HRESULT hr = S_OK;
    DWORD dwStatus = 0;
    bool Result = false;
    MULTI_QI mutliQi = {&IID_IXMLHTTPRequest2, nullptr, S_OK};

    // Canary request is being processed in a sync manner. This instance
    // will be released once the request is completed on either success or
    // failure.
    hr = CoCreateInstanceFromApp(CLSID_FreeThreadedXMLHTTP60,
        nullptr,
        CLSCTX_INPROC,
        nullptr,
        1,
        &mutliQi);
    if (FAILED(mutliQi.hr))
    {
        hr = mutliQi.hr;
        goto Exit;
    }

    if (FAILED(hr))
    {
        goto Exit;
    }

    xhr.Attach(static_cast<IXMLHTTPRequest2*>(mutliQi.pItf));
    mutliQi.pItf = NULL;

    try
    {
        hr = MakeAndInitialize<XMLHTTPRequest2Callback>(&myCallback,
            ref new RequestSucceedHandler(this, &CommModule::OnCanarySuccess),
            ref new RequestFailHandler(this, &CommModule::OnCanaryFailure));
        if (FAILED(hr))
        {
            goto Exit;
        }
    }
    catch (Exception^)
    {
        hr = E_OUTOFMEMORY;
        goto Exit;
    }

    hr = myCallback.As(&xhrCallback);
    if (FAILED(hr))
    {
        goto Exit;
    }

    hr = xhr->Open(L"HEAD",
        url->Begin(),
        xhrCallback.Get(),
        L"",
        L"",
        L"",
        L"");
    if (FAILED(hr))
    {
        goto Exit;
    }

    hr = xhr->Send(nullptr, 0);
    if (FAILED(hr))
    {
        goto Exit;
    }

    m_XhrLock.lock();

    m_xhr = xhr.Detach();
    xhr = nullptr;

    m_XhrLock.unlock();

    Result = true;

Exit:

    if (xhr != nullptr)
    {
        xhr->Abort();
    }

    return Result;
}

bool CommModule::SendCanaryRequest()
{
    bool result = false;

    // In this sample, the canary request is employed to handle authentication
    // issues so that the always connected XHR transport can be established
    // properly.
    switch(m_canaryRequestStatus)
    {
    case CanaryRequestStatus::CanaryInProcess:
    case CanaryRequestStatus::CanarySucceed:
        result = false;
        break;

    case CanaryRequestStatus::CanaryNotStarted:
        m_XhrLock.lock();
        m_canaryRequestStatus = CanaryRequestStatus::CanaryInProcess;
        m_XhrLock.unlock();
        result = SendCanaryRequestHelper(m_url);
        break;
    }

    return result;
}

TSQueue::TSQueue()
{
    queue=new std::queue<String^,std::deque<String^,std::allocator<String^>>>();
}

TSQueue::~TSQueue()
{
    delete queue;
    queue=nullptr;
}

void TSQueue::Enqueue(String^ data)
{
    CSection.lock();
    {
        queue->push(data);
    }
    CSection.unlock();
}

String^ TSQueue::Dequeue()
{
    String^ outdata;
    CSection.lock();
    {
        if(queue->empty())
        {
            outdata=nullptr;
        }
        else
        {
            outdata = queue->front();
            queue->pop();
        }
    }
    CSection.unlock();
    return outdata;
}
