'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports SDKTemplate
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Windows.Storage
Imports Windows.Storage.BulkAccess
Imports Windows.Storage.FileProperties
Imports Windows.Storage.Search
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

Partial Public NotInheritable Class Scenario3
    Inherits SDKTemplate.Common.LayoutAwarePage
    Public Sub New()
        Me.InitializeComponent()
        AddHandler GetFilesButton.Click, AddressOf GetFilesButton_Click
    End Sub

    Const CopyrightProperty As String = "System.Copyright"
    Const ColorSpaceProperty As String = "System.Image.ColorSpace"

    Private Function GetPropertyDisplayValue(prop As Object) As String
        Dim value As String
        If prop Is Nothing OrElse String.IsNullOrEmpty(prop.ToString) Then
            value = "none"
        Else
            value = prop.ToString
        End If

        Return value
    End Function

    Private Async Sub GetFilesButton_Click(sender As Object, e As RoutedEventArgs)
        ' Reset output.
        OutputPanel.Children.Clear()

        ' Set up file type filter.
        Dim fileTypeFilter As New List(Of String)()
        fileTypeFilter.Add(".jpg")
        fileTypeFilter.Add(".png")
        fileTypeFilter.Add(".bmp")
        fileTypeFilter.Add(".gif")

        ' Create query options.
        Dim queryOptions = New QueryOptions(CommonFileQuery.OrderByName, fileTypeFilter)

        ' Set up property prefetch - use the PropertyPrefetchOptions for top-level properties
        ' and a list for additional properties.
        Dim propertyNames As New List(Of String)()
        propertyNames.Add(CopyrightProperty)
        propertyNames.Add(ColorSpaceProperty)
        queryOptions.SetPropertyPrefetch(PropertyPrefetchOptions.ImageProperties, propertyNames)

        ' Set up thumbnail prefetch if needed, e.g. when creating a picture gallery view.
        '
        '            const uint requestedSize = 190
        '            const ThumbnailMode thumbnailMode = ThumbnailMode.PicturesView
        '            const ThumbnailOptions thumbnailOptions = ThumbnailOptions.UseCurrentScale
        '            queryOptions.SetThumbnailPrefetch(thumbnailMode, requestedSize, thumbnailOptions)
        '            


        ' Set up the query and retrieve files.
        Dim query = KnownFolders.PicturesLibrary.CreateFileQueryWithOptions(queryOptions)
        Dim fileList As IReadOnlyList(Of StorageFile) = Await query.GetFilesAsync()
        For Each file As StorageFile In fileList
            OutputPanel.Children.Add(CreateHeaderTextBlock(file.Name))

            ' GetImagePropertiesAsync will return synchronously when prefetching has been able to
            ' retrieve the properties in advance.
            Dim properties = Await file.Properties.GetImagePropertiesAsync()
            OutputPanel.Children.Add(CreateLineItemTextBlock("Dimensions: " & properties.Width & "x" & properties.Height))

            ' Similarly, extra properties are retrieved asynchronously but may
            ' return immediately when prefetching has fulfilled its task.
            Dim extraProperties As IDictionary(Of String, Object) = Await file.Properties.RetrievePropertiesAsync(propertyNames)
            Dim propValue = extraProperties(CopyrightProperty)
            OutputPanel.Children.Add(CreateLineItemTextBlock("Copyright: " & GetPropertyDisplayValue(propValue)))
            propValue = extraProperties(ColorSpaceProperty)

            ' Thumbnails can also be retrieved and used.
            ' Dim thumbnail = await file.GetThumbnailAsync(thumbnailMode, requestedSize, thumbnailOptions)
            OutputPanel.Children.Add(CreateLineItemTextBlock("Color space: " & GetPropertyDisplayValue(propValue)))
        Next
    End Sub

    Private Function CreateHeaderTextBlock(contents As String) As TextBlock
        Dim textBlock As New TextBlock()
        textBlock.Text = contents
        textBlock.Style = DirectCast(Application.Current.Resources("H2Style"), Style)
        textBlock.TextWrapping = TextWrapping.Wrap
        Return textBlock
    End Function

    Private Function CreateLineItemTextBlock(contents As String) As TextBlock
        Dim textBlock As New TextBlock()
        textBlock.Text = contents
        textBlock.Style = DirectCast(Application.Current.Resources("BasicTextStyle"), Style)
        textBlock.TextWrapping = TextWrapping.Wrap
        Dim margin As Thickness = textBlock.Margin
        margin.Left = 20
        textBlock.Margin = margin
        Return textBlock
    End Function
End Class
