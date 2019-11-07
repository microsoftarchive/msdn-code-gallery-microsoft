'****************************** Module Header ******************************\
' Module Name:  UserService.svc.vb
' Project:      VBRESTfulWCFServiceProvider
' Copyright (c) Microsoft Corporation.
'
' WCF Service class to provide operations
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
''' WCF Service class to provide operations
''' </summary>
''' <remarks></remarks>
Friend Class UserService
    Implements IUserService

#Region "Methods"

    Public Sub CreateUser(ByVal user As User) Implements IUserService.CreateUser
        user.UserObject.CreateUser(user)
    End Sub

    Public Sub DeleteUser(ByVal id As String) Implements IUserService.DeleteUser
        User.UserObject.DeleteUser(id)
    End Sub

    Public Function GetAllUsers() As System.Collections.Generic.List(Of User) Implements IUserService.GetAllUsers
        Return User.UserObject.GetAllUsers()
    End Function

    Public Sub UpdateUser(ByVal user As User) Implements IUserService.UpdateUser
        user.UserObject.UpdateUser(user)
    End Sub

#End Region

End Class
