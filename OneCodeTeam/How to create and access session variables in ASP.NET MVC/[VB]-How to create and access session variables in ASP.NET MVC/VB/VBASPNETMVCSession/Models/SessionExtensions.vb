'***************************** Module Header ******************************\
' Module Name:  SessionExtensions.vb
' Project:      VBASPNETMVCSession
' Copyright (c) Microsoft Corporation.
'  
' This sample demonstrates how to use Session in ASPNET MVC. 
' This class is the Extension for HttpSessionStateBase class.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
'  
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/


Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

Public Module SessionExtensions
    Sub New()
    End Sub
    ''' <summary>
    ''' Get value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="session"></param>
    ''' <param name="key"></param>
    ''' <returns></returns>
    <System.Runtime.CompilerServices.Extension> _
    Public Function GetDataFromSession(Of T)(session As HttpSessionStateBase, key As String) As T
        Return DirectCast(session(key), T)
    End Function

    ''' <summary>
    ''' Set value.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    ''' <param name="session"></param>
    ''' <param name="key"></param>
    ''' <param name="value"></param>
    <System.Runtime.CompilerServices.Extension> _
    Public Sub SetDataToSession(Of T)(session As HttpSessionStateBase, key As String, value As Object)
        session(key) = value
    End Sub
End Module
