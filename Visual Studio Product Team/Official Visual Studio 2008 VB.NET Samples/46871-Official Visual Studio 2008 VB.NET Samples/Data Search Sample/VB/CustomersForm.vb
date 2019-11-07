' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class CustomersForm

    Private _customerIDValue As String
    Public Property CustomerID() As String
        Get
            Return _customerIDValue
        End Get
        Set(ByVal value As String)
            _customerIDValue = value
        End Set
    End Property

    Private Sub bindingNavigatorSaveItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Me.Validate()
        Me.CustomersBindingSource.EndEdit()
        Me.CustomersTableAdapter.Update(Me.NorthwindDataSet.Customers)
    End Sub

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadData()
    End Sub
    Public Sub LoadData()
        If Me.CustomerID.Trim().Length > 0 Then
            Me.CustomersTableAdapter.FillByCustomerID(Me.NorthwindDataSet.Customers, Me.CustomerID)
        End If
    End Sub
    Public Overloads Sub Show(ByVal customerId As String)
        Me.CustomerID = customerId
        LoadData()
        MyBase.Show()
        Me.BringToFront()
    End Sub

End Class