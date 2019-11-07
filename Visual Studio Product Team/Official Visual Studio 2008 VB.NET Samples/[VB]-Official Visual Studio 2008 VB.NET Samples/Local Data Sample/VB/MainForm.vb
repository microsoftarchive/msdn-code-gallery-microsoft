' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm

    Private Sub bindingNavigatorSaveItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OrdersBindingNavigator.Click
        Me.Validate()
        Me.OrdersBindingSource.EndEdit()
        Me.OrdersTableAdapter.Update(Me.NwindDataSet.Orders)
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'NwindDataSet.Orders' table. You can move, or remove it, as needed.
        Me.OrdersTableAdapter.Fill(Me.NwindDataSet.Orders)
    End Sub
End Class