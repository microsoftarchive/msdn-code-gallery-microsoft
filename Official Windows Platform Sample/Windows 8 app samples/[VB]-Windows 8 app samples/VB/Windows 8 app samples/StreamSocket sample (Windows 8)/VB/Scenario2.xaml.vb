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
Imports Windows.Networking
Imports Windows.Networking.Sockets
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' A page for second scenario.
''' </summary>
Partial Public NotInheritable Class Scenario2
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
    ''' This is the click handler for the 'ConnectSocket' button.
    ''' </summary>
    ''' <param name="sender">Object for which the event was generated.</param>
    ''' <param name="e">Event's parameters.</param>
    Private Async Sub ConnectSocket_Click(sender As Object, e As RoutedEventArgs)
        If CoreApplication.Properties.ContainsKey("clientSocket") Then
            rootPage.NotifyUser("This step has already been executed. Please move to the next one.", NotifyType.ErrorMessage)
            Return
        End If

        Dim socket As New StreamSocket()

        ' Save the socket, so subsequent steps can use it.
        CoreApplication.Properties.Add("clientSocket", socket)

        Try
            ' Connect to the server (in our case the listener we created in previous step).
            Await socket.ConnectAsync(New HostName(HostNameForConnect.Text), ServiceNameForConnect.Text)

            rootPage.NotifyUser("Connected to the server", NotifyType.StatusMessage)

            ' Mark the socket as connected. Set the value to null, as we care only about the fact that the property is set.
            CoreApplication.Properties.Add("connected", Nothing)
        Catch exception As Exception
            ' If this is an unknown status it means that the error is fatal and retry will likely fail.
            If SocketError.GetStatus(exception.HResult) = SocketErrorStatus.Unknown Then
                Throw
            End If

            rootPage.NotifyUser("Connect failed with error: " & exception.Message, NotifyType.ErrorMessage)
        End Try
    End Sub
End Class
