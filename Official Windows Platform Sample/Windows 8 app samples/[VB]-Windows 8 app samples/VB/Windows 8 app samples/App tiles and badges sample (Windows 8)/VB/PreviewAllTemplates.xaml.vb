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
Imports Windows.Data.Xml.Dom
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.Storage.Pickers
Imports System.Collections.Generic
Imports Windows.Storage

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class PreviewAllTemplates
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        Branding.SelectedIndex = 0
        TemplateList.SelectedIndex = 10
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub


#Region "Scenario-Template Code"
    Private Async Sub ViewImages_Click(sender As Object, e As RoutedEventArgs)
        If Not AvailableImages.Text.Equals(String.Empty) Then
            AvailableImages.Text = String.Empty
            ViewImages.Content = "View local images"
        Else
            Dim output As String = "ms-appx:///images/bluewide.png " & vbLf & " ms-appx:///images/redWide.png " & vbLf & " ms-appx:///images/graySquare.png " & vbLf
            Dim files As IReadOnlyList(Of StorageFile) = Await Windows.Storage.ApplicationData.Current.LocalFolder.GetFilesAsync()
            For Each file As StorageFile In files
                If file.FileType.Equals(".png") OrElse file.FileType.Equals(".jpg") OrElse file.FileType.Equals(".jpeg") Then
                    output &= "ms-appdata:///local/" & file.Name & " " & vbLf
                End If
            Next
            ViewImages.Content = "Hide local images"
            AvailableImages.Text = output
        End If
    End Sub

    Private Async Sub CopyImages_Click(sender As Object, e As RoutedEventArgs)
        Dim picker As FileOpenPicker = New Windows.Storage.Pickers.FileOpenPicker()
        picker.ViewMode = PickerViewMode.Thumbnail
        picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary
        picker.FileTypeFilter.Add(".jpg")
        picker.FileTypeFilter.Add(".jpeg")
        picker.FileTypeFilter.Add(".png")
        picker.CommitButtonText = "Copy"
        Dim files As IReadOnlyList(Of StorageFile) = Await picker.PickMultipleFilesAsync()
        OutputTextBlock.Text = "Image(s) copied to application data local storage: " & vbLf
        For Each file As StorageFile In files
            Dim copyFile As StorageFile = Await file.CopyAsync(Windows.Storage.ApplicationData.Current.LocalFolder, file.Name, Windows.Storage.NameCollisionOption.GenerateUniqueName)
            OutputTextBlock.Text &= copyFile.Path & vbLf & " "
        Next
    End Sub

    Private Sub TemplateList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        Dim item As ComboBoxItem = TryCast(TemplateList.SelectedItem, ComboBoxItem)
        Dim templateName As String = If((item IsNot Nothing), item.Name, "TileSquareImage")
        Dim tileXml As XmlDocument = TileUpdateManager.GetTemplateContent(DirectCast(TemplateList.SelectedIndex, TileTemplateType))

        Dim tileTextAttributes As XmlNodeList = tileXml.GetElementsByTagName("text")
        For i As Integer = 0 To TextInputs.Children.Count - 1
            If i < tileTextAttributes.Length Then
                TextInputs.Children(i).Visibility = Windows.UI.Xaml.Visibility.Visible
            Else
                TextInputs.Children(i).Visibility = Windows.UI.Xaml.Visibility.Collapsed
            End If
        Next

        Dim tileImageAttributes As XmlNodeList = tileXml.GetElementsByTagName("image")
        For i As Integer = 0 To ImageInputs.Children.Count - 1
            If i < tileImageAttributes.Length Then
                ImageInputs.Children(i).Visibility = Windows.UI.Xaml.Visibility.Visible
            Else
                ImageInputs.Children(i).Visibility = Windows.UI.Xaml.Visibility.Collapsed
            End If
        Next

        Preview.Source = New BitmapImage(New Uri("ms-appx:///images/tiles/" & templateName & ".png"))
    End Sub
#End Region

    Private Sub ClearTile_Click(sender As Object, e As RoutedEventArgs)
        TileUpdateManager.CreateTileUpdaterForApplication().Clear()
        OutputTextBlock.Text = "Tile cleared"
    End Sub

    Private Sub UpdateTileNotification_Click(sender As Object, e As RoutedEventArgs)
        Dim item As ComboBoxItem = DirectCast(Branding.SelectedItem, ComboBoxItem)
        Dim branding__1 As String = If((item.Name.Equals("BrandName")), "Name", item.Name)

        UpdateTile(DirectCast(TemplateList.SelectedIndex, TileTemplateType), branding__1)
    End Sub

    Private Sub UpdateTile(templateType As TileTemplateType, branding As String)
        ' This example uses the GetTemplateContent method to get the notification as xml instead of NotificationExtensions            

        Dim tileXml As XmlDocument = TileUpdateManager.GetTemplateContent(templateType)
        '#Region "input handling"

        Dim textElements As XmlNodeList = tileXml.GetElementsByTagName("text")
        For i As Integer = 0 To textElements.Length - 1
            Dim tileText As String = String.Empty
            Dim panel As StackPanel = TryCast(TextInputs.Children(i), StackPanel)
            If panel IsNot Nothing Then
                Dim box As TextBox = TryCast(panel.Children(1), TextBox)
                If box IsNot Nothing Then
                    tileText = box.Text
                    If tileText.Equals("") Then
                        tileText = "Text field " & i
                    End If
                End If
            End If
            textElements.ElementAt(CUInt(i)).AppendChild(tileXml.CreateTextNode(tileText))
        Next

        Dim imageElements As XmlNodeList = tileXml.GetElementsByTagName("image")
        For i As Integer = 0 To imageElements.Length - 1
            Dim imageElement As XmlElement = DirectCast(imageElements.ElementAt(CUInt(i)), XmlElement)
            Dim imageSource As String = String.Empty
            Dim panel As StackPanel = TryCast(ImageInputs.Children(i), StackPanel)
            If panel IsNot Nothing Then
                Dim box As TextBox = TryCast(panel.Children(1), TextBox)
                If box IsNot Nothing Then
                    imageSource = box.Text
                    If imageSource.Equals(String.Empty) Then
                        imageSource = "ms-appx:///images/redWide.png"
                    End If
                End If
            End If
            imageElement.SetAttribute("src", imageSource)
        Next
        '#End Region

        ' Set branding, the logo and display name are declared in the manifest
        ' defaults to logo if omitted
        Dim bindingElement As XmlElement = DirectCast(tileXml.GetElementsByTagName("binding").ElementAt(0), XmlElement)
        bindingElement.SetAttribute("branding", branding)

        Dim lang__1 As String = Lang.Text
        ' this needs to be a BCP47 tag
        If Not String.IsNullOrEmpty(lang__1) Then
            ' specify the language of the text in the notification 
            ' this ensure the correct font is used to render the text
            Dim visualElement As XmlElement = DirectCast(tileXml.GetElementsByTagName("visual").ElementAt(0), XmlElement)
            visualElement.SetAttribute("lang", lang__1)
        End If

        Dim tile As New TileNotification(tileXml)
        TileUpdateManager.CreateTileUpdaterForApplication().Update(tile)

        OutputTextBlock.Text = tileXml.GetXml()
    End Sub
End Class
