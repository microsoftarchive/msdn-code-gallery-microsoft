Imports System
Imports System.Diagnostics
Imports System.Threading
Imports Windows.ApplicationModel.Background
Imports Windows.Foundation
Imports Windows.Storage
Imports Windows.Networking.Connectivity
Imports System.IO

'
' The namespace for the background tasks.
' 
'
' A background task always implements the IBackgroundTask interface.
'
Public NotInheritable Class NetworkStatusBackgroundTask
    Implements IBackgroundTask

    Private localSettings As ApplicationDataContainer = ApplicationData.Current.LocalSettings

    '
    ' The Run method is the entry point of a background task.
    '
    Public Sub Run(taskInstance As IBackgroundTaskInstance) Implements IBackgroundTask.Run
        Debug.WriteLine("Background " + taskInstance.Task.Name + " Starting...")
        '
        ' Associate a cancellation handler with the background task.
        '
        AddHandler taskInstance.Canceled, AddressOf OnCanceled

        Try
            Dim profile As ConnectionProfile = NetworkInformation.GetInternetConnectionProfile()
            If profile Is Nothing Then
                localSettings.Values("InternetProfile") = "Not connected to Internet"
                localSettings.Values("NetworkAdapterId") = "Not connected to Internet"
            Else
                localSettings.Values("InternetProfile") = profile.ProfileName

                Dim networkAdapterInfo = profile.NetworkAdapter
                If networkAdapterInfo Is Nothing Then
                    localSettings.Values("NetworkAdapterId") = "Not connected to Internet"
                Else
                    localSettings.Values("NetworkAdapterId") = networkAdapterInfo.NetworkAdapterId.ToString()
                End If
            End If
        Catch e As Exception
            Debug.WriteLine("Unhandled exception: " + e.ToString())
        End Try
    End Sub

    '
    ' Handles background task cancellation.
    '
    Private Sub OnCanceled(sender As IBackgroundTaskInstance, reason As BackgroundTaskCancellationReason)
        ' Indicate that the background task is canceled.
        Debug.WriteLine("Background " + sender.Task.Name + " Cancel Requested...")
    End Sub

    
End Class



