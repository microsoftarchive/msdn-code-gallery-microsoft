'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Popups

Partial Public NotInheritable Class CancelCommand
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Click handler for the 'CancelCommandButton' button.
    ''' Demonstrates setting the command to be invoked when the 'escape' key is pressed.
    ''' Also demonstrates retrieval of the label of the chosen command and setting a callback to a function.
    ''' A message will be displayed indicating which command was invoked.
    ''' In this scenario, 'Try again' is selected as the default choice, and the 'escape' key will invoke the command named 'Close'
    ''' </summary>
    ''' <param name="sender">The Object that caused this event to be fired.</param>
    ''' <param name="e">State information and event data associated with the routed event.</param>
    Private Async Sub CancelCommandButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Create the message dialog and set its content
        Dim messageDialog = New MessageDialog("No internet connection has been found.")

        ' Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
        messageDialog.Commands.Add(New UICommand("Try again", New UICommandInvokedHandler(AddressOf Me.CommandInvokedHandler)))
        messageDialog.Commands.Add(New UICommand("Close", New UICommandInvokedHandler(AddressOf Me.CommandInvokedHandler)))

        ' Set the command that will be invoked by default
        messageDialog.DefaultCommandIndex = 0

        ' Set the command to be invoked when escape is pressed
        messageDialog.CancelCommandIndex = 1

        ' Show the message dialog
        Await messageDialog.ShowAsync()
    End Sub

#Region "Commands"
    ''' <summary>
    ''' Callback function for the invocation of the dialog commands.
    ''' </summary>
    ''' <param name="command">The command that was invoked.</param>
    Private Sub CommandInvokedHandler(ByVal command As IUICommand)
        ' Display message showing the label of the command that was invoked
        rootPage.NotifyUser("The '" & command.Label & "' command has been selected.", NotifyType.StatusMessage)
    End Sub
#End Region
End Class

