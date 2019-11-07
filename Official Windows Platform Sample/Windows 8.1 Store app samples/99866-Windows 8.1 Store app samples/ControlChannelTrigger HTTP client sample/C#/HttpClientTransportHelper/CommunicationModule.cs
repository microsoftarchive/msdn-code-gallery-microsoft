// Copyright (c) Microsoft Corporation. All rights reserved.

using DiagnosticsHelper;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

namespace HttpClientTransportHelper
{
    public class CommunicationModule : IDisposable
    {
        public ControlChannelTrigger channel;
        public Uri serverUri;
        private string origin;
        private HttpClient httpClient;
        private HttpRequestMessage httpRequest;
        private IInputStream inputStream;
        private IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> sendRequestOperation;
        private IAsyncOperationWithProgress<IInputStream, ulong> readAsInputStreamOperation;
        private IAsyncOperationWithProgress<IBuffer, uint> readOperation;
        private static Guid kaTaskId = Guid.Empty;
        private static Guid pushNotifyTaskId = Guid.Empty;

        public CommunicationModule()
        {
        }

        private void DisposeProperties()
        {
            Diag.DebugPrint("Entering cleanup");
            if (httpClient != null)
            {
                httpClient.Dispose();
                httpClient = null;
            }
            if (channel != null)
            {
                channel.Dispose();
                channel = null;
            }
            Diag.DebugPrint("Exiting cleanup");
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
                Diag.DebugPrint("CommunicationModule is being reset.");

                if (channel != null)
                {
                    if (((IDictionary<string, object>)CoreApplication.Properties).ContainsKey(channel.ControlChannelTriggerId))
                    {
                        CoreApplication.Properties.Remove(channel.ControlChannelTriggerId);
                    }
                }

                ResetRequest();
                DisposeProperties();

                Diag.DebugPrint("CommunicationModule has been reset.");
            }
        }

        private void ResetRequest()
        {
            if (sendRequestOperation != null)
            {
                sendRequestOperation.Cancel();
                sendRequestOperation = null;
            }
            if (readAsInputStreamOperation != null)
            {
                readAsInputStreamOperation.Cancel();
                readAsInputStreamOperation = null;
            }
            if (readOperation != null)
            {
                readOperation.Cancel();
                readOperation = null;
            }
            if (inputStream != null)
            {
                inputStream.Dispose();
                inputStream = null;
            }
            if (httpRequest != null)
            {
                httpRequest.Dispose();
                httpRequest = null;
            }
        }

        private void SendHttpRequest()
        {
            // Tie the transport method to the ControlChannelTrigger object to push enable it.
            // Note that if the transport's TCP connection is broken at a later point of time,
            // the ControlChannelTrigger object can be reused to plug in a new transport by
            // calling UsingTransport again.
            Diag.DebugPrint("Calling UsingTransport() ...");
            channel.UsingTransport(httpRequest);

            // Call the SendRequestAsync function to kick start the TCP connection establishment
            // process for this HTTP request.
            Diag.DebugPrint("Calling SendRequestAsync() ...");
            sendRequestOperation = httpClient.SendRequestAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead);
            sendRequestOperation.Completed += OnSendRequestCompleted;

            // Call WaitForPushEnabled API to make sure the TCP connection has been established, 
            // which will mean that the OS will have allocated any hardware or software slot for this TCP connection.

            ControlChannelTriggerStatus status = channel.WaitForPushEnabled();
            Diag.DebugPrint("WaitForPushEnabled() completed with status: " + status);
            if (status != ControlChannelTriggerStatus.HardwareSlotAllocated
                && status != ControlChannelTriggerStatus.SoftwareSlotAllocated)
            {
                throw new Exception("Hardware/Software slot not allocated");
            }

