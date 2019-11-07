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
Partial Public NotInheritable Class CachedFileUpdater_Local
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Public Sub New()
        Me.InitializeComponent()
        AddHandler FileIsCurrentButton.Click, AddressOf FileIsCurrentButton_Click
        AddHandler ProvideUpdatedVersionButton.Click, AddressOf ProvideUpdatedVersionButton_Click
    End Sub

    Private Async Sub OutputFileAsync(ByVal file As StorageFile)
        Dim fileContent As String = Await FileIO.ReadTextAsync(file)
        OutputFileName.Text = String.Format("Received file: {0}", file.Name)
        OutputFileContent.Text = String.Format("File content:{0}{1}", System.Environment.NewLine, fileContent)
    End Sub

    Private Sub UpdateUI(ByVal uiStatus As UIStatus)
        If uiStatus = uiStatus.Complete Then
            FileIsCurrentButton.IsEnabled = False
            ProvideUpdatedVersionButton.IsEnabled = False
        End If
    End Sub

    Private Sub FileIsCurrentButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim fileUpdateRequest As FileUpdateRequest = CachedFileUpdaterPage.Current.fileUpdateRequest
        Dim fileUpdateRequestDeferral As FileUpdateRequestDeferral = CachedFileUpdaterPage.Current.fileUpdateRequestDeferral

        OutputFileAsync(fileUpdateRequest.File)

        fileUpdateRequest.Status = FileUpdateStatus.Complete
        fileUpdateRequestDeferral.Complete()

        UpdateUI(CachedFileUpdaterPage.Current.cachedFileUpdaterUI.UIStatus)
    End Sub

    Private Async Sub ProvideUpdatedVersionButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim fileUpdateRequest As FileUpdateRequest = CachedFileUpdaterPage.Current.fileUpdateRequest
        Dim fileUpdateRequestDeferral As FileUpdateRequestDeferral = CachedFileUpdaterPage.Current.fileUpdateRequestDeferral

        Await FileIO.AppendTextAsync(fileUpdateRequest.File, String.Format("{0}New content added @ {1}", System.Environment.NewLine, Date.Now.ToString()))
        OutputFileAsync(fileUpdateRequest.File)

        fileUpdateRequest.Status = FileUpdateStatus.Complete
        fileUpdateRequestDeferral.Complete()

        UpdateUI(CachedFileUpdaterPage.Current.cachedFileUpdaterUI.UIStatus)
    End Sub
End Class

