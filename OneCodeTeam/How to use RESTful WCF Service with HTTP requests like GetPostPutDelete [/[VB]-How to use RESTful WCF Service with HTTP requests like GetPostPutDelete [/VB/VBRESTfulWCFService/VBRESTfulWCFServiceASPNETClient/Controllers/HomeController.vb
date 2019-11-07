'****************************** Module Header ******************************
'Module Name:  HomeController.vb
'Project:      VBRESTfulWCFServiceASPNETClient
'Copyright (c) Microsoft Corporation.

'HomeController class to Create/Delete/Update/GetAll users

'This source is subject to the Microsoft Public License.
'See http://www.microsoft.com/en-us/openness/licenses.aspx.
'All other rights reserved.

'THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************

Public Class HomeController
    Inherits System.Web.Mvc.Controller

#Region "Fields"

    Private Const WCFURL As String = "http://localhost:50500/UserService.svc"
    Private m_Profile As RESTContext(Of User)

#End Region

#Region "Properties"

    ''' <summary>
    ''' Profile property to create RESTContext object
    ''' </summary>
    Public ReadOnly Property Profile() As RESTContext(Of User)
        Get
            If m_Profile Is Nothing Then
                m_Profile = New RESTContext(Of User)(WCFURL)
            End If

            Return m_Profile
        End Get
    End Property

#End Region

#Region "Methods: Action"

    ''' <summary>
    ''' Home page: Index
    ''' </summary>
    ''' <returns></returns>
    Public Function Index() As ActionResult
        Dim lUser As List(Of User) = Profile.GetAll()
        ShowMessage(Profile.StrMessage)

        Return View(lUser)
    End Function

    ''' <summary>
    ''' Edit
    ''' </summary>
    ''' <param name="id">User id</param>
    ''' <returns></returns>
    Public Function Edit(ByVal id As Integer) As ActionResult
        Return View(Profile.GetAll().[Single](Function(u) u.Id = id))
    End Function

    ''' <summary>
    ''' Edit
    ''' </summary>
    ''' <param name="user">User object</param>
    ''' <returns></returns>
    <HttpPost()> _
    Public Function Edit(ByVal user As User) As ActionResult
        If ModelState.IsValid Then
            Profile.Update(user)
            ShowMessage(Profile.StrMessage)

            Return RedirectToAction("Index")
        End If

        Return View(user)
    End Function

    ''' <summary>
    ''' Delete
    ''' </summary>
    ''' <param name="id">User id</param>
    ''' <returns></returns>
    Public Function Delete(ByVal id As Integer) As ActionResult
        Profile.Delete(Of Integer)(id)
        ShowMessage(Profile.StrMessage)

        Return RedirectToAction("Index")
    End Function

    ''' <summary>
    ''' Create
    ''' </summary>
    ''' <returns></returns>
    Public Function Create() As ActionResult
        Return View()
    End Function

    ''' <summary>
    ''' Create
    ''' </summary>
    ''' <param name="user">USer object</param>
    ''' <returns></returns>
    <HttpPost>
    Public Function Create(user As User) As ActionResult
        If ModelState.IsValid Then
            Dim lUser As List(Of User) = Profile.GetAll()
            ShowMessage(Profile.StrMessage)

            If lUser IsNot Nothing Then
                user.Id = If(lUser.Count = 0, 1, lUser.Max(Function(u) u.Id) + 1)
                Profile.Create(user)

                Return RedirectToAction("Index")
            End If
        End If

        Return View(user)
    End Function

    ''' <summary>
    ''' Show error message
    ''' </summary>
    ''' <param name="strMessage">message</param>
    Private Sub ShowMessage(strMessage As String)
        TempData("error") = strMessage
    End Sub

#End Region

End Class
