'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports SDKTemplate
Imports Windows.Data.Xml.Dom
Imports Windows.UI.Notifications
Imports Windows.UI.StartScreen
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class SecondaryTileNotification
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private Appbar As AppBar

    Public Sub New()
        Me.InitializeComponent()
        Dim tileExists As Boolean = SecondaryTile.Exists(MainPage.dynamicTileId)
        SendBadgeNotification.IsEnabled = tileExists
        SendTileNotification.IsEnabled = tileExists
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Appbar = rootPage.BottomAppBar
        rootPage.BottomAppBar = Nothing
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be navigated out in a Frame
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        rootPage.BottomAppBar = AppBar
    End Sub


    ''' <summary>
    ''' This is the click handler for the 'Pin Tile' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub PinLiveTile_Click(sender As Object, e As RoutedEventArgs)
        Dim button As Button = TryCast(sender, Button)
        If button IsNot Nothing Then
            ' Prepare package images for use as the Tile Logo and small Logo in our tile to be pinned
            Dim logo As New Uri("ms-appx:///Assets/squareTile-sdk.png")
            Dim wideLogo As New Uri("ms-appx:///Assets/tile-sdk.png")

            ' During creation of secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
            ' These arguments should be meaningful to the application. In this sample, we'll pass in the date and time the secondary tile was pinned.
            Dim tileActivationArguments As String = MainPage.dynamicTileId + " WasPinnedAt=" + DateTime.Now.ToLocalTime().ToString

            ' Create a Secondary tile
            Dim secondaryTile As New SecondaryTile(MainPage.dynamicTileId, "A Live Secondary Tile", "Secondary Tile Sample Live Secondary Tile", tileActivationArguments, TileOptions.ShowNameOnLogo Or TileOptions.ShowNameOnWideLogo, logo, _
                wideLogo)

            ' Specify a foreground text value.
            ' The tile background color is inherited from the parent unless a separate value is specified.
            secondaryTile.ForegroundText = ForegroundText.Light

            ' OK, the tile is created and we can now attempt to pin the tile.
            ' Note that the status message is updated when the async operation to pin the tile completes.
            Dim isPinned As Boolean = Await secondaryTile.RequestCreateForSelectionAsync(MainPage.GetElementRect(DirectCast(sender, FrameworkElement)), Windows.UI.Popups.Placement.Right)

            If isPinned Then
                rootPage.NotifyUser("Secondary tile successfully pinned.", NotifyType.StatusMessage)
                SendTileNotification.IsEnabled = True
                SendBadgeNotification.IsEnabled = True
            Else
                rootPage.NotifyUser("Secondary tile not pinned.", NotifyType.ErrorMessage)
            End If
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Sending tile notification' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SendTileNotification_Click(sender As Object, e As RoutedEventArgs)
        Dim button As Button = TryCast(sender, Button)
        If button IsNot Nothing Then
            ' Get a XML DOM version of a specific template by using the getTemplateContent
            Dim tileXml As XmlDocument = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText04)

            ' Refer to template documentation to determine how many text fields a particular template has
            ' get the text attributes for this template and fill them in
            Dim tileTextElements As XmlNodeList = tileXml.GetElementsByTagName("text")
            tileTextElements.ElementAt(0).AppendChild(tileXml.CreateTextNode("Sent to a secondary tile!"))

            Dim squareTileXml As XmlDocument = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquareText04)
            Dim squareTileTextElements As XmlNodeList = squareTileXml.GetElementsByTagName("text")
            squareTileTextElements.ElementAt(0).AppendChild(squareTileXml.CreateTextNode("Sent to a secondary tile!"))

            ' Include the square template in the notification
            Dim subNode As IXmlNode = tileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").ElementAt(0), True)
            tileXml.GetElementsByTagName("visual").ElementAt(0).AppendChild(subNode)

            ' Create the notification from the XML
            Dim tileNotification As New TileNotification(tileXml)

            ' Send the notification to the secondary tile by creating a secondary tile updater
            TileUpdateManager.CreateTileUpdaterForSecondaryTile(MainPage.dynamicTileId).Update(tileNotification)
            rootPage.NotifyUser("Tile notification sent to " + MainPage.dynamicTileId, NotifyType.StatusMessage)
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Other' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub SendBadgeNotification_Click(sender As Object, e As RoutedEventArgs)
        Dim button As Button = TryCast(sender, Button)
        If button IsNot Nothing Then
            ' Get a XML DOM version of a specific badge template using getTemplateContent
            Dim badgeXml As XmlDocument = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber)
            Dim badgeElement As XmlElement = DirectCast(badgeXml.SelectSingleNode("/badge"), XmlElement)
            ' We are setting attribute value 6
            badgeElement.SetAttribute("value", "6")

            ' Create a badge notification from XML
            Dim badgeNotification As New BadgeNotification(badgeXml)

            ' Send the notification to the secondary tile
            BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(MainPage.dynamicTileId).Update(badgeNotification)
            rootPage.NotifyUser("Badge notification sent to " + MainPage.dynamicTileId, NotifyType.StatusMessage)
        End If
    End Sub

End Class
