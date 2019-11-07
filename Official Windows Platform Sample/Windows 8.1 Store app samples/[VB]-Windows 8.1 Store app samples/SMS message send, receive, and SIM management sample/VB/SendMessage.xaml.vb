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

Partial Public NotInheritable Class SendMessage
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private device As SmsDevice

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
        SendToText.Text = ""
        SendMessageText.Text = ""
    End Sub

    ' Clean-up when scenario page is left. This is called when the
    ' user navigates away from the scenario page.
    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        ' Release the device.
        device = Nothing
    End Sub

    ' Handle a request to send a text message.
    Private Async Sub Send_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
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

        Try
            ' Create a text message - set the entered destination number and message text.
            Dim msg As New SmsTextMessage()
            msg.To = SendToText.Text
            msg.Body = SendMessageText.Text

            ' Send the message asynchronously
            rootPage.NotifyUser("Sending message ...", NotifyType.StatusMessage)
            Await device.SendMessageAsync(msg)
            rootPage.NotifyUser("Text message sent", NotifyType.StatusMessage)
        Catch err As Exception
            rootPage.NotifyUser(err.Message, NotifyType.ErrorMessage)

            ' On failure, release the device. If the user revoked access or the device
            ' is removed, a new device object must be obtained.
            device = Nothing
        End Try
    End Sub
End Class
