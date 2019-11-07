'*************************** Module Header ******************************'
' Module Name:  FTPUploadClient.vb
' Project:	    VBFTPUpload
' Copyright (c) Microsoft Corporation.
' 
' This class is used to upload a file to a FTP server. When the upload 
' starts, it will upload the file in a background thread. 
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

Imports System.Linq
Imports System.Text
Imports System.Threading
Imports System.IO
Imports System.Net

Partial Public Class FTPClientManager

    Public Class FTPUploadClient
       
        ' 2K bytes.
        Public Const BufferSize As Integer = 2048

        Private _manager As FTPClientManager

        Public Event FileUploadCompleted As EventHandler(Of FileUploadCompletedEventArgs)

        Public Event AllFilesUploadCompleted As EventHandler

        Public Sub New(ByVal manager As FTPClientManager)
            If manager Is Nothing Then
                Throw New ArgumentNullException("FTPClientManager cannot be null.")
            End If

            Me._manager = manager
        End Sub

        ''' <summary>
        ''' Upload files, directories and their subdirectories.
        ''' </summary>
        Public Sub UploadDirectoriesAndFiles(ByVal fileSysInfos As IEnumerable(Of FileSystemInfo),
                                             ByVal serverPath As Uri)
            If fileSysInfos Is Nothing Then
                Throw New ArgumentNullException("The files to upload cannot be null.")
            End If

            ' Create a thread to upload data.
            Dim threadStart As New ParameterizedThreadStart(
                AddressOf StartUploadDirectoriesAndFiles)

            Dim uploadThread As New Thread(threadStart)
            uploadThread.IsBackground = True
            uploadThread.Start(New Object() {fileSysInfos, serverPath})

        End Sub

        ''' <summary>
        ''' Upload files, directories and their subdirectories.
        ''' </summary>
        Private Sub StartUploadDirectoriesAndFiles(ByVal state As Object)
            Dim paras = TryCast(state, Object())

            Dim fileSysInfos As IEnumerable(Of FileSystemInfo) = TryCast(paras(0), 
                IEnumerable(Of FileSystemInfo))

            Dim serverPath As Uri = TryCast(paras(1), Uri)

            For Each fileSys In fileSysInfos
                UploadDirectoryOrFile(fileSys, serverPath)
            Next fileSys

            Me.OnAllFilesUploadCompleted(EventArgs.Empty)
        End Sub

        ''' <summary>
        ''' Upload a single file or directory.
        ''' </summary>
        Private Sub UploadDirectoryOrFile(ByVal fileSystem As FileSystemInfo,
                                          ByVal serverPath As Uri)

            ' Upload the file directly.
            If TypeOf fileSystem Is FileInfo Then
                UploadFile(TryCast(fileSystem, FileInfo), serverPath)

                ' Upload a directory.
            Else

                ' Construct the sub directory Uri.
                Dim subDirectoryPath As New Uri(serverPath, fileSystem.Name & "/")

                Me._manager.CreateDirectoryOnFTPServer(serverPath, fileSystem.Name & "/")

                ' Get the sub directories and files.
                Dim subDirectoriesAndFiles =
                    (TryCast(fileSystem, DirectoryInfo)).GetFileSystemInfos()

                ' Upload the files in the folder and sub directories.
                For Each subFile In subDirectoriesAndFiles
                    UploadDirectoryOrFile(subFile, subDirectoryPath)
                Next subFile
            End If
        End Sub

        ''' <summary>
        ''' Upload a single file directly.
        ''' </summary>
        Private Sub UploadFile(ByVal file As FileInfo, ByVal serverPath As Uri)
            If file Is Nothing Then
                Throw New ArgumentNullException(" The file to upload is null. ")
            End If

            Dim destPath As New Uri(serverPath, file.Name)

            ' Create a request to upload the file.
            Dim request As FtpWebRequest =
                TryCast(WebRequest.Create(destPath), FtpWebRequest)

            request.Credentials = Me._manager.Credentials

            ' Upload file.
            request.Method = WebRequestMethods.Ftp.UploadFile

            Dim response As FtpWebResponse = Nothing
            Dim requestStream As Stream = Nothing
            Dim localFileStream As FileStream = Nothing

            Try

                ' Retrieve the response from the server.
                response = TryCast(request.GetResponse(), FtpWebResponse)

                ' Open the local file to read.
                localFileStream = file.OpenRead()

                ' Retrieve the request stream.
                requestStream = request.GetRequestStream()

                Dim bytesSize As Integer = 0
                Dim uploadBuffer(FTPUploadClient.BufferSize - 1) As Byte

                Do

                    ' Read a buffer of local file.
                    bytesSize = localFileStream.Read(uploadBuffer, 0, uploadBuffer.Length)

                    If bytesSize = 0 Then
                        Exit Do
                    Else

                        ' Write the buffer to the request stream.
                        requestStream.Write(uploadBuffer, 0, bytesSize)

                    End If
                Loop

                Dim fileUploadCompletedEventArgs = New FileUploadCompletedEventArgs With {.LocalFile = file, .ServerPath = destPath}

                Me.OnFileUploadCompleted(fileUploadCompletedEventArgs)

            Catch webEx As System.Net.WebException
                Dim ftpResponse As FtpWebResponse = TryCast(webEx.Response, FtpWebResponse)

                Dim msg As String = String.Format(
                    "There is an error while upload {0}. " _
                    & " StatusCode: {1}  StatusDescription: {2} ",
                    file.FullName,
                    ftpResponse.StatusCode.ToString(),
                    ftpResponse.StatusDescription)
                Dim errorException As New ApplicationException(msg, webEx)

                ' Fire the ErrorOccurred event with the error.
                Dim e As ErrorEventArgs = New ErrorEventArgs _
                                          With {.ErrorException = errorException}

                Me._manager.OnErrorOccurred(e)
            Catch ex As Exception
                Dim msg As String = String.Format(
                    "There is an error while upload {0}. " _
                    & " See InnerException for detailed error. ",
                    file.FullName)
                Dim errorException As New ApplicationException(msg, ex)

                ' Fire the ErrorOccurred event with the error.
                Dim e As ErrorEventArgs = New ErrorEventArgs _
                                          With {.ErrorException = errorException}

                Me._manager.OnErrorOccurred(e)
            Finally
                If response IsNot Nothing Then
                    response.Close()
                End If

                If requestStream IsNot Nothing Then
                    requestStream.Close()
                End If

                If localFileStream IsNot Nothing Then
                    localFileStream.Close()
                End If
            End Try
        End Sub

        Protected Overridable Sub OnFileUploadCompleted(ByVal e As FileUploadCompletedEventArgs)

            RaiseEvent FileUploadCompleted(Me, e)
        End Sub

        Protected Overridable Sub OnAllFilesUploadCompleted(ByVal e As EventArgs)
            RaiseEvent AllFilesUploadCompleted(Me, e)
        End Sub
    End Class
End Class

