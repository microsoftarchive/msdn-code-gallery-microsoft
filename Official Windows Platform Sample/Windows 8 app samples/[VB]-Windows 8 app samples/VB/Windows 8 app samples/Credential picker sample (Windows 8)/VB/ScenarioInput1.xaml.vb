' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Display
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports Windows.Security.Credentials.UI


Partial Public NotInheritable Class ScenarioInput1
    Inherits Page
    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

#End Region

#Region "Use this code if you need access to elements in the output frame - otherwise delete"
    Private Sub rootPage_OutputFrameLoaded(sender As Object, e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame.

        ' Get a pointer to the content within the OutputFrame.
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)

        ' Go find the elements that we need for this scenario.
        ' ex: flipView1 = outputFrame.FindName("FlipView1") as FlipView;
    End Sub
    Private Sub SetError(err As String)
        rootPage.NotifyUser(err, NotifyType.ErrorMessage)
    End Sub

    Private Sub SetResult(res As CredentialPickerResults)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim status As TextBox = TryCast(outputFrame.FindName("Status"), TextBox)
        If res.ErrorCode = 0 Then
            status.Text = String.Format("OK")
        Else
            status.Text = String.Format("Error returned from CredPicker API: {0}", res.ErrorCode)
        End If
        Dim domain As TextBox = TryCast(outputFrame.FindName("Domain"), TextBox)
        domain.Text = res.CredentialDomainName
        Dim username As TextBox = TryCast(outputFrame.FindName("Username"), TextBox)
        username.Text = res.CredentialUserName
        Dim password As TextBox = TryCast(outputFrame.FindName("Password"), TextBox)
        'todo: dump this as binary
        password.Text = res.CredentialPassword
        Dim credsaved As TextBox = TryCast(outputFrame.FindName("CredentialSaved"), TextBox)
        credsaved.Text = (If(res.CredentialSaved, "true", "false"))
        Dim checkboxState As TextBox = TryCast(outputFrame.FindName("CheckboxState"), TextBox)
        Select Case res.CredentialSaveOption
            Case CredentialSaveOption.Hidden
                checkboxState.Text = "Hidden"
                Exit Select
            Case CredentialSaveOption.Selected
                checkboxState.Text = "Selected"
                Exit Select
            Case CredentialSaveOption.Unselected
                checkboxState.Text = "Unselected"
                Exit Select
        End Select
    End Sub

#End Region

#Region "Sample Click Handlers - modify if you need them, delete them otherwise"
    ''' <summary>
    ''' This is the click handler for the 'Default' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub Launch_Click(sender As Object, e As RoutedEventArgs)
        Dim message__1 As String = Message.Text
        Dim target__2 As String = Target.Text
        Try
            Dim credPickerResults As CredentialPickerResults = Await Windows.Security.Credentials.UI.CredentialPicker.PickAsync(target__2, message__1)
            SetResult(credPickerResults)
        Catch err As Exception
            SetError(err.Message)
        End Try


    End Sub

#End Region
End Class
