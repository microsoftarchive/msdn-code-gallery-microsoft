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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class FileSavePicker_FailToSave
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    Private fileSavePickerUI As FileSavePickerUI = FileSavePickerPage.Current.fileSavePickerUI
    Private dispatch As CoreDispatcher = Window.Current.Dispatcher

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Async Sub OnTargetFileRequested(ByVal sender As FileSavePickerUI, ByVal e As TargetFileRequestedEventArgs)
        ' This scenario demonstrates how the app can go about handling the TargetFileRequested event on the UI thread, from
        ' which the app can manipulate the UI, show error dialogs, etc.

        ' Requesting a deferral allows the app to return from this event handler and complete the request at a later time.
        ' In this case, the deferral is required as the app intends on handling the TargetFileRequested event on the UI thread.
        ' Note that the deferral can be requested more than once but calling Complete on the deferral a single time will complete
        ' original TargetFileRequested event.
        Dim deferral = e.Request.GetDeferral()

        Await dispatch.RunAsync(CoreDispatcherPriority.Normal, Async Sub()
                                                                   ' This method will be called on the app's UI thread, which allows for actions like manipulating
                                                                   ' the UI or showing error dialogs
                                                                   ' Display a dialog indicating to the user that a corrective action needs to occur
                                                                   ' Set the targetFile property to null and complete the deferral to indicate failure once the user has closed the
                                                                   ' dialog.  This will allow the user to take any neccessary corrective action and click the Save button once again.
                                                                   Dim errorDialog = New Windows.UI.Popups.MessageDialog("If the app needs the user to correct a problem before the app can save the file, the app can use a message like this to tell the user about the problem and how to correct it.")
                                                                   Await errorDialog.ShowAsync()
                                                                   e.Request.TargetFile = Nothing
                                                                   deferral.Complete()
                                                               End Sub)
    End Sub

    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        AddHandler fileSavePickerUI.TargetFileRequested, AddressOf OnTargetFileRequested
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler fileSavePickerUI.TargetFileRequested, AddressOf OnTargetFileRequested
    End Sub
End Class

