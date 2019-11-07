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
Imports Windows.ApplicationModel.DataTransfer

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario7
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private dataTransferManager As DataTransferManager = Nothing
    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        AddHandler WebView7.LoadCompleted, AddressOf WebView7_LoadCompleted
        WebView7.Navigate(New Uri("http://www.wsj.com"))
    End Sub

    Private Sub WebView7_LoadCompleted(sender As Object, e As NavigationEventArgs)
        WebView7.Visibility = Windows.UI.Xaml.Visibility.Visible
        BlockingRect.Visibility = Windows.UI.Xaml.Visibility.Collapsed
        ProgressRing1.IsActive = False
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Share Content' button.
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Share_Click(sender As Object, e As RoutedEventArgs)
        dataTransferManager = dataTransferManager.GetForCurrentView()
        AddHandler dataTransferManager.DataRequested, AddressOf dataTransferManager_DataRequested
        dataTransferManager.ShowShareUI()
    End Sub

    Private Sub dataTransferManager_DataRequested(sender As DataTransferManager, args As DataRequestedEventArgs)
        Dim request As DataRequest = args.Request
        Dim p As DataPackage = WebView7.DataTransferPackage

        If p.GetView().Contains(StandardDataFormats.Text) Then
            p.Properties.Title = "WebView Sharing Excerpt"
            p.Properties.Description = "This is a snippet from the content hosted in the WebView control"
            request.Data = p
        Else
            request.FailWithDisplayText("Nothing to share")
        End If
        RemoveHandler dataTransferManager.DataRequested, AddressOf dataTransferManager_DataRequested
    End Sub
End Class
