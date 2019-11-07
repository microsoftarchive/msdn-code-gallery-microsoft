'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System
Imports Windows.System.UserProfile

Partial Public NotInheritable Class GetUserFirstLastName
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
    End Sub

    Private Async Sub GetFirstNameButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim firstName As String = Await UserInformation.GetFirstNameAsync()
        If String.IsNullOrEmpty(firstName) Then
            rootPage.NotifyUser("No First Name was returned", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("First Name = " & firstName, NotifyType.StatusMessage)
        End If
    End Sub

    Private Async Sub GetLastNameButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim lastName As String = Await UserInformation.GetLastNameAsync()
        If String.IsNullOrEmpty(lastName) Then
            rootPage.NotifyUser("No Last Name was returned", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("Last Name = " & lastName, NotifyType.StatusMessage)
        End If
    End Sub
End Class
