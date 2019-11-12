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
Imports Windows.UI.StartScreen
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class PinFromAppbar
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private pinToAppBar As Button
    Private rightPanel As StackPanel

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Init()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be navigated out in a Frame
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        If rightPanel IsNot Nothing Then
            RemoveHandler rootPage.BottomAppBar.Opened, AddressOf BottomAppBar_Opened
            RemoveHandler pinToAppBar.Click, AddressOf pinToAppBar_Click
            rightPanel.Children.Remove(pinToAppBar)
            rightPanel = Nothing
        End If
    End Sub

    ' This toggles the Pin and unpin button in the app bar
    Private Sub ToggleAppBarButton(showPinButton As Boolean)
        If pinToAppBar IsNot Nothing Then
            pinToAppBar.Style = If((showPinButton), TryCast(Me.Resources("PinAppBarButtonStyle"), Style), TryCast(Me.Resources("UnpinAppBarButtonStyle"), Style))
        End If
    End Sub

    Private Sub Init()
        rootPage.NotifyUser("To show the bar swipe up from the bottom of the screen, right-click, or press Windows Logo + z. To dismiss the bar, swipe, right-click, or press Windows Logo + z again.", NotifyType.StatusMessage)
        rootPage.BottomAppBar.IsOpen = True
        rightPanel = TryCast(rootPage.FindName("RightPanel"), StackPanel)
        rootPage.BottomAppBar.IsOpen = False

        If rightPanel IsNot Nothing Then
            pinToAppBar = New Button()
            ToggleAppBarButton(Not SecondaryTile.Exists(MainPage.appbarTileId))
            ' Set up the Click handler for the new button
            AddHandler pinToAppBar.Click, AddressOf pinToAppBar_Click
            AddHandler rootPage.BottomAppBar.Opened, AddressOf BottomAppBar_Opened
            rightPanel.Children.Add(pinToAppBar)
        End If
    End Sub

    Private Async Sub pinToAppBar_Click(sender As Object, e As RoutedEventArgs)
        rootPage.BottomAppBar.IsSticky = True
        ' Let us first verify if we need to pin or unpin
        If SecondaryTile.Exists(MainPage.appbarTileId) Then
            ' First prepare the tile to be unpinned
            Dim secondaryTile__1 As New SecondaryTile(MainPage.appbarTileId)
            ' Now make the delete request.
            Dim isUnpinned As Boolean = Await secondaryTile__1.RequestDeleteForSelectionAsync(MainPage.GetElementRect(DirectCast(sender, FrameworkElement)), Windows.UI.Popups.Placement.Above)
            If isUnpinned Then
                rootPage.NotifyUser(MainPage.appbarTileId & " unpinned.", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser(MainPage.appbarTileId & " not unpinned.", NotifyType.ErrorMessage)
            End If

            ToggleAppBarButton(isUnpinned)
        Else

            ' Prepare package images for use as the Tile Logo in our tile to be pinned
            Dim logo As New Uri("ms-appx:///Assets/squareTile-sdk.png")

            ' During creation of secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
            ' These arguments should be meaningful to the application. In this sample, we'll pass in the date and time the secondary tile was pinned.
            Dim tileActivationArguments As String = MainPage.appbarTileId & " WasPinnedAt=" & DateTime.Now.ToLocalTime().ToString()

            ' Create a 1x1 Secondary tile
            Dim secondaryTile__1 As New SecondaryTile(MainPage.appbarTileId, "Secondary tile pinned via AppBar", "SDK Sample Secondary Tile pinned from AppBar", tileActivationArguments, TileOptions.ShowNameOnLogo, logo)

            ' Specify a foreground text value.
            ' The tile background color is inherited from the parent unless a separate value is specified.
            secondaryTile__1.ForegroundText = ForegroundText.Dark

            ' OK, the tile is created and we can now attempt to pin the tile.
            ' Note that the status message is updated when the async operation to pin the tile completes.
            Dim isPinned As Boolean = Await secondaryTile__1.RequestCreateForSelectionAsync(MainPage.GetElementRect(DirectCast(sender, FrameworkElement)), Windows.UI.Popups.Placement.Above)
            If isPinned Then
                rootPage.NotifyUser(MainPage.appbarTileId & " successfully pinned.", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser(MainPage.appbarTileId & " not pinned.", NotifyType.ErrorMessage)
            End If

            ToggleAppBarButton(Not isPinned)
        End If
        rootPage.BottomAppBar.IsSticky = False
    End Sub

    Private Sub BottomAppBar_Opened(sender As Object, e As Object)
        ToggleAppBarButton(Not SecondaryTile.Exists(MainPage.appbarTileId))
    End Sub
End Class
