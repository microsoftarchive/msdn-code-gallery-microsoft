'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Networking.NetworkOperators
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class RequestResponse
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
    ''' This is the click handler for the 'SendButton' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub SendButton_Click(sender As Object, e As RoutedEventArgs)
        ' Get the USSD message text.
        If MessageText.Text = "" Then
            rootPage.NotifyUser("Message cannot be empty", NotifyType.ErrorMessage)
            Return
        End If

        Try
            ' Get the network account ID.
            Dim networkAccIds As IReadOnlyList(Of String) = Windows.Networking.NetworkOperators.MobileBroadbandAccount.AvailableNetworkAccountIds

            If networkAccIds.Count = 0 Then
                rootPage.NotifyUser("No network account ID found", NotifyType.ErrorMessage)
                Return
            End If
            ' For the sake of simplicity, assume we want to use the first account.
            ' Refer to the MobileBroadbandAccount API's for how to select a specific account ID.
            Dim networkAccountId As String = networkAccIds(0)

            SendButton.IsEnabled = False
            rootPage.NotifyUser("Sending USSD request", NotifyType.StatusMessage)

            ' Create a USSD session for the specified network acccount ID.
            Dim session As UssdSession = UssdSession.CreateFromNetworkAccountId(networkAccountId)

            ' Send a message to the network and wait for the reply. This message is
            ' specific to the network operator and must be selected accordingly.
            Dim reply As UssdReply = Await session.SendMessageAndGetReplyAsync(New UssdMessage(MessageText.Text))

            ' Display the network reply. The reply always contains a ResultCode.
            Dim code As UssdResultCode = reply.ResultCode
            If code = UssdResultCode.ActionRequired OrElse code = UssdResultCode.NoActionRequired Then
                ' If the actionRequired or noActionRequired ResultCode is returned, the reply contains
                ' a message from the network.
                Dim replyMessage As UssdMessage = reply.Message
                Dim payloadAsText As String = replyMessage.PayloadAsText
                If payloadAsText <> "" Then
                    ' The message may be sent using various encodings. If Windows supports
                    ' the encoding, the message can be accessed as text and will not be empty.
                    ' Therefore, the test for an empty string is sufficient.
                    rootPage.NotifyUser("Response: " & payloadAsText, NotifyType.StatusMessage)
                Else
                    ' If Windows does not support the encoding, the application may check
                    ' the DataCodingScheme used for encoding and access the raw message
                    ' through replyMessage.GetPayload
                    rootPage.NotifyUser("Unsupported data coding scheme 0x" & replyMessage.DataCodingScheme.ToString("X"), NotifyType.StatusMessage)
                End If
            Else
                rootPage.NotifyUser("Request failed: " & code.ToString, NotifyType.StatusMessage)
            End If
            If code = UssdResultCode.ActionRequired Then
                ' Close the session from our end
                session.Close()
            End If
        Catch ex As Exception
            rootPage.NotifyUser("Unexpected exception occured: " & ex.ToString, NotifyType.ErrorMessage)
        End Try
        SendButton.IsEnabled = True
    End Sub
End Class
