'****************************** Module Header ******************************\
' Module Name:  WMIOperation.vb
' Project:	    VBWMIEnableDisableNetworkAdapter
' Copyright (c) Microsoft Corporation.
' 
' This is a class which used to handle some operation of a WMI object.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System
Imports System.Management

Public Class WMIOperation

    Public Shared Function WMIQuery(ByVal strwQuery As String) _
        As ManagementObjectCollection

        Dim oQuery As New ObjectQuery(strwQuery)
        Dim oSearcher As New ManagementObjectSearcher(oQuery)
        Return oSearcher.Get
    End Function

End Class
