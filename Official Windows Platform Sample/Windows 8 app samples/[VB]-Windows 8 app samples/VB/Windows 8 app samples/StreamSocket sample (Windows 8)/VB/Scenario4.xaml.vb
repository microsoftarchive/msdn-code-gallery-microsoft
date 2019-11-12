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
Imports System
Imports System.Diagnostics
Imports Windows.ApplicationModel.Core
Imports Windows.Networking
Imports Windows.Networking.Sockets
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' A page for fourth scenario.
''' </summary>
Partial Public NotInheritable Class Scenario4
    Inherits SDKTemplate.Common.LayoutAwarePage
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
    ''' This is the click handler for the 'CloseSockets' button.
    ''' </summary>
    ''' <param name="sender">Object for which the event was generated.</param>
    ''' <param name="e">Event's parameters.</param>
    Private Sub CloseSockets_Click(sender As Object, e As RoutedEventArgs)
        Dim outValue As Object = Nothing
        If CoreApplication.Properties.TryGetValue("clientDataWriter", outValue) Then
            ' Remove the data writer from the list of application properties as we are about to close it.
            CoreApplication.Properties.Remove("clientDataWriter")
            Dim dataWriter As DataWriter = DirectCast(outValue, DataWriter)

            ' To reuse the socket with other data writer, application has to detach the stream from the writer
            ' before disposing it. This is added for completeness, as this sample closes the socket in
            ' very next block.
            dataWriter.DetachStream()
            dataWriter.Dispose()
        End If

        If CoreApplication.Properties.TryGetValue("clientSocket", outValue) Then
            ' Remove the socket from the list of application properties as we are about to close it.
            CoreApplication.Properties.Remove("clientSocket")
            Dim socket As StreamSocket = DirectCast(outValue, StreamSocket)
            socket.Dispose()
        End If

        If CoreApplication.Properties.TryGetValue("listener", outValue) Then
            ' Remove the listener from the list of application properties as we are about to close it.
            CoreApplication.Properties.Remove("listener")
            Dim listener As StreamSocketListener = DirectCast(outValue, StreamSocketListener)
            listener.Dispose()
        End If

        If CoreApplication.Properties.ContainsKey("connected") Then
            CoreApplication.Properties.Remove("connected")
        End If

        rootPage.NotifyUser("Socket and listener closed", NotifyType.StatusMessage)
    End Sub

End Class
