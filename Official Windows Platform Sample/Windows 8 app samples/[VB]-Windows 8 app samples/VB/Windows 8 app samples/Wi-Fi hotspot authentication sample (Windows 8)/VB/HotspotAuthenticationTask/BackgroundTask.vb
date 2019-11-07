'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports System.Diagnostics
Imports System.Threading
Imports Windows.ApplicationModel.Background
Imports Windows.Storage
Imports Windows.Networking.NetworkOperators

' The namespace for the background tasks.
' A background task always implements the IBackgroundTask interface.
Public NotInheritable Class AuthenticationTask
    Implements IBackgroundTask

    Private Const _foregroundAppId As String = "HotspotAuthenticationApp.App"

    Private _cancelRequested As Boolean = False

    ' The Run method is the entry point of a background task.
    Public Sub Run(taskInstance As IBackgroundTaskInstance) Implements IBackgroundTask.Run
        Debug.WriteLine("Background " & taskInstance.Task.Name & " starting...")

        ' Associate a cancelation handler with the background task for handlers
        ' that may take a considerable time to complete.
        AddHandler taskInstance.Canceled, AddressOf OnCanceled

        ' Do the background task activity. First, get the authentication context.
        Debug.WriteLine("Getting event details")
        Dim details = TryCast(taskInstance.TriggerDetails, HotspotAuthenticationEventDetails)

        Dim context As HotspotAuthenticationContext = Nothing
        If Not HotspotAuthenticationContext.TryGetAuthenticationContext(details.EventToken, context) Then
            ' The event is not targetting this application. There is no further processing to do.
            Debug.WriteLine("Failed to get event context")
            Return
        End If

        ' If the event targets this application, the event handler must ensure that it always
        ' handles the event even in case of an internal error.
        ' A try-catch block can be used to handle unexpected errors.

        ' Default value in case the configuration cannot be loaded.
        Dim markAsManualConnect As Boolean = False
        Dim handleUnexpectedError As Boolean = False
        Try
            Dim ssid As Byte() = context.WirelessNetworkId
            Debug.WriteLine("SSID: " & System.Text.UTF8Encoding.UTF8.GetString(ssid, 0, ssid.Length))
            Debug.WriteLine("AuthenticationUrl: " & context.AuthenticationUrl.OriginalString)
            Debug.WriteLine("RedirectMessageUrl: " & context.RedirectMessageUrl.OriginalString)
            Debug.WriteLine("RedirectMessageXml: " & context.RedirectMessageXml.GetXml())

            ' Get configuration from application storage.
            markAsManualConnect = ConfigStore.MarkAsManualConnect

            ' In this sample, the AuthenticationUrl is always checked in the background task handler
            ' to avoid launching the foreground app in case the authentication host is not trusted.
            If ConfigStore.AuthenticationHost <> context.AuthenticationUrl.Host Then
                ' Hotspot is not using the trusted authentication server.
                ' Abort authentication and disconnect.
                Debug.WriteLine("Authentication server is untrusted")
                context.AbortAuthentication(markAsManualConnect)
                Return
            End If

            ' Check if authentication is handled by foreground app.
            If Not ConfigStore.AuthenticateThroughBackgroundTask Then
                Debug.WriteLine("Triggering foreground application")
                ' Pass event token to application
                ConfigStore.AuthenticationToken = details.EventToken
                ' Trigger application
                context.TriggerAttentionRequired(_foregroundAppId, "")
                Return
            End If

            ' Handle authentication in background task.

            ' In case this handler performs more complex tasks, it may get canceled at runtime.
            ' Check if task was canceled by now.
            If _cancelRequested Then
                ' In case the task handler takes too long to generate credentials and gets canceled,
                ' the handler should terminate the authentication by aborting it
                Debug.WriteLine("Aborting authentication")
                context.AbortAuthentication(markAsManualConnect)
            Else
                ' The most common way of handling an authentication attempts is by providing WISPr credentials
                ' through the IssueCredentials API.
                ' Alternatively, an application could run its own business logic to authentication with the
                ' hotspot. In this case it should call the SkipAuthentication API. Note that it should call
                ' SkipAuthentication after it has authenticated to allow Windows to refresh the network connectivity
                ' state instantly.
                Debug.WriteLine("Issuing credentials")
                context.IssueCredentials(ConfigStore.UserName, ConfigStore.Password, ConfigStore.ExtraParameters, markAsManualConnect)
            End If
        Catch e As Exception
            Debug.WriteLine("Unhandled expection: " & e.ToString)
            handleUnexpectedError = True
        End Try

        ' The background task handler should always handle the authentication context.
        If handleUnexpectedError Then
            Try
                context.AbortAuthentication(markAsManualConnect)
            Catch e As Exception
                Debug.WriteLine("Unhandled expection: " & e.ToString)
            End Try
        End If

        Debug.WriteLine("Background " & taskInstance.Task.Name & " completed")
    End Sub

    ' Handles background task cancellation.
    Private Sub OnCanceled(sender As IBackgroundTaskInstance, reason As BackgroundTaskCancellationReason)
        ' Indicate that the background task is canceled.
        _cancelRequested = True

        Debug.WriteLine("Background " & sender.Task.Name & " cancel requested...")
    End Sub
End Class
