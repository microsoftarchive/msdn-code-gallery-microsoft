'***************************** Module Header ******************************\
' Module Name:	UserData.vb
' Project:		WCFRESTService
' Copyright (c) Microsoft Corporation.
' 
' This sample will show you how to create HTTPWebReqeust, and get HTTPWebResponse.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/
Imports System.Runtime.Serialization

<DataContract([Namespace]:="http://msdn.microsoft.com")> _
Public Class UserData
    <DataMember> _
    Public Property ID() As String
        Get
            Return m_ID
        End Get
        Set(value As String)
            m_ID = value
        End Set
    End Property
    Private m_ID As String

    <DataMember> _
    Public Property UserName() As String
        Get
            Return m_UserName
        End Get
        Set(value As String)
            m_UserName = value
        End Set
    End Property
    Private m_UserName As String

End Class

Public Class UserDataList
    Private userDataList As List(Of UserData)
    Public Sub New()
        userDataList = New List(Of UserData)()
        userDataList.Add(New UserData() With { _
             .ID = "1", _
             .UserName = "Sam" _
        })
        userDataList.Add(New UserData() With { _
             .ID = "2", _
             .UserName = "Lucy" _
        })
        userDataList.Add(New UserData() With { _
            .ID = "3", _
            .UserName = "Dandy" _
        })
        userDataList.Add(New UserData() With { _
             .ID = "4", _
             .UserName = "Alex" _
        })
    End Sub
    Public Function getUserDataList() As List(Of UserData)
        Return userDataList
    End Function
End Class
