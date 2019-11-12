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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario5
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
        AddHandler WebView5.LoadCompleted, AddressOf WebView5_LoadCompleted
        WebView5.Navigate(New Uri("http://kexp.org/Playlist"))
    End Sub

    Private Sub WebView5_LoadCompleted(sender As Object, e As NavigationEventArgs)
        Dim allowedUris As New List(Of Uri)()
        allowedUris.Add(New Uri("http://kexp.org"))
        WebView5.AllowedScriptNotifyUris = allowedUris
        AddHandler WebView5.ScriptNotify, AddressOf WebView5_ScriptNotify
        Dim args As String() = {"document.title;"}
        Dim foo As String = WebView5.InvokeScript("eval", args)
        rootPage.NotifyUser(String.Format("Title is: {0}", foo), NotifyType.StatusMessage)
    End Sub

    Private Sub WebView5_ScriptNotify(sender As Object, e As NotifyEventArgs)
        rootPage.NotifyUser(e.Value, NotifyType.StatusMessage)
    End Sub


    ''' <summary>
    ''' This is the click handler for the 'Default' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Default_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            rootPage.NotifyUser("You clicked the " & b.Name & " button", NotifyType.StatusMessage)
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Other' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Other_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            rootPage.NotifyUser("You clicked the " & b.Name & " button", NotifyType.StatusMessage)
        End If
    End Sub

End Class
