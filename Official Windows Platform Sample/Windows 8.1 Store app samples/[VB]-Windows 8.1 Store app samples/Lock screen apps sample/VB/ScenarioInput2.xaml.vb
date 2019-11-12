' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports NotificationsExtensions.BadgeContent
Imports NotificationsExtensions.TileContent
Imports Windows.UI.Notifications

Partial Public NotInheritable Class ScenarioInput2
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub ClearBadge_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        BadgeUpdateManager.CreateBadgeUpdaterForApplication().Clear()
        rootPage.NotifyUser("Badge notification cleared", NotifyType.StatusMessage)
    End Sub

    Private Sub SendBadge_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim badgeContent As New BadgeNumericNotificationContent(6)
        BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badgeContent.CreateNotification())
        rootPage.NotifyUser("Badge notification sent", NotifyType.StatusMessage)
    End Sub

    Private Sub SendBadgeWithStringManipulation_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim badgeXmlString As String = "<badge value='6'/>"
        Dim badgeDOM As New Windows.Data.Xml.Dom.XmlDocument()
        badgeDOM.LoadXml(badgeXmlString)
        Dim badge As New BadgeNotification(badgeDOM)
        BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(badge)
        rootPage.NotifyUser("Badge notification sent", NotifyType.StatusMessage)
    End Sub

    Private Sub SendTile_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim tileContent As ITileWide310x150SmallImageAndText03 = TileContentFactory.CreateTileWide310x150SmallImageAndText03()
        tileContent.TextBodyWrap.Text = "This tile notification has an image, but it won't be displayed on the lock screen"
        tileContent.Image.Src = "ms-appx:///images/tile-sdk.png"
        tileContent.RequireSquare150x150Content = False
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification())

        rootPage.NotifyUser("Tile notification sent", NotifyType.StatusMessage)
    End Sub

    Private Sub SendTileWithStringManipulation_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim tileXmlString As String = "<tile>" & "<visual version='2'>" & "<binding template='TileWide310x150SmallImageAndText03' fallback='TileWideSmallImageAndText03'>" & "<image id='1' src='ms-appx:///images/tile-sdk.png'/>" & "<text id='1'>This tile notification has an image, but it won't be displayed on the lock screen</text>" & "</binding>" & "</visual>" & "</tile>"

        Dim tileDOM As New Windows.Data.Xml.Dom.XmlDocument()
        tileDOM.LoadXml(tileXmlString)
        Dim tile As New TileNotification(tileDOM)
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tile)
        rootPage.NotifyUser("Tile notification sent", NotifyType.StatusMessage)
    End Sub

    Private Sub ClearTile_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        TileUpdateManager.CreateTileUpdaterForApplication().Clear()
        rootPage.NotifyUser("Tile notification cleared", NotifyType.StatusMessage)
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub
End Class
