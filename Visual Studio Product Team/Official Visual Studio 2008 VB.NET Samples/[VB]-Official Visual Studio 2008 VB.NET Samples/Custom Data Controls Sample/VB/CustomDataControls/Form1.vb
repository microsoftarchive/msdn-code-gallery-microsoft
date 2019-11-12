' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class Form1

    Private Sub bindingNavigatorSaveItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bindingNavigatorSaveItem.Click
        Me.EmployeesBindingSource.EndEdit()
        Me.EmployeesTableAdapter.Update(Me.NorthwindDataSet.Employees)

    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'NorthwindDataSet.Employees' table. You can move, or remove it, as needed.
        Me.EmployeesTableAdapter.Fill(Me.NorthwindDataSet.Employees)

    End Sub
End Class
