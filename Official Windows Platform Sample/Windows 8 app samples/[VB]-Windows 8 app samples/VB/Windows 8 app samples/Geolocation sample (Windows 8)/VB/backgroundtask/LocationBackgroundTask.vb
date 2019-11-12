'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System.Threading
Imports Windows.ApplicationModel.Background
Imports Windows.Devices.Geolocation
Imports Windows.Storage

' A background task always implements the IBackgroundTask interface
Public NotInheritable Class LocationBackgroundTask
    Implements IBackgroundTask

    Private _cts As CancellationTokenSource = Nothing

    ' The Run method is the entry point of a background task
    Public Async Sub Run(taskInstance As IBackgroundTaskInstance) Implements IBackgroundTask.Run
        ' Get the deferral object from the task instance
        Dim deferral As BackgroundTaskDeferral = taskInstance.GetDeferral()
        Dim settings = ApplicationData.Current.LocalSettings

        ' Associate a cancellation handler with the background task
        AddHandler taskInstance.Canceled, AddressOf OnCanceled

        Try
            ' Get cancellation token
            _cts = New CancellationTokenSource()
            Dim token As CancellationToken = _cts.Token

            ' Get a geolocator object
            Dim geolocator As New Geolocator

            ' Carry out the operation
            Dim pos As Geoposition = Await geolocator.GetGeopositionAsync().AsTask(token)

            settings.Values("Status") = "Time: " + pos.Coordinate.Timestamp.ToString()
            settings.Values("Latitude") = pos.Coordinate.Latitude.ToString()
            settings.Values("Longitude") = pos.Coordinate.Longitude.ToString()
            settings.Values("Accuracy") = pos.Coordinate.Accuracy.ToString()
        Catch generatedExceptionName As UnauthorizedAccessException
            settings.Values("Status") = "Disabled"
            settings.Values("Latitude") = "No data"
            settings.Values("Longitude") = "No data"
            settings.Values("Accuracy") = "No data"
        Catch generatedExceptionName As TaskCanceledException
            ' Safely catch the cancelation
        Finally
            _cts = Nothing
            deferral.Complete()
        End Try
    End Sub

    ' Handles background task cancellation
    Private Sub OnCanceled(sender As IBackgroundTaskInstance, reason As BackgroundTaskCancellationReason)
        ' Cancel the active Geoposition request
        If _cts IsNot Nothing Then
            _cts.Cancel()
            _cts = Nothing
        End If
    End Sub
End Class
