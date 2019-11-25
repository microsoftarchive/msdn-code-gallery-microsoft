'****************************** Module Header ******************************\
' Module Name: LoginPage.aspx.vb
' Project:     VBASPNETAutoRedirectLoginPage
' Copyright (c) Microsoft Corporation
'
' The project illustrates how to develop an asp.net code-sample that will be 
' redirect to login page when page Session is expired or time out automatically. 
' It will ask the user to extend the Session at one minutes before
' expiring. If the user does not has any actions, the web page will be redirected
' to login page automatically, and also note that it need to work in one or
' more browser's tabs. 
' 
' The login page use to login in and prevent users who want to skip the login
' step by visit specified pages.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/



Public Class LoginPage
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Prevent the users who try to skip the login step by visit specified page.
        If Not Page.IsPostBack Then
            Session.Abandon()
        End If
        If Request.QueryString("info") IsNot Nothing Then
            Dim message As String = Request.QueryString("info").ToString()
            If message = "0" Then
                Response.Write("<strong>you need login first to visit user page.</strong>")
            End If
        End If

    End Sub
    ''' <summary>
    ''' User login method.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnLogin_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnLogin.Click
        Dim username As String = tbUserName.Text.Trim()
        If tbUserName.Text.Trim() = "username" AndAlso tbPassword.Text.Trim() = "password" Then
            Session("username") = username
            Response.Redirect("UserPage2.aspx")
        Else
            Response.Write("<strong>User name or pass word error.</strong>")
        End If

    End Sub
End Class