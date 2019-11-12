'*************************** Module Header ******************************.
' Module Name:  FileDownloadCompletedEventArgs.vb
' Project:	    VBFTPDownload
' Copyright (c) Microsoft Corporation.
' 
' The class FileDownloadCompletedEventArgs defines the arguments used by the 
' FileDownloadCompleted event of FTPClient.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************.

Imports System.IO

Public Class FileDownloadCompletedEventArgs
    Inherits EventArgs

    Dim _serverPath As Uri
    Public Property ServerPath() As Uri
        Get
            Return _serverPath
        End Get
        Set(ByVal value As Uri)
            _serverPath = value
        End Set
    End Property

    Dim _localFile As FileInfo
    Public Property LocalFile() As FileInfo
        Get
            Return _localFile
        End Get
        Set(ByVal value As FileInfo)
            _localFile = value
        End Set
    End Property
End Class
