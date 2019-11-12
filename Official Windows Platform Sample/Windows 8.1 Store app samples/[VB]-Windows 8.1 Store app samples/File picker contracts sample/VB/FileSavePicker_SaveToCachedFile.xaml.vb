'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Storage.Pickers.Provider
Imports Windows.Storage.Provider

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class FileSavePicker_SaveToCachedFile
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private fileSavePickerUI As FileSavePickerUI = FileSavePickerPage.Current.fileSavePickerUI

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Async Sub OnTargetFileRequested(ByVal sender As FileSavePickerUI, ByVal e As TargetFileRequestedEventArgs)
        ' This scenario demonstrates how to handle the TargetFileRequested event on the background thread on which it was raised

        ' Requesting a deferral allows the app to call another asynchronous method and complete the request at a later time
        Dim deferral = e.Request.GetDeferral()

        Dim file As StorageFile = Await ApplicationData.Current.LocalFolder.CreateFileAsync(sender.FileName, CreationCollisionOption.ReplaceExisting)
        CachedFileUpdater.SetUpdateInformation(file, "CachedFile", ReadActivationMode.NotNeeded, WriteActivationMode.AfterWrite, CachedFileOptions.RequireUpdateOnAccess)
        e.Request.TargetFile = file

        ' Complete the deferral to let the Picker know the request is finished
        deferral.Complete()
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        AddHandler fileSavePickerUI.TargetFileRequested, AddressOf OnTargetFileRequested
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler fileSavePickerUI.TargetFileRequested, AddressOf OnTargetFileRequested
    End Sub
End Class

