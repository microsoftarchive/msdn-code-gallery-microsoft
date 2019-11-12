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
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current

    Public Sub New()
        Me.InitializeComponent()

        ' AddHandler Address.KeyUp, AddressOf Address_KeyUp
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Address.Text = "http://www.msn.com"
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Navigation' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub NavigateButton_Click(sender As Object, e As RoutedEventArgs)
        ProgressRing1.IsActive = True

        ' Provide an indication as to where we are trying to navigate to
        rootPage.NotifyUser(String.Format("Navigating to: {0}", Address.Text), NotifyType.StatusMessage)

        ' Hook the LoadCompleted event for the WebView to know when the URL is fully loaded
        AddHandler WebView1.LoadCompleted, AddressOf WebView1_LoadCompleted

        ' Attempt to navigate to the specified URL.  Notice that a malformed URL will raise a FormatException
        ' which we catch and let the user know that the URL is bad and to enter a new well-formed one.
        Try
            Dim targetUri As New Uri(Address.Text)
            WebView1.Navigate(targetUri)
        Catch myE As FormatException
            ' Bad address
            rootPage.NotifyUser(String.Format("Address is invalid, try again.  Details --> {0}", myE.Message), NotifyType.ErrorMessage)
        End Try
    End Sub

    ''' <summary>
    ''' The LoadCompleted handler allows us to know when the requested URL is navigated to (the document is completely
    ''' loaded into the WebView).
    ''' </summary>
    ''' <param name="sender">The initiator of the event (WebView1)</param>
    ''' <param name="e">The standard NavigationEventArgs</param>
    Private Sub WebView1_LoadCompleted(sender As Object, e As Windows.UI.Xaml.Navigation.NavigationEventArgs)
        WebView1.Visibility = Windows.UI.Xaml.Visibility.Visible
        BlockingRect.Visibility = Windows.UI.Xaml.Visibility.Collapsed
        ProgressRing1.IsActive = False

        ' Tell the user that the page has loaded
        rootPage.NotifyUser("Page loaded", NotifyType.StatusMessage)
    End Sub


    Private Sub Address_KeyUp(sender As Object, e As Object)
        'If e.Key = Windows.System.VirtualKey.Enter Then
        '    NavigateButton_Click(Me, New RoutedEventArgs())
        'End If
    End Sub

End Class
