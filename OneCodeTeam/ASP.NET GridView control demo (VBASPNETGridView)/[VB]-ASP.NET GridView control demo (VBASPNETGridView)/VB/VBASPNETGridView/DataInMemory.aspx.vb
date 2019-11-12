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



Public Class DataInMemory
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' The Page is accessed for the first time.
        If Not IsPostBack Then
            ' Initialize the DataTable and store it in ViewState.
            InitializeDataSource()

            ' Enable the GridView paging option and specify the page size.
            gvPerson.AllowPaging = True
            gvPerson.PageSize = 5

            ' Enable the GridView sorting option.
            gvPerson.AllowSorting = True

            ' Initialize the sorting expression.
            ViewState("SortExpression") = "PersonID ASC"

            ' Populate the GridView.
            BindGridView()
        End If

    End Sub
    ' Initialize the DataTable.
    Private Sub InitializeDataSource()
        ' Create a DataTable object named dtPerson.
        Dim dtPerson As New DataTable()

        ' Add four columns to the DataTable.
        dtPerson.Columns.Add("PersonID")
        dtPerson.Columns.Add("LastName")
        dtPerson.Columns.Add("FirstName")

        ' Specify PersonID column as an auto increment column
        ' and set the starting value and increment.
        dtPerson.Columns("PersonID").AutoIncrement = True
        dtPerson.Columns("PersonID").AutoIncrementSeed = 1
        dtPerson.Columns("PersonID").AutoIncrementStep = 1

        ' Set PersonID column as the primary key.
        Dim dcKeys As DataColumn() = New DataColumn(0) {}
        dcKeys(0) = dtPerson.Columns("PersonID")
        dtPerson.PrimaryKey = dcKeys

        ' Add new rows into the DataTable.
        dtPerson.Rows.Add(Nothing, "Davolio", "Nancy")
        dtPerson.Rows.Add(Nothing, "Fuller", "Andrew")
        dtPerson.Rows.Add(Nothing, "Leverling", "Janet")
        dtPerson.Rows.Add(Nothing, "Dodsworth", "Anne")
        dtPerson.Rows.Add(Nothing, "Buchanan", "Steven")
        dtPerson.Rows.Add(Nothing, "Suyama", "Michael")
        dtPerson.Rows.Add(Nothing, "Callahan", "Laura")

        ' Store the DataTable in ViewState. 
        ViewState("dtPerson") = dtPerson
    End Sub

    Private Sub BindGridView()
        If ViewState("dtPerson") IsNot Nothing Then
            ' Get the DataTable from ViewState.
            Dim dtPerson As DataTable = DirectCast(ViewState("dtPerson"), DataTable)

            ' Convert the DataTable to DataView.
            Dim dvPerson As New DataView(dtPerson)

            ' Set the sort column and sort order.
            dvPerson.Sort = ViewState("SortExpression").ToString()

            ' Bind the GridView control.
            gvPerson.DataSource = dvPerson
            gvPerson.DataBind()
        End If
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
        If ViewState("dtPerson") IsNot Nothing Then
            ' Get the DataTable from ViewState.
            Dim dtPerson As DataTable = DirectCast(ViewState("dtPerson"), DataTable)

            ' Get the PersonID of the selected row.
            Dim strPersonID As String = gvPerson.Rows(e.RowIndex).Cells(2).Text

            ' Find the row in DateTable.
            Dim drPerson As DataRow = dtPerson.Rows.Find(strPersonID)

            ' Retrieve edited values and updating respective items.
            drPerson("LastName") = DirectCast(gvPerson.Rows(e.RowIndex).FindControl("TextBox1"), TextBox).Text
            drPerson("FirstName") = DirectCast(gvPerson.Rows(e.RowIndex).FindControl("TextBox2"), TextBox).Text

            ' Exit edit mode.
            gvPerson.EditIndex = -1

            ' Rebind the GridView control to show data after updating.
            BindGridView()

            ' Show the Add button.
            lbtnAdd.Visible = True
        End If
    End Sub

    ' GridView.RowDeleting Event
    Protected Sub gvPerson_RowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs)
        If ViewState("dtPerson") IsNot Nothing Then
            ' Get the DataTable from ViewState.
            Dim dtPerson As DataTable = DirectCast(ViewState("dtPerson"), DataTable)

            ' Get the PersonID of the selected row.
            Dim strPersonID As String = gvPerson.Rows(e.RowIndex).Cells(2).Text

            ' Find the row in DateTable.
            Dim drPerson As DataRow = dtPerson.Rows.Find(strPersonID)

            ' Remove the row from the DataTable.
            dtPerson.Rows.Remove(drPerson)

            ' Rebind the GridView control to show data after deleting.
            BindGridView()
        End If
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
        If ViewState("dtPerson") IsNot Nothing Then
            ' Get the DataTable from ViewState and inserting new data to it.
            Dim dtPerson As DataTable = DirectCast(ViewState("dtPerson"), DataTable)

            ' Add the new row.
            dtPerson.Rows.Add(Nothing, tbLastName.Text, tbFirstName.Text)

            ' Rebind the GridView control to show inserted data.
            BindGridView()
        End If

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