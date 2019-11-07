'*************************** Module Header ******************************'
' Module Name:  FileUploadCompletedEventArgs.vb
' Project:	    VBFTPUpload
' Copyright (c) Microsoft Corporation.
' 
' The class FileUploadCompletedEventArgs defines the arguments used by the 
' FileUploadCompleted event of FTPClient.
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

Public Class FileUploadCompletedEventArgs
    Inherits EventArgs
    Public Property ServerPath() As Uri
    Public Property LocalFile() As FileInfo
End Class
