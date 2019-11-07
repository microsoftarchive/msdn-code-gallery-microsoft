// OperatorNotificationTask.cpp
#include "pch.h"
#include "OperatorNotificationTask.h"

using namespace OperatorNotificationTask;
using namespace Platform;
using namespace Windows::Networking::NetworkOperators;
using namespace Windows::ApplicationModel::Background;
using namespace Windows::Data::Xml::Dom;
using namespace Windows::UI::Notifications;
using namespace Windows::Storage;


void OperatorNotification::Run(IBackgroundTaskInstance^ taskInstance)
{
    //
    // Get the notification event details
    //
    NetworkOperatorNotificationEventDetails^ notificationEventData = (NetworkOperatorNotificationEventDetails^) taskInstance->TriggerDetails;

    //
    // This sample only handles notification types that typically have message content
    // The message is displayed in a toast
    //
    if ((notificationEventData->NotificationType == NetworkOperatorEventMessageType::Gsm) ||
        (notificationEventData->NotificationType == NetworkOperatorEventMessageType::Cdma) ||
        (notificationEventData->NotificationType == NetworkOperatorEventMessageType::Ussd))
    {

        XmlDocument^ toastXml = ToastNotificationManager::GetTemplateContent(ToastTemplateType::ToastText02);
        XmlNodeList^ stringElements = toastXml->GetElementsByTagName("text");
        stringElements->Item(0)->AppendChild(toastXml->CreateTextNode("MobileBroadband Message: "));
        stringElements->Item(1)->AppendChild(toastXml->CreateTextNode(notificationEventData->Message));
        ToastNotification^ notification = ref new ToastNotification(toastXml);
        ToastNotificationManager::CreateToastNotifier()->Show(notification);
    }

    //
    // Provide status to application via local settings storage
    //
    auto settings = ApplicationData::Current->LocalSettings;
    auto key = taskInstance->Task->TaskId.ToString();
    settings->Values->Insert(key, "Completed");
}