' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports NotificationsExtensionsVB.BadgeContent
Imports NotificationsExtensionsVB.TileContent
Imports Windows.UI.Notifications
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate

Partial Public NotInheritable Class ScenarioInput2
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
        AddHandler SendBadge.Click, AddressOf SendBadge_Click
        AddHandler ClearBadge.Click, AddressOf ClearBadge_Click
        AddHandler SendTile.Click, AddressOf SendTile_Click

        AddHandler ClearTile.Click, AddressOf ClearTile_Click
    End Sub

    Private Sub ClearBadge_Click(sender As Object, e As RoutedEventArgs)
        BadgeUpdateManager.CreateBadgeUpdaterForApplication.Clear()
        rootPage.NotifyUser("Badge notification cleared", NotifyType.StatusMessage)
    End Sub

    Private Sub SendBadge_Click(sender As Object, e As RoutedEventArgs)
        Dim badgeContent As New NotificationsExtensionsVB.BadgeNumericNotificationContent(6)
        BadgeUpdateManager.CreateBadgeUpdaterForApplication.Update(badgeContent.CreateNotification)
        rootPage.NotifyUser("Badge notification sent", NotifyType.StatusMessage)
    End Sub

    Private Sub SendTile_Click(sender As Object, e As RoutedEventArgs)
        Dim tileContent As ITileWideSmallImageAndText03 = TileContentFactory.CreateTileWideSmallImageAndText03
        tileContent.TextBodyWrap.Text = "This tile notification has an image, but it won't be displayed on the lock screen"
        tileContent.Image.Src = "ms-appx:///Assets/tile-sdk.png"
        tileContent.RequireSquareContent = False
        TileUpdateManager.CreateTileUpdaterForApplication.Update(tileContent.CreateNotification)

        rootPage.NotifyUser("Tile notification sent", NotifyType.StatusMessage)
    End Sub

    Private Sub ClearTile_Click(sender As Object, e As RoutedEventArgs)
        TileUpdateManager.CreateTileUpdaterForApplication.Clear()
        rootPage.NotifyUser("Tile notification cleared", NotifyType.StatusMessage)
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub
End Class
