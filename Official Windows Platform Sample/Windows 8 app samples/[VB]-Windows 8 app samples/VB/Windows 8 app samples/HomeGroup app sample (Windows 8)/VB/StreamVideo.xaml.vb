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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class StreamVideo
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    ''' <summary>
    ''' This is the click handler for the PickVideo button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub PickVideo_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            Dim picker As New Windows.Storage.Pickers.FileOpenPicker()
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.HomeGroup
            picker.FileTypeFilter.Clear()
            picker.FileTypeFilter.Add(".mp4")
            picker.FileTypeFilter.Add(".wmv")
            Dim file As Windows.Storage.StorageFile = Await picker.PickSingleFileAsync()

            If file IsNot Nothing Then
                rootPage.NotifyUser("Selected file '" & file.Path & "'", NotifyType.StatusMessage)

                Dim stream = Await file.OpenAsync(Windows.Storage.FileAccessMode.Read)
                VideoBox.SetSource(stream, file.ContentType)
                VideoBox.Stop()
                VideoBox.Play()
            Else
                rootPage.NotifyUser("Did not select a file.", NotifyType.ErrorMessage)
            End If
        End If
    End Sub
End Class
