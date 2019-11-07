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
Imports Windows.UI.Notifications
Imports NotificationsExtensions.TileContent

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class NotificationExpiration
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

    Private Sub UpdateTileExpiring_Click(sender As Object, e As RoutedEventArgs)
        Dim seconds As Integer
        If Not Int32.TryParse(Time.Text, seconds) Then
            seconds = 10
        End If

        Dim tileContent As ITileWideText04 = TileContentFactory.CreateTileWideText04()
        tileContent.TextBodyWrap.Text = "This notification will expire at " & DateTimeOffset.UtcNow.AddSeconds(seconds).ToString

        Dim squareTileContent As ITileSquareText04 = TileContentFactory.CreateTileSquareText04()
        squareTileContent.TextBodyWrap.Text = "This notification will expire at " & DateTimeOffset.UtcNow.AddSeconds(seconds).ToString
        tileContent.SquareContent = squareTileContent

        Dim tileNotification As TileNotification = tileContent.CreateNotification()

        ' set the expirationTime
        tileNotification.ExpirationTime = DateTimeOffset.UtcNow.AddSeconds(seconds)
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification)

        OutputTextBlock.Text = "Tile notification sent. It will expire in " & seconds & " seconds."
    End Sub
End Class
