'****************************** Module Header ******************************\
' Module Name:    Default.aspx.vb
' Project:        VBASPNETStaticCodeByPageMethod
' Copyright (c) Microsoft Corporation
'
' This page is used to call server side function without web service .
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/

Imports System.Web.Services

Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    <WebMethod()> _
Public Shared Function sayHello(ByVal name As String) As String
        Return "Hello," & name
    End Function

    <WebMethod()> _
    Public Shared Function getData(ByVal t As TestUser) As Object
        Return New With { _
         Key .Name = t.Name & "-Test", _
         Key .Value = t.Phone _
        }
    End Function

    Public Class TestUser
        Public Property Name() As String
            Get
                Return m_Name
            End Get
            Set(ByVal value As String)
                m_Name = value
            End Set
        End Property
        Private m_Name As String
        Public Property BirthDate() As DateTime
            Get
                Return m_BirthDate
            End Get
            Set(ByVal value As DateTime)
                m_BirthDate = value
            End Set
        End Property
        Private m_BirthDate As DateTime
        Public Property Phone() As IList(Of String)
            Get
                Return m_Phone
            End Get
            Set(ByVal value As IList(Of String))
                m_Phone = value
            End Set
        End Property
        Private m_Phone As IList(Of String)
    End Class

End Class