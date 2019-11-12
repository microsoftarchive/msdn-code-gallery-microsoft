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

Partial Public NotInheritable Class ReadMessage
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
        ReadIdText.Text = ""
        DateText.Text = ""
        ReadFromText.Text = ""
        ReadMessageText.Text = ""
    End Sub

    ' Clean-up when scenario page is left. This is called when the
    ' user navigates away from the scenario page.
    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        ' Release the device.
        device = Nothing
    End Sub

    ' Handle a request to read a message.
    Private Async Sub Read_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
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

        ' Clear message display.
        DateText.Text = ""
        ReadFromText.Text = ""
        ReadMessageText.Text = ""

        Try
            ' Parse the message ID - must be number between 1 and maximum message count.
            Dim id As UInteger
            If UInteger.TryParse(ReadIdText.Text, id) AndAlso (id >= 1) AndAlso (id <= device.MessageStore.MaxMessages) Then
                rootPage.NotifyUser("Reading message ...", NotifyType.StatusMessage)

                ' Get the selected message from message store asynchronously.
                Dim msg As ISmsMessage = Await device.MessageStore.GetMessageAsync(id)

                ' See if this is a text message by querying for the text message interface.
                Dim textMsg As ISmsTextMessage = TryCast(msg, ISmsTextMessage)
                If textMsg Is Nothing Then
                    ' If it is a binary message then try to convert it to a text message.
                    If TypeOf msg Is SmsBinaryMessage Then
                        textMsg = SmsTextMessage.FromBinaryMessage(TryCast(msg, SmsBinaryMessage))
                    End If
                End If

                ' Display the text message information.
                If textMsg IsNot Nothing Then
                    DateText.Text = textMsg.Timestamp.DateTime.ToString()
                    ReadFromText.Text = textMsg.From
                    ReadMessageText.Text = textMsg.Body

                    rootPage.NotifyUser("Message read.", NotifyType.StatusMessage)
                End If
            Else
                rootPage.NotifyUser("Invalid ID number entered.", NotifyType.ErrorMessage)
            End If
        Catch ex As Exception
            rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)

            ' On failure, release the device. If the user revoked access or the device
            ' is removed, a new device object must be obtained.
            device = Nothing
        End Try
    End Sub
End Class
