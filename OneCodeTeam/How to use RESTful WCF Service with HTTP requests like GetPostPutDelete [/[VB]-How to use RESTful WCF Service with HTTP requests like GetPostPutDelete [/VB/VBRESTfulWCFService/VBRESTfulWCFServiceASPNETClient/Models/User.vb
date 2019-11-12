'****************************** Module Header ******************************\
' Module Name:  User.vb
' Project:      VBRESTfulWCFServiceASPNETClient
' Copyright (c) Microsoft Corporation.
'
' User model class
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.ComponentModel.DataAnnotations

Public Class User

#Region "Properties"

    ''' <summary>
    ''' Id property
    ''' </summary>
    Public Property Id() As Integer
        Get
            Return m_Id
        End Get
        Set(ByVal value As Integer)
            m_Id = value
        End Set
    End Property
    Private m_Id As Integer

    ''' <summary>
    ''' Name property
    ''' </summary>
    <Required(ErrorMessage:="Name can't be empty")> _
    <StringLength(40, ErrorMessage:="Must be under 40 characters")> _
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(ByVal value As String)
            m_Name = value
        End Set
    End Property
    Private m_Name As String

    ''' <summary>
    ''' Age property
    ''' </summary>
    <Required(ErrorMessage:="Age can't be empty")> _
    <Range(0, 120, ErrorMessage:="Age must be between 0 and 120")> _
    Public Property Age() As Integer
        Get
            Return m_Age
        End Get
        Set(ByVal value As Integer)
            m_Age = value
        End Set
    End Property
    Private m_Age As Integer

    ''' <summary>
    ''' Sex property
    ''' </summary>
    Public Property Sex() As Sex
        Get
            Return m_Sex
        End Get
        Set(ByVal value As Sex)
            m_Sex = value
        End Set
    End Property
    Private m_Sex As Sex

    ''' <summary>
    ''' Comments property
    ''' </summary>
    Public Property Comments() As String
        Get
            Return m_Comments
        End Get
        Set(ByVal value As String)
            m_Comments = value
        End Set
    End Property
    Private m_Comments As String
#End Region

End Class

Public Enum Sex
    ' Fields
    Female = 1
    Male = 0
End Enum
