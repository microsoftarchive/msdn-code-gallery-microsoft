// WinCCTomponent.cpp
#include "pch.h"
#include "CommModule.h"
#include <ppltasks.h>
#include "DiagnosticsHelper.h"

using namespace concurrency;
using namespace Platform;
using namespace Platform::Collections::Details::WFC;
using namespace StreamWebSocketTransportHelper;
using namespace StreamWebSocketTransportHelper::DiagnosticsHelper;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Foundation;
using namespace Windows::Networking;

CoreDispatcher^ Diag::coreDispatcher=nullptr;
TextBlock^ Diag::debugOutputTextBlock=nullptr;
TSQueue^ AppContext::messageQueue_=nullptr;

CommModule::CommModule(AppRole appRole) : TIMEOUT(30000),MAX_BUFFER_LENGTH(100)
{
}

AppContext::AppContext(CommModule^ commInstance, StreamWebSocket^ socket, ControlChannelTrigger^ channel, String^ id)
{
    SocketHandle = socket;
    Channel = channel;
    ChannelId = id;
    CommInstance = commInstance;
    messageQueue = ref new TSQueue();
}
void CommModule::Reset()
{
    concurrency::critical_section::scoped_lock slock(lock);
    readPacket = nullptr;
    writePacket = nullptr;
    socket = nullptr;
    if (channel != nullptr)
    {
        if (CoreApplication::Properties->HasKey(channel->ControlChannelTriggerId))
        {
            CoreApplication::Properties->Remove(channel->ControlChannelTriggerId);
        }
        
        // Call the Dispose() method on the controlchanneltrigger object to release any 
        // OS maintained resources for this channel object. 
        delete channel_;
        channel_ = nullptr;
    }
    Diag::DebugPrint("CommModule has been reset.");    
}

bool CommModule::RegisterWithCCT(String^ serverUri)
{
    // To simplify consistency issues for the commModule instance, 
    // demonstrate the core registration path to use async tasks
    // but wait for the entire operation to complete before returning from this method.
    // The transport setup routine can be triggered by user control, by network state change
    // or by keepalive task and a typical app must be resilient against all of this.
    bool result = false;

    socket = ref new StreamWebSocket();

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
    
    // Create the controlchanneltrigger object and request a hardware slot for this app.
    // If the app is not on LockScreen, then the ControlChannelTrigger constructor will 
    // fail right away.
    try
    {
        channel = 
            ref new ControlChannelTrigger(channelId, serverKeepAliveInterval, ControlChannelTriggerResourceType::RequestHardwareSlot);                
    } 
    catch (AccessDeniedException^ e)
    {
        Diag::DebugPrint("Error: " + e->Message + " Please add the app on lockscreen.");
        return result;
    }
    
    // Register the apps background task with the trigger for keepalive.
    //
    // IMPORTANT: Note that this is a websocket sample, therefore the 
    // keepalive task class is provided by Windows for websockets. 
    // For websockets, the system does the keepalive on behalf of the
    // app but the app still needs to specify this well known keepalive task.
    // This should be done here in the background registration as well 
    // as in the package manifest.
    auto keepAliveBuilder = ref new BackgroundTaskBuilder();
    keepAliveBuilder->Name = "KeepaliveTaskForChannelOne";
    keepAliveBuilder->TaskEntryPoint = "Windows.Networking.Sockets.WebSocketKeepAlive";
    keepAliveBuilder->SetTrigger(channel->KeepAliveTrigger);
    keepAliveBuilder->Register();

    // Register the apps background task with the trigger for push notification task.
    auto pushNotifyBuilder = ref new BackgroundTaskBuilder();
    pushNotifyBuilder->Name = "PushNotificationTaskForChannelOne";
    pushNotifyBuilder->TaskEntryPoint = "Background.PushNotifyTask";
    pushNotifyBuilder->SetTrigger(channel->PushNotificationTrigger);
    pushNotifyBuilder->Register();

    // Tie the transport method to the controlchanneltrigger object to push enable it.
    // Note that if the transport's TCP connection is broken at a later point of time,
    // the controlchanneltrigger object can be reused to plugin a new transport by
    // calling UsingTransport API again.
    try
    {
        Diag::DebugPrint("Calling UsingTransport() ...");
        channel->UsingTransport(socket);
    }
    catch (Exception^ e)
    {
        Diag::DebugPrint("Error: " + e->Message);
        return result;
    }
    
    // Connect the socket
    //
    // If connect fails or times out it will throw exception.
    create_task(socket->ConnectAsync(ref new Uri(serverUri))).then([this, &status, &result](task<void> connectTask)
    {
        try
        {
            // Try getting any connect exception.
            connectTask.get();
            Diag::DebugPrint("Connected");
            
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
                result = false;
                throw ref new Exception(E_FAIL, "Neither hardware nor software slot could be allocated.");
            }
            
            // Store the objects created in the property bag for later use.
            // NOTE: make sure these objects are free threaded. STA/Both objects can 
            // cause deadlocks when foreground threads are suspended.
            if (CoreApplication::Properties->HasKey(channel->ControlChannelTriggerId))
            {
                CoreApplication::Properties->Remove(channel->ControlChannelTriggerId);
            }
            auto appContext = 
                ref new AppContext(this, socket, channel, channel->ControlChannelTriggerId);
            CoreApplication::Properties->Insert(channel->ControlChannelTriggerId, appContext);

            result = true;
            Diag::DebugPrint("RegisterCCT Completed.");
            
            // Almost done. Post a read since we are using stream web socket
            // to allow push notifications to be received.
            PostSocketRead(MAX_BUFFER_LENGTH);
        }
        catch (Exception^ exp)
        {
            Diag::DebugPrint("RegisterCCT Task failed with: " + exp->Message);
            
            // Exceptions may be thrown for example if the application has not 
            // registered the background task class id for using ControlChannelTrigger
            // in the package appx manifest or if the application was not on lockscreen
            // or if the connect failed or the app tried a loopback connect.
        }
    }).wait();
    return result;
}

