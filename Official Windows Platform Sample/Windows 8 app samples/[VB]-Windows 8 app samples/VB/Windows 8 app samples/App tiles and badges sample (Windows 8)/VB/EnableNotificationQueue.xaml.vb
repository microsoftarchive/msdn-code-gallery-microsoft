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
Partial Public NotInheritable Class EnableNotificationQueue
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

    Private Sub ClearTile_Click(sender As Object, e As RoutedEventArgs)
        TileUpdateManager.CreateTileUpdaterForApplication().Clear()
        OutputTextBlock.Text = "Tile cleared"
    End Sub

    Private Sub UpdateTile_Click(sender As Object, e As RoutedEventArgs)
        Dim tileContent As ITileWideText03 = TileContentFactory.CreateTileWideText03()
        tileContent.TextHeadingWrap.Text = TextContent.Text

        Dim squareTileContent As ITileSquareText04 = TileContentFactory.CreateTileSquareText04()
        squareTileContent.TextBodyWrap.Text = TextContent.Text
        tileContent.SquareContent = squareTileContent

        Dim tileNotification As TileNotification = tileContent.CreateNotification()

        Dim tag As String = "TestTag01"
        If Not Id.Text.Equals(String.Empty) Then
            tag = Id.Text
        End If
        ' set the tag on the notification
        tileNotification.Tag = tag
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification)

        OutputTextBlock.Text = "Tile notification sent. It is tagged with " & tag
    End Sub

    Private Sub EnableNotificationCycling_Click(sender As Object, e As RoutedEventArgs)
        TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(True)
        OutputTextBlock.Text = "Notification cycling enabled"
    End Sub

    Private Sub DisableNotificationCycling_Click(sender As Object, e As RoutedEventArgs)
        TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(False)
        OutputTextBlock.Text = "Notification cycling disabled"
    End Sub
End Class
