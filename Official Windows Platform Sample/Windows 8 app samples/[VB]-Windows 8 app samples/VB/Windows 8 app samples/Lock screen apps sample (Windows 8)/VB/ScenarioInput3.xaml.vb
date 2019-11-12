' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports NotificationsExtensionsVB.BadgeContent
Imports NotificationsExtensionsVB.TileContent
Imports System
Imports Windows.Foundation
Imports Windows.UI.Notifications
Imports Windows.UI.Popups
Imports Windows.UI.StartScreen
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports NotificationsExtensionsVB

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

    Private Async Sub CreateBadgeTile_Click(sender As Object, e As RoutedEventArgs)
        If Not SecondaryTile.Exists(BADGE_TILE_ID) Then
            Dim secondTile As New SecondaryTile(BADGE_TILE_ID, "LockBadge", "LockScreen VB - Badge only", "BADGE_ARGS", TileOptions.ShowNameOnLogo, New Uri("ms-appx:///assets/squareTile-sdk.png"))
            secondTile.LockScreenBadgeLogo = New Uri("ms-appx:///assets/badgelogo.png")

            Dim isPinned As Boolean = Await secondTile.RequestCreateForSelectionAsync(GetElementRect(DirectCast(sender, FrameworkElement)), Placement.Above)
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

    Private Async Sub CreateBadgeAndTextTile_Click(sender As Object, e As RoutedEventArgs)
        If Not SecondaryTile.Exists(TEXT_TILE_ID) Then
            Dim secondTile As New SecondaryTile(TEXT_TILE_ID, "LockText", "LockScreen VB - Badge and tile text", "TEXT_ARGS", TileOptions.ShowNameOnLogo Or TileOptions.ShowNameOnWideLogo, New Uri("ms-appx:///assets/squareTile-sdk.png"), _
                New Uri("ms-appx:///assets/tile-sdk.png"))
            secondTile.LockScreenBadgeLogo = New Uri("ms-appx:///assets/badgelogo.png")
            secondTile.LockScreenDisplayBadgeAndTileText = True

            Dim isPinned As Boolean = Await secondTile.RequestCreateForSelectionAsync(GetElementRect(DirectCast(sender, FrameworkElement)), Placement.Above)
            If isPinned Then
                Dim tileContent As ITileWideText03 = TileContentFactory.CreateTileWideText03()
                tileContent.TextHeadingWrap.Text = "Text for the lock screen"
                tileContent.RequireSquareContent = False
                TileUpdateManager.CreateTileUpdaterForSecondaryTile(TEXT_TILE_ID).Update(tileContent.CreateNotification())
                rootPage.NotifyUser("Secondary tile created and updated. Go to PC settings to add it to the lock screen.", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("Tile not created.", NotifyType.ErrorMessage)

            End If
        Else
            rootPage.NotifyUser("Badge and text secondary tile already exists.", NotifyType.ErrorMessage)
        End If
    End Sub

    Private Function GetElementRect(element As FrameworkElement) As Rect
        Dim buttonTransform As GeneralTransform = element.TransformToVisual(Nothing)
        Dim point As Point = buttonTransform.TransformPoint(New Point())
        Return New Rect(point, New Size(element.ActualWidth, element.ActualHeight))
    End Function

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub
End Class
