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

namespace StreamSocketTransportHelper
{
    public enum AppRole
    {
        ClientRole,
        ServerRole
    };
    public class CommModule : IDisposable
    {
        const int TIMEOUT = 30000;
        const int MAX_BUFFER_LENGTH = 100;

        public AppRole appRole { get; set; }


        // Below data members are used for client role.
        StreamSocket socket;
        public ControlChannelTrigger channel;
        DataReader readPacket;
        DataWriter writePacket;

        public string serverName, serverPort;

        // Used for server role.
        StreamSocket serverSocket;
        StreamSocketListener serverListener;

        public CommModule(AppRole Role)
        {
            appRole = Role;
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

                if (serverSocket != null)
                {
                    serverSocket.Dispose();
                    serverSocket = null;
                }

                if (serverListener != null)
                {
                    serverListener.Dispose();
                    serverListener = null;
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

        private bool RegisterWithControlChannelTrigger(string serverHostName, string serverPort)
        {
            // To simplify consistency issues for the commModule instance, 
            // demonstrate the core registration path to use async await 
            // but wait for the entire operation to complete before returning from this method.
            // The transport setup routine can be triggered by user control, by network state change
            // or by keepalive task and a typical app must be resilient against all of 
            Task<bool> registerTask = RegisterWithControlChannelTriggerHelper(serverHostName, serverPort);
            return registerTask.Result;
        }

        async Task<bool> RegisterWithControlChannelTriggerHelper(string serverHostName, string serverPort)
        {
            bool result = false;
            socket = new StreamSocket();

            // Specify the keepalive interval expected by the server for this app
            // in order of minutes.
            const int serverKeepAliveInterval = 30;

            // Specify the channelId string to differentiate this
            // channel instance from any other channel instance.
            // When background task fires, the channel object is provided
            // as context and the channel id can be used to adapt the behavior
            // of the app as required.
            const string channelId = "channelOne";

            // Try creating the controlchanneltrigger if this has not been already created and stored
            // in the property bag.
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
                Diag.DebugPrint("Please add the app to the lockscreen. " + exp.Message);
                return result;
            }

            // Register the apps background task with the trigger for keepalive.
            var keepAliveBuilder = new BackgroundTaskBuilder();
            keepAliveBuilder.Name = "KeepaliveTaskForChannelOne";
            keepAliveBuilder.TaskEntryPoint = "BackgroundTaskHelper.KATask";
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
            // the ControlChannelTrigger object can be reused to plugin a new transport by
            // calling UsingTransport API again.
            try
            {
                Diag.DebugPrint("Calling UsingTransport() ...");
                channel.UsingTransport(socket);

                // Connect the socket
                HostName hostName = new HostName(serverHostName);

                // If connect fails or times out it will throw exception.
                await socket.ConnectAsync(hostName, serverPort);

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
                if (((IDictionary<string, object>)CoreApplication.Properties).ContainsKey(channel.ControlChannelTriggerId))
                {
                    CoreApplication.Properties.Remove(channel.ControlChannelTriggerId);
                }

                var appContext = new AppContext(this, socket, channel, channel.ControlChannelTriggerId);
                ((IDictionary<string, object>)CoreApplication.Properties).Add(channel.ControlChannelTriggerId, appContext);

                Diag.DebugPrint("RegisterWithCCTHelper Completed.");
                result = true;

                // We are all set. Post a read to ensure push notificiation fires.
                PostSocketRead(MAX_BUFFER_LENGTH);
            }
            catch (Exception exp)
            {
                Diag.DebugPrint("RegisterWithCCTHelper failed with: " + exp.Message);

                // Exceptions may be thrown for example if the application has not 
                // registered the background task class id for using real time communications 
                // broker in the package appx manifest.
            }
            return result;
        }

        public static async Task AcceptConnection(string serviceName, CommModule myCommModule)
        {

            // Create and store a streamsocketlistener in the class. This way, new connections
            // can be automatically accepted.
            if (myCommModule.serverListener == null)
            {
                myCommModule.serverListener = new StreamSocketListener();
            }

            myCommModule.serverListener.ConnectionReceived += (op, evt) =>
            {
                // For simplicity, the server can talk to only one client at a time.
                myCommModule.serverSocket = evt.Socket;
                if (myCommModule.writePacket != null)
                {
                    myCommModule.writePacket.DetachStream();
                    myCommModule.writePacket = null;
                }

                Diag.DebugPrint("Connection Received!");
            };
            await myCommModule.serverListener.BindServiceNameAsync(serviceName);
            return;
        }

        bool StartListening(string serviceName)
        {
            bool result = false;
            try
            {
                Task acceptTask = AcceptConnection(serviceName, this);
                result = true;
            }
            catch (Exception exp)
            {
                Diag.DebugPrint("Failed to accept" + exp.ToString());
            }
            return result;
        }

        public bool SetupTransport(string serverHostName, string servicePort)
        {
            bool result = false;
            lock (this)
            {
                if (appRole == AppRole.ClientRole)
                {
                    // Save these to help reconnect later.
                    serverName = serverHostName;
                    serverPort = servicePort;

                    // Set up the ControlChannelTrigger with the stream socket.
                    result = RegisterWithControlChannelTrigger(serverHostName, serverPort);
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
                else
                {
                    //
                    // start listening on the port.
                    //
                    serverSocket = null;
                    result = StartListening(servicePort);
                    if (result == false)
                    {
                        Diag.DebugPrint("Failed to listen");
                    }
                }
            }

            return result;
        }

        // The read completion handler is guaranteed to fire before the 
        // pushnotification background task's Run method is invoked. The OS
        // has internal synchronization to wait for an app to return from the read
        // completion callback (in which the app typically quickly processes the 
        // data or the error that the transport has to notify). The message itself
        // is processed within the context of the Run method. In this sample, this point
        // is illustrated by using a message queue that the read completion inserts the
        // message into and the background task later processes.
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

            // check if buffLen is 0 and treat that as fatal error.
            if (buffLen == 0)
            {
                Diag.DebugPrint("Received zero bytes from the socket. Server must have closed the connection.");
                Diag.DebugPrint("Try disconnecting and reconnecting to the server");
                return;
            }

            // Perform minimal processing in the completion
            string message = readPacket.ReadString(buffLen);
            Diag.DebugPrint("Received Buffer : " + message);

            // Enqueue the message received to a queue that the push notify 
            // task will pick up.
            AppContext.messageQueue.Enqueue(message);

            // Post another receive to ensure future push notifications.
            PostSocketRead(MAX_BUFFER_LENGTH);
            Diag.DebugPrint("OnDataReadCompletion Exit");
        }

       void PostSocketRead(int length)
       {
           Diag.DebugPrint("Entering PostSocketRead");
           // IMPORTANT: When using winRT based transports such as StreamSocket with the ControlChannelTrigger,
           // we have to use the raw async pattern for handling reads instead of the await model. 
           // Using the raw async pattern allows Windows to synchronize the PushNotification task's 
           // IBackgroundTask::Run method with the return of the receive  completion callback. 
           // The Run method is invoked after the completion callback returns. This ensures that the app has
           // received the data/errors before the Run method is invoked.
           // It is important to note that the app has to post another read before it returns control from the completion callback.
           // It is also important to note that the DataReader is not directly used with the 
           // StreamSocket transport since that breaks the synchronization described above.
           // It is not supported to use DataReader's LoadAsync method directly on top of the transport. Instead,
           // the IBuffer returned by the transport's ReadAsync method can be later passed to DataReader::FromBuffer()
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

       public async void SendMessageHelper(StreamSocket socket, string message)
        {
            try
            {
                if (writePacket == null)
                {
                    writePacket = new DataWriter(socket.OutputStream);
                }

                writePacket.WriteString(message);
                Diag.DebugPrint("sending message:  " + message);
                await writePacket.StoreAsync();
            }
            catch (Exception exp)
            {
                Diag.DebugPrint("Socket write failed with error:  " + exp.Message);
            }
        }

        public void SendMessage(string message)
        {
            lock (this)
            {
                if (serverSocket != null)
                {
                    SendMessageHelper(serverSocket, message);
                }
                else
                {
                    Diag.DebugPrint("serverSocket has not been setup. click listen to accept incoming connection and try again.");
                }
            }
        }
        public bool SendKAMessage(string message)
        {
            bool result = true;
            lock (this)
            {
                if (socket != null)
                {
                    SendMessageHelper(socket, message);
                }
                else
                {
                    result = false;
                    Diag.DebugPrint("Socket does not exist. Create another CCT enabled transport.");
                }
            }
            return result;
        }
    }
    public class AppContext
    {
        public AppContext(CommModule commInstance, StreamSocket socket, ControlChannelTrigger channel, string id)
        {
            SocketHandle = socket;
            Channel = channel;
            ChannelId = id;
            CommInstance = commInstance;
            messageQueue = new ConcurrentQueue<string>();
        }

        public static ConcurrentQueue<string> messageQueue;

        public StreamSocket SocketHandle { get; set; }
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

