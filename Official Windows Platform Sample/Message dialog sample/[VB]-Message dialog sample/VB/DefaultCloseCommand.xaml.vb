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

Partial Public NotInheritable Class DefaultCloseCommand
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Click handler for the 'DefaultCloseCommandButton' button.
    ''' Demonstrates showing a message dialog with a default close command and content.
    ''' A message will be displayed indicating that the dialog has been closed.
    ''' In this scenario, the only command is the default 'Close' command that is used if no other commands are specified.
    ''' </summary>
    ''' <param name="sender">The Object that caused this event to be fired.</param>
    ''' <param name="e">State information and event data associated with the routed event.</param>
    Private Async Sub DefaultCloseCommandButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Create the message dialog and set its content; it will get a default "Close" button since there aren't any other buttons being added
        Dim messageDialog = New MessageDialog("You've exceeded your trial period.")

        ' Show the message dialog and wait
        Await messageDialog.ShowAsync()
        rootPage.NotifyUser("The dialog has now been closed", NotifyType.StatusMessage)
    End Sub
End Class

