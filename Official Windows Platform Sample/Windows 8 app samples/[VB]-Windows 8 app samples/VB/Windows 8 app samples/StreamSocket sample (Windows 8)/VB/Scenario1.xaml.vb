'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports SDKTemplate
Imports Windows.ApplicationModel.Core
Imports Windows.Networking.Sockets
Imports Windows.Storage.Streams
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' A page for first scenario.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

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

    ''' <summary>
    ''' This is the click handler for the 'StartListener' button.
    ''' </summary>
    ''' <param name="sender">Object for which the event was generated.</param>
    ''' <param name="e">Event's parameters.</param>
    Private Async Sub StartListener_Click(sender As Object, e As RoutedEventArgs)
        ' Overriding the listener here is safe as it will be deleted once all references to it are gone. However, in many cases this
        ' is a dangerous pattern to override data semi-randomly (each time user clicked the button) so we block it here.
        If CoreApplication.Properties.ContainsKey("listener") Then
            rootPage.NotifyUser("This step has already been executed. Please move to the next one.", NotifyType.ErrorMessage)
            Exit Sub
        End If

        Dim listener As New StreamSocketListener()
        AddHandler listener.ConnectionReceived, AddressOf OnConnection

        ' Save the socket, so subsequent steps can use it.
        CoreApplication.Properties.Add("listener", listener)

        ' Start listen operation.
        Try
            Await listener.BindServiceNameAsync(ServiceNameForListener.Text)
            rootPage.NotifyUser("Listening", NotifyType.StatusMessage)
        Catch exception As Exception
            ' If this is an unknown status it means that the error is fatal and retry will likely fail.
            If SocketError.GetStatus(exception.HResult) = SocketErrorStatus.Unknown Then
                Throw
            End If

            rootPage.NotifyUser("Start listening failed with error: " & exception.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    ''' <summary>
    ''' Invoked once a connection is accepted by StreamSocketListener.
    ''' </summary>
    ''' <param name="sender">The listener that accepted the connection.</param>
    ''' <param name="args">Parameters associated with the accepted connection.</param>
    Private Async Sub OnConnection(sender As StreamSocketListener, args As StreamSocketListenerConnectionReceivedEventArgs)
        Dim reader As New DataReader(args.Socket.InputStream)
        Try
            While True
                ' Read first 4 bytes (length of the subsequent string).
                Dim sizeFieldCount As UInteger = Await reader.LoadAsync(4)
                
                If sizeFieldCount <> System.Runtime.InteropServices.Marshal.SizeOf(GetType(UInteger)) Then
                    ' The underlying socket was closed before we were able to read the whole data.
                    Exit Sub
                End If

                ' Read the string.
                Dim stringLength As UInteger = reader.ReadUInt32()
                Dim actualStringLength As UInteger = Await reader.LoadAsync(stringLength)
                If stringLength <> actualStringLength Then
                    ' The underlying socket was closed before we were able to read the whole data.
                    Exit Sub
                End If

                ' Display the string on the screen. The event is invoked on a non-UI thread, so we need to marshal the text back to the UI thread.
                NotifyUserFromAsyncThread(String.Format("Receive data: ""{0}""", reader.ReadString(actualStringLength)), NotifyType.StatusMessage)
            End While
        Catch exception As Exception
            ' If this is an unknown status it means that the error is fatal and retry will likely fail.
            If SocketError.GetStatus(exception.HResult) = SocketErrorStatus.Unknown Then
                Throw
            End If

            NotifyUserFromAsyncThread("Read stream failed with error: " & exception.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    Private Async Sub NotifyUserFromAsyncThread(strMessage As String, type As NotifyType)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     rootPage.NotifyUser(strMessage, type)
                                                                 End Sub)

    End Sub
End Class
