'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Storage
Imports Windows.Storage.FileProperties
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Media.Imaging

Namespace Global.SDKTemplate
    Friend Class FileExtensions
        Public Shared ReadOnly Document() As String = New String() {".doc", ".xls", ".ppt", ".docx", ".xlsx", ".pptx", _
            ".pdf", ".txt", ".rtf"}
        Public Shared ReadOnly Image() As String = New String() {".jpg", ".png", ".bmp", ".gif", ".tif"}
        Public Shared ReadOnly Music() As String = New String() {".mp3", ".wma", ".m4a", ".aac"}
    End Class

    Friend Class Errors
        Public Shared ReadOnly NoExifThumbnail As String = "No result (no EXIF thumbnail or cached thumbnail available for fast retrieval)"
        Public Shared ReadOnly NoThumbnail As String = "No result (no thumbnail could be obtained from the selected file)"
        Public Shared ReadOnly NoAlbumArt As String = "No result (no album art available for this song)"
        Public Shared ReadOnly NoIcon As String = "No result (no icon available for this document type)"
        Public Shared ReadOnly NoImages As String = "No result (no thumbnail could be obtained from the selected folder - make sure that the folder contains images)"
        Public Shared ReadOnly FileGroupEmpty As String = "No result (unexpected error: retrieved file group was null)"
        Public Shared ReadOnly FileGroupLocation As String = "File groups are only available for Library locations, please select a folder from one of your Libraries"
        Public Shared ReadOnly Cancel As String = "No result (operation cancelled, no item selected)"
    End Class

    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "File Thumbnails Sample"

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Display a thumbnail for a picture", .ClassType = GetType(Global.FileThumbnails.Scenario1)},
                                                           New Scenario With {.Title = "Display album art for a song", .ClassType = GetType(Global.FileThumbnails.Scenario2)},
                                                           New Scenario With {.Title = "Display an icon for a document", .ClassType = GetType(Global.FileThumbnails.Scenario3)},
                                                           New Scenario With {.Title = "Display a thumbnail for a folder", .ClassType = GetType(Global.FileThumbnails.Scenario4)},
                                                           New Scenario With {.Title = "Display a thumbnail for a file group", .ClassType = GetType(Global.FileThumbnails.Scenario5)}
                                                          }

        Private _placeholder As BitmapImage
        Private ReadOnly Property PlaceHolderImage As BitmapImage
            Get
                If _placeholder Is Nothing Then
                    _placeholder = New BitmapImage(New Uri(Me.BaseUri, "Assets/placeholder-sdk.png"))
                End If
                Return _placeholder
            End Get
        End Property

        Friend Function EnsureUnsnapped() As Boolean
            ' FilePicker APIs will not work if the application is in a snapped state.
            ' If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
            Dim unsnapped As Boolean = ((ApplicationView.Value <> ApplicationViewState.Snapped) OrElse ApplicationView.TryUnsnap)
            If Not unsnapped Then
                NotifyUser("Cannot unsnap the sample.", NotifyType.StatusMessage)
            End If

            Return unsnapped
        End Function

        Public Shared Sub DisplayResult(image As Image, textBlock As TextBlock, thumbnailModeName As String, size As UInteger, item As IStorageItem, thumbnail As StorageItemThumbnail, _
            isGroup As Boolean)
            ' ErrorMessage.Visibility = Visibility.Collapsed
            Dim bitmapImage As New BitmapImage

            bitmapImage.SetSource(thumbnail)
            image.Source = bitmapImage

            textBlock.Text = String.Format("ThumbnailMode.{0}" & vbLf & "{1} used: {2}" & vbLf & "Requested size: {3}" & vbCrLf & "Returned size: {4}x{5}", thumbnailModeName, If(isGroup, "Group", If(item.IsOfType(StorageItemTypes.File), "File", "Folder")), item.Name, size, thumbnail.OriginalWidth,
                thumbnail.OriginalHeight)
        End Sub

        Public Sub ResetOutput(image As Image, output As TextBlock, Optional outputDetails As TextBlock = Nothing)
            image.Source = PlaceHolderImage
            NotifyUser("", NotifyType.ErrorMessage)
            NotifyUser("", NotifyType.StatusMessage)
            output.Text = ""
            If outputDetails IsNot Nothing Then
                outputDetails.Text = ""
            End If
        End Sub
    End Class

    Public Class Scenario
        Public Property Title As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = value
            End Set
        End Property
        Private m_Title As String

        Public Property ClassType As Type
            Get
                Return m_ClassType
            End Get
            Set(value As Type)
                m_ClassType = value
            End Set
        End Property
        Private m_ClassType As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace

