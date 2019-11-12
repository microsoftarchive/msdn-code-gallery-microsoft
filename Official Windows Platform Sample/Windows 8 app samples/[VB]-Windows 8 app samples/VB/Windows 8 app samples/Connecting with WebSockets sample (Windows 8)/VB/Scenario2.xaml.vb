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
Imports System.IO
Imports System.Runtime.InteropServices.WindowsRuntime
Imports System.Threading
Imports System.Threading.Tasks
Imports SDKTemplate
Imports Windows.Networking.Sockets
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.Web

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private streamWebSocket As StreamWebSocket
    Private readBuffer As Byte()

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
        Try
            ' Make a local copy to avoid races with the Closed event.
            Dim webSocket As StreamWebSocket = streamWebSocket

            ' Have we connected yet?
            If webSocket Is Nothing Then
                Dim server As New Uri(ServerAddressField.Text.Trim())

                rootPage.NotifyUser("Connecting to: " & server.ToString, NotifyType.StatusMessage)

                webSocket = New StreamWebSocket()
                AddHandler webSocket.Closed, AddressOf Scenario2Closed

                Await webSocket.ConnectAsync(server)
                streamWebSocket = webSocket
                ' Only store it after successfully connecting.
                readBuffer = New Byte(999) {}

                ' Start a background task to continuously read for incoming data
                Dim receiving As Task = Task.Factory.StartNew(AddressOf Scenario2ReceiveData, webSocket.InputStream.AsStreamForRead(), TaskCreationOptions.LongRunning)

                ' Start a background task to continuously write outgoing data
                Dim sending As Task = Task.Factory.StartNew(AddressOf Scenario2SendData, webSocket.OutputStream, TaskCreationOptions.LongRunning)

                rootPage.NotifyUser("Connected", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("Already connected", NotifyType.StatusMessage)
            End If
        Catch ex As Exception
            ' For debugging
            Dim status As WebErrorStatus = WebSocketError.GetStatus(ex.GetBaseException().HResult)

            If status = WebErrorStatus.CannotConnect OrElse status = WebErrorStatus.NotFound OrElse status = WebErrorStatus.RequestTimeout Then
                rootPage.NotifyUser("Cannot connect to the server. Please make sure " & "to run the server setup script before running the sample.", NotifyType.ErrorMessage)
            Else
                rootPage.NotifyUser("Error: " & status, NotifyType.ErrorMessage)
            End If

            OutputField.Text &= ex.Message & vbCr & vbLf
        End Try
    End Sub

    ' Continuously write outgoing data. For writing data we'll show how to use data.AsBuffer() to get an 
    ' IBuffer for use with webSocket.OutputStream.WriteAsync.  Alternatively you can call 
    ' webSocket.OutputStream.AsStreamForWrite() to use .NET streams.
    Private Async Sub Scenario2SendData(state As Object)
        Dim dataSent As Integer = 0
        Dim data As Byte() = New Byte() {&H0, &H1, &H2, &H3, &H4, &H5, &H6, &H7, &H8, &H9}

        MarshalText(OutputField, "Background sending data in " & data.Length & " byte chunks each second." & vbCr & vbLf)

        Try
            Dim writeStream As IOutputStream = DirectCast(state, IOutputStream)

            ' Send until the socket gets closed/stopped
            While True
                ' using System.Runtime.InteropServices.WindowsRuntime;
                Await writeStream.WriteAsync(data.AsBuffer())

                dataSent += data.Length

                MarshalText(DataSentField, dataSent.ToString(), False)

                ' Delay so the user can watch what's going on.
                Await Task.Delay(TimeSpan.FromSeconds(1))
            End While
        Catch generatedExceptionName As ObjectDisposedException
            MarshalText(OutputField, "Background write stopped." & vbCr & vbLf)
        Catch ex As Exception
            Dim status As WebErrorStatus = WebSocketError.GetStatus(ex.GetBaseException().HResult)
            MarshalText(OutputField, "Error: " & status & vbCr & vbLf)
            If status = WebErrorStatus.OperationCanceled Then
                MarshalText(OutputField, "Background write canceled." & vbCr & vbLf)
            Else
                MarshalText(OutputField, ex.Message & vbCr & vbLf)
            End If
        End Try
    End Sub

    ' Continuously read incoming data. For reading data we'll show how to use webSocket.InputStream.AsStream()
    ' to get a .NET stream. Alternatively you could call readBuffer.AsBuffer() to use IBuffer with 
    ' webSocket.InputStream.ReadAsync.
    Private Async Sub Scenario2ReceiveData(state As Object)
        Dim bytesReceived As Integer = 0
        Try
            Dim readStream As Stream = DirectCast(state, Stream)
            MarshalText(OutputField, "Background read starting." & vbCr & vbLf)

            While True
                ' Until closed and ReadAsync fails.
                Dim read As Integer = Await readStream.ReadAsync(readBuffer, 0, readBuffer.Length)
                bytesReceived += read

                ' Do something with the data.
                MarshalText(DataReceivedField, bytesReceived.ToString(), False)
            End While
        Catch generatedExceptionName As ObjectDisposedException
            MarshalText(OutputField, "Background read stopped." & vbCr & vbLf)
        Catch ex As Exception
            Dim status As WebErrorStatus = WebSocketError.GetStatus(ex.GetBaseException().HResult)
            MarshalText(OutputField, "Error: " & status.ToString & vbCr & vbLf)
            If status = WebErrorStatus.OperationCanceled Then
                MarshalText(OutputField, "Background read canceled." & vbCr & vbLf)
            Else
                MarshalText(OutputField, ex.Message & vbCr & vbLf)
            End If
        End Try
    End Sub

    Private Sub Stop_Click(sender As Object, e As RoutedEventArgs)
        Try
            ' Use Interlocked to avoid races with the Closed event.
            Dim webSocket As StreamWebSocket = Interlocked.Exchange(streamWebSocket, Nothing)
            If webSocket IsNot Nothing Then
                rootPage.NotifyUser("Stopping", NotifyType.StatusMessage)
                webSocket.Close(1000, "Closed due to user request.")
            Else
                rootPage.NotifyUser("There is no active socket to stop.", NotifyType.StatusMessage)
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
    Private Sub Scenario2Closed(sender As IWebSocket, args As WebSocketClosedEventArgs)
        MarshalText(OutputField, "Closed; Code: " & args.Code & ", Reason: " & args.Reason & vbCr & vbLf)

        ' This is invoked on another thread so use Interlocked to avoid races with the Start/Stop/Reset methods.
        Dim webSocket As StreamWebSocket = Interlocked.Exchange(streamWebSocket, Nothing)
        'If webSocket IsNot Nothing Then
        '    webSocket.Dispose()
        'End If
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
