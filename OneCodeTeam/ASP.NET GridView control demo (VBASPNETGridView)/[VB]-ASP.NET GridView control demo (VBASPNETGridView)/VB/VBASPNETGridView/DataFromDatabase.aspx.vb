'****************************** Module Header ******************************\
' Module Name:  DataInMemory.aspx.vb
' Project:      VBASPNETGridView
' Copyright (c) Microsoft Corporation.
' 
' The DataInMemory sample describes how to populate ASP.NET GridView 
' control with simple DataTable and how to implement Insert, Edit, Update, 
' Delete, Paging and Sorting functions in ASP.NET GridView control. The 
' DataTable is stored in ViewState to persist data across postbacks. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/



Imports System.Data.SqlClient

Public Class DataFromDatabase
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' The Page is accessed for the first time.
        If Not IsPostBack Then
            ' Enable the GridView paging option and 
            ' specify the page size.
            gvPerson.AllowPaging = True
            gvPerson.PageSize = 15

            ' Enable the GridView sorting option.
            gvPerson.AllowSorting = True

            ' Initialize the sorting expression.
            ViewState("SortExpression") = "PersonID ASC"

            ' Populate the GridView.
            BindGridView()
        End If

    End Sub
    Private Sub BindGridView()
        ' Get the connection string from Web.config. 
        ' When we use Using statement, 
        ' we don't need to explicitly dispose the object in the code, 
        ' the using statement takes care of it.
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLServer2005DBConnectionString").ToString())
            ' Create a DataSet object.
            Dim dsPerson As New DataSet()

            ' Create a SELECT query.
            Dim strSelectCmd As String = "SELECT PersonID,LastName,FirstName FROM Person"

            ' Create a SqlDataAdapter object
            ' SqlDataAdapter represents a set of data commands and a 
            ' database connection that are used to fill the DataSet and 
            ' update a SQL Server database. 
            Dim da As New SqlDataAdapter(strSelectCmd, conn)

            ' Open the connection
            conn.Open()

            ' Fill the DataTable named "Person" in DataSet with the rows
            ' returned by the query.new n
            da.Fill(dsPerson, "Person")

            ' Get the DataView from Person DataTable.
            Dim dvPerson As DataView = dsPerson.Tables("Person").DefaultView

            ' Set the sort column and sort order.
            dvPerson.Sort = ViewState("SortExpression").ToString()

            ' Bind the GridView control.
            gvPerson.DataSource = dvPerson
            gvPerson.DataBind()
        End Using
    End Sub

    ' GridView.RowDataBound Event
    Protected Sub gvPerson_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        ' Make sure the current GridViewRow is a data row.
        If e.Row.RowType = DataControlRowType.DataRow Then
            ' Make sure the current GridViewRow is either 
            ' in the normal state or an alternate row.
            If e.Row.RowState = DataControlRowState.Normal OrElse e.Row.RowState = DataControlRowState.Alternate Then
                ' Add client-side confirmation when deleting.
                DirectCast(e.Row.Cells(1).Controls(0), LinkButton).Attributes("onclick") = "if(!confirm('Are you certain you want to delete this person ?')) return false;"
            End If
        End If
    End Sub

    ' GridView.PageIndexChanging Event
    Protected Sub gvPerson_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)
        ' Set the index of the new display page. 
        gvPerson.PageIndex = e.NewPageIndex

        ' Rebind the GridView control to 
        ' show data in the new page.
        BindGridView()
    End Sub

    ' GridView.RowEditing Event
    Protected Sub gvPerson_RowEditing(ByVal sender As Object, ByVal e As GridViewEditEventArgs)
        ' Make the GridView control into edit mode 
        ' for the selected row. 
        gvPerson.EditIndex = e.NewEditIndex

        ' Rebind the GridView control to show data in edit mode.
        BindGridView()

        ' Hide the Add button.
        lbtnAdd.Visible = False
    End Sub

    ' GridView.RowCancelingEdit Event
    Protected Sub gvPerson_RowCancelingEdit(ByVal sender As Object, ByVal e As GridViewCancelEditEventArgs)
        ' Exit edit mode.
        gvPerson.EditIndex = -1

        ' Rebind the GridView control to show data in view mode.
        BindGridView()

        ' Show the Add button.
        lbtnAdd.Visible = True
    End Sub

    ' GridView.RowUpdating Event
    Protected Sub gvPerson_RowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs)
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLServer2005DBConnectionString").ToString())
            ' Create a command object.
            Dim cmd As New SqlCommand()

            ' Assign the connection to the command.
            cmd.Connection = conn

            ' Set the command text
            ' SQL statement or the name of the stored procedure 
            cmd.CommandText = "UPDATE Person SET LastName = @LastName, FirstName = @FirstName WHERE PersonID = @PersonID"

            ' Set the command type
            ' CommandType.Text for ordinary SQL statements; 
            ' CommandType.StoredProcedure for stored procedures.
            cmd.CommandType = CommandType.Text

            ' Get the PersonID of the selected row.
            Dim strPersonID As String = gvPerson.Rows(e.RowIndex).Cells(2).Text
            Dim strLastName As String = DirectCast(gvPerson.Rows(e.RowIndex).FindControl("TextBox1"), TextBox).Text
            Dim strFirstName As String = DirectCast(gvPerson.Rows(e.RowIndex).FindControl("TextBox2"), TextBox).Text

            ' Append the parameters.
            cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = strPersonID
            cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = strLastName
            cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = strFirstName

            ' Open the connection.
            conn.Open()

            ' Execute the command.
            cmd.ExecuteNonQuery()
        End Using

        ' Exit edit mode.
        gvPerson.EditIndex = -1

        ' Rebind the GridView control to show data after updating.
        BindGridView()

        ' Show the Add button.
        lbtnAdd.Visible = True
    End Sub

    ' GridView.RowDeleting Event
    Protected Sub gvPerson_RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs)
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLServer2005DBConnectionString").ToString())
            ' Create a command object.
            Dim cmd As New SqlCommand()

            ' Assign the connection to the command.
            cmd.Connection = conn

            ' Set the command text
            ' SQL statement or the name of the stored procedure 
            cmd.CommandText = "DELETE FROM Person WHERE PersonID = @PersonID"

            ' Set the command type
            ' CommandType.Text for ordinary SQL statements; 
            ' CommandType.StoredProcedure for stored procedures.
            cmd.CommandType = CommandType.Text

            ' Get the PersonID of the selected row.
            Dim strPersonID As String = gvPerson.Rows(e.RowIndex).Cells(2).Text

            ' Append the parameter.
            cmd.Parameters.Add("@PersonID", SqlDbType.Int).Value = strPersonID

            ' Open the connection.
            conn.Open()

            ' Execute the command.
            cmd.ExecuteNonQuery()
        End Using

        ' Rebind the GridView control to show data after deleting.
        BindGridView()
    End Sub

    ' GridView.Sorting Event
    Protected Sub gvPerson_Sorting(ByVal sender As Object, ByVal e As GridViewSortEventArgs)
        Dim strSortExpression As String() = ViewState("SortExpression").ToString().Split(" "c)

        ' If the sorting column is the same as the previous one, 
        ' then change the sort order.
        If strSortExpression(0) = e.SortExpression Then
            If strSortExpression(1) = "ASC" Then
                ViewState("SortExpression") = Convert.ToString(e.SortExpression) & " " & "DESC"
            Else
                ViewState("SortExpression") = Convert.ToString(e.SortExpression) & " " & "ASC"
            End If
        Else
            ' If sorting column is another column,  
            ' then specify the sort order to "Ascending".
            ViewState("SortExpression") = Convert.ToString(e.SortExpression) & " " & "ASC"
        End If

        ' Rebind the GridView control to show sorted data.
        BindGridView()
    End Sub

    Protected Sub lbtnAdd_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Hide the Add button and showing Add panel.
        lbtnAdd.Visible = False
        pnlAdd.Visible = True
    End Sub

    Protected Sub lbtnSubmit_Click(ByVal sender As Object, ByVal e As EventArgs)
        Using conn As New SqlConnection(ConfigurationManager.ConnectionStrings("SQLServer2005DBConnectionString").ToString())
            ' Create a command object.
            Dim cmd As New SqlCommand()

            ' Assign the connection to the command.
            cmd.Connection = conn

            ' Set the command text
            ' SQL statement or the name of the stored procedure 
            cmd.CommandText = "INSERT INTO Person ( LastName, FirstName ) VALUES ( @LastName, @FirstName )"

            ' Set the command type
            ' CommandType.Text for ordinary SQL statements; 
            ' CommandType.StoredProcedure for stored procedures.
            cmd.CommandType = CommandType.Text

            ' Append the parameters.
            cmd.Parameters.Add("@LastName", SqlDbType.NVarChar, 50).Value = tbLastName.Text
            cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar, 50).Value = tbFirstName.Text

            ' Open the connection.
            conn.Open()

            ' Execute the command.
            cmd.ExecuteNonQuery()
        End Using

        ' Rebind the GridView control to show inserted data.
        BindGridView()

        ' Empty the TextBox controls.
        tbLastName.Text = ""
        tbFirstName.Text = ""

        ' Show the Add button and hiding the Add panel.
        lbtnAdd.Visible = True
        pnlAdd.Visible = False
    End Sub

    Protected Sub lbtnCancel_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Empty the TextBox controls.
        tbLastName.Text = ""
        tbFirstName.Text = ""

        ' Show the Add button and hiding the Add panel.
        lbtnAdd.Visible = True
        pnlAdd.Visible = False
    End Sub

End Class