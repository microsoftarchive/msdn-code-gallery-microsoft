// Copyright (c) Microsoft Corporation. All rights reserved.

using DiagnosticsHelper;
using HttpClientTransportHelper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Data.Xml.Dom;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.UI.Notifications;

namespace BackgroundTaskHelper
{
    // This class illustrates one way to set up a CCT enabled transport when 
    // a system event (such as network state change) occurs.
    public sealed class NetworkChangeTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            if (taskInstance == null)
            {
                Diag.DebugPrint("NetworkChangeTask: taskInstance was null");
                return;
            }

            NetworkStateChangeEventDetails details = taskInstance.TriggerDetails as NetworkStateChangeEventDetails;

            // Only restart CCT connection if network connectivity level changes.
            if (details.HasNewNetworkConnectivityLevel == false)
            {
                return;
            }

            Diag.DebugPrint("System task - " + taskInstance.Task.Name + " starting ...");

            // In this example, the channel name has been hardcoded to lookup the property bag
            // for any previous contexts. The channel name may be used in more sophisticated ways
            // in case an app has multiple ControlChannelTrigger objects.
            string channelId = "channelOne";
            if (((IDictionary<string, object>)CoreApplication.Properties).ContainsKey(channelId))
            {
                try
                {
                    AppContext appContext = null;
                    lock (CoreApplication.Properties)
                    {
                        appContext = ((IDictionary<string, object>)CoreApplication.Properties)[channelId] as AppContext;
                    }
                    if (appContext != null && appContext.CommunicationInstance != null)
                    {
                        CommunicationModule communicationInstance = appContext.CommunicationInstance;

                        // Clear any existing channels, sockets etc.
                        communicationInstance.Reset();

                        // Create CCT enabled transport.
                        communicationInstance.SetUpTransport(communicationInstance.serverUri, GetType().Name);
                    }
                }
                catch (Exception ex)
                {
                    Diag.DebugPrint("Registering with CCT failed with: " + ex.ToString());
                }
            }
            else
            {
                Diag.DebugPrint("Cannot find AppContext key channelOne");
            }

            Diag.DebugPrint("System task - " + taskInstance.Task.Name + " finished.");
        }
    }

    public sealed class KATask : IBackgroundTask
    {

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            if (taskInstance == null)
            {
                Diag.DebugPrint("KATask: taskInstance was null");
                return;
            }

            Diag.DebugPrint("KATask " + taskInstance.Task.Name + " Starting...");

            // Use the ControlChannelTriggerEventDetails object to derive the context for this background task.
            // The context happens to be the channelId that apps can use to differentiate between
            // various instances of the channel.
            var channelEventArgs = (IControlChannelTriggerEventDetails)taskInstance.TriggerDetails;

            ControlChannelTrigger channel = channelEventArgs.ControlChannelTrigger;
            if (channel == null)
            {
                Diag.DebugPrint("Channel object may have been deleted.");
                return;
            }

            string channelId = channel.ControlChannelTriggerId;

            if (((IDictionary<string, object>)CoreApplication.Properties).ContainsKey(channelId))
            {
                try
                {
                    AppContext appContext = null;
                    lock (CoreApplication.Properties)
                    {
                        appContext = ((IDictionary<string, object>)CoreApplication.Properties)[channelId] as AppContext;
                    }
                    if (appContext != null && appContext.CommunicationInstance != null)
                    {
                        CommunicationModule communicationModule = appContext.CommunicationInstance;
                        Task<bool> result;
                        lock (communicationModule)
                        {
                            result = communicationModule.SendKAMessage(GetType().Name);
                        }
                        if (result != null && result.Result == true)
                        {
                            // Call FlushTransport on the channel object to ensure
                            // the packet is out of the process and on the wire before
                            // exiting the keepalive task.
                            communicationModule.channel.FlushTransport();
                        }
                        else
                        {
                            // Socket has not been set up. reconnect the transport and plug in to the ControlChannelTrigger object.
                            communicationModule.Reset();

                            // Create CCT enabled transport.
                            communicationModule.SetUpTransport(communicationModule.serverUri, GetType().Name);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Diag.DebugPrint("KA Task failed with: " + ex.ToString());
                }
            }
            else
            {
                Diag.DebugPrint("Cannot find AppContext key channelOne");
            }

            Diag.DebugPrint("KATask " + taskInstance.Task.Name + " finished.");
        }
    }

    public sealed class PushNotifyTask : IBackgroundTask
    {
        void InvokeSimpleToast(string messageReceived)
        {
            // GetTemplateContent returns a Windows.Data.Xml.Dom.XmlDocument object containing
            // the toast XML
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText02);

            // You can use the methods from the XML document to specify all of the
            // required parameters for the toast
            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");
            stringElements.Item(0).AppendChild(toastXml.CreateTextNode("Push notification message:"));
            stringElements.Item(1).AppendChild(toastXml.CreateTextNode(messageReceived));

            // Audio tags are not included by default, so must be added to the XML document. For more information,
            // see http://go.microsoft.com/fwlink/?LinkId=306749
            string audioSrc = "ms-winsoundevent:Notification.IM";
            XmlElement audioElement = toastXml.CreateElement("audio");
            audioElement.SetAttribute("src", audioSrc);

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            toastNode.AppendChild(audioElement);

            // Create a toast from the Xml, then create a ToastNotifier object to show
            // the toast
            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public void Run(Windows.ApplicationModel.Background.IBackgroundTaskInstance taskInstance)
        {
            if (taskInstance == null)
            {
                Diag.DebugPrint("PushNotifyTask: taskInstance was null");
                return;
            }

            Diag.DebugPrint("PushNotifyTask " + taskInstance.Task.Name + " Starting...");

            // Use the ControlChannelTriggerEventDetails object to derive the context for this background task.
            // The context happens to be the channelId that apps can use to differentiate between
            // various instances of the channel.
            var channelEventArgs = taskInstance.TriggerDetails as IControlChannelTriggerEventDetails;

            ControlChannelTrigger channel = channelEventArgs.ControlChannelTrigger;
            if (channel == null)
            {
                Diag.DebugPrint("Channel object may have been deleted.");
                return;
            }

            string channelId = channel.ControlChannelTriggerId;

            if (((IDictionary<string, object>)CoreApplication.Properties).ContainsKey(channelId))
            {
                try
                {
                    string messageReceived = "PushNotification Received";
                    AppContext appContext = null;
                    lock (CoreApplication.Properties)
                    {
                        appContext = ((IDictionary<string, object>)CoreApplication.Properties)[channelId] as AppContext;
                    }
                    
                    // Display the response received from the server

                    while (AppContext.messageQueue.TryDequeue(out messageReceived) == true)
                    {
                        Diag.DebugPrint("Push notification message: " + messageReceived);
                        InvokeSimpleToast(messageReceived);
                    }
                }
                catch (Exception ex)
                {
                    Diag.DebugPrint("PushNotifyTask failed with: " + ex.Message);
                }
            }
            else
            {
                Diag.DebugPrint("Cannot find AppContext key " + channelId);
            }

            Diag.DebugPrint("PushNotifyTask " + taskInstance.Task.Name + " finished.");
        }
    }
}
