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
Imports Windows.Storage.Pickers
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
Partial Public NotInheritable Class ScenarioInput2
    Inherits Page
    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing
    Private outputField As TextBox = Nothing

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

        ' An application must enumerate uploads when it gets started to prevent stale downloads/uploads.
        ' Typically this can be done in the App class by overriding OnLaunched() and checking for
        ' "args.Kind == ActivationKind.Launch" to detect an actual app launch.
        ' We do it here in the sample to keep the sample code consolidated.
        DiscoverActiveUploads()
    End Sub

    ' Enumerate the uploads that were going on in the background while the app was closed.
    Private Async Sub DiscoverActiveUploads()
        Try
            Dim uploads As IReadOnlyList(Of UploadOperation) = Await BackgroundUploader.GetCurrentUploadsAsync()
            Log("Loading background uploads: " & uploads.Count)

            For Each upload As UploadOperation In uploads
                Log(String.Format("Discovered background upload: {0}, Status: {1}", upload.Guid, upload.Progress.Status))

                ' Attach progress and completion handlers.
                HandleUploadAsync(upload, False)
            Next
        Catch ex As Exception
            LogException("Discovery error", ex)
        End Try
    End Sub

    Private Async Sub StartUpload_Click(sender As Object, e As RoutedEventArgs)
        Try
            Dim uri As New Uri(serverAddressField.Text.Trim())
            Dim picker As New FileOpenPicker()
            picker.FileTypeFilter.Add("*")
            Dim file As StorageFile = Await picker.PickSingleFileAsync()

            If file Is Nothing Then
                Log("No file selected")
                Return
            End If

            Dim uploader As New BackgroundUploader()
            uploader.SetRequestHeader("Filename", file.Name)

            Dim upload As UploadOperation = uploader.CreateUpload(uri, file)
            Log(String.Format("Uploading {0} to {1}, {2}", file.Name, uri.AbsoluteUri, upload.Guid))

            ' Attach progress and completion handlers.
            HandleUploadAsync(upload, True)
        Catch ex As Exception
            LogException("Upload Error", ex)
        End Try
    End Sub

    Private Async Sub StartMultipartUpload_Click(sender As Object, e As RoutedEventArgs)
        Try
            Dim uri As New Uri(serverAddressField.Text.Trim())
            Dim picker As New FileOpenPicker()
            picker.FileTypeFilter.Add("*")
            Dim files As IReadOnlyList(Of StorageFile) = Await picker.PickMultipleFilesAsync()

            If files.Count = 0 Then
                Log("No file selected")
                Return
            End If

            Dim parts As New List(Of BackgroundTransferContentPart)()

            For i As Integer = 0 To files.Count - 1
                Dim part As New BackgroundTransferContentPart("File" & i, files(i).Name)
                part.SetFile(files(i))
                parts.Add(part)
            Next

            Dim uploader As New BackgroundUploader()
            Dim upload As UploadOperation = Await uploader.CreateUploadAsync(uri, parts)

            Dim fileNames As String = files(0).Name
            For i As Integer = 1 To files.Count - 1
                fileNames += ", " & files(i).Name
            Next

            Log(String.Format("Uploading {0} to {1}, {2}", fileNames, uri.AbsoluteUri, upload.Guid))

            ' Attach progress and completion handlers.
            HandleUploadAsync(upload, True)
        Catch ex As Exception
            LogException("Multipart Upload Error", ex)
        End Try
    End Sub

    Private Sub CancelAll_Click(sender As Object, e As RoutedEventArgs)
        Log("Canceling all active uploads")

        cts.Cancel()
        cts.Dispose()

        ' Re-create the CancellationTokenSource and activeUploads for future uploads.
        cts = New CancellationTokenSource()
    End Sub

    ' Note that this event is invoked on a background thread, so we cannot access the UI directly.
    Private Sub UploadProgress(upload As UploadOperation)
        MarshalLog(String.Format("Progress: {0}, Status: {1}", upload.Guid, upload.Progress.Status))

        Dim percent As Double = 100
        If upload.Progress.TotalBytesToReceive > 0 Then
            percent = upload.Progress.BytesReceived * 100 / upload.Progress.TotalBytesToReceive
        End If

        MarshalLog(String.Format(" - Transfered bytes: {0} of {1}, {2}%", upload.Progress.BytesReceived, upload.Progress.TotalBytesToReceive, percent))

        If upload.Progress.HasRestarted Then
            MarshalLog(" - Upload restarted")
        End If

        If upload.Progress.HasResponseChanged Then
            ' We've received new response headers from the server.

            ' If you want to stream the response data this is a good time to start.
            ' upload.GetResultStreamAt(0);
            MarshalLog(" - Response updated; Header count: " & upload.GetResponseInformation().Headers.Count)
        End If
    End Sub

    Private Async Sub HandleUploadAsync(upload As UploadOperation, start As Boolean)
        Try
            Dim progressCallback As New Progress(Of UploadOperation)(AddressOf UploadProgress)
            If start Then
                ' Start the upload and attach a progress handler.
                Await upload.StartAsync().AsTask(cts.Token, progressCallback)
            Else
                ' The upload was already running when the application started, re-attach the progress handler.
                Await upload.AttachAsync().AsTask(cts.Token, progressCallback)
            End If

            Dim response As ResponseInformation = upload.GetResponseInformation()
            Log(String.Format("Completed: {0}, Status Code: {1}", upload.Guid, response.StatusCode))
        Catch generatedExceptionName As TaskCanceledException
            Log("Upload cancelled.")
        Catch ex As Exception
            LogException("Error", ex)
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
