' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Diagnostics
Imports System.Threading
Imports Windows.ApplicationModel.Background
Imports Windows.Storage

'
' A background task always implements the IBackgroundTask interface.
'
Public NotInheritable Class ServicingComplete
    Implements IBackgroundTask

    Dim CancelRequested As Boolean = False

    '
    ' The Run method is the entry point of a background task.
    '
    Public Sub Run(taskInstance As IBackgroundTaskInstance) Implements IBackgroundTask.Run
        Debug.WriteLine("ServicingComplete " & taskInstance.Task.Name & " starting...")

        '
        ' Do background task activity for servicing complete.
        '
        Dim progress As UInteger
        For progress = 0 To 100 Step 10
            '
            ' If the cancellation handler indicated that the task was canceled, stop doing the task.
            '
            If CancelRequested Then
                Exit For
            End If

            '
            ' Indicate progress to foreground application.
            '
            taskInstance.Progress = progress
        Next

        Dim key = taskInstance.Task.Name
        Dim settings = ApplicationData.Current.LocalSettings

        '
        ' Write to LocalSettings to indicate that this background task ran.
        '
        settings.Values(key) = (If((progress < 100), " Canceled", " Completed"))
        Debug.WriteLine("ServicingComplete " & taskInstance.Task.Name + (If((progress < 100), " Canceled", " Completed")))
    End Sub

    '
    ' Handles background task cancellation.
    '
    Private Sub OnCanceled(sender As IBackgroundTaskInstance, reason As BackgroundTaskCancellationReason)
        '
        ' Indicate that the background task is canceled.
        '
        CancelRequested = True

        Debug.WriteLine("ServicingComplete " & sender.Task.Name & " Cancel Requested...")
    End Sub

    
End Class
