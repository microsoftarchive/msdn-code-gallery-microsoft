Public Class Form1

    Private Sub bindingNavigatorSaveItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles bindingNavigatorSaveItem.Click
        Me.Order_DetailsBindingSource.EndEdit()
        Me.Order_DetailsTableAdapter.Update(Me.NorthwindDataSet.Order_Details)

    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'NorthwindDataSet.Order_Details' table. You can move, or remove it, as needed.
        Me.Order_DetailsTableAdapter.Fill(Me.NorthwindDataSet.Order_Details)

    End Sub
End Class