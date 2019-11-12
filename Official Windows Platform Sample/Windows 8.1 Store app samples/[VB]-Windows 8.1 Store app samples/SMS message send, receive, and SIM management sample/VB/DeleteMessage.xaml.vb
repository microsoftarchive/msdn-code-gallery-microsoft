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

Partial Public NotInheritable Class DeleteMessage
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
        DeleteIdText.Text = ""
    End Sub

    ' Clean-up when scenario page is left. This is called when the
    ' user navigates away from the scenario page.
    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        ' Release the device.
        device = Nothing
    End Sub

    ' Handle a request to delete a message.
    Private Async Sub Delete_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Parse the entered message ID and pass it to the common delete method
        Dim id As UInteger
        If UInteger.TryParse(DeleteIdText.Text, id) Then
            Await DoDeleteAsync(id)
        Else
            rootPage.NotifyUser("Invalid message ID entered", NotifyType.ErrorMessage)
        End If
    End Sub

    ' Handle a request to delete all messages.
    Private Async Sub DeleteAll_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Call the common delete method with MaxValue to indicate delete all.
        Await DoDeleteAsync(UInteger.MaxValue)
    End Sub

    ' Delete one or all messages.
    ' The ID of the message to delete is passed as a parameter. An ID of MaxValue
    ' specifies that all messages should be deleted.
    Private Async Function DoDeleteAsync(ByVal messageId As UInteger) As Task
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
            ' Delete one or all messages.
            If messageId < UInteger.MaxValue Then
                ' Verify ID is within range (1 to message store capacity). Note that a SIM
                ' can have gaps in its message array, so all valid IDs do not necessarily map
                ' to messages.
                If messageId >= 1 AndAlso messageId <= device.MessageStore.MaxMessages Then
                    ' Delete the selected message asynchronously.
                    rootPage.NotifyUser("Deleting message ...", NotifyType.StatusMessage)
                    Await device.MessageStore.DeleteMessageAsync(messageId)
                    rootPage.NotifyUser("Message " & messageId & " deleted", NotifyType.StatusMessage)
                Else
                    rootPage.NotifyUser("Message ID entered is out of range", NotifyType.ErrorMessage)
                End If
            Else
                ' Delete all messages asynchronously.
                rootPage.NotifyUser("Deleting all messages ...", NotifyType.StatusMessage)
                Await device.MessageStore.DeleteMessagesAsync(SmsMessageFilter.All)
                rootPage.NotifyUser("All messages deleted", NotifyType.StatusMessage)
            End If
        Catch ex As Exception
            rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)

            ' On failure, release the device. If the user revoked access or the device
            ' is removed, a new device object must be obtained.
            device = Nothing
        End Try
    End Function
End Class
