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
Imports Windows.Foundation
Imports Windows.Storage
Imports Windows.System.Threading

'
' A background task always implements the IBackgroundTask interface.
'
Public NotInheritable Class SampleBackgroundTask
    Implements IBackgroundTask

    Private CancelRequested As Boolean = False
    Private Deferral As BackgroundTaskDeferral = Nothing
    Private PeriodicTimer As ThreadPoolTimer = Nothing
    Private Progress As UInteger = 0
    Private Instance As IBackgroundTaskInstance = Nothing

    '
    ' The Run method is the entry point of a background task.
    '
    Public Sub Run(taskInstance As IBackgroundTaskInstance) Implements IBackgroundTask.Run
        Debug.WriteLine("Background " & taskInstance.Task.Name & " Starting...")

        '
        ' Associate a cancellation handler with the background task.
        '
        AddHandler taskInstance.Canceled, AddressOf OnCanceled

        '
        ' Get the deferral object from the task instance, and take a reference to the taskInstance;
        '
        Deferral = taskInstance.GetDeferral()
        Instance = taskInstance

        PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(AddressOf PeriodicTimerCallback, TimeSpan.FromMilliseconds(500))
    End Sub

    '
    ' Handles background task cancellation.
    '
    Private Sub OnCanceled(sender As IBackgroundTaskInstance, reason As BackgroundTaskCancellationReason)
        '
        ' Indicate that the background task is canceled.
        '
        CancelRequested = True

        Debug.WriteLine("Background " & sender.Task.Name & " Cancel Requested...")
    End Sub

    '
    ' Simulate the background task activity.
    '
    Private Sub PeriodicTimerCallback(timer As ThreadPoolTimer)
        If (CancelRequested = False) AndAlso (Progress < 100) Then
            Progress += 10
            Instance.Progress = Progress
        Else
            PeriodicTimer.Cancel()

            Dim settings = ApplicationData.Current.LocalSettings
            Dim key = Instance.Task.Name

            '
            ' Write to LocalSettings to indicate that this background task ran.
            '
            settings.Values(key) = (If((Progress < 100), " Canceled", " Completed"))
            Debug.WriteLine("Background " & Instance.Task.Name & (If((Progress < 100), " Canceled", " Completed")))

            '
            ' Indicate that the background task has completed.
            '
            Deferral.Complete()
        End If
    End Sub
End Class
