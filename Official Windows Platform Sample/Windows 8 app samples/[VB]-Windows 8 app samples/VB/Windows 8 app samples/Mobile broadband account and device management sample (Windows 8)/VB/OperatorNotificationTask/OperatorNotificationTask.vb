Imports System.Diagnostics
Imports System.Threading
Imports Windows.ApplicationModel.Background
Imports Windows.Data.Xml.Dom
Imports Windows.Networking.NetworkOperators
Imports Windows.UI.Notifications


Public NotInheritable Class OperatorNotification
    Implements IBackgroundTask

    '
    ' The Run method is the entry point of a background task.
    '
    Public Sub Run(taskInstance As IBackgroundTaskInstance) Implements IBackgroundTask.Run
        '
        ' Get the notificaiton event details  
        '
        Dim notificationEventData As NetworkOperatorNotificationEventDetails = DirectCast(taskInstance.TriggerDetails, NetworkOperatorNotificationEventDetails)

        '
        ' This sample only handles notification types that typically have message content
        ' The message is displayed in a toast
        '
        If (notificationEventData.NotificationType = NetworkOperatorEventMessageType.Gsm) OrElse (notificationEventData.NotificationType = NetworkOperatorEventMessageType.Cdma) OrElse (notificationEventData.NotificationType = NetworkOperatorEventMessageType.Ussd) Then
            Dim toastXml As XmlDocument = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02)

            Dim stringElements As XmlNodeList = toastXml.GetElementsByTagName("text")
            stringElements.ElementAt(0).AppendChild(toastXml.CreateTextNode("MobileBroadband Message:"))
            stringElements.ElementAt(1).AppendChild(toastXml.CreateTextNode(notificationEventData.Message))

            Dim notification As New ToastNotification(toastXml)
            ToastNotificationManager.CreateToastNotifier().Show(notification)
        End If

        ' Provide status to application via local settings storage
        Dim settings = Windows.Storage.ApplicationData.Current.LocalSettings
        settings.Values(taskInstance.Task.TaskId.ToString) = "Completed"

        Debug.WriteLine("Background " & taskInstance.Task.Name & " process ran")
    End Sub

    
End Class

