'****************************** Module Header ******************************\
' Module Name:    Show.aspx.vb
' Project:        VBASPNETSearchEngine
' Copyright (c) Microsoft Corporation
'
' This page shows an individual record from database according to Query String
' parameter "id".
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
'*****************************************************************************/

Partial Public Class Show
    Inherits System.Web.UI.Page
    ''' <summary>
    ''' The record which is displaying.
    ''' </summary>
    Protected Data As Article

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim id As Long = 0

        ' Only query database in the first load and ensure the input parameter is valid.
        If Not IsPostBack _
            AndAlso Not String.IsNullOrEmpty(Request.QueryString("id")) _
            AndAlso Long.TryParse(Request.QueryString("id"), id) Then
            Dim dataAccess As New DataAccess()
            Data = dataAccess.GetArticle(id)
        End If

        ' Ensure the result is not null.
        If Data Is Nothing Then
            Data = New Article()
        End If
    End Sub
End Class