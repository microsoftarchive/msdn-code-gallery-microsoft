'****************************** Module Header ******************************\
' Module Name:  Person.vb
' Project:      VBWindowsStoreAppHttpClientPostJson
' Copyright (c) Microsoft Corporation.
' 
' This is post data class
'  
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.Runtime.Serialization

<DataContract> _
Public Class Person
    <DataMember(Order:=1)> _
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(value As String)
            m_Name = value
        End Set
    End Property
    Private m_Name As String
    <DataMember(Order:=2)> _
    Public Property Age() As Integer
        Get
            Return m_Age
        End Get
        Set(value As Integer)
            m_Age = value
        End Set
    End Property
    Private m_Age As Integer
End Class

