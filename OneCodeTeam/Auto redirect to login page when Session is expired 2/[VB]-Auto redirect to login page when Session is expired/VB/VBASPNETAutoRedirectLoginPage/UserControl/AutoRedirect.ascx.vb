'**************************** Module Header ******************************\
' Module Name: AutoRedirect.ascx.vb
' Project:     VBASPNETAutoRedirectLoginPage
' Copyright (c) Microsoft Corporation
'
' The project illustrates how to develop an asp.net code-sample that will 
' automatically redirect to login page when page Session is expired or 
' time out. It will ask the user to extend the Session at 1 minutes before
' expiring. If the user doesn't has any actions, the web page will redirect 
' to login page automatically, and also note that it need to work in one or
' more browser's tabs. 
' 
' The user control use to check the user session is expired or time out,
' and update the expired time when users has new behaviour. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'****************************************************************************




Imports System.Globalization

Public Class AutoRedirect
    Inherits System.Web.UI.UserControl
    Public LoginDate As String
    Public ExpressDate As String

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Check session is expire or timeout.
        If Session("username") Is Nothing Then
            Response.Redirect("LoginPage.aspx?info=0")
        End If

        ' Get user login time or last activity time.
        Dim [date] As DateTime = DateTime.Now
        LoginDate = [date].ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "")
        Dim sessionTimeout As Integer = Session.Timeout
        Dim dateExpress As DateTime = [date].AddMinutes(sessionTimeout)
        ExpressDate = dateExpress.ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "")
    End Sub

End Class