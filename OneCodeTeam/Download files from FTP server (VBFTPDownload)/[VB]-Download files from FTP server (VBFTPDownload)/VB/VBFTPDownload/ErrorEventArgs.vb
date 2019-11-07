'*************************** Module Header ******************************'
' Module Name:  ErrorEventArgs.vb
' Project:	    VBFTPDownload
' Copyright (c) Microsoft Corporation.
' 
' The class ErrorEventArgs defines the arguments used by the ErrorOccurred 
' event of FTPClient.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Public Class ErrorEventArgs
    Inherits EventArgs
    Public Property ErrorException() As Exception
End Class
