' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Data.SqlClient
Imports System.Data

Public Class MainForm

    Protected dvProducts As DataView

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        LoadData()
    End Sub

    ''' <summary>
    ''' Display a status message saying that we're attempting to connect to SQL Server.
    ''' This only needs to be done the very first time a connection is
    ''' attempted.  After we've determined that SQL Express or SQL Server is
    ''' installed, this message no longer needs to be displayed.
    ''' </summary>
    Private Sub LoadData()

        Dim frmStatusMessage As New Status()

        frmStatusMessage.Show("Connecting to SQL Server")

        Try
            Me.ProductsTableAdapter.Fill(Me.NorthwindDataSet.Products)

            ' Create the dataview; use a constructor to specify
            ' the sort, filter criteria for performance purposes
            dvProducts = New DataView(Me.NorthwindDataSet.Products, "", "ProductName ASC", DataViewRowState.OriginalRows)
        Catch ex As Exception
            ' Unable to connect to SQL Server or SQL Express
            frmStatusMessage.Close()
            Dim strMessage As String = "To run this sample, you must have SQL " & _
            "or SQL Express with the Northwind database installed.  " & _
            "To change the connection string, open the Settings Designer " & _
            "by double clicking on My Project in the Solution Explorer, " & _
            "select the Settings tab and change the value for the NorthwindConnectionString. " & vbCrLf & _
            " For instructions on installing SQL Express view ReadMe."
            MessageBox.Show(strMessage, "Bind Data to a ComboBox", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ' Quit the program; could not connect to either SQL Server 
            Application.Exit()
        End Try

        frmStatusMessage.Close()
    End Sub

    ''' <summary>
    ''' Bind to a simple array of string entries for colors.
    ''' </summary>
    Private Sub btnArray_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnArray.Click
        Dim myColors() As String = {"AQUA", "BLACK", "BLUE", "GREEN", "RED", "WHITE", "YELLOW"}

        ComboBox1.DataSource = myColors
        ComboBox1.SelectedIndex = 0
        lblDataSource.Text = "Array"
    End Sub

    ''' <summary>
    ''' Bind to a simple arraylist that has entries for different shapes.
    ''' </summary>
    Private Sub btnArrayList_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnArrayList.Click
        Dim myShapes As New ArrayList
        With myShapes
            .Add("Circle")
            .Add("Octagon")
            .Add("Rectangle")
            .Add("Squre")
            .Add("Trapezoid")
            .Add("Triange")
        End With

        ComboBox1.DataSource = myShapes
        ComboBox1.SelectedIndex = 0
        lblDataSource.Text = "ArrayList"
    End Sub

    ''' <summary>
    ''' Bind to an arraylist that contains entries based on the structure
    ''' that has been defined for sales divisions.
    ''' </summary>
    Private Sub btnArrayListA_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnArrayListA.Click

        Dim myDivisions As New ArrayList

        ' Add division structure entries to the arraylist
        With myDivisions
            .Add(New Divisions("CENTRAL", 1))
            .Add(New Divisions("EAST", 2))
            .Add(New Divisions("NORTH", 3))
            .Add(New Divisions("SOUTH", 4))
        End With

        ' Bind the arraylist to the combo box
        With ComboBox1
            .DataSource = myDivisions
            .DisplayMember = "Name"
            .ValueMember = "Id"
        End With

        ComboBox1.SelectedIndex = 0
        lblDataSource.Text = "ArrayList - Advanced"
        lblAssocValue.Text = ComboBox1.SelectedValue.ToString
    End Sub

    ''' <summary>
    ''' Bind to the products table from the Northwind database that has 
    ''' previously been loaded into the Northwind DataSet.
    ''' Note that the table has not been sorted in any particular order.
    ''' </summary>
    Private Sub btnDS_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDS.Click
        With ComboBox1
            .DataSource = Me.NorthwindDataSet.Products
            .DisplayMember = "ProductName"
            .ValueMember = "ProductID"
        End With

        ComboBox1.SelectedIndex = 0
        lblDataSource.Text = "DataSet"
        lblAssocValue.Text = ComboBox1.SelectedValue.ToString
    End Sub

    Private Sub btnDV_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDV.Click
        ' Bind to the sorted view of the products table
        'on the product name column, ascending.  
        With ComboBox1
            .DataSource = dvProducts
            .DisplayMember = "ProductName"
            .ValueMember = "ProductID"
        End With

        ComboBox1.SelectedIndex = 0
        lblDataSource.Text = "DataView"
        lblAssocValue.Text = ComboBox1.SelectedValue.ToString
    End Sub

    ''' <summary>
    ''' Bind to the BindingSource that binds to the NorthwindDataset Products table.
    ''' </summary>
    Private Sub btnDC_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnDC.Click
        With ComboBox1
            .DataSource = Me.ProductsBindingSource
            .DisplayMember = "ProductName"
            .ValueMember = "ProductID"
        End With

        ComboBox1.SelectedIndex = 0
        lblDataSource.Text = "BindingSource"
        lblAssocValue.Text = ComboBox1.SelectedValue.ToString
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ComboBox1.SelectedIndexChanged
        ' Display the associated value for the item selected from the combox,
        ' if one is available. To determine a value is available
        ' check the visibility of the groupbox. This attribute has been set to false
        ' during binding by code if a value is not available, to true if it is.
        If Me.ComboBox1.SelectedIndex >= 0 Then
            lblAssocValue.Text = ComboBox1.SelectedValue.ToString
        Else
            lblAssocValue.Text = "Nothing selected"
        End If
    End Sub

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub ProductsBindingNavigatorSaveItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProductsBindingNavigatorSaveItem.Click
        Me.Validate()
        Me.ProductsBindingSource.EndEdit()
        Me.ProductsTableAdapter.Update(Me.NorthwindDataSet.Products)
    End Sub
End Class
