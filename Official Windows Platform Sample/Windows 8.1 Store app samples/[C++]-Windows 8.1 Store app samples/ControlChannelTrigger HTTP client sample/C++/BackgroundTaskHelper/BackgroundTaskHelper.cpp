// Copyright (c) Microsoft Corporation. All rights reserved.

#include "pch.h"
#include "BackgroundTaskHelper.h"

using namespace BackgroundTaskHelper;
using namespace HttpClientTransportHelper;
using namespace HttpClientTransportHelper::DiagnosticsHelper;
using namespace Platform;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::Data::Xml::Dom;
using namespace Windows::Networking::Connectivity;
using namespace Windows::Networking::Sockets;
using namespace Windows::UI::Notifications;

void NetworkChangeTask::Run(_In_ IBackgroundTaskInstance^ taskInstance)
{
    NetworkStateChangeEventDetails^ details = static_cast<NetworkStateChangeEventDetails^>(taskInstance->TriggerDetails);

    // Only restart CCT connection if network connectivity level changes.
    if (details->HasNewNetworkConnectivityLevel == false)
    {
        return;
    }

    Diag::DebugPrint("System task - " + taskInstance->Task->Name + " starting ...");
    
    // In this example, the channel name has been hardcoded to look up the property bag
    // for any previous contexts. The channel name may be used in more sophisticated ways
    // in case an app has multiple ControlChannelTrigger objects.
    String^ channelId = "channelOne";
    if (CoreApplication::Properties->HasKey(channelId))
    {
        try
        {
            auto appContext = dynamic_cast<AppContext^>(CoreApplication::Properties->Lookup(channelId));
            if (appContext != nullptr && appContext->CommunicationInstance != nullptr)
            {
                CommunicationModule^ communicationInstance = appContext->CommunicationInstance;
                
                // Clear any existing channels, sockets etc.
                communicationInstance->Reset();
                
                // Create CCT enabled transport
                communicationInstance->SetUpTransport("NetworkChangeTask");
            }
        }
        catch (Exception^ ex)
        {
            Diag::DebugPrint("Registering with CCT failed with: " + ex->Message);
        }
    }
    else
    {
        Diag::DebugPrint("Cannot find AppContext key channelOne");
    }

    Diag::DebugPrint("System task - " + taskInstance->Task->Name + " finished.");
}

void KATask::Run(_In_ IBackgroundTaskInstance^ taskInstance)
{
    if (taskInstance == nullptr)
    {
        Diag::DebugPrint("KATask: taskInstance was null");
        return;
    }

    Diag::DebugPrint("KATask " + taskInstance->Task->Name + " Starting...");
    
    // Use the ControlChannelTriggerEventDetails object to derive the context for this background task.
    // The context happens to be the channelId that apps can use to differentiate between
    // various instances of the channel.
    auto channelEventArgs = 
        dynamic_cast<IControlChannelTriggerEventDetails^>(taskInstance->TriggerDetails);

    ControlChannelTrigger^ channel = channelEventArgs->ControlChannelTrigger;
    if (channel == nullptr)
    {
        Diag::DebugPrint("Channel object may have been deleted.");
        return;
    }

    String^ channelId = channel->ControlChannelTriggerId;

    if (CoreApplication::Properties->HasKey(channelId))
    {
        try
        {
            auto appContext = dynamic_cast<AppContext^>(CoreApplication::Properties->Lookup(channelId));
            String^ KeepAliveMessage = "KeepAlive Message";
            if (appContext != nullptr && appContext->CommunicationInstance != nullptr)
            {
                CommunicationModule^ communicationModule = appContext->CommunicationInstance;
                bool result = communicationModule->SendKAMessage(KeepAliveMessage);
                if (result == true)
                {
                    // Ensure the packet is out of the process and on the wire before
                    // exiting the keepalive task.
                    communicationModule->Channel->FlushTransport();
                }
                else
                {
                    // Socket has not been set up. Reconnect the transport and plug in to the ControlChannelTrigger object.
                    communicationModule->Reset();
                    
                    // Create RTC enabled transport
                    communicationModule->SetUpTransport("KATask");
                }
            }
        }
        catch (Exception^ ex)
        {
            Diag::DebugPrint("KA Task failed with: " + ex->Message);
        }
    }
    else
    {
        Diag::DebugPrint("Cannot find AppContext key channelOne");
    }

    Diag::DebugPrint("KATask " + taskInstance->Task->Name + " finished.");
}

void PushNotifyTask::InvokeSimpleToast(_In_ String^ messageReceived)
{
    // GetTemplateContent returns a Windows.Data.Xml.Dom.XmlDocument object containing
    // the toast XML
    XmlDocument^ toastXml = ToastNotificationManager::GetTemplateContent(ToastTemplateType::ToastImageAndText02);

    // You can use the methods from the XML document to specify all of the
    // required parameters for the toast
    XmlNodeList^ stringElements = toastXml->GetElementsByTagName("text");
    stringElements->Item(0)->AppendChild(toastXml->CreateTextNode("Push notification message:"));
    stringElements->Item(1)->AppendChild(toastXml->CreateTextNode(messageReceived));

    // Audio tags are not included by default, so must be added to the XML document. For more information,
    // see http://go.microsoft.com/fwlink/?LinkId=306749
    String^ audioSrc = "ms-winsoundevent:Notification.IM";
    XmlElement^ audioElement = toastXml->CreateElement("audio");
    audioElement->SetAttribute("src", audioSrc);

    IXmlNode^ toastNode = toastXml->SelectSingleNode("/toast");
    toastNode->AppendChild(audioElement);

    // Create a toast from the XML, then create a ToastNotifier object to show the toast.
    ToastNotification^ toast = ref new ToastNotification(toastXml);
    ToastNotificationManager::CreateToastNotifier()->Show(toast);
}

void PushNotifyTask::Run(_In_ IBackgroundTaskInstance^ taskInstance)
{
    if (taskInstance == nullptr)
    {
        Diag::DebugPrint("PushNotifyTask: taskInstance was null");
        return;
    }

    Diag::DebugPrint("PushNotifyTask " + taskInstance->Task->Name + " Starting...");
    
    // Use the ControlChannelTriggerEventDetails object to derive the context for this background task.
    // The context happens to be the channelId that apps can use to differentiate between
    // various instances of the channel.
    auto channelEventArgs = dynamic_cast<IControlChannelTriggerEventDetails^>(taskInstance->TriggerDetails);

    ControlChannelTrigger^ channel = channelEventArgs->ControlChannelTrigger;
    if (channel == nullptr)
    {
        Diag::DebugPrint("Channel object may have been deleted.");
        return;
    }

    String^ channelId = channel->ControlChannelTriggerId;

    if (CoreApplication::Properties->HasKey(channelId))
    {
        try
        {
            auto appContext = dynamic_cast<AppContext^>(CoreApplication::Properties->Lookup(channelId));
            
            // Process any messages that have been enqueued by the receive completion handler.
            String^ messageReceived = appContext->MessageQueue->Dequeue();
            while (messageReceived != nullptr)
            {
                Diag::DebugPrint("Message: " + messageReceived);
                InvokeSimpleToast(messageReceived);

                messageReceived = appContext->MessageQueue->Dequeue();
            }
        }
        catch (Exception^ ex)
        {
            Diag::DebugPrint("PushNotifyTask failed with: " + ex->Message);
        }
    }
    else
    {
        Diag::DebugPrint("Cannot find AppContext key " + channelId);
    }

    Diag::DebugPrint("PushNotifyTask " + taskInstance->Task->Name + " finished.");
}
