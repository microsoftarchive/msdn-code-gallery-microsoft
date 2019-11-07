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
Partial Public NotInheritable Class SendWebImageTile
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

    Private Sub UpdateTileWithWebImage_Click(sender As Object, e As RoutedEventArgs)
        ' Note: This sample contains an additional project, NotificationsExtensions.
        ' NotificationsExtensions exposes an object model for creating notifications, but you can also modify the xml
        ' of the notification directly using TileUpdateManager.GetTemplateContent(TileTemplateType)

        ' Create notification content based on a visual template.
        Dim tileContent As ITileWideImageAndText01 = TileContentFactory.CreateTileWideImageAndText01()

        tileContent.TextCaptionWrap.Text = "This tile notification uses web images."

        ' !Important!
        ' The Internet (Client) capability must be checked in the manifest in the Capabilities tab
        ' to display web images in tiles (either the http:// or https:// protocols)

        tileContent.Image.Src = ImageUrl.Text
        tileContent.Image.Alt = "Web image"

        ' Users can resize tiles to square or wide.
        ' Apps can choose to include only square assets (meaning the app's tile can never be wide), or
        ' include both wide and square assets (the user can resize the tile to square or wide).
        ' Apps cannot include only wide assets.

        ' Apps that support being wide should include square tile notifications since users
        ' determine the size of the tile.

        ' Create square notification content based on a visual template.
        Dim squareContent As ITileSquareImage = TileContentFactory.CreateTileSquareImage()

        squareContent.Image.Src = ImageUrl.Text
        squareContent.Image.Alt = "Web image"

        ' include the square template.
        tileContent.SquareContent = squareContent

        ' send the notification to the app's application tile
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification())

        OutputTextBlock.Text = tileContent.GetContent()
    End Sub

    Private Sub ClearTile_Click(sender As Object, e As RoutedEventArgs)
        TileUpdateManager.CreateTileUpdaterForApplication().Clear()
        OutputTextBlock.Text = "Tile cleared"
    End Sub
End Class
