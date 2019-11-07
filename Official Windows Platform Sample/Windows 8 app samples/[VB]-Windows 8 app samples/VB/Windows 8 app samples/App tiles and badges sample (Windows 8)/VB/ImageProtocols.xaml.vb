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
Imports Windows.Storage.Pickers
Imports Windows.Storage
Imports System.Threading.Tasks
Imports NotificationsExtensions.TileContent

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ImageProtocols
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private imageRelativePath As String = String.Empty
    'used for copying an image to localstorage
    Public Sub New()
        Me.InitializeComponent()
        ProtocolList.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    Private Async Sub PickImage_Click(sender As Object, e As RoutedEventArgs)
        Await CopyImageToLocalFolderAsync()
    End Sub

    Private Async Function CopyImageToLocalFolderAsync() As Task
        Dim picker As FileOpenPicker = New Windows.Storage.Pickers.FileOpenPicker()
        picker.ViewMode = PickerViewMode.Thumbnail
        picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary
        picker.FileTypeFilter.Add(".jpg")
        picker.FileTypeFilter.Add(".jpeg")
        picker.FileTypeFilter.Add(".png")
        picker.CommitButtonText = "Copy"
        Dim file As StorageFile = Await picker.PickSingleFileAsync()
        Dim newFile As StorageFile = Await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync(file.Name, Windows.Storage.CreationCollisionOption.GenerateUniqueName)
        Await file.CopyAndReplaceAsync(newFile)
        Me.imageRelativePath = newFile.Path.Substring(newFile.Path.LastIndexOf("\") + 1)
        OutputTextBlock.Text = "Image copied to application data local storage: " & newFile.Path
    End Function

    Private Sub ProtocolList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        LocalFolder.Visibility = Visibility.Collapsed
        http.Visibility = Visibility.Collapsed

        If ProtocolList.SelectedItem Is appdata Then
            LocalFolder.Visibility = Visibility.Visible
        ElseIf ProtocolList.SelectedItem Is HTTPStack Then
            HTTPStack.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub SendTileNotification_Click(sender As Object, e As RoutedEventArgs)
        Dim tileContent As IWideTileNotificationContent = Nothing
        If ProtocolList.SelectedItem Is package Then
            'using the ms-appx:/// protocol
            Dim wideContent As ITileWideImageAndText01 = TileContentFactory.CreateTileWideImageAndText01()

            wideContent.RequireSquareContent = False
            wideContent.TextCaptionWrap.Text = "The image is in the appx package"
            wideContent.Image.Src = "ms-appx:///images/redWide.png"
            wideContent.Image.Alt = "Red image"

            tileContent = wideContent
        ElseIf ProtocolList.SelectedItem = appdata Then
            'using the appdata:///local/ protocol
            Dim wideContent As ITileWideImage = TileContentFactory.CreateTileWideImage()

            wideContent.RequireSquareContent = False
            wideContent.Image.Src = "ms-appdata:///local/" & Me.imageRelativePath
            wideContent.Image.Alt = "App data"

            tileContent = wideContent
        ElseIf ProtocolList.SelectedItem = http Then
            'using http:// protocol
            ' Important - The Internet (Client) capability must be checked in the manifest in the Capabilities tab
            Dim wideContent As ITileWidePeekImageCollection04 = TileContentFactory.CreateTileWidePeekImageCollection04()

            wideContent.RequireSquareContent = False
            wideContent.BaseUri = HTTPBaseURI.Text
            wideContent.TextBodyWrap.Text = "The base URI is " & HTTPBaseURI.Text
            wideContent.ImageMain.Src = HTTPImage1.Text
            wideContent.ImageSmallColumn1Row1.Src = HTTPImage2.Text
            wideContent.ImageSmallColumn1Row2.Src = HTTPImage3.Text
            wideContent.ImageSmallColumn2Row1.Src = HTTPImage4.Text
            wideContent.ImageSmallColumn2Row2.Src = HTTPImage5.Text

            tileContent = wideContent
        End If

        tileContent.RequireSquareContent = False
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tileContent.CreateNotification())

        OutputTextBlock.Text = tileContent.GetContent()
    End Sub

    Private Sub ProtocolList_SelectionChanged_1(sender As Object, e As SelectionChangedEventArgs)

    End Sub
End Class
