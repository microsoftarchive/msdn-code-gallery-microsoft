'************************** Module Header ******************************\
' Module Name:  FTPClient.vb
' Project:      VBWindowsStoreAppFTPDownloader
' Copyright (c) Microsoft Corporation.
' 
' This class is used to download files from a FTP server using WebRequest. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************/

Imports System.Net
Imports System.Runtime.InteropServices.WindowsRuntime
Imports Windows.Storage
Imports Windows.Storage.Streams

Namespace FTP
    Public Module FTPClient
        ''' <summary>
        ''' Download a single file from FTP server using WebRequest.
        ''' </summary>
        Public Async Function DownloadFTPFileAsync(ByVal item As FTPFileSystem,
                                                   ByVal targetFile As StorageFile,
                                                   ByVal credential As ICredentials) _
                                               As Task(Of DownloadCompletedEventArgs)

            Dim result As DownloadCompletedEventArgs =
                New DownloadCompletedEventArgs With {.RequestFile = item.Url, .LocalFile = targetFile}


            ' This request is FtpWebRequest in fact.
            Dim request As WebRequest = WebRequest.Create(item.Url)

            If credential IsNot Nothing Then
                request.Credentials = credential
            End If

            request.Proxy = WebRequest.DefaultWebProxy

            ' Set the method to Download File
            request.Method = "RETR"

            Try

                ' Open the file for write.
                Using fileStream As IRandomAccessStream = Await targetFile.OpenAsync(FileAccessMode.ReadWrite)

                    ' Get response.
                    Using response As WebResponse = Await request.GetResponseAsync()

                        ' Get response stream.
                        Using responseStream As Stream = response.GetResponseStream()
                            Dim downloadBuffer(2047) As Byte
                            Dim bytesSize As Integer = 0

                            ' Download the file until the download is completed.
                            Do

                                ' Read a buffer of data from the stream.
                                bytesSize = responseStream.Read(downloadBuffer, 0, downloadBuffer.Length)
                                If bytesSize = 0 Then
                                    Exit Do
                                End If

                                ' Write buffer to the file.
                                Await fileStream.WriteAsync(downloadBuffer.AsBuffer())
                            Loop
                        End Using
                    End Using
                End Using

            Catch exception As Exception
                result.DownloadError = exception
            End Try


            Return result
        End Function
    End Module
End Namespace
