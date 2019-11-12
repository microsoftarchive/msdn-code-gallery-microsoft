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

Partial Public NotInheritable Class Scenario1
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
            ' Pick a photo
            Dim openPicker As New FileOpenPicker
            For Each extension In FileExtensions.Image
                openPicker.FileTypeFilter.Add(extension)
            Next

            Dim file As StorageFile = Await openPicker.PickSingleFileAsync
            If file IsNot Nothing Then
                Dim thumbnailModeName As String = DirectCast(ModeComboBox.SelectedItem, ComboBoxItem).Name
                Dim thumbnailMode As ThumbnailMode = DirectCast(System.Enum.Parse(GetType(ThumbnailMode), thumbnailModeName), ThumbnailMode)

                Dim fastThumbnail As Boolean = FastThumbnailCheckBox.IsChecked.Value
                Dim thumbnailOptions__1 As ThumbnailOptions = ThumbnailOptions.UseCurrentScale
                If fastThumbnail Then
                    thumbnailOptions__1 = thumbnailOptions__1 Or ThumbnailOptions.ReturnOnlyIfCached
                End If

                Const size As UInteger = 200
                Using thumbnail As StorageItemThumbnail = Await file.GetThumbnailAsync(thumbnailMode, size, thumbnailOptions__1)
                    If thumbnail IsNot Nothing Then
                        MainPage.DisplayResult(ThumbnailImage, OutputTextBlock, thumbnailModeName, size, file, thumbnail, _
                            False)
                    ElseIf fastThumbnail Then
                        rootPage.NotifyUser(Errors.NoExifThumbnail, NotifyType.StatusMessage)
                    Else
                        rootPage.NotifyUser(Errors.NoThumbnail, NotifyType.StatusMessage)
                    End If
                End Using
            Else
                rootPage.NotifyUser(Errors.Cancel, NotifyType.StatusMessage)
            End If
        End If
    End Sub
End Class
