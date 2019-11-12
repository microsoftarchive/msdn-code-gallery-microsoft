'*************************** Module Header ******************************.
' Module Name:  FileDownloadCompletedEventArgs.vb
' Project:	    VBFTPDownload
' Copyright (c) Microsoft Corporation.
' 
' The class FileDownloadCompletedEventArgs defines the arguments used by the 
' FileDownloadCompleted event of FTPClient.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************.

Imports System.IO

Public Class FileDownloadCompletedEventArgs
    Inherits EventArgs
    Public Property ServerPath() As Uri
    Public Property LocalFile() As FileInfo
End Class
