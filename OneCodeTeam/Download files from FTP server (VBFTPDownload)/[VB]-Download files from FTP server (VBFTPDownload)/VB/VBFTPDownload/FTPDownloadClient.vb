'*************************** Module Header ******************************'
' Module Name:  FTPDownloadClient.vb
' Project:	    VBFTPDownload
' Copyright (c) Microsoft Corporation.
' 
' This class is used to download files from a FTP server. When the download 
' starts, it will download the file in a background thread. The downloaded 
' data is stored in a MemoryStream first, and then written to local file.
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.IO
Imports System.Net
Imports System.Threading

Partial Public Class FTPClientManager

    Public Class FTPDownloadClient
        ' 2M bytes.
        Public Const MaxCacheSize As Integer = 2097152

        ' 2K bytes.
        Public Const BufferSize As Integer = 2048

        Private _manager As FTPClientManager

        Public Event FileDownloadCompleted As EventHandler(Of FileDownloadCompletedEventArgs)

        Public Event AllFilesDownloadCompleted As EventHandler

        Public Sub New(ByVal manager As FTPClientManager)
            If manager Is Nothing Then
                Throw New ArgumentNullException("FTPClientManager cannot be null.")
            End If

            Me._manager = manager
        End Sub

        ''' <summary>
        ''' Download files, directories and their subdirectories.
        ''' </summary>
        Public Sub DownloadDirectoriesAndFiles(ByVal files As IEnumerable(Of FTPFileSystem),
                                               ByVal localPath As String)
            If files Is Nothing Then
                Throw New ArgumentNullException("The files to download cannot be null.")
            End If

            ' Create a thread to download data.
            Dim threadStart As New ParameterizedThreadStart(
                AddressOf StartDownloadDirectoriesAndFiles)
            Dim downloadThread As New Thread(threadStart)
            downloadThread.IsBackground = True
            downloadThread.Start(New Object() {files, localPath})
        End Sub

        ''' <summary>
        ''' Download files, directories and their subdirectories.
        ''' </summary>
        Private Sub StartDownloadDirectoriesAndFiles(ByVal state As Object)
            Dim paras = TryCast(state, Object())

            Dim files As IEnumerable(Of FTPFileSystem) = TryCast(paras(0), 
                IEnumerable(Of FTPFileSystem))
            Dim localPath As String = TryCast(paras(1), String)

            For Each file In files
                DownloadDirectoryOrFile(file, localPath)
            Next file

            Me.OnAllFilesDownloadCompleted(EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Download a single file or directory.
        ''' </summary>
        Private Sub DownloadDirectoryOrFile(ByVal fileSystem As FTPFileSystem,
                                            ByVal localPath As String)

            ' Download the file directly.
            If Not fileSystem.IsDirectory Then
                DownloadFile(fileSystem, localPath)

                ' Download a directory.
            Else

                ' Construct the directory Path.
                Dim directoryPath As String = localPath & "\" & fileSystem.Name

                If Not Directory.Exists(directoryPath) Then
                    Directory.CreateDirectory(directoryPath)
                End If

                ' Get the sub directories and files.
                Dim subDirectoriesAndFiles =
                    Me._manager.GetSubDirectoriesAndFiles(fileSystem.Url)

                ' Download the files in the folder and sub directories.
                For Each subFile In subDirectoriesAndFiles
                    DownloadDirectoryOrFile(subFile, directoryPath)
                Next subFile
            End If
        End Sub

        ''' <summary>
        ''' Download a single file directly.
        ''' </summary>
        Private Sub DownloadFile(ByVal file As FTPFileSystem, ByVal localPath As String)
            If file.IsDirectory Then
                Throw New ArgumentException(
                    "The FTPFileSystem to download is a directory in fact")
            End If

            Dim destPath As String = localPath & "\" & file.Name

            ' Create a request to the file to be  downloaded.
            Dim request As FtpWebRequest =
                TryCast(WebRequest.Create(file.Url), FtpWebRequest)

            request.Credentials = Me._manager.Credentials

            ' Download file.
            request.Method = WebRequestMethods.Ftp.DownloadFile

            Dim response As FtpWebResponse = Nothing
            Dim responseStream As Stream = Nothing
            Dim downloadCache As MemoryStream = Nothing


            Try

                ' Retrieve the response from the server and get the response stream.
                response = TryCast(request.GetResponse(), FtpWebResponse)

                Me._manager.OnNewMessageArrived(New NewMessageEventArg _
                                                With {.NewMessage = response.StatusDescription})

                responseStream = response.GetResponseStream()

                ' Cache data in memory.
                downloadCache = New MemoryStream(FTPDownloadClient.MaxCacheSize)
                Dim downloadBuffer(FTPDownloadClient.BufferSize - 1) As Byte

                Dim bytesSize As Integer = 0
                Dim cachedSize As Integer = 0

                ' Download the file until the download is completed.
                Do

                    ' Read a buffer of data from the stream.
                    bytesSize =
                        responseStream.Read(downloadBuffer, 0, downloadBuffer.Length)

                    ' If the cache is full, or the download is completed, write 
                    ' the data in cache to local file.
                    If bytesSize = 0 OrElse MaxCacheSize < cachedSize + bytesSize Then
                        Try
                            ' Write the data in cache to local file.
                            WriteCacheToFile(downloadCache, destPath, cachedSize)

                            ' Stop downloading the file if the download is paused, 
                            ' canceled or completed. 
                            If bytesSize = 0 Then
                                Exit Do
                            End If

                            ' Reset cache.
                            downloadCache.Seek(0, SeekOrigin.Begin)
                            cachedSize = 0
                        Catch ex As Exception
                            Dim msg As String =
                                String.Format("There is an error while downloading {0}. " _
                                              & " See InnerException for detailed error. ",
                                              file.Url)
                            Dim errorException As New ApplicationException(msg, ex)

                            ' Fire the DownloadCompleted event with the error.
                            Dim e As ErrorEventArgs = New ErrorEventArgs _
                                                      With {.ErrorException = errorException}

                            Me._manager.OnErrorOccurred(e)

                            Return
                        End Try

                    End If

                    ' Write the data from the buffer to the cache in memory.
                    downloadCache.Write(downloadBuffer, 0, bytesSize)
                    cachedSize += bytesSize
                Loop

                Dim fileDownloadCompletedEventArgs_Renamed =
                    New FileDownloadCompletedEventArgs _
                    With {.LocalFile = New FileInfo(destPath), .ServerPath = file.Url}

                Me.OnFileDownloadCompleted(fileDownloadCompletedEventArgs_Renamed)
            Finally
                If response IsNot Nothing Then
                    response.Close()
                End If

                If responseStream IsNot Nothing Then
                    responseStream.Close()
                End If

                If downloadCache IsNot Nothing Then
                    downloadCache.Close()
                End If
            End Try
        End Sub

        ''' <summary>
        ''' Write the data in cache to local file.
        ''' </summary>
        Private Sub WriteCacheToFile(ByVal downloadCache As MemoryStream,
                                     ByVal downloadPath As String,
                                     ByVal cachedSize As Integer)
            Using fileStream_Renamed As New FileStream(downloadPath, FileMode.Append)
                Dim cacheContent(cachedSize - 1) As Byte
                downloadCache.Seek(0, SeekOrigin.Begin)
                downloadCache.Read(cacheContent, 0, cachedSize)
                fileStream_Renamed.Write(cacheContent, 0, cachedSize)
            End Using
        End Sub

        Protected Overridable Sub OnFileDownloadCompleted(ByVal e As FileDownloadCompletedEventArgs)

            RaiseEvent FileDownloadCompleted(Me, e)
        End Sub

        Protected Overridable Sub OnAllFilesDownloadCompleted(ByVal e As EventArgs)
            RaiseEvent AllFilesDownloadCompleted(Me, e)
        End Sub
    End Class
End Class