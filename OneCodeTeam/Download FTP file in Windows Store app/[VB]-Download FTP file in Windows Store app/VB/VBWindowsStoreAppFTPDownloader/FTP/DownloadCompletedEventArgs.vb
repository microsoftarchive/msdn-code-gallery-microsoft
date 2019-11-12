'************************** Module Header ******************************\
' Module Name:    DownloadCompletedEventArgs.vb
' Project:        VBWindowsStoreAppFTPDownloader
' Copyright (c) Microsoft Corporation.
' 
' The event args when a file is downloaded.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'************************************************************************/

Namespace FTP
    Public Class DownloadCompletedEventArgs
        Inherits EventArgs
        Public Property RequestFile() As Uri
        Public Property LocalFile() As Windows.Storage.IStorageFile
        Public Property DownloadError() As Exception
    End Class
End Namespace
