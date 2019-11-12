

Partial Public Class NorthwindDataSet
    Partial Class Order_DetailsDataTable

        Private Sub Order_DetailsDataTable_ColumnChanging(ByVal sender As System.Object, ByVal e As System.Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.QuantityColumn.ColumnName) Then
                If CInt(e.ProposedValue) <= 0 Then
                    e.Row.SetColumnError(e.Column, "Quantity must be greater than 0")
                Else
                    e.Row.SetColumnError(e.Column, "")
                End If
            End If
        End Sub

    End Class

End Class
