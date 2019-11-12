' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Threading
Imports System.Threading.Tasks
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Display
Imports Windows.Networking.BackgroundTransfer
Imports Windows.Storage
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports Windows.Web
Imports SDKTemplate

<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")> _
    Partial Public NotInheritable Class ScenarioInput1
    Inherits Page
    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing
    Private outputField As TextBox = Nothing

    Private activeDownloads As List(Of DownloadOperation)
    Private cts As CancellationTokenSource

    Public Sub New()
        InitializeComponent()

        cts = New CancellationTokenSource()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

    Private Sub rootPage_OutputFrameLoaded(sender As Object, e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame.

        ' Get a pointer to the content within the OutputFrame.
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        outputField = TryCast(outputFrame.FindName("outputField"), TextBox)

        ' An application must enumerate downloads when it gets started to prevent stale downloads/uploads.
        ' Typically this can be done in the App class by overriding OnLaunched() and checking for 
        ' "args.Kind == ActivationKind.Launch" to detect an actual app launch.
        ' We do it here in the sample to keep the sample code consolidated.
        DiscoverActiveDownloads()
    End Sub

    ' Enumerate the downloads that were going on in the background while the app was closed.
    Private Async Sub DiscoverActiveDownloads()
        activeDownloads = New List(Of DownloadOperation)()

        Try
            Dim downloads As IReadOnlyList(Of DownloadOperation) = Await BackgroundDownloader.GetCurrentDownloadsAsync()
            Log("Loading background downloads: " & downloads.Count)

            For Each download As DownloadOperation In downloads
                Log(String.Format("Discovered background download: {0}, Status: {1}", download.Guid, download.Progress.Status))

                ' Attach progress and completion handlers.
                HandleDownloadAsync(download, False)
            Next
        Catch ex As Exception
            LogException("Discovery error", ex)
        End Try
    End Sub

    Private Async Sub StartDownload_Click(sender As Object, e As RoutedEventArgs)
        Try
            Dim source As New Uri(serverAddressField.Text.Trim())
            Dim destination As String = fileNameField.Text.Trim()

            If destination = "" Then
                Log("A local file name is required.")
                Return
            End If

            Dim destinationFile As StorageFile = Await KnownFolders.PicturesLibrary.CreateFileAsync(destination, CreationCollisionOption.GenerateUniqueName)

            Dim downloader As New BackgroundDownloader()
            Dim download As DownloadOperation = downloader.CreateDownload(source, destinationFile)

            Log(String.Format("Downloading {0} to {1}, {2}", source.AbsoluteUri, destinationFile.Name, download.Guid))

            ' Attach progress and completion handlers.
            HandleDownloadAsync(download, True)
        Catch ex As Exception
            LogException("Download Error", ex)
        End Try
    End Sub

    Private Sub PauseAll_Click(sender As Object, e As RoutedEventArgs)
        Try
            Log("Downloads: " & activeDownloads.Count)

            For Each download As DownloadOperation In activeDownloads
                If download.Progress.Status = BackgroundTransferStatus.Running Then
                    download.Pause()
                    Log("Paused: " & download.Guid.ToString)
                Else
                    Log(String.Format("Skipped: {0}, Status: {1}", download.Guid, download.Progress.Status))
                End If
            Next
        Catch ex As Exception
            LogException("Pause error", ex)
        End Try
    End Sub

    Private Sub ResumeAll_Click(sender As Object, e As RoutedEventArgs)
        Try
            Log("Downloads: " & activeDownloads.Count)

            For Each download As DownloadOperation In activeDownloads
                If download.Progress.Status = BackgroundTransferStatus.PausedByApplication OrElse download.Progress.Status = BackgroundTransferStatus.PausedCostedNetwork OrElse download.Progress.Status = BackgroundTransferStatus.PausedNoNetwork Then
                    download.[Resume]()
                    Log("Resumed: " & download.Guid.ToString)
                Else
                    Log(String.Format("Skipped: {0}, Status: {1}", download.Guid, download.Progress.Status))
                End If
            Next
        Catch ex As Exception
            LogException("Resume error", ex)
        End Try
    End Sub

    Private Sub CancelAll_Click(sender As Object, e As RoutedEventArgs)
        Log("Canceling Downloads: " & activeDownloads.Count)

        cts.Cancel()
        cts.Dispose()

        ' Re-create the CancellationTokenSource and activeDownloads for future downloads.
        cts = New CancellationTokenSource()
        activeDownloads = New List(Of DownloadOperation)()
    End Sub

    ' Note that this event is invoked on a background thread, so we cannot access the UI directly.
    Private Sub DownloadProgress(download As DownloadOperation)
        MarshalLog(String.Format("Progress: {0}, Status: {1}", download.Guid, download.Progress.Status))

        Dim percent As Double = 100
        If download.Progress.TotalBytesToReceive > 0 Then
            percent = download.Progress.BytesReceived * 100 / download.Progress.TotalBytesToReceive
        End If

        MarshalLog(String.Format(" - Transfered bytes: {0} of {1}, {2}%", download.Progress.BytesReceived, download.Progress.TotalBytesToReceive, percent))

        If download.Progress.HasRestarted Then
            MarshalLog(" - Download restarted")
        End If

        If download.Progress.HasResponseChanged Then
            ' We've received new response headers from the server.

            ' If you want to stream the response data this is a good time to start.
            ' download.GetResultStreamAt(0);
            MarshalLog(" - Response updated; Header count: " & download.GetResponseInformation().Headers.Count)
        End If
    End Sub

    Private Async Sub HandleDownloadAsync(download As DownloadOperation, start As Boolean)
        Try
            ' Store the download so we can pause/resume.
            activeDownloads.Add(download)

            Dim progressCallback As New Progress(Of DownloadOperation)(AddressOf DownloadProgress)

            If start Then
                ' Start the download and attach a progress handler.
                Await download.StartAsync().AsTask(cts.Token, progressCallback)
            Else
                ' The download was already running when the application started, re-attach the progress handler.
                Await download.AttachAsync().AsTask(cts.Token, progressCallback)
            End If

            Dim response As ResponseInformation = download.GetResponseInformation()
            Log(String.Format("Completed: {0}, Status Code: {1}", download.Guid, response.StatusCode))
        Catch generatedExceptionName As TaskCanceledException
            Log("Download cancelled.")
        Catch ex As Exception
            LogException("Error", ex)
        Finally
            activeDownloads.Remove(download)
        End Try
    End Sub

    Private Sub LogException(title As String, ex As Exception)
        Dim [error] As WebErrorStatus = BackgroundTransferError.GetStatus(ex.HResult)
        If [error] = WebErrorStatus.Unknown Then
            Log(title & ": " & ex.ToString)
        Else
            Log(title & ": " & [error])
        End If
    End Sub

    ' When operations happen on a background thread we have to marshal UI updates back to the UI thread.
    Private Async Sub MarshalLog(value As String)
        Await Me.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                        Log(value)
                                                                                    End Sub)
		End Sub

    Private Sub Log(message As String)
        outputField.Text += message + vbCr & vbLf
    End Sub
End Class
