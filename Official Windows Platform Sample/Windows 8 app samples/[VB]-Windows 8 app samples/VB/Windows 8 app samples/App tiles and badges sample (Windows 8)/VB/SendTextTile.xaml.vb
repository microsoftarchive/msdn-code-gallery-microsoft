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
Imports System.Xml
Imports System.Text
Imports NotificationsExtensions.TileContent

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class SendTextTile
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Sub UpdateTileWithText_Click(sender As Object, e As RoutedEventArgs)
        ' Note: This sample contains an additional project, NotificationsExtensions.
        ' NotificationsExtensions exposes an object model for creating notifications, but you can also modify the xml
        ' of the notification directly using TileUpdateManager.GetTemplateContent(TileTemplateType)

        ' create the wide template
        Dim tileContent As ITileWideText03 = TileContentFactory.CreateTileWideText03()
        tileContent.TextHeadingWrap.Text = "Hello World! My very own tile notification"

        ' Users can resize tiles to square or wide.
        ' Apps can choose to include only square assets (meaning the app's tile can never be wide), or
        ' include both wide and square assets (the user can resize the tile to square or wide).
        ' Apps cannot include only wide assets.

        ' Apps that support being wide should include square tile notifications since users
        ' determine the size of the tile.

        ' create the square template and attach it to the wide template
        Dim squareContent As ITileSquareText04 = TileContentFactory.CreateTileSquareText04()
        squareContent.TextBodyWrap.Text = "Hello World! My very own tile notification"
        tileContent.SquareContent = squareContent

        ' send the notification
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification())

        OutputTextBlock.Text = tileContent.GetContent()
    End Sub

    Private Sub ClearTile_Click(sender As Object, e As RoutedEventArgs)
        TileUpdateManager.CreateTileUpdaterForApplication().Clear()
        OutputTextBlock.Text = "Tile cleared"
    End Sub


    Private Function PrettyPrint(inputString As String) As String
        Dim settings As New XmlWriterSettings()
        settings.Indent = True
        settings.OmitXmlDeclaration = True
        settings.NewLineOnAttributes = True
        settings.ConformanceLevel = ConformanceLevel.Auto
        Dim outputString As New StringBuilder()
        Dim writer = XmlWriter.Create(outputString, settings)
        writer.WriteString(inputString)
        writer.Flush()
        Return outputString.ToString
    End Function
End Class
