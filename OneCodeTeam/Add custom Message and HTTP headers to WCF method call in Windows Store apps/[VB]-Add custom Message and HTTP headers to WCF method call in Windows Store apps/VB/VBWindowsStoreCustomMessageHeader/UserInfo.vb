'****************************** Module Header ******************************\
' Module Name:  UserInfo.vb
' Project:      VBWindowsStoreCustomMessageHeader
' Copyright (c) Microsoft Corporation.
' 
' This is a UserInfo class. It will be passed in as a MessageHeader.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Public NotInheritable Class UserInfo
    Private _firstName As String
    Private _lastName As String
    Private _age As Integer

    ''' <summary>
    ''' FirstName of the user.
    ''' </summary>
    Public Property FirstName() As String
        Get
            Return _firstName
        End Get
        Set(value As String)
            _firstName = value
        End Set
    End Property

    ''' <summary>
    ''' LastName of the user.
    ''' </summary>
    Public Property LastName() As String
        Get
            Return _lastName
        End Get
        Set(value As String)
            _lastName = value
        End Set
    End Property

    ''' <summary>
    ''' Age of the user.
    ''' </summary>
    Public Property Age() As Integer
        Get
            Return _age
        End Get
        Set(value As Integer)
            _age = value
        End Set
    End Property
End Class
