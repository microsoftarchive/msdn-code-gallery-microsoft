' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports System.Diagnostics
Imports System.Threading
Imports Windows.ApplicationModel.Background
Imports Windows.Storage
Imports Windows.Devices.Sms
Imports Windows.Data.Xml.Dom
Imports Windows.UI.Notifications
Imports System.Threading.Tasks


' The namespace for the background tasks.
' A background task always implements the IBackgroundTask interface.
Public NotInheritable Class SampleSmsBackgroundTask
    Implements IBackgroundTask

    ' The Run method is the entry point of a background task.
    Public Async Sub Run(taskInstance As IBackgroundTaskInstance) Implements IBackgroundTask.Run

        ' Associate a cancellation handler with the background task.
        AddHandler taskInstance.Canceled, AddressOf OnCanceled

        '
        ' Do the background task activity.
        '
        Dim deferral As BackgroundTaskDeferral = taskInstance.GetDeferral()
        Await DisplayToastAsync(taskInstance)

        '
        ' Provide status to application via local settings storage
        '
        Dim settings = Windows.Storage.ApplicationData.Current.LocalSettings
        settings.Values(taskInstance.Task.TaskId.ToString) = "Completed"

        Debug.WriteLine("Background " & taskInstance.Task.Name & ("process ran"))

        deferral.Complete()
    End Sub


    Private Async Function DisplayToastAsync(taskInstance As IBackgroundTaskInstance) As Task
        Dim smsDetails As SmsReceivedEventDetails = DirectCast(taskInstance.TriggerDetails, SmsReceivedEventDetails)

        Dim smsDevice__1 As SmsDevice = DirectCast(Await SmsDevice.FromIdAsync(smsDetails.DeviceId), SmsDevice)

        Dim smsEncodedmsg As SmsBinaryMessage = DirectCast(Await smsDevice__1.MessageStore.GetMessageAsync(smsDetails.MessageIndex), SmsBinaryMessage)

        Dim smsTextMessage As SmsTextMessage = Windows.Devices.Sms.SmsTextMessage.FromBinaryMessage(smsEncodedmsg)

        Dim toastXml As XmlDocument = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02)

        Dim stringElements As XmlNodeList = toastXml.GetElementsByTagName("text")

        stringElements.ElementAt(0).AppendChild(toastXml.CreateTextNode(smsTextMessage.From))

        stringElements.ElementAt(1).AppendChild(toastXml.CreateTextNode(smsTextMessage.Body))

        Dim notification As New ToastNotification(toastXml)
        ToastNotificationManager.CreateToastNotifier().Show(notification)
    End Function



    ' Handles background task cancellation.
    Private Sub OnCanceled(sender As IBackgroundTaskInstance, reason As BackgroundTaskCancellationReason)
        ' Indicate that the background task is canceled.
        Dim settings = Windows.Storage.ApplicationData.Current.LocalSettings
        settings.Values(sender.Task.TaskId.ToString) = "Canceled"

        Debug.WriteLine("Background " & sender.Task.Name & " Cancel Requested...")
    End Sub

End Class
