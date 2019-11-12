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
Imports System.Text
Imports Windows.Storage
Imports Windows.Storage.FileProperties
Imports Windows.Storage.Pickers
Imports Windows.Storage.Search
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

Partial Public NotInheritable Class Scenario5
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler GetThumbnailButton.Click, AddressOf GetThumbnailButton_Click
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        rootPage.ResetOutput(ThumbnailImage, OutputTextBlock, OutputDetails)
    End Sub

    Private Async Sub GetThumbnailButton_Click(sender As Object, e As RoutedEventArgs)
        rootPage.ResetOutput(ThumbnailImage, OutputTextBlock, OutputDetails)

        If rootPage.EnsureUnsnapped Then
            ' Pick a folder
            Dim folderPicker As New FolderPicker
            For Each extension In FileExtensions.Image
                folderPicker.FileTypeFilter.Add(extension)
            Next

            Dim folder As StorageFolder = Await folderPicker.PickSingleFolderAsync
            If folder IsNot Nothing Then
                Const monthShape As CommonFolderQuery = CommonFolderQuery.GroupByMonth
                ' Verify queries are supported because they are not supported in all picked locations.
                If folder.IsCommonFolderQuerySupported(monthShape) Then
                    ' Convert folder to file group and query for items
                    Dim months As IReadOnlyList(Of StorageFolder) = Await folder.CreateFolderQuery(monthShape).GetFoldersAsync

                    If months IsNot Nothing AndAlso months.Count > 0 Then
                        Const thumbnailMode__1 As ThumbnailMode = ThumbnailMode.PicturesView
                        Const size As UInteger = 200
                        Dim firstMonth As StorageFolder = months(0)
                        Using thumbnail As StorageItemThumbnail = Await firstMonth.GetThumbnailAsync(thumbnailMode__1, size)
                            If thumbnail IsNot Nothing Then
                                MainPage.DisplayResult(ThumbnailImage, OutputTextBlock, thumbnailMode__1.ToString, size, firstMonth, thumbnail, _
                                    True)

                                ' Also display the hierarchy of the file group to better visualize where the thumbnail comes from
                                Dim files As IReadOnlyList(Of StorageFile) = Await firstMonth.GetFilesAsync

                                If files IsNot Nothing Then
                                    Dim output As New StringBuilder(vbCrLf & "List of files in this group:" & vbCrLf & vbCrLf)
                                    For Each file In files
                                        output.AppendFormat("{0}" & vbCrLf, file.Name)
                                    Next
                                    OutputDetails.Text = output.ToString
                                End If
                            Else
                                rootPage.NotifyUser(Errors.NoImages, NotifyType.StatusMessage)
                            End If
                        End Using
                    Else
                        rootPage.NotifyUser(Errors.FileGroupEmpty, NotifyType.StatusMessage)
                    End If
                Else
                    rootPage.NotifyUser(Errors.FileGroupLocation, NotifyType.StatusMessage)
                End If
            Else
                rootPage.NotifyUser(Errors.Cancel, NotifyType.StatusMessage)
            End If
        End If
    End Sub
End Class
