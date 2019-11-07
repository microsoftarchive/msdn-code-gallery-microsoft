using DiagnosticsHelper;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
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
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace HttpTransportHelper
{
    public class CommModule : IDisposable
    {
        const int TIMEOUT = 30000;

        HttpClient httpClient;
        HttpRequestMessage httpRequest;
        public ControlChannelTrigger channel;
        public Uri serverUri;
        private static Guid kaTaskId = Guid.Empty;
        private static Guid pushNotifyTaskId = Guid.Empty;

        public CommModule()
        {
        }

        private void DisposeProperties()
        {
            Diag.DebugPrint("Entering cleanup");
            if (httpRequest != null)
            {
                httpRequest.Dispose();
                httpRequest = null;
            }
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
        }

        public void Reset()
        {
            lock (this)
            {
                Diag.DebugPrint("CommModule is being reset.");

                if (channel != null)
                {
                    if (((IDictionary<string, object>)CoreApplication.Properties).ContainsKey(channel.ControlChannelTriggerId))
                    {
                        CoreApplication.Properties.Remove(channel.ControlChannelTriggerId);
                    }
                }

                DisposeProperties();

                Diag.DebugPrint("CommModule has been reset.");
            }
        }

        private void SendHttpRequest()
        {

            if (httpRequest == null)
            {
                throw new Exception("HttpRequest object is null");
            }

            // Tie the transport method to the controlchanneltrigger object to push enable it.
            // Note that if the transport's TCP connection is broken at a later point of time,
            // the controlchanneltrigger object can be reused to plugin a new transport by
            // calling UsingTransport API again.
            Diag.DebugPrint("Calling UsingTransport() ...");
            channel.UsingTransport(httpRequest);

            // Call the SendAsync function to kick start the TCP connection establishment
            // process for this http request.
            Task<HttpResponseMessage> httpResponseTask = httpClient.SendAsync(httpRequest);

            // Call WaitForPushEnabled API to make sure the TCP connection has been established, 
            // which will mean that the OS will have allocated any hardware slot for this TCP connection.
            ControlChannelTriggerStatus status = channel.WaitForPushEnabled();
            Diag.DebugPrint("WaitForPushEnabled() completed with status: " + status);
            if (status != ControlChannelTriggerStatus.HardwareSlotAllocated
                && status != ControlChannelTriggerStatus.SoftwareSlotAllocated)
            {
                throw new Exception("Hardware/Software slot not allocated");
            }

            Diag.DebugPrint("In SetupHttpRequestAndSendToHttpServer httpResponse is prepared to read response from server.");

            // IMPORTANT: This sample is noticably different from other transports based on ControlChannelTrigger
            // The HttpClient receive callback is delivered via a Task to the app since the HttpClient code is purely
            // managed code based. This means push notification task will fire as soon as the data or error is dispatched
            // to the application. Hence, in this sample we Enqueue the responseTask returned by httpClient.sendAsync 
            // into a queue that the push notify task will pick up and process inline.
            AppContext.messageQueue.Enqueue(httpResponseTask);
        }

        public string ReadResponse(Task<HttpResponseMessage> httpResponseTask)
        {
            string message = null;
            try
            {
                if (httpResponseTask.IsCanceled || httpResponseTask.IsFaulted)
                {
                    Diag.DebugPrint("Task is cancelled or has failed");
                    return message;
                }
                // We'll wait until we got the whole response. This is the only supported
                // scenario for HttpClient for ControlChannelTrigger.
                HttpResponseMessage httpResponse = httpResponseTask.Result;
                Diag.DebugPrint("Reading from httpresponse");
                if (httpResponse == null || httpResponse.Content == null)
                {
                    Diag.DebugPrint("Cant read from httpresponse, as either httpResponse or its content is null. try to reset connection.");
                }
                else
                {
                    // This is likely being processed in the context of a background task and so
                    // synchronously read the Content's results inline so that the Toast can be shown.
                    // before we exit the Run method.
                    message = httpResponse.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception exp)
            {
                Diag.DebugPrint("Failed to read from httpresponse with error:  " + exp.ToString());
            }
            return message;
        }

        private void SetupHttpRequestAndSendToHttpServer()
        {
            try
            {
                Diag.DebugPrint("SetupHttpRequestAndSendToHttpServer started with uri: " + serverUri);

                // IMPORTANT:
                // For HTTP based transports that use the RTC broker, whenever we send next request, we will abort the earlier 
                // outstanding http request and start new one.
                // For example in case when http server is taking longer to reply, and keep alive trigger is fired in-between 
                // then keep alive task will abort outstanding http request and start a new request which should be finished 
                // before next keep alive task is triggered.
                if (httpRequest != null)
                {
                    httpRequest.Dispose();
                }

                httpRequest = RtcRequestFactory.Create(HttpMethod.Get, serverUri);

                SendHttpRequest();
            }
            catch (Exception e)
            {
                Diag.DebugPrint("Connect failed with: " + e.ToString());
                throw;
            }
        }

        private bool RegisterWithCCT()
        {

            // Make sure the objects are created in a system thread that are guaranteed
            // to run in an MTA. Any objects that are required for use within background
            // tasks must not be affinitized to the ASTA.
            Task<bool> registerTask = Task<bool>.Factory.StartNew(() =>
            {
            
                // Add code to handle traversing authenticating proxies which might be on the network.
                // This is typical in many business environments. The code below allows for the current
                // logged on user's Windows network credentials to be passed automatically to the proxy
                // for all web requests using HttpClient. Doing this allows for a better user experience.
                // Without this, additional code would be needed to handle the 407 HTTP response errors.
                //
                // If there is a proxy on the network that doesn't require authentication or there are no
                // proxies on the network, then this code results in no effect.
                WebRequest.DefaultWebProxy.Credentials = CredentialCache.DefaultCredentials;

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
            // When background task fires, the channel object is provided
            // as context and the channel id can be used to adapt the behavior
            // of the app as required.
            const string channelId = "channelOne";

            // Try creating the controlchanneltrigger if this has not been already created and stored
            // in the property bag.
            Diag.DebugPrint("RegisterWithCCTHelper Starting...");

            Diag.DebugPrint("Create ControlChannelTrigger ...");

            // Create the controlchanneltrigger object and request a hardware slot for this app.
            try
            {
                channel = new ControlChannelTrigger(channelId, serverKeepAliveInterval, ControlChannelTriggerResourceType.RequestHardwareSlot);
            }
            catch (Exception exp)
            {
                Diag.DebugPrint("Error while creating controlchanneltrigger" + exp.Message + " Please add the app to the lockscreen.");
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
                keepAliveBuilder.TaskEntryPoint = "Background.KATask";
                keepAliveBuilder.SetTrigger(channel.KeepAliveTrigger);
                BackgroundTaskRegistration KATask = keepAliveBuilder.Register();
                kaTaskId = KATask.TaskId;
            }

            if (registerPushNotify)
            {
                // Register the apps background task with the trigger for push notification task.
                BackgroundTaskBuilder pushNotifyBuilder = new BackgroundTaskBuilder();
                pushNotifyBuilder.Name = "PushNotificationTaskForChannelOne";
                pushNotifyBuilder.TaskEntryPoint = "Background.PushNotifyTask";
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

            AppContext appContext = new AppContext(this, httpClient, channel, channel.ControlChannelTriggerId);
            lock (CoreApplication.Properties)
            {
                ((IDictionary<string, object>)CoreApplication.Properties).Add(channel.ControlChannelTriggerId, appContext);
            }

            try
            {
                // Send Http Request
                SetupHttpRequestAndSendToHttpServer();

                Diag.DebugPrint("Connected!");

                result = true;

                Diag.DebugPrint("RegisterWithCCTHelper Completed.");
            }
            catch (Exception exp)
            {
                Diag.DebugPrint("RegisterWithCCTHelper Task failed with: " + exp.ToString());

                // Exceptions may be thrown for example if the application has not 
                // registered the background task class id for using real time communications 
                //  in the package appx manifest.
                result = false;
            }
            return result;
        }

        public bool SetupTransport(Uri serverUriIn)
        {
            bool result = false;
            lock (this)
            {
                try
                {
                    // Save these to help reconnect later.
                    serverUri = serverUriIn;

                    // Set up the CCT channel with the stream socket.
                    result = RegisterWithCCT();
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
                SetupHttpRequestAndSendToHttpServer();
                return true;
            }
            catch (Exception ex)
            {
                Diag.DebugPrint("httpClient write failed with error:  " + ex.ToString());
                return false;
            }
        }

        public Task<bool> SendKAMessage()
        {
            // Here KA task will abort earlier httprequest and send a new httprequest
            // As same httprequest cannot be sent twice.
            // We will try to get uri from earlier httprequest, if it’s not null. and then use same uri to send httprequest from KA task.
            Task<bool> registerTask = null;
            try
            {
                if (httpClient != null)
                {
                    if (httpRequest != null)
                    {
                        Diag.DebugPrint("Sending http request from ka task with uri: " + serverUri.ToString());

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
                    Diag.DebugPrint("httpClient does not exist. Create another CCT enabled transport.");
                }
            }
            catch (Exception ex)
            {
                registerTask = null;
                Diag.DebugPrint("Exception occured in SendKAMessage. exception: " + ex.ToString());
            }

            return registerTask;
        }

        public bool SendHttpQuery()
        {
            bool result = true;
            lock (this)
            {
                if (httpClient != null)
                {
                    Diag.DebugPrint("Sending HttpRequest to server");
                    result = SendMessageToHttpServer();
                }
                else
                {
                    result = false;
                    Diag.DebugPrint("httpClient does not exist. Create another CCT enabled transport.");
                }
            }
            return result;
        }
    }

    public class AppContext
    {
        public AppContext(CommModule commInstance, HttpClient httpClient, ControlChannelTrigger channel, string id)
        {
            HttpClient = httpClient;
            Channel = channel;
            ChannelId = id;
            CommInstance = commInstance;
            messageQueue = new ConcurrentQueue<Task<HttpResponseMessage>>();
        }

        public static ConcurrentQueue<Task<HttpResponseMessage>> messageQueue;

        public HttpClient HttpClient { get; set; }
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



