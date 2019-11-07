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

Partial Public NotInheritable Class SendPduMessage
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
        PduMessageText.Text = ""
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
            ' Convert the entered message from hex to a byte array.
            Dim data() As Byte = Nothing
            ParseHexString(PduMessageText.Text, data)

            ' Create a binary message and set the data.
            Dim msg As New SmsBinaryMessage()
            msg.SetData(data)

            ' Set format based on the SMS device cellular type (GSM or CDMA)
            msg.Format = If(device.CellularClass = CellularClass.Gsm, SmsDataFormat.GsmSubmit, SmsDataFormat.CdmaSubmit)

            ' Send message asynchronously.
            rootPage.NotifyUser("Sending message ...", NotifyType.StatusMessage)
            Await device.SendMessageAsync(msg)
            rootPage.NotifyUser("Sent message sent in PDU format", NotifyType.StatusMessage)
        Catch ex As Exception
            rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)

            ' On failure, release the device. If the user revoked access or the device
            ' is removed, a new device object must be obtained.
            device = Nothing
        End Try
    End Sub

    ' Convert a hex string to an equivalent byte array.
    ' The string must contain an even number of hex digits.
    Private Sub ParseHexString(ByVal hex As String, ByRef data() As Byte)
        If hex.Length > 0 AndAlso (hex.Length And 1) = 0 Then
            Dim byteCount As Integer = hex.Length \ 2
            data = New Byte(byteCount - 1) {}

            For i As Integer = 0 To byteCount - 1
                data(i) = Byte.Parse(hex.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber)
            Next i
        Else
            Throw New FormatException("Input string must have an even number of hex digits")
        End If
    End Sub
End Class
