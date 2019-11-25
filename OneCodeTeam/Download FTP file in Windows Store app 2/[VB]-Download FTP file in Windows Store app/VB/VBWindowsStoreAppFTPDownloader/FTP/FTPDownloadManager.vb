'*************************** Module Header ******************************\
' Module Name:  FTPDownloadManager.vb
' Project:	    VBWindowsStoreAppFTPDownloader
' Copyright (c) Microsoft Corporation.
' 
' The class FTPClientManager supplies following features:
' 1. List subdirectories and files of a folder on the FTP server.
' 2. Download files from the FTP server.
'    a. If the file is less than 1MB, download it using WebRequest.
'    b. Download large file using BackgroundDownloader.
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.Net
Imports Windows.Networking.BackgroundTransfer
Imports Windows.Storage

Namespace FTP
    Public Class FTPDownloadManager
        Public Shared ReadOnly Instance As New FTPDownloadManager()

        ''' <summary>
        ''' The backgroundDownloaders that are active
        ''' </summary>
        Public Property ActiveBackgroundDownloaders() As List(Of DownloadOperation)

        Public Event DownloadCompleted As EventHandler(Of DownloadCompletedEventArgs)

        Private Sub New()
            ActiveBackgroundDownloaders = New List(Of DownloadOperation)()
        End Sub

        ''' <summary>
        ''' Get the active downloads. 
        ''' This method is called when app is launched.
        ''' </summary>
        Public Async Sub Initialize()
            Dim downloads = Await BackgroundDownloader.GetCurrentDownloadsAsync()
            ActiveBackgroundDownloaders.AddRange(downloads)

            Dim tasks As New List(Of Task)()

            For Each download As DownloadOperation In Me.ActiveBackgroundDownloaders
                Dim progressCallback As New Progress(Of DownloadOperation)(AddressOf DownloadProgress)
                tasks.Add(download.AttachAsync().AsTask(progressCallback))
            Next download

            Await Task.WhenAll(tasks)
        End Sub

        ''' <summary>
        ''' Get the sub directories and files of the Url. It will be used in enumerate 
        ''' all the folders.
        ''' When run the FTP LIST protocol method to get a detailed listing of the files  
        ''' on an FTP server, the server will response many records of information. Each 
        ''' record represents a file. 
        ''' </summary>
        Public Async Function ListFtpContentAsync(ByVal url As String, ByVal credential As ICredentials) As Task(Of IEnumerable(Of FTPFileSystem))
            Dim currentUri As Uri = Nothing
            If (Not Uri.TryCreate(url, UriKind.Absolute, currentUri)) OrElse currentUri Is Nothing Then
                Throw New ArgumentException("URL: " & url & " is not valid.")
            End If

            Return Await ListFtpContentAsync(currentUri, credential)
        End Function

        ''' <summary>
        ''' Get the sub directories and files of the Url. It will be used in enumerate 
        ''' all the folders.
        ''' When run the FTP LIST protocol method to get a detailed listing of the files  
        ''' on an FTP server, the server will response many records of information. Each 
        ''' record represents a file or a directory. 
        ''' </summary>
        Public Async Function ListFtpContentAsync(ByVal url As Uri, ByVal credential As ICredentials) As Task(Of IEnumerable(Of FTPFileSystem))
            Dim currentUri As Uri = url

            ' This request is FtpWebRequest in fact.
            Dim request As WebRequest = WebRequest.Create(currentUri)
            If credential IsNot Nothing Then
                request.Credentials = credential
            End If

            request.Proxy = WebRequest.DefaultWebProxy

            ' Set the method to LIST.
            request.Method = "LIST"

            ' Get response.
            Using response As WebResponse = Await request.GetResponseAsync()

                ' Get response stream.
                Using responseStream As Stream = response.GetResponseStream()

                    Using reader As New StreamReader(responseStream)
                        Dim subDirs As New List(Of FTP.FTPFileSystem)()

                        Dim item As String = reader.ReadLine()

                        ' Find out the FTP Directory Listing Style from the recordString.
                        Dim style As FTP.FTPDirectoryListingStyle = FTP.FTPDirectoryListingStyle.MSDOS
                        If Not String.IsNullOrEmpty(item) Then
                            style = FTP.FTPFileSystem.GetDirectoryListingStyle(item)
                        End If
                        Do While Not String.IsNullOrEmpty(item)
                            subDirs.Add(FTP.FTPFileSystem.ParseRecordString(currentUri, item, style))
                            item = reader.ReadLine()
                        Loop

                        Return subDirs
                    End Using
                End Using
            End Using

        End Function

        ''' <summary>
        ''' Download files, directories and their subdirectories.
        ''' </summary>
        Public Async Sub DownloadFTPItemsAsync(ByVal itemsToDownload As IEnumerable(Of FTPFileSystem),
                                               ByVal targetFolder As StorageFolder, ByVal credential As NetworkCredential)

            ' Create a BackgroundDownloader
            Dim downloader As New BackgroundDownloader()

            ' Download sub folders and files.
            For Each item As FTPFileSystem In itemsToDownload.ToList()
                If item.IsDirectory Then

                    ' Create a local folder.
                    Dim subFolder = Await targetFolder.CreateFolderAsync(item.Name, CreationCollisionOption.OpenIfExists)

                    ' Download the whole folder.
                    DownloadFTPDirectoryAsync(downloader, credential, item, subFolder)
                Else

                    ' Create a local file.
                    Dim file = Await targetFolder.CreateFileAsync(item.Name, CreationCollisionOption.ReplaceExisting)

                    ' Download the file.
                    DownloadFTPFileAsync(downloader, credential, item, file)
                End If

            Next item
        End Sub

        ''' <summary>
        ''' Download a whole directory.
        ''' </summary>
        Private Async Sub DownloadFTPDirectoryAsync(ByVal downloader As BackgroundDownloader, ByVal credential As NetworkCredential, ByVal item As FTPFileSystem, ByVal targetFolder As StorageFolder)

            ' List the sub folders and files.
            Dim subItems = Await Me.ListFtpContentAsync(item.Url, credential)

            ' Download the sub folders and files.
            For Each subitem As FTPFileSystem In subItems
                If subitem.IsDirectory Then

                    ' Create a local folder.
                    Dim subFolder = Await targetFolder.CreateFolderAsync(subitem.Name, CreationCollisionOption.ReplaceExisting)

                    ' Download the whole folder.
                    DownloadFTPDirectoryAsync(downloader, credential, subitem, subFolder)
                Else

                    ' Create a local file.
                    Dim file = Await targetFolder.CreateFileAsync(subitem.Name, CreationCollisionOption.GenerateUniqueName)

                    ' Download the file.
                    DownloadFTPFileAsync(downloader, credential, subitem, file)
                End If
            Next subitem
        End Sub

        ''' <summary>
        ''' Download a single file directly.
        ''' </summary>
        Private Async Sub DownloadFTPFileAsync(ByVal downloader As BackgroundDownloader, ByVal credential As NetworkCredential, ByVal item As FTPFileSystem, ByVal targetFile As StorageFile)
            If item.Size > 1048576 Then ' 1M Byte
                Dim progressCallback As New Progress(Of DownloadOperation)(AddressOf DownloadProgress)

                Dim urlWithCredential As Uri = item.Url

                If credential IsNot Nothing Then

                    urlWithCredential = New Uri(item.Url.ToString().ToLower().Replace(
                                                "ftp://",
                                                String.Format("ftp://{0}:{1}@",
                                                              FTPFileSystem.EncodeUrl(credential.UserName),
                                                             FTPFileSystem.EncodeUrl(credential.Password))))
                End If

                Dim download As DownloadOperation = downloader.CreateDownload(urlWithCredential, targetFile)
                ActiveBackgroundDownloaders.Add(download)
                Await download.StartAsync().AsTask(progressCallback)
            Else
                Await FTPClient.DownloadFTPFileAsync(item, targetFile, credential).ContinueWith(New Action(Of Task(Of DownloadCompletedEventArgs))(AddressOf DownloadProgress))
            End If

        End Sub

        ''' <summary>
        ''' Raise DownloadCompleted event when a file is downloaded.
        ''' </summary>
        Private Sub DownloadProgress(ByVal task As Task(Of DownloadCompletedEventArgs))
                RaiseEvent DownloadCompleted(Me, task.Result)
        End Sub

        ''' <summary>
        ''' Raise DownloadCompleted event when a file is downloaded.
        ''' </summary>
        Private Sub DownloadProgress(ByVal download As DownloadOperation)
            If download.Progress.Status = BackgroundTransferStatus.Completed Then
                ActiveBackgroundDownloaders.Remove(download)

                RaiseEvent DownloadCompleted(Me, New DownloadCompletedEventArgs With
                                                 {.RequestFile = download.RequestedUri,
                                                  .LocalFile = download.ResultFile})
            ElseIf download.Progress.Status = BackgroundTransferStatus.Error Then
                ActiveBackgroundDownloaders.Remove(download)

                RaiseEvent DownloadCompleted(Me, New DownloadCompletedEventArgs With
                                 {.RequestFile = download.RequestedUri,
                                  .LocalFile = download.ResultFile,
                                  .DownloadError = New Exception("Failed to download file")})
            End If


        End Sub
    End Class
End Namespace