bool CommModule::SetupTransport(String^ serverUri)
{       
    concurrency::critical_section::scoped_lock slock(lock);
    bool result = false;

    // Save these to help reconnect later.
    ServerUri = serverUri;
    
    // Set up the CCT channel with the stream socket.
    result = RegisterWithCCT(serverUri);
    if (result == false)
    {
        Diag::DebugPrint("Failed to sign on and connect");
        socket = nullptr;
        readPacket = nullptr;
        if (channel != nullptr)
        {
            // Explicitly dispose any slot/resources that were allocated.
            delete channel;
            channel = nullptr;
        }
    }
    
    return result;
}

void CommModule::PostSocketRead(int length)
{
    Diag::DebugPrint("Entering PostSocketRead");

    // IMPORTANT: When using winRT based transports such as StreamWebSocket with the ControlChannelTrigger,
    // we have to use the raw async pattern for handling reads instead of the ppl tasks model. 
    // Using the raw async pattern allows Windows to synchronize the PushNotification task's 
    // IBackgroundTask::Run method with the return of the receive  completion callback. 
    // The Run method is invoked after the completion callback returns. This ensures that the app has
    // received the data/errors before the Run method is invoked.
    // It is important to note that the app has to post another read before it returns control from the completion callback.
    // It is also important to note that the DataReader is not directly used with the 
    // StreamWebSocket transport since that breaks the synchronization described above.
    // It is not supported to use DataReader's LoadAsync method directly on top of the transport. Instead,
    // the IBuffer returned by the transport's ReadAsync method can be later passed to DataReader::FromBuffer()
    // for further processing.
    auto readBuf = ref new Buffer(static_cast<unsigned int>(length));
    auto readOp = socket->InputStream->ReadAsync(readBuf, length, InputStreamOptions::Partial);
    readOp->Completed = 
        ref new AsyncOperationWithProgressCompletedHandler<IBuffer^, unsigned int>([this](IAsyncOperationWithProgress<IBuffer^, unsigned int>^ asyncOp, AsyncStatus asyncStatus)
    {
        switch (asyncStatus)
        {
        case AsyncStatus::Completed:
        case AsyncStatus::Error:            
            try
            {
                // GetResults in AsyncStatus::Error is called as it throws a user friendly error string.
                auto localReadBuf = asyncOp->GetResults();
                unsigned int bytesRead = localReadBuf->Length;
                readPacket = DataReader::FromBuffer(localReadBuf);
                OnDataReadCompletion(bytesRead, readPacket);
            } 
            catch(Exception^ e)
            {
                Diag::DebugPrint("Read completion failed: " + e->Message);
            }
            break;
        case AsyncStatus::Canceled:

            // Read is not cancelled in this sample.
            break;
        }
    });
    Diag::DebugPrint("Leaving PostSocketRead");
}

