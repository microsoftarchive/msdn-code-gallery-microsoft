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
Imports Windows.Storage
Imports Windows.Storage.FileProperties
Imports Windows.Storage.Pickers
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

Partial Public NotInheritable Class Scenario3
    Inherits SDKTemplate.Common.LayoutAwarePage

    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler GetThumbnailButton.Click, AddressOf GetThumbnailButton_Click
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        rootPage.ResetOutput(ThumbnailImage, OutputTextBlock)
    End Sub

    Private Async Sub GetThumbnailButton_Click(sender As Object, e As RoutedEventArgs)
        rootPage.ResetOutput(ThumbnailImage, OutputTextBlock)

        If rootPage.EnsureUnsnapped Then
            ' Pick a document
            Dim openPicker As New FileOpenPicker
            For Each extension In FileExtensions.Document
                openPicker.FileTypeFilter.Add(extension)
            Next

            Dim file As StorageFile = Await openPicker.PickSingleFileAsync
            If file IsNot Nothing Then
                Const thumbnailMode__1 As ThumbnailMode = ThumbnailMode.DocumentsView
                Const size As UInteger = 100
                Using thumbnail As StorageItemThumbnail = Await file.GetThumbnailAsync(thumbnailMode__1, size)
                    If thumbnail IsNot Nothing Then
                        MainPage.DisplayResult(ThumbnailImage, OutputTextBlock, thumbnailMode__1.ToString, size, file, thumbnail, _
                            False)
                    Else
                        rootPage.NotifyUser(Errors.NoIcon, NotifyType.StatusMessage)
                    End If
                End Using
            Else
                rootPage.NotifyUser(Errors.Cancel, NotifyType.StatusMessage)
            End If
        End If
    End Sub
End Class
