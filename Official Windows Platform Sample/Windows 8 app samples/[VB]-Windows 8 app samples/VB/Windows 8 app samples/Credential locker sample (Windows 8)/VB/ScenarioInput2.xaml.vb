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
Imports Windows.Security.Authentication.Web
Imports Windows.Security.Cryptography.Core
Imports Windows.Security.Cryptography
Imports Windows.Storage.Streams
Imports System.Text
Imports System.Net
Imports System.IO
Imports SDKTemplate
Imports Windows.Security.Credentials

Partial Public NotInheritable Class ScenarioInput2
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
#End Region

    '
    ' Custom UrlEncode
    '
    Private Sub DebugPrint(Trace As String)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim ErrorMessage As TextBox = TryCast(outputFrame.FindName("ErrorMessage"), TextBox)
        ErrorMessage.Text &= Trace + vbCr & vbLf
    End Sub

    Private Sub CleanCombobox()
        Dim inputFrame As Page = DirectCast(rootPage.InputFrame.Content, Page)
        Dim SelectUser As ComboBox = TryCast(inputFrame.FindName("SelectUser"), ComboBox)
        Try
            SelectUser.SelectedIndex = -1
            SelectUser.ItemsSource = New List(Of String)()
        Catch ErrorMessage As Exception
            DebugPrint(ErrorMessage.ToString)
        End Try

    End Sub

    Private Sub Launch_Click(sender As Object, e As RoutedEventArgs)

        Try
            Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
            Dim inputFrame As Page = DirectCast(rootPage.InputFrame.Content, Page)
            Dim itemsource As New List(Of String)()
            Dim l As New List(Of Object)
            Reset1()
            Dim AuthenticationFailCheck As CheckBox = TryCast(outputFrame.FindName("AuthenticationFailCheck"), CheckBox)
            Dim WelcomeMessage As TextBox = TryCast(inputFrame.FindName("WelcomeMessage"), TextBox)
            Dim SelectUser As ComboBox = TryCast(inputFrame.FindName("SelectUser"), ComboBox)
            If AuthenticationFailCheck.IsChecked = True Then
                WelcomeMessage.Text = "Blocked"
            Else

                Try
                    Dim vault As New Windows.Security.Credentials.PasswordVault()
                    Dim creds As IReadOnlyList(Of PasswordCredential) = vault.FindAllByResource("Scenario 2")

                    For Each c In DirectCast(creds, IEnumerable(Of PasswordCredential))
                        itemsource.Insert(0, c.UserName)
                    Next


                    itemsource.Sort()
                Catch ErrorMessage As Exception
                    ' If there are no stored credentials, no list to populate
                    DebugPrint(ErrorMessage.ToString)
                End Try
                itemsource.Insert(0, "Add new user")
                itemsource.Insert(0, "")
                l.AddRange(itemsource)
                SelectUser.ItemsSource = l
                SelectUser.SelectedIndex = 0

                WelcomeMessage.Text = "Scenario is ready, please sign in"

            End If
        Catch ErrorMessage As Exception
            '
            ' Bad Parameter, Machine infor Unavailable errors are to be handled here.
            '
            DebugPrint(ErrorMessage.ToString)
        End Try
    End Sub

    Private Sub ChangeUser_Click(sender As Object, e As RoutedEventArgs)

        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim inputFrame As Page = DirectCast(rootPage.InputFrame.Content, Page)
        Try


            Dim vault As New Windows.Security.Credentials.PasswordVault()
            Dim creds As IReadOnlyList(Of PasswordCredential) = vault.FindAllByResource("Scenario 2")
            For Each c As PasswordCredential In creds
                Try
                    vault.Remove(c)
                Catch ErrorMessage As Exception
                    ' Stored credential was deleted
                    DebugPrint(ErrorMessage.ToString)
                End Try

            Next
        Catch ErrorMessage As Exception
            ' No stored credentials, so none to delete
            DebugPrint(ErrorMessage.ToString)
        End Try
        Reset1()
        Dim AuthenticationFailCheck As CheckBox = TryCast(outputFrame.FindName("AuthenticationFailCheck"), CheckBox)
        AuthenticationFailCheck.IsChecked = False
        Dim WelcomeMessage As TextBox = TryCast(inputFrame.FindName("WelcomeMessage"), TextBox)
        WelcomeMessage.Text = "User has been changed, please resign in with new credentials, choose save and launch scenario again"
    End Sub

    Private Sub Signin_Click(sender As Object, e As RoutedEventArgs)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim inputFrame As Page = DirectCast(rootPage.InputFrame.Content, Page)
        Dim InputUserNameValue As TextBox = TryCast(outputFrame.FindName("InputUserNameValue"), TextBox)
        Dim InputPasswordValue As PasswordBox = TryCast(outputFrame.FindName("InputPasswordValue"), PasswordBox)
        Dim WelcomeMessage As TextBox = TryCast(inputFrame.FindName("WelcomeMessage"), TextBox)
        Dim SaveCredCheck As CheckBox = TryCast(outputFrame.FindName("SaveCredCheck"), CheckBox)
        If InputUserNameValue.Text = "" OrElse InputPasswordValue.Password = "" Then
            Dim ErrorMessage As TextBox = TryCast(outputFrame.FindName("ErrorMessage"), TextBox)
            ErrorMessage.Text = "User name and password are not allowed to be empty, Please input user name and password"
        Else
            Try
                Dim vault As New Windows.Security.Credentials.PasswordVault()
                Dim c As New PasswordCredential("Scenario 2", InputUserNameValue.Text, InputPasswordValue.Password)
                If CBool(SaveCredCheck.IsChecked.Value) Then
                    vault.Add(c)
                End If

                WelcomeMessage.Text = "Welcome to " & c.Resource & ", " & c.UserName
            Catch ErrorMessage As Exception
                ' No stored credentials, so none to delete
                DebugPrint(ErrorMessage.ToString)
            End Try
        End If
        Reset1()

        Dim AuthenticationFailCheck As CheckBox = TryCast(outputFrame.FindName("AuthenticationFailCheck"), CheckBox)
        AuthenticationFailCheck.IsChecked = False
    End Sub

    Private Sub SelectUser_Click(sender As Object, e As SelectionChangedEventArgs)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim inputFrame As Page = DirectCast(rootPage.InputFrame.Content, Page)
        Dim InputUserNameValue As TextBox = TryCast(outputFrame.FindName("InputUserNameValue"), TextBox)
        Dim InputPasswordValue As PasswordBox = TryCast(outputFrame.FindName("InputPasswordValue"), PasswordBox)
        Dim SelectUser As ComboBox = TryCast(inputFrame.FindName("SelectUser"), ComboBox)

        Try
            If SelectUser.SelectedIndex > 1 Then
                InputUserNameValue.Text = SelectUser.SelectedItem.ToString

                Dim vault As New Windows.Security.Credentials.PasswordVault()
                Dim cred As PasswordCredential = vault.Retrieve("Scenario 2", DirectCast(SelectUser.SelectedItem, String))
                If cred.Password <> "" Then
                    InputPasswordValue.Password = cred.Password
                End If
            End If
        Catch ErrorMessage As Exception
            '
            ' Bad Parameter, Machine infor Unavailable errors are to be handled here.
            '
            DebugPrint(ErrorMessage.ToString)
        End Try
    End Sub

    Private Sub Reset1()

        Try
            Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
            Dim inputFrame As Page = DirectCast(rootPage.InputFrame.Content, Page)
            Dim InputUserNameValue As TextBox = TryCast(outputFrame.FindName("InputUserNameValue"), TextBox)
            InputUserNameValue.Text = ""
            Dim InputPasswordValue As PasswordBox = TryCast(outputFrame.FindName("InputPasswordValue"), PasswordBox)
            InputPasswordValue.Password = ""
            Dim ErrorMessage As TextBox = TryCast(outputFrame.FindName("ErrorMessage"), TextBox)
            ErrorMessage.Text = ""
            Dim WelcomeMessage As TextBox = TryCast(inputFrame.FindName("WelcomeMessage"), TextBox)
            Dim SaveCredCheck As CheckBox = TryCast(outputFrame.FindName("SaveCredCheck"), CheckBox)
            SaveCredCheck.IsChecked = False
            CleanCombobox()
        Catch ErrorMessage As Exception
            '
            ' Bad Parameter, Machine infor Unavailable errors are to be handled here.
            '
            DebugPrint(ErrorMessage.ToString)
        End Try
    End Sub

    Private Sub Reset_Click(sender As Object, e As RoutedEventArgs)
        Reset1()
        Try
            Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
            Dim inputFrame As Page = DirectCast(rootPage.InputFrame.Content, Page)
            Dim WelcomeMessage As TextBox = TryCast(inputFrame.FindName("WelcomeMessage"), TextBox)
            WelcomeMessage.Text = ""
            CleanCombobox()
        Catch ErrorMessage As Exception

            DebugPrint(ErrorMessage.ToString)
        End Try
    End Sub

End Class