void CommModule::OnDataReadCompletion(unsigned int bytesRead, DataReader^ readPacket)
{
    Diag::DebugPrint("OnDataReadCompletion Entry");
    if (readPacket == nullptr) 
    {
        Diag::DebugPrint("DataReader is null");
        
        // Ideally when read completion returns error, 
        // apps should be resilient and try to 
        // recover if there is an error by posting another recv
        // after creating a new transport, if required.
        return;
    }
    unsigned int buffLen = readPacket->UnconsumedBufferLength;
    Diag::DebugPrint("bytesRead: " + bytesRead + ", unconsumedbufflength: " + buffLen);            
    
    // Check if buffLen is 0 and treat that as fatal error.
    if (buffLen == 0)
    {
        Diag::DebugPrint("Received zero bytes from the socket. Server must have closed the connection.");
        Diag::DebugPrint("Try disconnecting and reconnecting to the server");
        return;
    }

    // Perform minimal processing in the completion.
    String^ message = readPacket->ReadString(buffLen);
    Diag::DebugPrint("Received Buffer : " + message);
    
    // Enqueue the message received to a queue that the push notify task will pick up.
    auto appContext = dynamic_cast<AppContext^>(CoreApplication::Properties->Lookup("channelOne"));
    appContext->messageQueue->Enqueue(message);
    
    // Post another receive to ensure future push notifications.
    PostSocketRead(MAX_BUFFER_LENGTH);
    Diag::DebugPrint("OnDataReadCompletion Exit");
}

void CommModule::SendMessage(String^ message)
{
    concurrency::critical_section::scoped_lock slock(lock);
    if (socket == nullptr)
    {
        Diag::DebugPrint("Please setup connection with the server first.");
    }
    else
    {
        if (writePacket == nullptr)
        {
            try
            {
                writePacket = ref new DataWriter(socket->OutputStream);
            } 
            catch (Exception^ e)
            {
                Diag::DebugPrint("Could not attach data writer to the socket");
                
                return;
            };
        }
        Diag::DebugPrint("Sending message to server: " + message);
        
        // Buffer any data we want to send.
        writePacket->UnicodeEncoding = Windows::Storage::Streams::UnicodeEncoding::Utf8;
        writePacket->WriteString(message);
        
        // Send the data as one complete message.
        create_task(writePacket->StoreAsync()).then([this](task<unsigned int> previousTask)
        {
            try
            {
                previousTask.get();
            }
            catch (Exception^ exception)
            {
                Diag::DebugPrint("Write task failed with error: " + exception->Message);
            }
        });
    }
    
}

TSQueue::TSQueue()
{
    queue = new std::queue<String^,std::deque<String^,std::allocator<String^>>>();
}

void TSQueue::Enqueue(String^ data)
{
    concurrency::critical_section::scoped_lock slock(lock);
    queue->push(data);
    
}

String^ TSQueue::Dequeue()
{
    String^ outdata;
    concurrency::critical_section::scoped_lock slock(lock);
    {
        if(queue->empty())
        {
            outdata = nullptr;
        }
        else
        {
            outdata = queue->front();
            queue->pop();
        }
    }
    
    return outdata;
}

