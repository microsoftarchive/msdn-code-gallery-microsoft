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
Imports Windows.Media.PlayTo
Imports Windows.Foundation
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Core

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private receiver As Windows.Media.PlayTo.PlayToReceiver = Nothing
    Private IsReceiverStarted As Boolean = False
    Private IsSeeking As Boolean = False

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
    ''' This is the click handler for the 'Default' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub startPlayToReceiver(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            Try
                InitialisePlayToReceiver()
                startDMRButton.IsEnabled = False
                stopDMRButton.IsEnabled = True
                Await receiver.StartAsync()
                IsReceiverStarted = True
                rootPage.NotifyUser("PlayToReceiver started", NotifyType.StatusMessage)
            Catch ecp As Exception
                startDMRButton.IsEnabled = True
                stopDMRButton.IsEnabled = False
                rootPage.NotifyUser("PlayToReceiver start failed, Error " + ecp.Message, NotifyType.ErrorMessage)
            End Try
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Other' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub stopPlayToReceiver(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            Try
                startDMRButton.IsEnabled = True
                stopDMRButton.IsEnabled = False
                Await receiver.StopAsync()
                IsReceiverStarted = False
                rootPage.NotifyUser("PlayToReceiver stopped", NotifyType.StatusMessage)
            Catch ecp As Exception
                IsReceiverStarted = True
                startDMRButton.IsEnabled = False
                stopDMRButton.IsEnabled = True
                rootPage.NotifyUser("PlayToReceiver stop failed, Error " + ecp.Message, NotifyType.ErrorMessage)
            End Try
        End If
    End Sub

    Private Sub InitialisePlayToReceiver()
        Try
            If receiver Is Nothing Then
                receiver = New Windows.Media.PlayTo.PlayToReceiver()
                AddHandler receiver.PlayRequested, AddressOf receiver_PlayRequested
                AddHandler receiver.PauseRequested, AddressOf receiver_PauseRequested
                AddHandler receiver.StopRequested, AddressOf receiver_StopRequested
                AddHandler receiver.TimeUpdateRequested, AddressOf receiver_TimeUpdateRequested
                AddHandler receiver.CurrentTimeChangeRequested, AddressOf receiver_CurrentTimeChangeRequested
                AddHandler receiver.SourceChangeRequested, AddressOf receiver_SourceChangeRequested
                AddHandler receiver.MuteChangeRequested, AddressOf receiver_MuteChangeRequested
                AddHandler receiver.PlaybackRateChangeRequested, AddressOf receiver_PlaybackRateChangeRequested
                AddHandler receiver.VolumeChangeRequested, AddressOf receiver_VolumeChangeRequested

                receiver.SupportsAudio = True
                receiver.SupportsVideo = True
                receiver.SupportsImage = False

                receiver.FriendlyName = "PlayToReceiver  Sample"
            End If
        Catch e As Exception
            startDMRButton.IsEnabled = False
            stopDMRButton.IsEnabled = True
            rootPage.NotifyUser("PlayToReceiver initialization failed, Error: " + e.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    Private Async Sub receiver_PlayRequested(recv As Windows.Media.PlayTo.PlayToReceiver, args As Object)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                   If dmrVideo IsNot Nothing Then
                                                                       dmrVideo.Play()
                                                                   End If
                                                               End Sub)
    End Sub

    Private Async Sub receiver_PauseRequested(recv As Windows.Media.PlayTo.PlayToReceiver, args As Object)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                   If dmrVideo IsNot Nothing Then
                                                                       dmrVideo.Pause()
                                                                   End If
                                                               End Sub)
    End Sub

    Private Async Sub receiver_StopRequested(recv As Windows.Media.PlayTo.PlayToReceiver, args As Object)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                   If dmrVideo IsNot Nothing Then
                                                                       dmrVideo.[Stop]()
                                                                   End If
                                                               End Sub)
    End Sub

    Private Async Sub receiver_TimeUpdateRequested(recv As Windows.Media.PlayTo.PlayToReceiver, args As Object)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                   If IsReceiverStarted Then
                                                                       If dmrVideo IsNot Nothing Then
                                                                           receiver.NotifyTimeUpdate(dmrVideo.Position)
                                                                       End If
                                                                   End If
                                                               End Sub)
    End Sub

    Private Async Sub receiver_CurrentTimeChangeRequested(recv As Windows.Media.PlayTo.PlayToReceiver, args As CurrentTimeChangeRequestedEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                   If IsReceiverStarted Then
                                                                       If dmrVideo IsNot Nothing Then
                                                                           dmrVideo.Position = args.Time
                                                                           receiver.NotifySeeking()
                                                                           IsSeeking = True
                                                                       End If
                                                                   End If
                                                               End Sub)
    End Sub

    Private Async Sub receiver_SourceChangeRequested(recv As Windows.Media.PlayTo.PlayToReceiver, args As SourceChangeRequestedEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                   If dmrVideo IsNot Nothing AndAlso args.Stream IsNot Nothing Then
                                                                       dmrVideo.SetSource(args.Stream, args.Stream.ContentType)
                                                                   End If
                                                               End Sub)
    End Sub

    Private Async Sub receiver_MuteChangeRequested(recv As Windows.Media.PlayTo.PlayToReceiver, args As MuteChangeRequestedEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                   If dmrVideo IsNot Nothing Then
                                                                       dmrVideo.IsMuted = args.Mute
                                                                   End If
                                                               End Sub)
    End Sub

    Private Async Sub receiver_PlaybackRateChangeRequested(recv As Windows.Media.PlayTo.PlayToReceiver, args As PlaybackRateChangeRequestedEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                   If dmrVideo IsNot Nothing Then
                                                                       dmrVideo.PlaybackRate = args.Rate
                                                                   End If
                                                               End Sub)
    End Sub

    Private Async Sub receiver_VolumeChangeRequested(recv As Windows.Media.PlayTo.PlayToReceiver, args As VolumeChangeRequestedEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                   If dmrVideo IsNot Nothing Then
                                                                       dmrVideo.Volume = args.Volume
                                                                   End If
                                                               End Sub)
    End Sub

    Private Sub dmrVideo_VolumeChanged(sender As Object, e As RoutedEventArgs)
        If IsReceiverStarted Then
            receiver.NotifyVolumeChange(dmrVideo.Volume, dmrVideo.IsMuted)
        End If
    End Sub

    Private Sub dmrVideo_RateChanged(sender As Object, e As Windows.UI.Xaml.Media.RateChangedRoutedEventArgs)
        If IsReceiverStarted Then
            receiver.NotifyRateChange(dmrVideo.PlaybackRate)
        End If

    End Sub

    Private Sub dmrVideo_MediaOpened(sender As Object, e As RoutedEventArgs)
        If IsReceiverStarted Then
            receiver.NotifyLoadedMetadata()
            receiver.NotifyDurationChange(dmrVideo.NaturalDuration.TimeSpan)
        End If
    End Sub

    Private Sub dmrVideo_CurrentStateChanged(sender As Object, e As RoutedEventArgs)
        If IsReceiverStarted Then
            Select Case dmrVideo.CurrentState
                Case MediaElementState.Playing
                    receiver.NotifyPlaying()
                    Exit Select
                Case MediaElementState.Paused
                    receiver.NotifyPaused()
                    Exit Select
                Case MediaElementState.Opening
                    receiver.NotifyStopped()
                    Exit Select
                Case MediaElementState.Closed
                    Exit Select
                Case MediaElementState.Buffering
                    Exit Select
                Case Else
                    Exit Select
            End Select
        End If
    End Sub

    Private Sub dmrVideo_MediaEnded(sender As Object, e As RoutedEventArgs)
        If IsReceiverStarted Then
            receiver.NotifyEnded()
            If dmrVideo IsNot Nothing Then
                dmrVideo.[Stop]()
            End If
        End If
    End Sub

    Private Sub dmrVideo_MediaFailed(sender As Object, e As ExceptionRoutedEventArgs)
        If IsReceiverStarted Then
            receiver.NotifyError()
        End If
    End Sub

    Private Sub dmrVideo_SeekCompleted(sender As Object, e As RoutedEventArgs)
        If IsReceiverStarted Then
            If IsSeeking Then
                receiver.NotifySeeking()
            End If
            receiver.NotifySeeked()
            IsSeeking = False
        End If
    End Sub

End Class
