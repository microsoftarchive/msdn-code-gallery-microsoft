#include "pch.h"
#include "Background.h"

using namespace Background;
using namespace Platform;
using namespace StreamSocketTransportHelper;
using namespace StreamSocketTransportHelper::DiagnosticsHelper;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::Data::Xml::Dom;
using namespace Windows::Networking::Sockets;
using namespace Windows::UI::Notifications;


void NetworkChangeTask::Run(IBackgroundTaskInstance^ taskInstance)
{
    if (taskInstance == nullptr)
    {
        Diag::DebugPrint("NetworkChangeTask: taskInstance was null");
        return;
    }

    // In this example, the channel name has been hardcoded to lookup the property bag
    // for any previous contexts. The channel name may be used in more sophisticated ways
    // in case an app has multiple controlchanneltrigger objects.
    String^ channelId = "channelOne";
    if (CoreApplication::Properties->HasKey(channelId))
    {
        try
        {
            auto appContext = dynamic_cast<AppContext^>(CoreApplication::Properties->Lookup(channelId));
            if (appContext != nullptr && appContext->CommInstance != nullptr)
            {
                CommModule^ commInstance = appContext->CommInstance;
                
                // Clear any existing channels, sockets etc.
                commInstance->Reset();
                
                // Create CCT enabled transport
                commInstance->SetupTransport(commInstance->serverName, commInstance->serverPort);
            }
        }
        catch (Exception^ exp)
        {
            Diag::DebugPrint("Registering with CCT failed with: " + exp->Message);
        }
    }
    else
    {
        Diag::DebugPrint("Cannot find AppContext key channelOne");
    }

    Diag::DebugPrint("Systemtask - " + taskInstance->Task->Name + " finished.");
}

void KATask::Run(IBackgroundTaskInstance^ taskInstance)
{
    if (taskInstance == nullptr)
    {
        Diag::DebugPrint("KATask: taskInstance was null");
        return;
    }

    Diag::DebugPrint("KATask " + taskInstance->Task->Name + " Starting...");
    
    // Use the ControlChannelTriggerEventDetails object to derive the context for this background task.
    // The context happens to be the channelId that apps can use to differentiate between
    // various instances of the channel..
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
            if (appContext != nullptr && appContext->CommInstance != nullptr)
            {
                CommModule^ commModule = appContext->CommInstance;
                bool result = commModule->SendKAMessage(KeepAliveMessage);
                if (result == true)
                {
                    // Call FlushTransport on the channel object to ensure
                    // the packet is out of the process and on the wire before
                    // exiting the keepalive task.
                    commModule->channel->FlushTransport();
                }
                else
                {
                    // Socket has not been set up. Reconnect the transport and plug in to the controlchanneltrigger object.
                    commModule->Reset();
                    
                    // Create CCT enabled transport.
                    commModule->SetupTransport(commModule->serverName, commModule->serverPort);
                }
            }
        }
        catch (Exception^ exp)
        {
            Diag::DebugPrint("KA Task failed with: " + exp->Message);
        }
    }
    else
    {
        Diag::DebugPrint("Cannot find AppContext key channelOne");
    }

    Diag::DebugPrint("KATask " + taskInstance->Task->Name + " finished.");
}

void PushNotifyTask::InvokeSimpleToast(String^ msgReceived)
{
    // GetTemplateContent returns a Windows.Data.Xml.Dom.XmlDocument object containing
    // the toast XML.
    XmlDocument^ toastXml = ToastNotificationManager::GetTemplateContent(ToastTemplateType::ToastImageAndText02);

    // You can use the methods from the XML document to specify all of the
    // required parameters for the toast.
    XmlNodeList^ stringElements = toastXml->GetElementsByTagName("text");
    stringElements->Item(0)->AppendChild(toastXml->CreateTextNode("Push notification message:"));
    stringElements->Item(1)->AppendChild(toastXml->CreateTextNode(msgReceived));

    // Audio tags are not included by default, so must be added to the
    // XML document.
    String^ audioSrc = "ms-winsoundevent:Notification.IM";
    XmlElement^ audioElement = toastXml->CreateElement("audio");
    audioElement->SetAttribute("src", audioSrc);

    IXmlNode^ toastNode = toastXml->SelectSingleNode("/toast");
    toastNode->AppendChild(audioElement);            

    // Create a toast from the Xml, then create a ToastNotifier object to show the toast.
    ToastNotification^ toast = ref new ToastNotification(toastXml);
    ToastNotificationManager::CreateToastNotifier()->Show(toast);
}

void PushNotifyTask::Run(IBackgroundTaskInstance^ taskInstance)
{
    if (taskInstance == nullptr)
    {
        Diag::DebugPrint("PushNotifyTask: taskInstance was null");
        return;
    }

    Diag::DebugPrint("PushNotifyTask " + taskInstance->Task->Name + " Starting...");
    
    // Use the ControlChannelTriggerEventDetails object to derive the context for this background task.
    // The context happens to be the channelId that apps can use to differentiate between
    // various instances of the channel..
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
            String^ messageReceived = appContext->messageQueue->Dequeue();
            if (messageReceived != nullptr)
            {
                Diag::DebugPrint("Message: " + messageReceived);
                InvokeSimpleToast(messageReceived);
            }
            else
            {
                Diag::DebugPrint("There was no message for this push notification: ");
            }
        }
        catch (Exception^ exp)
        {
            Diag::DebugPrint("PushNotifyTask failed with: " + exp->Message);
        }
    }
    else
    {
        Diag::DebugPrint("Cannot find AppContext key " + channelId);
    }

    Diag::DebugPrint("PushNotifyTask " + taskInstance->Task->Name + " finished.");
}