'****************************** Module Header ******************************\
' Module Name:  JsonHelp.vb
' Project:      VBRESTfulWCFServiceASPNETClient
' Copyright (c) Microsoft Corporation.
'
' JsonHelp class to Serialize/DeSerialize json data
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.Web.Script.Serialization

''' <summary>
''' JSon help class
''' </summary>
''' <remarks></remarks>
Public Class JsonHelp

    Private Shared m_JsonSerialize As JavaScriptSerializer

#Region "Methods"

    Friend Shared Function JsonDeserialize(Of T)(ByVal strJson As String) As T
        JsonHelp.m_JsonSerialize = New JavaScriptSerializer
        Return JsonHelp.m_JsonSerialize.Deserialize(Of T)(strJson)
    End Function

    Friend Shared Function JsonSerialize(Of T)(ByVal objList As T) As String
        JsonHelp.m_JsonSerialize = New JavaScriptSerializer
        Return JsonHelp.m_JsonSerialize.Serialize(objList)
    End Function

#End Region

End Class
