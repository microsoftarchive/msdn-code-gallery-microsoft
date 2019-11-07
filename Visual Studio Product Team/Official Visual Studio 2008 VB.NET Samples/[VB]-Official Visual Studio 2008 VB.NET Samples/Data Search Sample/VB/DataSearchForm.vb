' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class DataSearchForm

    Private Sub bindingNavigatorSaveItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bindingNavigatorSaveItem.Click
        Me.CustomersBindingSource.EndEdit()
        Me.CustomersTableAdapter.Update(Me.NorthwindDataSet.Customers)

    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub searchToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles searchToolStripButton.Click
        Me.CustomersTableAdapter.FillByCompanyName(Me.NorthwindDataSet.Customers, searchToolStripTextBox.Text)
    End Sub

    Private Sub showAllToolStripButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles showAllToolStripButton.Click
        Me.CustomersTableAdapter.Fill(Me.NorthwindDataSet.Customers)
    End Sub

    Private Sub CustomersDataGridView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles CustomersDataGridView.DoubleClick

        Dim row As NorthwindDataSet.CustomersRow
        row = CType(CType(Me.CustomersBindingSource.Current, DataRowView).Row, NorthwindDataSet.CustomersRow)
        My.Forms.CustomersForm.Show(row.CustomerID)
    End Sub

End Class
