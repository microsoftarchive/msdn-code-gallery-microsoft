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

Partial Public NotInheritable Class CustomCommand
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Click handler for the 'CustomCommandButton' button.
    ''' Demonstrates the use of a message dialog with custom commands, command callbacks, and a default command.
    ''' A message will be displayed indicating which command was invoked on the dialog.
    ''' In this scenario, 'Install updates' is selected as the default choice.
    ''' </summary>
    ''' <param name="sender">The Object that caused this event to be fired.</param>
    ''' <param name="e">State information and event data associated with the routed event.</param>
    Private Async Sub CustomCommandButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Create the message dialog and set its content and title
        Dim messageDialog = New MessageDialog("New updates have been found for this program. Would you like to install the new updates?", "Updates available")

        ' Add commands and set their callbacks
        messageDialog.Commands.Add(New UICommand("Don't install", Sub(command) rootPage.NotifyUser("The 'Don't install' command has been selected.", NotifyType.StatusMessage)))

        messageDialog.Commands.Add(New UICommand("Install updates", Sub(command) rootPage.NotifyUser("The 'Install updates' command has been selected.", NotifyType.StatusMessage)))

        ' Set the command that will be invoked by default
        messageDialog.DefaultCommandIndex = 1

        ' Show the message dialog
        Await messageDialog.ShowAsync()
    End Sub
End Class

