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

Partial Public NotInheritable Class ScenarioInput3
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

    Private Sub SetPasswordExplainVisibility(isShown As Boolean)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim text1 As TextBlock = TryCast(outputFrame.FindName("PasswordExplain1"), TextBlock)
        Dim text2 As TextBlock = TryCast(outputFrame.FindName("PasswordExplain2"), TextBlock)
        If isShown Then
            text1.Visibility = Windows.UI.Xaml.Visibility.Visible
            text2.Visibility = Windows.UI.Xaml.Visibility.Visible
        Else
            text1.Visibility = Windows.UI.Xaml.Visibility.Collapsed
            text2.Visibility = Windows.UI.Xaml.Visibility.Collapsed
        End If
    End Sub

    Private Sub SetResult(res As CredentialPickerResults)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim status As TextBox = TryCast(outputFrame.FindName("Status"), TextBox)
        status.Text = String.Format("OK (Returned Error Code: {0})", res.ErrorCode)
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

    ''' <summary>
    ''' This is the click handler for the 'Launch' button. 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub Launch_Click(sender As Object, e As RoutedEventArgs)
        Dim credPickerOptions As New CredentialPickerOptions()
        Try
            credPickerOptions.Message = Message.Text
            credPickerOptions.Caption = Caption.Text
            credPickerOptions.TargetName = Target.Text
        Catch err As Exception
            'handle validation exceptions here
            SetError(err.Message)
            Return
        End Try
        credPickerOptions.AlwaysDisplayDialog = (AlwaysShowDialog.IsChecked.Value = True)
        If Protocol.SelectedItem Is Nothing Then
            'default protocol, if no selection
            credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Negotiate
        Else
            Dim selectedProtocol As String = DirectCast(Protocol.SelectedItem, ComboBoxItem).Content.ToString
            If selectedProtocol.Equals("Kerberos", StringComparison.CurrentCultureIgnoreCase) Then
                credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Kerberos
            ElseIf selectedProtocol.Equals("Negotiate", StringComparison.CurrentCultureIgnoreCase) Then
                credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Negotiate
            ElseIf selectedProtocol.Equals("NTLM", StringComparison.CurrentCultureIgnoreCase) Then
                credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Ntlm
            ElseIf selectedProtocol.Equals("CredSsp", StringComparison.CurrentCultureIgnoreCase) Then
                credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.CredSsp
            ElseIf selectedProtocol.Equals("Basic", StringComparison.CurrentCultureIgnoreCase) Then
                credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Basic
            ElseIf selectedProtocol.Equals("Digest", StringComparison.CurrentCultureIgnoreCase) Then
                credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Digest
            ElseIf selectedProtocol.Equals("Custom", StringComparison.CurrentCultureIgnoreCase) Then
                If CustomProtocol.Text IsNot Nothing AndAlso CustomProtocol.Text <> String.Empty Then
                    credPickerOptions.AuthenticationProtocol = AuthenticationProtocol.Custom
                    credPickerOptions.CustomAuthenticationProtocol = CustomProtocol.Text
                Else
                    rootPage.NotifyUser("Please enter a custom protocol", NotifyType.ErrorMessage)
                End If
            End If
        End If

        If CheckboxState.SelectedItem IsNot Nothing Then
            Dim checkboxState__1 As String = DirectCast(CheckboxState.SelectedItem, ComboBoxItem).Content.ToString
            If checkboxState__1.Equals("Hidden", StringComparison.CurrentCultureIgnoreCase) Then
                credPickerOptions.CredentialSaveOption = CredentialSaveOption.Hidden
            ElseIf checkboxState__1.Equals("Selected", StringComparison.CurrentCultureIgnoreCase) Then
                credPickerOptions.CredentialSaveOption = CredentialSaveOption.Selected
            ElseIf checkboxState__1.Equals("Unselected", StringComparison.CurrentCultureIgnoreCase) Then
                credPickerOptions.CredentialSaveOption = CredentialSaveOption.Unselected
            End If
        End If

        Try
            Dim credPickerResults As CredentialPickerResults = Await Windows.Security.Credentials.UI.CredentialPicker.PickAsync(credPickerOptions)
            SetResult(credPickerResults)
        Catch err As Exception
            SetError(err.Message)
        End Try
    End Sub

    Private Sub Protocol_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim box As ComboBox = TryCast(sender, ComboBox)
        If Protocol Is Nothing OrElse Protocol.SelectedItem Is Nothing Then
            'return if this was triggered before all components are initialized
            Return
        End If

        Dim selectedProtocol As String = DirectCast(Protocol.SelectedItem, ComboBoxItem).Content.ToString

        If selectedProtocol.Equals("Custom", StringComparison.CurrentCultureIgnoreCase) Then
            CustomProtcolStackPanel.Visibility = Windows.UI.Xaml.Visibility.Visible
        Else
            CustomProtcolStackPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed
            'Basic and Digest return plaintext passwords
            If selectedProtocol.Equals("Basic", StringComparison.CurrentCultureIgnoreCase) Then
                SetPasswordExplainVisibility(False)
            ElseIf selectedProtocol.Equals("Digest", StringComparison.CurrentCultureIgnoreCase) Then
                SetPasswordExplainVisibility(False)
            Else
                SetPasswordExplainVisibility(True)
            End If
        End If
    End Sub
End Class
