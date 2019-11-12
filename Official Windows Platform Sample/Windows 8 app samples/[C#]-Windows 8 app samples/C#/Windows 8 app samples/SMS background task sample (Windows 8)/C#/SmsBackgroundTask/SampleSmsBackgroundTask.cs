// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Diagnostics;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.Devices.Sms;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using System.Threading.Tasks;

//
// The namespace for the background tasks.
//
namespace SmsBackgroundTask
{
    //
    // A background task always implements the IBackgroundTask interface.
    //
    public sealed class SampleSmsBackgroundTask : IBackgroundTask
    {
        //
        // The Run method is the entry point of a background task.
        //
        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            //
            // Associate a cancellation handler with the background task.
            //
            taskInstance.Canceled += new BackgroundTaskCanceledEventHandler(OnCanceled);

            //
            // Do the background task activity.
            //
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            await DisplayToastAsync(taskInstance);

            //
            // Provide status to application via local settings storage
            //
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values[taskInstance.Task.TaskId.ToString()] = "Completed";

            Debug.WriteLine("Background " + taskInstance.Task.Name + ("process ran"));

            deferral.Complete();
        }

        async Task DisplayToastAsync(IBackgroundTaskInstance taskInstance)
        {
            SmsReceivedEventDetails smsDetails = (SmsReceivedEventDetails)taskInstance.TriggerDetails;

            SmsDevice smsDevice = (SmsDevice)await SmsDevice.FromIdAsync(smsDetails.DeviceId);

            SmsBinaryMessage smsEncodedmsg = (SmsBinaryMessage)await smsDevice.MessageStore.GetMessageAsync(smsDetails.MessageIndex);

            SmsTextMessage smsTextMessage = Windows.Devices.Sms.SmsTextMessage.FromBinaryMessage(smsEncodedmsg);

            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);

            XmlNodeList stringElements = toastXml.GetElementsByTagName("text");

            stringElements.Item(0).AppendChild(toastXml.CreateTextNode(smsTextMessage.From));

            stringElements.Item(1).AppendChild(toastXml.CreateTextNode(smsTextMessage.Body));

            ToastNotification notification = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(notification);
        }


        //
        // Handles background task cancellation.
        //
        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            //
            // Indicate that the background task is canceled.
            //
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            settings.Values[sender.Task.TaskId.ToString()] = "Canceled";

            Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...");
        }
    }
}
