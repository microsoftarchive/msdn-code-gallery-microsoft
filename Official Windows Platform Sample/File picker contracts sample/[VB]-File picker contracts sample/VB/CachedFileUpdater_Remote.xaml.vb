'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Storage.Provider

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class CachedFileUpdater_Remote
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Public Sub New()
        Me.InitializeComponent()
        AddHandler OverwriteButton.Click, AddressOf OverwriteButton_Click
        AddHandler RenameButton.Click, AddressOf RenameButton_Click
    End Sub

    Private Async Sub OutputFileAsync(ByVal file As StorageFile)
        Dim fileContent As String = Await FileIO.ReadTextAsync(file)
        OutputFileName.Text = String.Format("File Name: {0}", file.Name)
        OutputFileContent.Text = String.Format("File Content:{0}{1}", System.Environment.NewLine, fileContent)
    End Sub

    Private Sub UpdateUI(ByVal uiStatus As UIStatus)
        If uiStatus = uiStatus.Complete Then
            OverwriteButton.IsEnabled = False
            RenameButton.IsEnabled = False
        End If
    End Sub

    Private Sub OverwriteButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim fileUpdateRequest As FileUpdateRequest = CachedFileUpdaterPage.Current.fileUpdateRequest
        Dim fileUpdateRequestDeferral As FileUpdateRequestDeferral = CachedFileUpdaterPage.Current.fileUpdateRequestDeferral

        ' update the remote version of file...
        ' Printing the file content
        Me.OutputFileAsync(fileUpdateRequest.File)

        fileUpdateRequest.Status = FileUpdateStatus.Complete
        fileUpdateRequestDeferral.Complete()

        UpdateUI(CachedFileUpdaterPage.Current.cachedFileUpdaterUI.UIStatus)
    End Sub

    Private Async Sub RenameButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim fileUpdateRequest As FileUpdateRequest = CachedFileUpdaterPage.Current.fileUpdateRequest
        Dim fileUpdateRequestDeferral As FileUpdateRequestDeferral = CachedFileUpdaterPage.Current.fileUpdateRequestDeferral

        Dim file As StorageFile = Await fileUpdateRequest.File.CopyAsync(ApplicationData.Current.LocalFolder, fileUpdateRequest.File.Name, NameCollisionOption.GenerateUniqueName)
        CachedFileUpdater.SetUpdateInformation(file, "CachedFile", ReadActivationMode.NotNeeded, WriteActivationMode.AfterWrite, CachedFileOptions.RequireUpdateOnAccess)
        fileUpdateRequest.UpdateLocalFile(file)

        Me.OutputFileAsync(file)

        fileUpdateRequest.Status = FileUpdateStatus.CompleteAndRenamed
        fileUpdateRequestDeferral.Complete()

        UpdateUI(CachedFileUpdaterPage.Current.cachedFileUpdaterUI.UIStatus)
    End Sub
End Class

