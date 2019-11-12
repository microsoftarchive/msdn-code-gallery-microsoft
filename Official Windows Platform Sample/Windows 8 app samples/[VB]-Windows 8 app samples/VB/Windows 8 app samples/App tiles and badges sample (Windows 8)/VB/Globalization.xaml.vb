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
Imports Windows.ApplicationModel.Resources.Core


''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Globalization
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

    Private Sub ViewCurrentResources_Click(sender As Object, e As RoutedEventArgs)
        Dim asls As String = ""
        ResourceManager.Current.DefaultContext.QualifierValues.TryGetValue("Language", asls)

        Dim scale As String = ""
        ResourceManager.Current.DefaultContext.QualifierValues.TryGetValue("Scale", scale)

        Dim contrast As String = ""
        ResourceManager.Current.DefaultContext.QualifierValues.TryGetValue("Contrast", contrast)

        OutputTextBlock.Text = "Your system is currently set to the following values: Application Language: " & asls & ", Scale: " & scale & ", Contrast: " & contrast & ". If using web images and AddImageQuery, the following query string would be appened to the URL: ?ms-lang=" & asls & "&ms-scale=" & scale & "&ms-contrast=" & contrast
    End Sub

    Private Sub SendTileNotificationWithQueryStrings_Click(sender As Object, e As RoutedEventArgs)
        Dim tileContent As ITileWideImageAndText01 = TileContentFactory.CreateTileWideImageAndText01()
        tileContent.TextCaptionWrap.Text = "This tile notification uses query strings for the image src."

        tileContent.Image.Src = ImageUrl.Text
        tileContent.Image.Alt = "Web image"

        ' enable AddImageQuery on the notification
        tileContent.AddImageQuery = True

        Dim squareContent As ITileSquareImage = TileContentFactory.CreateTileSquareImage()
        squareContent.Image.Src = ImageUrl.Text
        squareContent.Image.Alt = "Web image"

        ' include the square template.
        tileContent.SquareContent = squareContent

        ' send the notification to the app's application tile
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification())

        OutputTextBlock.Text = tileContent.GetContent()
    End Sub

    Private Sub SendTileNotification_Click(sender As Object, e As RoutedEventArgs)
        Dim scale As String = ""
        ResourceManager.Current.DefaultContext.QualifierValues.TryGetValue("Scale", scale)

        Dim tileContent As ITileWideSmallImageAndText03 = TileContentFactory.CreateTileWideSmallImageAndText03()
        tileContent.TextBodyWrap.Text = "graySquare.png in the xml is actually graySquare.scale-" & scale & ".png"
        tileContent.Image.Src = "ms-appx:///images/graySquare.png"
        tileContent.Image.Alt = "Gray square"

        Dim squareTileContent As ITileSquareImage = TileContentFactory.CreateTileSquareImage()
        squareTileContent.Image.Src = "ms-appx:///images/graySquare.png"
        squareTileContent.Image.Alt = "Gray square"
        tileContent.SquareContent = squareTileContent

        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification())

        OutputTextBlock.Text = tileContent.GetContent()
    End Sub

    Private Sub SendTileNotificationText_Click(sender As Object, e As RoutedEventArgs)
        Dim tileContent As ITileWideText03 = TileContentFactory.CreateTileWideText03()

        ' check out /en-US/resources.resw to understand where this string will come from
        tileContent.TextHeadingWrap.Text = "ms-resource:greeting"

        Dim squareTileContent As ITileSquareText04 = TileContentFactory.CreateTileSquareText04()
        squareTileContent.TextBodyWrap.Text = "ms-resource:greeting"
        tileContent.SquareContent = squareTileContent

        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification())

        OutputTextBlock.Text = tileContent.GetContent()
    End Sub
End Class
