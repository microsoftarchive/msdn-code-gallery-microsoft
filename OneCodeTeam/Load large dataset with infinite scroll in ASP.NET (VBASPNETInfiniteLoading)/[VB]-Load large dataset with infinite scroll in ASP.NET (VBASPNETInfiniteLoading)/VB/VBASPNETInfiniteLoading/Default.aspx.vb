'/****************************** Module Header ******************************\
'* Module Name:    Default.aspx.vb
'* Project:        VBASPNETInfiniteLoading
'* Copyright (c) Microsoft Corporation
'*
'* This project illustrates how to scroll down to load new page content based
'* on the AJAX technology. 
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
'* All other rights reserved.
'*
'\*****************************************************************************/

Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.Services
Imports System.Data
Imports System.Text

Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    <System.Web.Services.WebMethod()>
    Public Shared Function Foo() As String
        Dim getPostsText As New StringBuilder()
        Using ds As New DataSet()
            ds.ReadXml(HttpContext.Current.Server.MapPath("App_Data/books.xml"))
            Dim dv As DataView = ds.Tables(0).DefaultView

            For Each myDataRow As DataRowView In dv
                getPostsText.AppendFormat("<p>author: {0}</br>", myDataRow("author"))
                getPostsText.AppendFormat("genre: {0}</br>", myDataRow("genre"))
                getPostsText.AppendFormat("price: {0}</br>", myDataRow("price"))
                getPostsText.AppendFormat("publish date: {0}</br>", myDataRow("publish_date"))
                getPostsText.AppendFormat("description: {0}</br></p>", myDataRow("description"))
            Next

            getPostsText.AppendFormat("<div style='height:15px;'></div>")
        End Using
        Return getPostsText.ToString()
    End Function


End Class