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
''' A page for third scenario.
''' </summary>
Partial Public NotInheritable Class Scenario3
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
    ''' This is the click handler for the 'SendHello' button.
    ''' </summary>
    ''' <param name="sender">Object for which the event was generated.</param>
    ''' <param name="e">Event's parameters.</param>
    Private Async Sub SendHello_Click(sender As Object, e As RoutedEventArgs)
        If Not CoreApplication.Properties.ContainsKey("connected") Then
            rootPage.NotifyUser("Please run previous steps before doing this one.", NotifyType.ErrorMessage)
            Return
        End If

        Dim outValue As Object = Nothing
        Dim socket As StreamSocket
        If Not CoreApplication.Properties.TryGetValue("clientSocket", outValue) Then
            rootPage.NotifyUser("Please run previous steps before doing this one.", NotifyType.ErrorMessage)
            Return
        End If

        socket = DirectCast(outValue, StreamSocket)

        ' Create a DataWriter if we did not create one yet. Otherwise use one that is already cached.
        Dim writer As DataWriter
        If Not CoreApplication.Properties.TryGetValue("clientDataWriter", outValue) Then
            writer = New DataWriter(socket.OutputStream)
            CoreApplication.Properties.Add("clientDataWriter", writer)
        Else
            writer = DirectCast(outValue, DataWriter)
        End If

        ' Write first the length of the string as UINT32 value followed up by the string. Writing data to the writer will just store data in memory.
        Dim stringToSend As String = "Hello"
        writer.WriteUInt32(writer.MeasureString(stringToSend))
        writer.WriteString(stringToSend)

        ' Write the locally buffered data to the network.
        Try
            Await writer.StoreAsync()
            SendOutput.Text = """" & stringToSend & """ sent successfully."
        Catch exception As Exception
            ' If this is an unknown status it means that the error if fatal and retry will likely fail.
            If SocketError.GetStatus(exception.HResult) = SocketErrorStatus.Unknown Then
                Throw
            End If

            rootPage.NotifyUser("Send failed with error: " & exception.Message, NotifyType.ErrorMessage)
        End Try
    End Sub
End Class
