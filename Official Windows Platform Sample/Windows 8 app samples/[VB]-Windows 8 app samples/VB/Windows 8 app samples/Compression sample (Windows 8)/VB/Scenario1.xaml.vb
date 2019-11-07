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
Imports Windows.UI.ViewManagement
Imports Windows.Storage
Imports Windows.Storage.Streams
Imports Windows.Storage.Pickers
Imports Windows.Storage.Compression
Imports SDKTemplate
Imports System

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
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
    ''' This is the main scenario worker.
    ''' </summary>
    ''' <param name="Algorithm">
    ''' Comression algorithm to use. If no value is provided compressor will be created using
    ''' Compressor(IInputStream) constructor, otherwise extended version will be used:
    ''' Compressor(IInputStream, CompressAlgorithm, uint)
    ''' </param>
    Private Async Sub DoScenario(Algorithm As System.Nullable(Of CompressAlgorithm))
        Try
            Progress.Text = ""

            ''' This scenario uses File Picker which doesn't work in snapped mode - try unsnap first
            ''' and fail gracefully if we can't
            If ApplicationView.Value = ApplicationViewState.Snapped And Not ApplicationView.TryUnsnap() Then
                Throw New NotSupportedException("Sample doesn't work in snapped mode")
            End If

            rootPage.NotifyUser("Working...", NotifyType.StatusMessage)

            Dim picker = New FileOpenPicker()
            picker.FileTypeFilter.Add("*")
            Dim originalFile = Await picker.PickSingleFileAsync()
            If originalFile Is Nothing Then
                Throw New OperationCanceledException("No file has been selected")
            End If

            Progress.Text &= String.Format("""{0}"" has been picked" & vbLf, originalFile.Name)

            Dim compressedFilename = originalFile.Name & ".compressed"
            Dim compressedFile = Await KnownFolders.DocumentsLibrary.CreateFileAsync(compressedFilename, CreationCollisionOption.GenerateUniqueName)
            Progress.Text &= String.Format("""{0}"" has been created to store compressed data" & vbLf, compressedFile.Name)

            ' ** DO COMPRESSION **
            ' Following code actually performs compression from original file to the newly created
            ' compressed file. In order to do so it:
            ' 1. Opens input for the original file.
            ' 2. Opens output stream on the file to be compressed and wraps it into Compressor object.
            ' 3. Copies original stream into Compressor wrapper.
            ' 4. Finalizes compressor - it puts termination mark into stream and flushes all intermediate
            '    buffers.
            Using originalInput = Await originalFile.OpenReadAsync()
                Using compressedOutput = Await compressedFile.OpenAsync(FileAccessMode.ReadWrite)
                    Using compressor = If(Not Algorithm.HasValue, New Compressor(compressedOutput.GetOutputStreamAt(0)), New Compressor(compressedOutput.GetOutputStreamAt(0), Algorithm.Value, 0))
                        Progress.Text &= "All streams wired for compression" & vbLf
                        Dim bytesCompressed = Await RandomAccessStream.CopyAsync(originalInput, compressor)
                        Dim finished = Await compressor.FinishAsync()
                        Progress.Text &= String.Format("Compressed {0} bytes into {1}" & vbLf, bytesCompressed, compressedOutput.Size)
                    End Using
                End Using
            End Using

            Dim decompressedFilename = originalFile.Name & ".decompressed"
            Dim decompressedFile = Await KnownFolders.DocumentsLibrary.CreateFileAsync(decompressedFilename, CreationCollisionOption.GenerateUniqueName)
            Progress.Text &= String.Format("""{0}"" has been created to store decompressed data" & vbLf, decompressedFile.Name)

            ' ** DO DECOMPRESSION **
            ' Following code performs decompression from the just compressed file to the
            ' decompressed file. In order to do so it:
            ' 1. Opens input stream on compressed file and wraps it into Decompressor object.
            ' 2. Opens output stream from the file that will store decompressed data.
            ' 3. Copies data from Decompressor stream into decompressed file stream.
            Using compressedInput = Await compressedFile.OpenSequentialReadAsync()
                Using decompressor = New Decompressor(compressedInput)
                    Using decompressedOutput = Await decompressedFile.OpenAsync(FileAccessMode.ReadWrite)
                        Progress.Text &= "All streams wired for decompression" & vbLf
                        Dim bytesDecompressed = Await RandomAccessStream.CopyAsync(decompressor, decompressedOutput)
                        Progress.Text &= String.Format("Decompressed {0} bytes of data" & vbLf, bytesDecompressed)
                    End Using
                End Using
            End Using

            rootPage.NotifyUser("All done", NotifyType.StatusMessage)
        Catch ex As Exception
            rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    Private Sub DefaultButton_Click(sender As Object, e As RoutedEventArgs)
        DoScenario(Nothing)
    End Sub

    Private Sub Xpress_Click(sender As Object, e As RoutedEventArgs)
        DoScenario(CompressAlgorithm.Xpress)
    End Sub

    Private Sub XpressHuff_Click(sender As Object, e As RoutedEventArgs)
        DoScenario(CompressAlgorithm.XpressHuff)
    End Sub

    Private Sub Mszip_Click(sender As Object, e As RoutedEventArgs)
        DoScenario(CompressAlgorithm.Mszip)
    End Sub

    Private Sub Lzms_Click(sender As Object, e As RoutedEventArgs)
        DoScenario(CompressAlgorithm.Lzms)
    End Sub
End Class
