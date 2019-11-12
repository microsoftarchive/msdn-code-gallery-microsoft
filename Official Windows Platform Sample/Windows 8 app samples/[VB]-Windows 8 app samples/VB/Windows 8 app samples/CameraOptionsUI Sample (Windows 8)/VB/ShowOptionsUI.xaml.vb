'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Devices.Enumeration
Imports Windows.Media
Imports Windows.Media.Capture
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ShowOptionsUI
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current
    Private mediaCaptureMgr As MediaCapture = Nothing
    Private previewStarted As Boolean
    Private uiDispatcher As CoreDispatcher = Window.Current.Dispatcher

    Public Sub New()
        Me.InitializeComponent()
        previewStarted = False
        ShowSettings.Visibility = Visibility.Collapsed
        ' Using the SoundLevelChanged event to determine when the app can stream from the webcam
        AddHandler Windows.Media.MediaControl.SoundLevelChanged, AddressOf MediaControl_SoundLevelChanged
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'StartPreview' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub StartPreview_Click(sender As Object, e As RoutedEventArgs)
        Try
            ' Check if the machine has a webcam
            Dim devices As DeviceInformationCollection
            devices = Await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture)
            If devices.Count > 0 Then
                rootPage.NotifyUser("", NotifyType.ErrorMessage)

                If mediaCaptureMgr Is Nothing Then
                    ' Using Windows.Media.Capture.MediaCapture APIs to stream from webcam
                    mediaCaptureMgr = New MediaCapture()
                    Await mediaCaptureMgr.InitializeAsync()

                    VideoStream.Source = mediaCaptureMgr
                    Await mediaCaptureMgr.StartPreviewAsync()
                    previewStarted = True

                    ShowSettings.Visibility = Visibility.Visible
                    StartPreview.IsEnabled = False
                End If
            Else
                rootPage.NotifyUser("A webcam is required to run this sample.", NotifyType.ErrorMessage)
            End If
        Catch ex As Exception
            mediaCaptureMgr = Nothing
            rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'ShowSettings' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ShowSettings_Click(sender As Object, e As RoutedEventArgs)
        Try
            If mediaCaptureMgr IsNot Nothing Then
                ' Using Windows.Media.Capture.CameraOptionsUI API to show webcam settings
                CameraOptionsUI.Show(mediaCaptureMgr)
            End If
        Catch ex As Exception
            rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    Private Async Sub MediaControl_SoundLevelChanged(sender As Object, e As Object)
        ' The callbacks for MediaControl_SoundLevelChanged and StopPreviewAsync may be invoked on threads other
        ' than the UI thread, so to ensure there's no synchronization issue, the Dispatcher is used here to
        ' ensure all operations run on the UI thread.
        Await uiDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                    Async Sub()
                                        If MediaControl.SoundLevel = SoundLevel.Muted Then
                                            If previewStarted Then
                                                Await mediaCaptureMgr.StopPreviewAsync()
                                                mediaCaptureMgr = Nothing
                                                previewStarted = False
                                                VideoStream.Source = Nothing
                                            End If
                                        Else
                                            If Not previewStarted Then
                                                ShowSettings.Visibility = Windows.UI.Xaml.Visibility.Collapsed
                                                StartPreview.IsEnabled = True
                                            End If
                                        End If
                                    End Sub)
    End Sub
End Class
