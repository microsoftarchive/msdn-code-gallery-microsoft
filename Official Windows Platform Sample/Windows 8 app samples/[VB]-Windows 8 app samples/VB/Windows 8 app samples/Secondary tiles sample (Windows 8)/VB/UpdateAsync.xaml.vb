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
Partial Public NotInheritable Class UpdateAsync
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private Appbar As AppBar

    Public Sub New()
        Me.InitializeComponent()
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
    ''' This is the click handler for the 'Update logo async' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub UpdateDefaultLogo_Click(sender As Object, e As RoutedEventArgs)
        Dim button As Button = TryCast(sender, Button)
        If button IsNot Nothing Then
            If Windows.UI.StartScreen.SecondaryTile.Exists(MainPage.logoSecondaryTileId) Then
                ' Add the properties we want to update (logo in this example)
                Dim secondaryTile As New SecondaryTile(MainPage.logoSecondaryTileId)
                secondaryTile.Logo = New Uri("ms-appx:///Assets/squareTileLogoUpdate-sdk.png")
                Dim isUpdated As Boolean = Await secondaryTile.UpdateAsync()

                If isUpdated Then
                    rootPage.NotifyUser("Secondary tile logo updated.", NotifyType.StatusMessage)
                Else
                    rootPage.NotifyUser("Secondary tile logo not updated.", NotifyType.ErrorMessage)
                End If
            Else
                rootPage.NotifyUser("Please pin a secondary tile using scenario 1 before updating the Logo.", NotifyType.ErrorMessage)
            End If
        End If
    End Sub
End Class
