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
Imports Windows.UI.Popups
Imports Windows.UI.StartScreen
Imports Windows.UI.Xaml.Media

Partial Public NotInheritable Class ScenarioInput3
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing
    Private Const BADGE_TILE_ID As String = "ST_BADGE"
    Private Const TEXT_TILE_ID As String = "ST_BADGE_AND_TEXT"

    Public Sub New()
        InitializeComponent()
        AddHandler CreateBadgeTile.Click, AddressOf CreateBadgeTile_Click
        AddHandler CreateBadgeAndTextTile.Click, AddressOf CreateBadgeAndTextTile_Click
    End Sub

    Private Async Sub CreateBadgeTile_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If Not SecondaryTile.Exists(BADGE_TILE_ID) Then
            Dim secondTile As New SecondaryTile(BADGE_TILE_ID, "LockScreen VB - Badge only", "BADGE_ARGS", New Uri("ms-appx:///images/squareTile-sdk.png"), TileSize.Square150x150)
            secondTile.LockScreenBadgeLogo = New Uri("ms-appx:///images/badgelogo-sdk.png")

            Dim isPinned As Boolean = Await secondTile.RequestCreateForSelectionAsync(GetElementRect(CType(sender, FrameworkElement)), Placement.Above)
            If isPinned Then
                Dim badgeContent As New BadgeNumericNotificationContent(2)
                BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(BADGE_TILE_ID).Update(badgeContent.CreateNotification())
                rootPage.NotifyUser("Secondary tile created and badge updated. Go to PC settings to add it to the lock screen.", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("Tile not created.", NotifyType.ErrorMessage)
            End If

        Else
            rootPage.NotifyUser("Badge secondary tile already exists.", NotifyType.ErrorMessage)
        End If
    End Sub

    Private Async Sub CreateBadgeAndTextTile_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If Not SecondaryTile.Exists(TEXT_TILE_ID) Then
            Dim secondTile As New SecondaryTile(TEXT_TILE_ID, "LockScreen VB - Badge and tile text", "TEXT_ARGS", New Uri("ms-appx:///images/squareTile-sdk.png"), TileSize.Wide310x150)
            secondTile.VisualElements.Wide310x150Logo = New Uri("ms-appx:///images/tile-sdk.png")
            secondTile.LockScreenBadgeLogo = New Uri("ms-appx:///images/badgelogo-sdk.png")
            secondTile.LockScreenDisplayBadgeAndTileText = True

            Dim isPinned As Boolean = Await secondTile.RequestCreateForSelectionAsync(GetElementRect(CType(sender, FrameworkElement)), Placement.Above)
            If isPinned Then
                Dim tileContent As ITileWide310x150Text03 = TileContentFactory.CreateTileWide310x150Text03()
                tileContent.TextHeadingWrap.Text = "Text for the lock screen"
                tileContent.RequireSquare150x150Content = False
                TileUpdateManager.CreateTileUpdaterForSecondaryTile(TEXT_TILE_ID).Update(tileContent.CreateNotification())
                rootPage.NotifyUser("Secondary tile created and updated. Go to PC settings to add it to the lock screen.", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("Tile not created.", NotifyType.ErrorMessage)
            End If

        Else
            rootPage.NotifyUser("Badge and text secondary tile already exists.", NotifyType.ErrorMessage)
        End If
    End Sub

    Private Function GetElementRect(ByVal element As FrameworkElement) As Rect
        Dim buttonTransform As GeneralTransform = element.TransformToVisual(Nothing)
        Dim point As Point = buttonTransform.TransformPoint(New Point())
        Return New Rect(point, New Size(element.ActualWidth, element.ActualHeight))
    End Function

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub
End Class
