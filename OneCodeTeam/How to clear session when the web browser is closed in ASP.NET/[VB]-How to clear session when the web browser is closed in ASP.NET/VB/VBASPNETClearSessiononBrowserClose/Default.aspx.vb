Imports System

Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Session("SessionCreatedTime") = System.DateTime.Now.ToString()
        Response.Write("Session Intialized at " + Session("SessionCreatedTime"))
    End Sub

End Class