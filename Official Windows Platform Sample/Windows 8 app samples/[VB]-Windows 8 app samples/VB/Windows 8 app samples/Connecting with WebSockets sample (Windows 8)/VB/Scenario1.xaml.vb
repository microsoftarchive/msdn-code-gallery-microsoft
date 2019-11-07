'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System
Imports Windows.Networking.Sockets
Imports Windows.Storage.Streams
Imports Windows.Web
Imports System.Threading

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private messageWebSocket As MessageWebSocket
    Private messageWriter As DataWriter

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    Private Async Sub Start_Click(sender As Object, e As RoutedEventArgs)
        If String.IsNullOrEmpty(InputField.Text) Then
            rootPage.NotifyUser("Please specify text to send", NotifyType.ErrorMessage)
            Return
        End If

        Try
            ' Make a local copy to avoid races with Closed events.
            Dim webSocket As MessageWebSocket = messageWebSocket

            ' Have we connected yet?
            If webSocket Is Nothing Then
                Dim server As New Uri(ServerAddressField.Text.Trim())

                rootPage.NotifyUser("Connecting to: " & server.ToString, NotifyType.StatusMessage)

                webSocket = New MessageWebSocket()
                webSocket.Control.MessageType = SocketMessageType.Utf8
                AddHandler webSocket.MessageReceived, AddressOf MessageReceived
                AddHandler webSocket.Closed, AddressOf Closed

                Await webSocket.ConnectAsync(server)
                messageWebSocket = webSocket
                ' Only store it after successfully connecting.
                messageWriter = New DataWriter(webSocket.OutputStream)

                rootPage.NotifyUser("Connected", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("Already connected", NotifyType.StatusMessage)
            End If

            Dim message As String = InputField.Text

            OutputField.Text &= "Sending Message:" & vbCr & vbLf & message & vbCr & vbLf

            ' Buffer any data we want to send.
            messageWriter.WriteString(message)

            ' Send the data as one complete message.
            Await messageWriter.StoreAsync()

            rootPage.NotifyUser("Send Complete", NotifyType.StatusMessage)
        Catch ex As Exception
            ' For debugging
            Dim status As WebErrorStatus = WebSocketError.GetStatus(ex.GetBaseException().HResult)

            If status = WebErrorStatus.CannotConnect OrElse status = WebErrorStatus.NotFound OrElse status = WebErrorStatus.RequestTimeout Then
                rootPage.NotifyUser("Cannot connect to the server. Please make sure " & "to run the server setup script before running the sample.", NotifyType.ErrorMessage)
            Else
                rootPage.NotifyUser("Error: " & status.ToString, NotifyType.ErrorMessage)
            End If

            OutputField.Text &= ex.Message & vbCr & vbLf
        End Try
    End Sub

    Private Sub MessageReceived(sender As MessageWebSocket, args As MessageWebSocketMessageReceivedEventArgs)
        Try
            MarshalText(OutputField, "Message Received; Type: " & args.MessageType & vbCr & vbLf)
            Using reader As DataReader = args.GetDataReader()
                reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8

                Dim read As String = reader.ReadString(reader.UnconsumedBufferLength)
                MarshalText(OutputField, read & vbCr & vbLf)
            End Using
        Catch ex As Exception
            ' For debugging
            Dim status As WebErrorStatus = WebSocketError.GetStatus(ex.GetBaseException().HResult)

            ' Normally we'd use the status to test for specific conditions we want to handle specially,
            ' and only use ex.Message for display purposes.  In this sample, we'll just output the
            ' status for debugging here, but still use ex.Message below.
            MarshalText(OutputField, "Error: " & status.ToString & vbCr & vbLf)

            MarshalText(OutputField, ex.Message & vbCr & vbLf)
        End Try
    End Sub

    Private Sub Close_Click(sender As Object, e As RoutedEventArgs)
        Try
            ' Use Interlocked to avoid races with the Closed event.
            Dim webSocket As MessageWebSocket = Interlocked.Exchange(messageWebSocket, Nothing)
            If webSocket Is Nothing Then
                rootPage.NotifyUser("No active WebSocket, send something first", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("Closing", NotifyType.StatusMessage)
                webSocket.Close(1000, "Closed due to user request.")
            End If
        Catch ex As Exception
            Dim status As WebErrorStatus = WebSocketError.GetStatus(ex.GetBaseException().HResult)

            ' Normally we'd use the status to test for specific conditions we want to handle specially,
            ' and only use ex.Message for display purposes.  In this sample, we'll just output the
            ' status for debugging here, but still use ex.Message below.
            rootPage.NotifyUser("Error: " & status.ToString, NotifyType.ErrorMessage)

            OutputField.Text &= ex.Message & vbCr & vbLf
        End Try
    End Sub

    ' This may be triggered remotely by the server or locally by Close/Dispose()
    Private Sub Closed(sender As IWebSocket, args As WebSocketClosedEventArgs)
        MarshalText(OutputField, "Closed; Code: " & args.Code & ", Reason: " & args.Reason & vbCr & vbLf)

        ' This is invoked on another thread so use Interlocked to avoid races with the Start/Close/Reset methods.
        Dim webSocket As MessageWebSocket = Interlocked.Exchange(messageWebSocket, Nothing)
        If webSocket IsNot Nothing Then
            webSocket.Dispose()
        End If
    End Sub

    Private Sub MarshalText(output As TextBox, value As String)
        MarshalText(output, value, True)
    End Sub

    ' When operations happen on a background thread we have to marshal UI updates back to the UI thread.
    Private Async Sub MarshalText(output As TextBox, value As String, append As Boolean)
        Await output.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                            If append Then
                                                                                                output.Text &= value
                                                                                            Else
                                                                                                output.Text = value
                                                                                            End If
                                                                                        End Sub)
    End Sub
End Class
