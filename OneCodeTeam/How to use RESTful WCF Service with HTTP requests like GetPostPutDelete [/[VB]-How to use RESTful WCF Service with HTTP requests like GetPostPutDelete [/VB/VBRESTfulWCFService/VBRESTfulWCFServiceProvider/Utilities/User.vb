'****************************** Module Header ******************************\
' Module Name:  User.vb
' Project:      VBRESTfulWCFServiceProvider
' Copyright (c) Microsoft Corporation.
'
' Utility class to provide data for UserService.svc
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

''' <summary>
''' Utilities User class
''' </summary>
''' <remarks></remarks>
<DataContract([Namespace]:="http://rest-server/datacontract/user")>
Public Class User

#Region "Fields"

    Private m_Id As Integer
    Private m_Name As String
    Private m_Age As Integer
    Private m_Sex As Sex
    Private m_Comments As String
    Private Shared m_UserObj As User
    Private m_LUser As List(Of User)

#End Region

#Region "Properties"

    ''' <summary>
    ''' Id property
    ''' </summary>
    <DataMember()>
    Public Property Id() As Integer
        Get
            Return m_Id
        End Get
        Set(ByVal value As Integer)
            m_Id = value
        End Set
    End Property

    ''' <summary>
    ''' Name property
    ''' </summary>
    <DataMember()>
    Public Property Name() As String
        Get
            Return m_Name
        End Get
        Set(ByVal value As String)
            m_Name = value
        End Set
    End Property

    ''' <summary>
    ''' Age property
    ''' </summary>
    <DataMember()>
    Public Property Age() As Integer
        Get
            Return m_Age
        End Get
        Set(ByVal value As Integer)
            m_Age = value
        End Set
    End Property

    ''' <summary>
    ''' Sex property
    ''' </summary>
    <DataMember()>
    Public Property Sex() As Sex
        Get
            Return m_Sex
        End Get
        Set(ByVal value As Sex)
            m_Sex = value
        End Set
    End Property

    ''' <summary>
    ''' Comments property
    ''' </summary>
    <DataMember()>
    Public Property Comments() As String
        Get
            Return m_Comments
        End Get
        Set(ByVal value As String)
            m_Comments = value
        End Set
    End Property

    ''' <summary>
    ''' User object property
    ''' </summary>
    Public Shared ReadOnly Property UserObject() As User
        Get
            If m_UserObj Is Nothing Then
                m_UserObj = New User()
            End If

            Return m_UserObj
        End Get
    End Property

    ''' <summary>
    ''' User list property
    ''' </summary>
    Public ReadOnly Property LUser() As List(Of User)
        Get
            If m_LUser Is Nothing Then
                m_LUser = New List(Of User)()

                m_LUser.Add(New User() With { _
                  .Id = 1, _
                  .Name = "Jason", _
                  .Age = 25, _
                  .Sex = Sex.Male, _
                  .Comments = "Jason is a boy!" _
                })

                m_LUser.Add(New User() With { _
                  .Id = 2, _
                  .Name = "Susan", _
                  .Age = 25, _
                  .Sex = Sex.Female, _
                  .Comments = "Susan is a girl!" _
                })

                m_LUser.Add(New User() With { _
                  .Id = 3, _
                  .Name = "Nancy", _
                  .Age = 18, _
                  .Sex = Sex.Female, _
                  .Comments = "Nancy is a girl!" _
                })
            End If

            Return m_LUser
        End Get
    End Property

#End Region

#Region "Methods"

    ''' <summary>
    ''' Fill data and get all users
    ''' </summary>
    ''' <returns>Return a user list</returns>
    Friend Function GetAllUsers() As List(Of User)
        Return LUser
    End Function

    ''' <summary>
    ''' Create a user
    ''' </summary>
    ''' <param name="user">User object</param>
    Friend Sub CreateUser(ByVal user As User)
        LUser.Add(user)
    End Sub

    ''' <summary>
    ''' Update a user
    ''' </summary>
    ''' <param name="user">User object</param>
    Friend Sub UpdateUser(ByVal user As User)
        Dim original = LUser.Find(Function(u) u.Id = user.Id)

        If original Is Nothing Then
            Throw New Exception(String.Format("User {0} does not exist!", user.Name))
        End If

        original.Name = user.Name
        original.Sex = user.Sex
        original.Age = user.Age
        original.Comments = user.Comments
    End Sub

    ''' <summary>
    ''' Delete a user
    ''' </summary>
    ''' <param name="id">User Id</param>
    Friend Sub DeleteUser(ByVal id As String)
        If Not LUser.Exists(Function(u) u.Id.ToString() = id) Then
            Throw New Exception("Special user does not exist!")
        End If

        LUser.Remove(LUser.Find(Function(u) u.Id.ToString() = id))
    End Sub

#End Region

End Class

''' <summary>
''' Utilities Sex enum
''' </summary>
Public Enum Sex
    Male = 0
    Female = 1
End Enum