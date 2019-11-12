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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class GetDomainName
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
    ''' This is the click handler for the 'GetDomain' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub GetDNSDomain_Click(sender As Object, e As RoutedEventArgs)
        If Not Windows.System.UserProfile.UserInformation.NameAccessAllowed Then
            DomainResultText.Text = "Access to user information is disabled by the user or administrator"
        Else
            DomainResultText.Text = "Beginning asynchronous operation."
            Dim dns As String = Await Windows.System.UserProfile.UserInformation.GetDomainNameAsync()
            If String.IsNullOrEmpty(dns) Then
                ' NULL may be returned in any of the following circumstances:
                ' The information can not be retrieved from the directory
                ' The calling user is not a member of a domain
                ' The user or administrator has disabled the privacy setting
                DomainResultText.Text = "No DNS domain name returned for the current user."
            Else
                DomainResultText.Text = dns
            End If
        End If
    End Sub
End Class
