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
Imports Windows.Devices.Sms

Partial Public NotInheritable Class ReceiveMessage
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private device As SmsDevice
    Private msgCount As Integer
    Private listening As Boolean

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    ' Constructor
    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ' Initialize variables and controls for the scenario.
    ' This method is called just before the scenario page is displayed.
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        listening = False
        msgCount = 0

        ReceivedFromText.Text = ""
        ReceivedMessageText.Text = ""
    End Sub

    ' Clean-up when scenario page is left. This is called when the
    ' user navigates away from the scenario page.
    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        ' Detach event handler
        If listening Then
            RemoveHandler device.SmsMessageReceived, AddressOf device_SmsMessageReceived
        End If

        ' Release the device
        device = Nothing
    End Sub


    ' Handle a request to listen for received messages.
    Private Async Sub Receive_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' If this is the first request, get the default SMS device. If this
        ' is the first SMS device access, the user will be prompted to grant
        ' access permission for this application.
        If device Is Nothing Then
            Try
                rootPage.NotifyUser("Getting default SMS device ...", NotifyType.StatusMessage)
                device = Await SmsDevice.GetDefaultAsync()
            Catch ex As Exception
                rootPage.NotifyUser("Failed to find SMS device" & vbLf & ex.Message, NotifyType.ErrorMessage)
                Return
            End Try
        End If

        ' Attach a message received handler to the device, if not already listening
        If Not listening Then
            Try
                msgCount = 0
                AddHandler device.SmsMessageReceived, AddressOf device_SmsMessageReceived
                listening = True
                rootPage.NotifyUser("Waiting for message ...", NotifyType.StatusMessage)
            Catch ex As Exception
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)

                ' On failure, release the device. If the user revoked access or the device
                ' is removed, a new device object must be obtained.
                device = Nothing
            End Try
        End If
    End Sub

    ' Handle a received message event.
    Private Async Sub device_SmsMessageReceived(ByVal sender As SmsDevice, ByVal args As SmsMessageReceivedEventArgs)
        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                     ' Get message from the event args.
                                                                                     Try
                                                                                         Dim msg As SmsTextMessage = args.TextMessage
                                                                                         msgCount += 1
                                                                                         ReceivedCountText.Text = msgCount.ToString()
                                                                                         ReceivedFromText.Text = msg.From
                                                                                         ReceivedMessageText.Text = msg.Body
                                                                                         rootPage.NotifyUser(msgCount.ToString() & (If(msgCount = 1, " message", " messages")) & " received", NotifyType.StatusMessage)
                                                                                     Catch ex As Exception
                                                                                         rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)
                                                                                     End Try
                                                                                 End Sub)
    End Sub
End Class
