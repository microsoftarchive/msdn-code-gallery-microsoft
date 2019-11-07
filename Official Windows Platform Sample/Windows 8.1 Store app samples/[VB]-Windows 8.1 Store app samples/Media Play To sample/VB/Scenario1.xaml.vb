'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Media.PlayTo
Imports Windows.Storage.Pickers
Imports Windows.Storage

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private playToManager As PlayToManager = Nothing
    Private dispatch As CoreDispatcher = Nothing

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        dispatch = Window.Current.CoreWindow.Dispatcher
        playToManager = playToManager.GetForCurrentView()
        AddHandler playToManager.SourceRequested, AddressOf playToManager_SourceRequested
    End Sub

    Private Sub playToManager_SourceRequested(ByVal sender As PlayToManager, ByVal args As PlayToSourceRequestedEventArgs)
        Dim deferral = args.SourceRequest.GetDeferral()
        Dim handler = dispatch.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                           args.SourceRequest.SetSource(VideoSource.PlayToSource)
                                                                           deferral.Complete()
                                                                       End Sub)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler playToManager.SourceRequested, AddressOf playToManager_SourceRequested
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'webContent' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub webContent_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            VideoSource.Source = New Uri("http://ie.microsoft.com/testdrive/Graphics/VideoFormatSupport/big_buck_bunny_trailer_480p_high.mp4")
            rootPage.NotifyUser("You are playing a web content", NotifyType.StatusMessage)
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'videoFile' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub videoFile_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            Dim filePicker As New FileOpenPicker()
            filePicker.SuggestedStartLocation = PickerLocationId.VideosLibrary
            filePicker.FileTypeFilter.Add(".mp4")
            filePicker.FileTypeFilter.Add(".wmv")
            filePicker.ViewMode = PickerViewMode.Thumbnail

            Dim localVideo As StorageFile = Await filePicker.PickSingleFileAsync()
            If localVideo IsNot Nothing Then
                Dim stream = Await localVideo.OpenAsync(FileAccessMode.Read)
                VideoSource.SetSource(stream, localVideo.ContentType)
                rootPage.NotifyUser("You are playing a local video file", NotifyType.StatusMessage)
            End If
        End If
    End Sub

    Private Sub playButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        VideoSource.Play()
    End Sub

    Private Sub pauseButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        VideoSource.Pause()
    End Sub

    Private Sub playToButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        playToManager.ShowPlayToUI()
    End Sub

End Class