            Diag.DebugPrint("Transport is ready to read response from server.");
        }

        private void OnSendRequestCompleted(IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> asyncInfo, AsyncStatus asyncStatus)
        {
            try
            {
                if (asyncStatus == AsyncStatus.Canceled)
                {
                    Diag.DebugPrint("HttpRequestMessage.SendRequestAsync was canceled.");
                    return;
                }

                if (asyncStatus == AsyncStatus.Error)
                {
                     
                    Diag.DebugPrint("HttpRequestMessage.SendRequestAsync failed: " + asyncInfo.ErrorCode);
                    return;
                }

                Diag.DebugPrint("HttpRequestMessage.SendRequestAsync succeeded.");

                HttpResponseMessage response = asyncInfo.GetResults();
                readAsInputStreamOperation = response.Content.ReadAsInputStreamAsync();
                readAsInputStreamOperation.Completed = OnReadAsInputStreamCompleted;
            }
            catch (Exception ex)
            {
                Diag.DebugPrint("Error in OnSendRequestCompleted: " + ex.ToString());
            }
        }

        private void OnReadAsInputStreamCompleted(IAsyncOperationWithProgress<IInputStream, ulong> asyncInfo, AsyncStatus asyncStatus)
        {
            try
            {
                if (asyncStatus == AsyncStatus.Canceled)
                {
                    Diag.DebugPrint("IHttpContent.ReadAsInputStreamAsync was canceled.");
                    return;
                }

                if (asyncStatus == AsyncStatus.Error)
                {
                    Diag.DebugPrint("IHttpContent.ReadAsInputStreamAsync failed: " + asyncInfo.ErrorCode);
                    return;
                }

                Diag.DebugPrint("IHttpContent.ReadAsInputStreamAsync succeeded.");

                inputStream = asyncInfo.GetResults();
                ReadMore();
            }
            catch (Exception ex)
            {
                Diag.DebugPrint("Error in OnReadAsInputStreamCompleted: " + ex.ToString());
            }
        }

        private void ReadMore()
        {
            IBuffer buffer = new Windows.Storage.Streams.Buffer(1024);
            readOperation = inputStream.ReadAsync(buffer, buffer.Capacity, InputStreamOptions.Partial);
            readOperation.Completed = OnReadCompleted;
        }

        private void OnReadCompleted(IAsyncOperationWithProgress<IBuffer, uint> asyncInfo, AsyncStatus asyncStatus)
        {
            try
            {
                if (asyncStatus == AsyncStatus.Canceled)
                {
                    Diag.DebugPrint("IInputStream.ReadAsync was canceled.");
                    return;
                }

                if (asyncStatus == AsyncStatus.Error)
                {
                    Diag.DebugPrint("IInputStream.ReadAsync failed: " + asyncInfo.ErrorCode);
                    return;
                }

                IBuffer buffer = asyncInfo.GetResults();

                Diag.DebugPrint("IInputStream.ReadAsync succeeded. " + buffer.Length + " bytes read.");

                if (buffer.Length == 0)
                {
                    // The response is complete
                    lock (this)
                    {
                        ResetRequest();
                    }
                    return;
                }

                byte[] rawBuffer = buffer.ToArray();
                Debug.Assert(buffer.Length <= Int32.MaxValue);

                // The test server content response is UTF-8 encoded. If another encoding is used,
                // change the encoding class in the following line.
                string responseString = Encoding.UTF8.GetString(rawBuffer, 0, (int)buffer.Length);

                // Add the message into a queue that the push notify task will pick up
                AppContext.messageQueue.Enqueue(responseString);

                ReadMore();
            }
            catch (Exception ex)
            {
                Diag.DebugPrint("Error in OnReadCompleted: " + ex.ToString());
            }
        }

        private void SetUpHttpRequestAndSendToHttpServer()
        {
            try
            {
                AppendOriginToUri();

                Diag.DebugPrint("SetUpHttpRequestAndSendToHttpServer started with URI: " + serverUri);

                // IMPORTANT:
                // For HTTP based transports that use ControlChannelTrigger, whenever we send the next request,
                // we will abort the earlier outstanding HTTP request and start a new one.
                // For example, when the HTTP server is taking longer to reply, and the keep alive trigger is fired
                // in-between then the keep alive task will abort the outstanding HTTP request and start a new request
                // which should be finished before the next keep alive task is triggered.
                ResetRequest();

                httpRequest = new HttpRequestMessage(HttpMethod.Get, serverUri);

                SendHttpRequest();
            }
            catch (Exception ex)
            {
                Diag.DebugPrint("Connect failed with: " + ex.ToString());
                throw;
            }
        }

        private void AppendOriginToUri()
        {
            // The origin allows to track where does the current request is being originated, i.e.: keep alive task,
            // network change task or click event.
            string uriString = serverUri.ToString();

            // Remove previous query string.
            if (uriString.Contains("?"))
            {
                int queryStringIndex = uriString.LastIndexOf('?');
                uriString = uriString.Substring(0, queryStringIndex);
            }

            serverUri = new Uri(uriString + "?origin=" + origin);
        }

        private bool RegisterWithCct()
        {
            // Make sure the objects are created in a system thread that is guaranteed
            // to run in an MTA. Any objects that are required for use within background
            // tasks must not be affinitized to the ASTA.
            Task<bool> registerTask = Task<bool>.Factory.StartNew(() =>
            {
                httpClient = new HttpClient();
                return RegisterWithCCTHelper();
            });
            return registerTask.Result;
        }

        private bool RegisterWithCCTHelper()
        {
            bool result = false;

            // Specify the keepalive interval expected by the server for this app
            // in order of minutes.
            const int serverKeepAliveInterval = 15;

            // Specify the channelId string to differentiate this
            // channel instance from any other channel instance.
            // When the background task runs, the channel object is provided
            // as context and the channel id can be used to adapt the behavior
            // of the app as required.
            const string channelId = "channelOne";

            // Try creating the ControlChannelTrigger if this has not been already created and stored
            // in the property bag.
            Diag.DebugPrint("RegisterWithCCTHelper Starting...");

            Diag.DebugPrint("Create ControlChannelTrigger ...");

            // Create the ControlChannelTrigger object and request a hardware slot for this app.
            try
            {
                channel = new ControlChannelTrigger(channelId, serverKeepAliveInterval, ControlChannelTriggerResourceType.RequestHardwareSlot);
            }
            catch (Exception ex)
            {
                Diag.DebugPrint("Error while creating ControlChannelTrigger: " + ex.Message + " Please add the app to the lock screen.");
                return result;
            }
            bool registerKA = true, registerPushNotify = true;
            if (kaTaskId != Guid.Empty || pushNotifyTaskId != Guid.Empty)
            {
                IReadOnlyDictionary<Guid, IBackgroundTaskRegistration> allTasks = BackgroundTaskRegistration.AllTasks;
                if (kaTaskId != Guid.Empty && allTasks.ContainsKey(kaTaskId))
                {
                    registerKA = false;
                }
                if (pushNotifyTaskId != Guid.Empty && allTasks.ContainsKey(pushNotifyTaskId))
                {
                    registerPushNotify = false;
                }
            }

            if (registerKA)
            {
                // Register the apps background task with the trigger for keepalive.
                BackgroundTaskBuilder keepAliveBuilder = new BackgroundTaskBuilder();
                keepAliveBuilder.Name = "KeepaliveTaskForChannelOne";
                keepAliveBuilder.TaskEntryPoint = "BackgroundTaskHelper.KATask";
                keepAliveBuilder.SetTrigger(channel.KeepAliveTrigger);
                BackgroundTaskRegistration KATask = keepAliveBuilder.Register();
                kaTaskId = KATask.TaskId;
            }

            if (registerPushNotify)
            {
                // Register the apps background task with the trigger for push notification.
                BackgroundTaskBuilder pushNotifyBuilder = new BackgroundTaskBuilder();
                pushNotifyBuilder.Name = "PushNotificationTaskForChannelOne";
                pushNotifyBuilder.TaskEntryPoint = "BackgroundTaskHelper.PushNotifyTask";
                pushNotifyBuilder.SetTrigger(channel.PushNotificationTrigger);
                BackgroundTaskRegistration pushNotifyTask = pushNotifyBuilder.Register();
                pushNotifyTaskId = pushNotifyTask.TaskId;
            }

            // Store the objects created in the property bag for later use.
            // NOTE: make sure these objects are free threaded. STA/Both objects can 
            // cause deadlocks when foreground threads are suspended.
            if (((IDictionary<string, object>)CoreApplication.Properties).ContainsKey(channel.ControlChannelTriggerId))
            {
                CoreApplication.Properties.Remove(channel.ControlChannelTriggerId);
            }

            AppContext appContext = new AppContext(this);
            lock (CoreApplication.Properties)
            {
                ((IDictionary<string, object>)CoreApplication.Properties).Add(channel.ControlChannelTriggerId, appContext);
            }

            try
            {
                // Send HTTP request
                SetUpHttpRequestAndSendToHttpServer();
                result = true;

                Diag.DebugPrint("RegisterWithCCTHelper Completed.");
            }
            catch (Exception ex)
            {
                Diag.DebugPrint("RegisterWithCCTHelper Task failed with: " + ex.ToString());

                // Exceptions may be thrown for example if the application has not 
                // registered the background task class id for using real time communications 
                // in the package appx manifest.
                result = false;
            }
            return result;
        }

        public bool SetUpTransport(Uri serverUriIn, string origin)
        {
            this.origin = origin;
            bool result = false;
            lock (this)
            {
                try
                {
                    // Save these to help reconnect later.
                    serverUri = serverUriIn;

                    // Set up the CCT channel with the stream socket.
                    result = RegisterWithCct();
                    if (result == false)
                    {
                        Diag.DebugPrint("Failed to sign on and connect");
                        DisposeProperties();
                    }
                }
                catch (Exception ex)
                {
                    Diag.DebugPrint("Failed to sign on and connect. Exception: " + ex.ToString());
                    DisposeProperties();
                }
            }
            return result;
        }

        public bool SendMessageToHttpServer()
        {
            try
            {
                SetUpHttpRequestAndSendToHttpServer();
                return true;
            }
            catch (Exception ex)
            {
                Diag.DebugPrint("HttpClient write failed with error:  " + ex.ToString());
                return false;
            }
        }

        public Task<bool> SendKAMessage(string origin)
        {
            this.origin = origin;

            // Here the keepalive task will abort earlier HTTP request and send a new HTTP request
            // as the same HTTP request cannot be sent twice.
            // We will try to get the URI from the earlier HTTP request, if it's not null and then use the same URI
            // to send the HTTP request from the keepalive task.
            Task<bool> registerTask = null;
            try
            {
                if (httpClient != null)
                {
                    if (httpRequest != null)
                    {
                        Diag.DebugPrint("Sending HTTP request from keepalive task with URI: " + serverUri.ToString());

                        registerTask = Task<bool>.Factory.StartNew(() =>
                        {
                            return SendMessageToHttpServer();
                        });
                        return registerTask;
                    }
                }
                else
                {
                    registerTask = null;
                    Diag.DebugPrint("HttpClient does not exist. Create another CCT enabled transport.");
                }
            }
            catch (Exception ex)
            {
                registerTask = null;
                Diag.DebugPrint("Exception occured in SendKAMessage: " + ex.ToString());
            }

            return registerTask;
        }
    }

    public class AppContext
    {
        public AppContext(CommunicationModule commInstance)
        {
            CommunicationInstance = commInstance;
            messageQueue = new ConcurrentQueue<string>();
        }

        public static ConcurrentQueue<string> messageQueue;

        public HttpClient HttpClient { get; set; }
        public ControlChannelTrigger Channel { get; set; }
        public string ChannelId { get; set; }
        public CommunicationModule CommunicationInstance { get; set; }
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
                try
                {
                    await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                        () =>
                        {
                            try
                            {
                                debugOutputTextBlock.Text = debugOutputTextBlock.Text + DateTime.Now.ToString(@"M/d/yyyy hh:mm:ss tt") + " " + msg + "\r\n";
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.ToString());
                            }

                        });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }
    }
}
