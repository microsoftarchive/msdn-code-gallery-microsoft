'****************************** Module Header ******************************\
' Module Name:    Default.aspx.vb
' Project:        VBASPNETRatingControlSelectCurrentValue
' Copyright (c) Microsoft Corporation
'
' This sample will demonstrate you how to solve the problem that using the Ajax 
' Rating control to select the currently selected option. Because the OnChanged 
' Event doesn't trigger resulted in the user cannot select the currently selected 
' items. The sample will load a list of books, when the user clicks the link in 
' one of the records, we can see the rating corresponds to the current record books. 
' When the user clicks on the current rating, the database will use the current 
' rating to insert a new record.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/

Imports System.Data.SqlClient

Partial Public Class _Default
    Inherits System.Web.UI.Page

    ' SQL connection.
    Shared connetionString As String = ConfigurationManager.ConnectionStrings("ConnectionString").ConnectionString
    Private conn As New SqlConnection(connetionString)

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        ' Bind data.
        BindData()
    End Sub

    ''' <summary>
    ''' Bind data to gdvBooks.
    ''' </summary>
    Private Sub BindData()
        Dim sda As New SqlDataAdapter("select * from bookInfo", conn)
        Dim ds As New DataSet()
        sda.Fill(ds)
        gdvBooks.DataSource = ds
        gdvBooks.DataBind()
    End Sub

    ''' <summary>
    ''' Get the rating of the selected item and then insert it into the database.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Store the rating of the selected item.
        Dim intRate As Integer = 0

        Select Case Rating1.CurrentRating
            Case 1
                intRate = 1
                Exit Select
            Case 2
                intRate = 2
                Exit Select
            Case 3
                intRate = 3
                Exit Select
            Case 4
                intRate = 4
                Exit Select
            Case 5
                intRate = 5
                Exit Select
        End Select

        Try
            ' Insert a new record.
            insertdataintosql("test1", "Microsoft", intRate)
        Catch ee As DataException
            lbResponse.Text = ee.Message
            lbResponse.ForeColor = System.Drawing.Color.Red
        Finally
            ' Bind data.
            BindData()
        End Try
    End Sub

    ''' <summary>
    ''' Bind the rating of the currently selected item to control.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub gdvBooks_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs)
        If e.CommandName = "RateDetail" Then
            Dim lb As LinkButton = DirectCast(e.CommandSource, LinkButton)
            ' Get control according to CommandSource
            Dim s As String = lb.CommandArgument
            Rating1.CurrentRating = Convert.ToInt32(lb.CommandArgument)
        End If
    End Sub

    ''' <summary>
    ''' Insert data to database.
    ''' </summary>
    ''' <param name="name">name</param>
    ''' <param name="Author">Author</param>
    ''' <param name="Rate">Rate</param>
    Public Sub insertdataintosql(ByVal name As String, ByVal Author As String, ByVal Rate As Integer)
        Dim cmd As New SqlCommand()
        cmd.Connection = conn
        cmd.CommandText = "insert into bookInfo(name,Author,Rate) values(@name,@Author,@Rate)"
        cmd.Parameters.Add("@name", SqlDbType.NVarChar).Value = name
        cmd.Parameters.Add("@Author", SqlDbType.NVarChar).Value = Author
        cmd.Parameters.Add("@Rate", SqlDbType.Int).Value = Rate
        cmd.CommandType = CommandType.Text
        conn.Open()
        cmd.ExecuteNonQuery()
        conn.Close()
    End Sub

End Class

