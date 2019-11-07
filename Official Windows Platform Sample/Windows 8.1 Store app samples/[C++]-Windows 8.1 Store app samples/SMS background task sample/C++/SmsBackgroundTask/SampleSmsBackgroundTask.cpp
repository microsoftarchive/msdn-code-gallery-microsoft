// SampleSmsBackgroundTask.cpp
#include "pch.h"
#include "SampleSmsBackgroundTask.h"
#include <ppltasks.h>
#include <ppl.h>

#include <wrl\client.h>
#include <wrl\event.h>
#include <wrl\internal.h>
#include <wrl\implements.h>


using namespace SmsBackgroundTask;
using namespace Platform;
using namespace Windows::Devices::Sms;
using namespace Windows::ApplicationModel::Background;
using namespace concurrency;
using namespace Windows::UI::Notifications;
using namespace Windows::Data::Xml::Dom;
using namespace Windows::Storage;


void SampleSmsBackgroundTask::Run(IBackgroundTaskInstance^ taskInstance)
{
    // Associate a cancellation handler with the background task.
    taskInstance->Canceled += ref new BackgroundTaskCanceledEventHandler(this, &SmsBackgroundTask::SampleSmsBackgroundTask::OnCanceled);
    BackgroundTaskDeferral^ deferral = taskInstance->GetDeferral();

    // Do the background task activity.
    DisplayToast(taskInstance);

    // Provide status to application via local settings storage
    auto settings = ApplicationData::Current->LocalSettings;
    auto key = taskInstance->Task->TaskId.ToString();
    settings->Values->Insert(key, "Completed");
    deferral->Complete();
}


void SampleSmsBackgroundTask::DisplayToast(IBackgroundTaskInstance^ taskInstance)
{
    SmsReceivedEventDetails^ smsDetails = (SmsReceivedEventDetails^) taskInstance->TriggerDetails;

    // Catch exception thrown when accessing the sms device and reading messages
    try
    {
        auto smstextmessage = SmsTextMessage::FromBinaryMessage(smsDetails->BinaryMessage);

        XmlDocument^ toastXml = ToastNotificationManager::GetTemplateContent(ToastTemplateType::ToastText02);

        XmlNodeList^ stringElements = toastXml->GetElementsByTagName("text");

        stringElements->Item(0)->AppendChild(toastXml->CreateTextNode(smstextmessage->From));

        stringElements->Item(1)->AppendChild(toastXml->CreateTextNode(smstextmessage->Body));

        ToastNotification^ notification = ref new ToastNotification(toastXml);
        ToastNotificationManager::CreateToastNotifier()->Show(notification);
    }
    catch (Platform::Exception^ ex)
    {
        // Indicate that the background task is canceled.
        auto settings = ApplicationData::Current->LocalSettings;
        auto key = taskInstance->Task->TaskId.ToString();
        settings->Values->Insert(key, "Error");
    }
}


void SampleSmsBackgroundTask::OnCanceled(IBackgroundTaskInstance^ sender, BackgroundTaskCancellationReason reason)
{
    // Indicate that the background task is canceled.
    auto settings = ApplicationData::Current->LocalSettings;
    auto key = sender->Task->TaskId.ToString();
    settings->Values->Insert(key, "Canceled");
}