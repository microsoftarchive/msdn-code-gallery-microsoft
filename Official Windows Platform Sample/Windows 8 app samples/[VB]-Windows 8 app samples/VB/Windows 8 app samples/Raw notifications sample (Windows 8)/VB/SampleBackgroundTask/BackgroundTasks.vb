Imports System.Diagnostics
Imports Windows.ApplicationModel.Background
Imports Windows.Networking.PushNotifications
Imports Windows.Storage

' You must use a sealed class, and make sure the output is a WINMD.
Public NotInheritable Class SampleBackgroundTask
    Implements Windows.ApplicationModel.Background.IBackgroundTask

    Public Sub Run(taskInstance As IBackgroundTaskInstance) Implements IBackgroundTask.Run
        ' Get the background task details
        Dim settings As ApplicationDataContainer = ApplicationData.Current.LocalSettings
        Dim taskName As String = taskInstance.Task.Name

        Debug.WriteLine("Background " & taskName & " starting...")

        ' Store the content received from the notification so it can be retrieved from the UI.
        Dim notification As RawNotification = DirectCast(taskInstance.TriggerDetails, RawNotification)
        settings.Values(taskName) = notification.Content

        Debug.WriteLine("Background " & taskName & " completed!")
    End Sub

End Class
