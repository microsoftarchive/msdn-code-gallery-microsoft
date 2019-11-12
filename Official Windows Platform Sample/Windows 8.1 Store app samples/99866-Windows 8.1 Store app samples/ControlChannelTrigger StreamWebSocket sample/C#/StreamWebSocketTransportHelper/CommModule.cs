using DiagnosticsHelper;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace StreamWebSocketTransportHelper
{

    public class CommModule : IDisposable
    {
        const int TIMEOUT = 30000;
        const int MAX_BUFFER_LENGTH = 100;

        StreamWebSocket socket;
        public ControlChannelTrigger channel;
        DataReader readPacket;
        public string serverUri;
        DataWriter writePacket;

        public CommModule()
        {
        }

        public void Dispose()
        {
            Reset();
            GC.SuppressFinalize(this);
        }

        public void Reset()
        {
            lock (this)
            {
                if (readPacket != null)
                {
                    try
                    {
                        readPacket.DetachStream();
                        readPacket = null;
                    }
                    catch (Exception exp)
                    {
                        Diag.DebugPrint("Could not detach DataReader: " + exp.Message);
                    }
                }

                if (writePacket != null)
                {
                    try
                    {
                        writePacket.DetachStream();
                        writePacket = null;
                    }
                    catch (Exception exp)
                    {
                        Diag.DebugPrint("Could not detach DataWriter: " + exp.Message);
                    }
                }

                if (socket != null)
                {
                    socket.Dispose();
                    socket = null;
                }

                if (channel != null)
                {
                    if (((IDictionary<string, object>)CoreApplication.Properties).ContainsKey(channel.ControlChannelTriggerId))
                    {
                        CoreApplication.Properties.Remove(channel.ControlChannelTriggerId);
                    }

                    // Call the Dispose() method on the controlchanneltrigger object to release any 
                    // OS maintained resources for this channel object. 
                    channel.Dispose();
                    channel = null;
                }
                Diag.DebugPrint("CommModule has been reset.");
            }
        }

        private bool RegisterWithControlChannelTrigger(string serverUri)
        {

            // Make sure the objects are created in a system thread that are guaranteed
            // to run in an MTA. Any objects that are required for use within background
            // tasks must not be affinitized to the ASTA.
            //
            // To simplify consistency issues for the commModule instance, 
            // demonstrate the core registration path to use async await 
            // but wait for the entire operation to complete before returning from this method.
            // The transport setup routine can be triggered by user control, by network state change
            // or by keepalive task and a typical app must be resilient against all of 
            Task<bool> registerTask = RegisterWithCCTHelper(serverUri);
            return registerTask.Result;
        }

        async Task<bool> RegisterWithCCTHelper(string serverUri)
        {
            bool result = false;
            socket = new StreamWebSocket();

            // Specify the keepalive interval expected by the server for this app
            // in order of minutes.
            const int serverKeepAliveInterval = 30;

            // Specify the channelId string to differentiate this
            // channel instance from any other channel instance.
            // When background task fires, the channel object is provided
            // as context and the channel id can be used to adapt the behavior
            // of the app as required.
            const string channelId = "channelOne";

            // IMPORTANT: Note that this is a websocket sample, therefore the 
            // keepalive task class is provided by Windows for websockets. 
            // For websockets, the system does the keepalive on behalf of the
            // app but the app still needs to specify this well known keepalive task.
            // This should be done here in the background registration as well 
            // as in the package manifest.
            const string WebSocketKeepAliveTask = "Windows.Networking.Sockets.WebSocketKeepAlive";

            // Try creating the controlchanneltrigger if this has not been already 
            // created and stored in the property bag.
            Diag.DebugPrint("RegisterWithCCTHelper Starting...");
            ControlChannelTriggerStatus status;
            Diag.DebugPrint("Create ControlChannelTrigger ...");

            // Create the ControlChannelTrigger object and request a hardware slot for this app.
            // If the app is not on LockScreen, then the ControlChannelTrigger constructor will 
            // fail right away.
            try
            {
                channel = new ControlChannelTrigger(channelId, serverKeepAliveInterval,
                                   ControlChannelTriggerResourceType.RequestHardwareSlot);
            }
            catch (UnauthorizedAccessException exp)
            {
                Diag.DebugPrint("Please add the app to the lock screen." + exp.Message);
                return result;
            }

            Uri serverUriInstance;
            try
            {
                serverUriInstance = new Uri(serverUri);
            }
            catch (Exception exp)
            {
                Diag.DebugPrint("Error creating URI: " + exp.Message);
                return result;
            }

            // Register the apps background task with the trigger for keepalive.
            var keepAliveBuilder = new BackgroundTaskBuilder();
            keepAliveBuilder.Name = "KeepaliveTaskForChannelOne";
            keepAliveBuilder.TaskEntryPoint = WebSocketKeepAliveTask;
            keepAliveBuilder.SetTrigger(channel.KeepAliveTrigger);
            keepAliveBuilder.Register();

            // Register the apps background task with the trigger for push notification task.
            var pushNotifyBuilder = new BackgroundTaskBuilder();
            pushNotifyBuilder.Name = "PushNotificationTaskForChannelOne";
            pushNotifyBuilder.TaskEntryPoint = "BackgroundTaskHelper.PushNotifyTask";
            pushNotifyBuilder.SetTrigger(channel.PushNotificationTrigger);
            pushNotifyBuilder.Register();

            // Tie the transport method to the ControlChannelTrigger object to push enable it.
            // Note that if the transport's TCP connection is broken at a later point of time,
            // the ControlChannelTrigger object can be reused to plug in a new transport by
            // calling UsingTransport API again.
            Diag.DebugPrint("Calling UsingTransport() ...");
            try
            {

                channel.UsingTransport(socket);

                // Connect the socket
                //
                // If connect fails or times out it will throw exception.
                await socket.ConnectAsync(serverUriInstance);

                Diag.DebugPrint("Connected");

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
                status = channel.WaitForPushEnabled();
                Diag.DebugPrint("WaitForPushEnabled() completed with status: " + status);
                if (status != ControlChannelTriggerStatus.HardwareSlotAllocated
                    && status != ControlChannelTriggerStatus.SoftwareSlotAllocated)
                {
                    throw new Exception(string.Format("Neither hardware nor software slot could be allocated. ChannelStatus is {0}", status.ToString()));
                }

                // Store the objects created in the property bag for later use.
                // NOTE: make sure these objects are free threaded. STA/Both objects can 
                // cause deadlocks when foreground threads are suspended.
                CoreApplication.Properties.Remove(channel.ControlChannelTriggerId);

                var appContext = new AppContext(this, socket, channel, channel.ControlChannelTriggerId);
                ((IDictionary<string, object>)CoreApplication.Properties).Add(channel.ControlChannelTriggerId, appContext);
                result = true;
                Diag.DebugPrint("RegisterWithCCTHelper Completed.");

                // Almost done. Post a read since we are using streamwebsocket
                // to allow push notifications to be received.
                PostSocketRead(MAX_BUFFER_LENGTH);
            }
            catch (Exception exp)
            {
                Diag.DebugPrint("RegisterWithCCTHelper Task failed with: " + exp.Message);

                // Exceptions may be thrown for example if the application has not 
                // registered the background task class id for using real time communications 
                // broker in the package appx manifest.
            }
            return result;
        }

        public bool SetupTransport(string serviceUri)
        {
            bool result = false;
            lock (this)
            {
                // Save these to help reconnect later.
                serverUri = serviceUri;

                // Set up the ControlChannelTrigger with the stream socket.
                result = RegisterWithControlChannelTrigger(serverUri);
                if (result == false)
                {
                    Diag.DebugPrint("Failed to sign on and connect");
                    if (socket != null)
                    {
                        socket.Dispose();
                        socket = null;
                        readPacket = null;
                    }
                    if (channel != null)
                    {
                        channel.Dispose();
                        channel = null;
                    }
                }
            }
            return result;
        }

        public void
        OnDataReadCompletion(uint bytesRead, DataReader readPacket)
        {
            Diag.DebugPrint("OnDataReadCompletion Entry");
            if (readPacket == null)
            {
                Diag.DebugPrint("DataReader is null");

                // Ideally when read completion returns error, 
                // apps should be resilient and try to 
                // recover if there is an error by posting another recv
                // after creating a new transport, if required.
                return;
            }
            uint buffLen = readPacket.UnconsumedBufferLength;
            Diag.DebugPrint("bytesRead: " + bytesRead + ", unconsumedbufflength: " + buffLen);

            // Check if buffLen is 0 and treat that as fatal error.
            if (buffLen == 0)
            {
                Diag.DebugPrint("Received zero bytes from the socket. Server must have closed the connection.");
                Diag.DebugPrint("Try disconnecting and reconnecting to the server");
                return;
            }

            // Perform minimal processing in the completion
            string message = readPacket.ReadString(buffLen);
            Diag.DebugPrint("Received Buffer : " + message);

            // Enqueue the message received to a queue that the push notify  task will pick up.
            AppContext.messageQueue.Enqueue(message);

            // Post another receive to ensure future push notifications.
            PostSocketRead(MAX_BUFFER_LENGTH);
            Diag.DebugPrint("OnDataReadCompletion Exit");
        }

        void PostSocketRead(int length)
        {
            Diag.DebugPrint("Entering PostSocketRead");
            // IMPORTANT: When using winRT based transports such as StreamWebSocket with the ControlChannelTrigger,
            // we have to use the raw async pattern for handling reads instead of the await model. 
            // Using the raw async pattern allows Windows to synchronize the PushNotification task's 
            // IBackgroundTask::Run method with the return of the receive  completion callback. 
            // The Run method is invoked after the completion callback returns. This ensures that the app has
            // received the data/errors before the Run method is invoked.
            // It is important to note that the app has to post another read before it returns control from the completion callback.
            // It is also important to note that the DataReader is not directly used with the 
            // StreamWebSocket transport since that breaks the synchronization described above.
            // It is not supported to use DataReader's LoadAsync method directly on top of the transport. Instead,
            // the IBuffer returned by the transport's  ReadAsync method can be later passed to DataReader::FromBuffer()
            // for further processing.
            try
            {
                var readBuf = new Windows.Storage.Streams.Buffer((uint)length);
                var readOp = socket.InputStream.ReadAsync(readBuf, (uint)length, InputStreamOptions.Partial);
                readOp.Completed = (IAsyncOperationWithProgress<IBuffer, uint> asyncAction, AsyncStatus asyncStatus) =>
                {
                    switch (asyncStatus)
                    {
                        case AsyncStatus.Completed:
                        case AsyncStatus.Error:
                            try
                            {
                                // GetResults in AsyncStatus::Error is called as it throws a user friendly error string.
                                IBuffer localBuf = asyncAction.GetResults();
                                uint bytesRead = localBuf.Length;
                                readPacket = DataReader.FromBuffer(localBuf);
                                OnDataReadCompletion(bytesRead, readPacket);
                            }
                            catch (Exception exp)
                            {
                                Diag.DebugPrint("Read operation failed:  " + exp.Message);
                            }
                            break;
                        case AsyncStatus.Canceled:

                            // Read is not cancelled in this sample.
                            break;
                    }
                };
            }
            catch (Exception exp)
            {
                Diag.DebugPrint("failed to post a read failed with error:  " + exp.Message);
            }
            Diag.DebugPrint("Leaving PostSocketRead");
        }

        public async void SendMessage(string message)
        {
            if (socket == null)
            {
                Diag.DebugPrint("Please setup connection with the server first.");
                return;
            }
            try
            {
                if (writePacket == null)
                {
                    writePacket = new DataWriter(socket.OutputStream);
                }
                Diag.DebugPrint("Sending message to server: " + message);

                // Buffer any data we want to send.
                writePacket.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                writePacket.WriteString(message);

                // Send the data as one complete message.
                await writePacket.StoreAsync();
            }
            catch (Exception exp)
            {
                Diag.DebugPrint("failed to write into the streamwebsocket: " + exp.Message);
            }
        }
        public string GetServerUri()
        {
            return serverUri;
        }
        public ControlChannelTrigger GetChannel()
        {
            return channel;
        }
    }

    public class AppContext
    {
        public AppContext(CommModule commInstance, StreamWebSocket webSocket, ControlChannelTrigger channel, string id)
        {
            WebSocketHandle = webSocket;
            Channel = channel;
            ChannelId = id;
            CommInstance = commInstance;
            messageQueue = new ConcurrentQueue<string>();
        }

        public static ConcurrentQueue<string> messageQueue;

        public StreamWebSocket WebSocketHandle { get; set; }
        public ControlChannelTrigger Channel { get; set; }
        public string ChannelId { get; set; }
        public CommModule CommInstance { get; set; }
    }
}


namespace DiagnosticsHelper
{
    public static class Diag
    {
        public static CoreDispatcher coreDispatcher;
        public static TextBlock debugOutputTextBlock;
        public static async void DebugPrint(string msg)
        {
            Debug.WriteLine(msg);
            if (coreDispatcher != null)
            {
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        debugOutputTextBlock.Text =
                            debugOutputTextBlock.Text +
                                DateTime.Now.ToString(@"M/d/yyyy hh:mm:ss tt") + " " + msg + "\r\n";
                    });
            }
        }
    }
}