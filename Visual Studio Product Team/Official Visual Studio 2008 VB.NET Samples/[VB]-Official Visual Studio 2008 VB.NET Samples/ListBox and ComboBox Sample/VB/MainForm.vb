' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.IO
Imports System.Data.SqlClient

Public Class MainForm

#Region " Form Load"
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LoadValues()
    End Sub

    Private Sub LoadValues()
        ' Populate the ComboBox with a list of values from an Enum
        cboDropDownStyle.DataSource = System.Enum.GetNames(GetType(ComboBoxStyle))
        cboDropDownStyle.DropDownStyle = ComboBoxStyle.DropDown
        ' Set properties of cboDemo
        With nudDropDownWidth
            .Value = cboDemo.DropDownWidth
            .Minimum = cboDemo.Width
            .Maximum = cboDemo.Width * 2
        End With

        nudDropDownItems.Value = cboDemo.MaxDropDownItems
    End Sub
#End Region

#Region " Code for Add Items tab "
    Private Sub btnFill1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFill1.Click
        AddItems()
    End Sub

    Private Sub AddItems()
        ' You can add items individually to a list or combo box.
        ' Because you can add any type of object, it's up to you to determine
        ' which property of the object is to be displayed. Set the 
        ' DisplayMember property to indicate the name of the property to display.
        ' In this case, set the DisplayMember property to display the ProcessName
        ' property.

        ' ValueMember property only works if you specify the DataSource
        ' property of the control.

        Try
            Dim prc As Process
            ' Remove existing items from the control.
            ' Notice we clear the Items collection, rather then the ListBox
            lstProcessesAddItem.Items.Clear()

            ' Fill the control. Indicate which member of 
            ' the items added to the ListBox.DataSource should be
            ' displayed.
            lstProcessesAddItem.DisplayMember = "ProcessName"
            For Each prc In Process.GetProcesses
                lstProcessesAddItem.Items.Add(prc)
            Next
            lstProcessesAddItem.Sorted = True
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OKOnly, Me.Text)
        End Try
    End Sub

    Private Sub lstProcessesAddItem_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstProcessesAddItem.SelectedIndexChanged
        ' Because you haven't set the DataSource property of the control, you can't
        ' retrieve the SelectedValue property. Instead, use the SelectedItem property,'
        ' and display the property you want.
        Try
            ' Typically you'll want to verify a value is actually selected before
            ' attempting to read an item out of the list.
            If lstProcessesAddItem.SelectedIndex > -1 Then
                lblFileName1.Text = CType(lstProcessesAddItem.SelectedItem, Process).MainModule.FileName
            Else
                lblFileName1.Text = String.Empty
            End If
        Catch ex As Exception
            ' In this case, do nothing if an exception occurs.
            lblFileName1.Text = String.Empty
        End Try
    End Sub
#End Region

#Region " Code for Bind to DataTable tab "
    Private Sub btnFill2_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFill2.Click
        ' Bind a ListBox to a DataTable containing information about files. 
        ' This could just as easily come from a real data source (such as SQL Server).
        '  In addition, you can bind a ListBox or ComboBox to many other 
        ' data sources -- see the IList interface in Help.
        Try
            Dim dt As DataTable = FillTable(My.Computer.FileSystem.SpecialDirectories.MyDocuments)

            If Not (dt Is Nothing) Then
                With lstFiles
                    .DisplayMember = "FileName"
                    .ValueMember = "Length"
                    .DataSource = dt
                End With
            Else
                lblFileInfo.Text = "Nothing in the array"
            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message, Me.Text)
        End Try

    End Sub

    ''' <summary>
    ''' Build a DataTable filled with information about files on your hard drive.
    ''' </summary>
    ''' <param name="path"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function FillTable(ByVal path As String) As DataTable
        Dim dt As New DataTable
        Dim dr As DataRow

        Try
            dt.Columns.Add("FileName", GetType(System.String))
            dt.Columns.Add("Length", GetType(System.Int64))

            ' This method uses My instead of System.IO directly.
            For Each FilePath As String In My.Computer.FileSystem.GetFiles(path)
                dr = dt.NewRow
                dr("FileName") = My.Computer.FileSystem.GetFileInfo(FilePath).Name
                dr("Length") = My.Computer.FileSystem.GetFileInfo(FilePath).Length
                dt.Rows.Add(dr)
            Next
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OKOnly, Me.Text)
        End Try

        Return dt
    End Function

    Private Sub lstFiles_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstFiles.SelectedIndexChanged
        ' Display information about the selected file.
        ' The ValueMember property is set to the Length field in the 
        ' DataTable filling the control.
        If lstFiles.SelectedIndex > -1 Then
            lblFileInfo.Text = "Length: " & lstFiles.SelectedValue.ToString
        Else
            lblFileInfo.Text = String.Empty
        End If
    End Sub
#End Region

#Region " Code for Selection Mode tab "
    Private Sub btnFill3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFill3.Click
        FillSelectionMode()
    End Sub

    Private Sub FillSelectionMode()
        Try
            With lstMultiSelect
                .DisplayMember = "Name"
                .ValueMember = "Length"
                .DataSource = My.Computer.FileSystem.GetDirectoryInfo(My.Computer.FileSystem.SpecialDirectories.MyDocuments).GetFiles()
                ' Initialize the combo box containing the 
                ' different selection modes:
                cboSelectionMode.Text = .SelectionMode.ToString
            End With
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OKOnly, Me.Text)
        End Try
    End Sub

    Private Sub cboSelectionMode_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboSelectionMode.SelectedIndexChanged
        ' Allow the user to select from one of the selection modes:
        ' One, MultipleSimple, MultipleExtended
        Try
            lstMultiSelect.ClearSelected()
            lstMultiSelect.SelectionMode = _
             CType(System.Enum.Parse(GetType(SelectionMode), cboSelectionMode.Text), _
             SelectionMode)

        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OKOnly, Me.Text)
        End Try
    End Sub

    Private Sub lstMultiSelect_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstMultiSelect.SelectedIndexChanged
        ' Display a list of selected indices.
        ' The SelectedIndices property returns a SelectedIndexCollection
        ' object. Use its CopyTo method to copy the items to 
        ' an array, so you can bind the list to a ListBox control.
        Try
            Dim aIndices(lstMultiSelect.SelectedIndices.Count - 1) As Integer
            lstMultiSelect.SelectedIndices.CopyTo(aIndices, 0)
            lstSelected.DataSource = aIndices

            ' Demonstrate how to "walk" the selected items list.
            With lstSelectedItems
                .Items.Clear()
                ' Begin/EndUpdate turn off/on display of the control
                ' as you're adding items. Just makes the update "cleaner".
                .BeginUpdate()
                Dim fi As FileInfo
                For Each fi In lstMultiSelect.SelectedItems
                    .Items.Add(fi.Name)
                Next
                .EndUpdate()
            End With
        Catch ex As Exception
            lstSelected.DataSource = Nothing
        End Try
    End Sub
#End Region

#Region " Code for Bind to Array tab"
    Private Sub btnFill4_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnFill4.Click
        BindToArray()
    End Sub

    Private Sub BindToArray()
        ' Binding to an array is simpler -- just set the 
        ' DataSource property of the control. In this case, you 
        ' can also set the ValueMember property:

        Try
            With lstProcessesDataSource
                .ValueMember = "MainModule"
                .DisplayMember = "ProcessName"
                .DataSource = Process.GetProcesses
            End With
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OKOnly, Me.Text)
        End Try
    End Sub

    Private Sub lstProcessesDataSource_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lstProcessesDataSource.SelectedIndexChanged
        ' Because the ValueMember property was set to MainModule
        ' you can retrieve the SelectedValue property of the 
        ' control. 
        Try
            If lstProcessesDataSource.SelectedIndex > -1 Then
                lblFileName2.Text = CType(lstProcessesDataSource.SelectedValue, System.Diagnostics.Process).Modules(0).FileName
            Else
                lblFileName2.Text = String.Empty
            End If
        Catch ex As Exception
            ' In this case, do nothing if an exception occurs.
            lblFileName2.Text = String.Empty
        End Try
    End Sub
#End Region

#Region " Code for ComboBox tab "

    Private Sub btnFill5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFill5.Click
        BindToDataSet()
    End Sub

    Private Sub BindToDataSet()
        Try
            LoadData()
            cboDemo.DataSource = Me.NorthwindDataSet.Employees
            cboDemo.DisplayMember = "FirstName"
            cboDemo.ValueMember = "EmployeeID"
            lblResults.Text = ""
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OKOnly, Me.Text)
        End Try
    End Sub

    Private Sub LoadData()
        ' Display a status message saying that we're attempting to connect to SQL Server.
        Dim statusForm As New Status()
        statusForm.Show("Connecting to SQL Server")

        ' Attempt to connect first to the local SQL server instance.  
        Try
            Me.EmployeesTableAdapter.Fill(Me.NorthwindDataSet.Employees)
            ' Data has been retrieved successfully.
        Catch ex As Exception
            ' Unable to connect to SQL Server.
            Dim message As String = "To run this sample, you must have SQL " & _
            "Server with the Northwind database installed."
            MsgBox(message, MsgBoxStyle.OkOnly, Me.Text)
        End Try

        statusForm.Close()
    End Sub

    Private Sub cboDemo_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboDemo.SelectedIndexChanged
        If cboDemo.SelectedIndex > -1 Then
            lblResults.Text = cboDemo.SelectedValue.ToString
        Else
            lblResults.Text = String.Empty
        End If
    End Sub

    Private Sub cboDropDownStyle_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboDropDownStyle.SelectedIndexChanged
        Try
            ' Retrieve the enumerated value from the combo box, 
            ' parsing the text in the combo box.
            cboDemo.DropDownStyle = _
              CType(System.Enum.Parse(GetType(ComboBoxStyle), cboDropDownStyle.Text), _
                ComboBoxStyle)
        Catch ex As Exception
            MsgBox(ex.Message, MsgBoxStyle.OKOnly, Me.Text)
        End Try
    End Sub

    Private Sub nudDropDownItems_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles nudDropDownItems.ValueChanged
        cboDemo.MaxDropDownItems = CInt(nudDropDownItems.Value)
    End Sub

    Private Sub nudDropDownWidth_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles nudDropDownWidth.ValueChanged
        cboDemo.DropDownWidth = CInt(nudDropDownWidth.Value)
    End Sub
#End Region

#Region " Code for BindingSource "
    Private Sub BindingSourceFillButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BindingSourceFillButton.Click
        ' When using a BindingSource you can change the DataSource
        ' on the BindingSource without having to reset all the bound controls
        ' In this case, the ComboBox and the two text boxes are rebound 
        ' to the new list
        Me.CustomerBindingSource.DataSource = GetCustomerList()
    End Sub
    ''' <summary>
    ''' Gets a collection of typed Customers
    ''' </summary>
    ''' <returns>Return a typed list of Customer objects</returns>
    ''' <remarks>You could use List(Of T) as well.  BindingList(Of T) provides additional events that may be useful</remarks>
    Private Function GetCustomerList() As System.ComponentModel.BindingList(Of Customer)
        ' Create a new typed list of customers
        Dim customerList As New System.ComponentModel.BindingList(Of Customer)
        ' Add each customer to the list using the Customer constructor
        customerList.Add(New Customer(1, "Alfreds Futterkiste"))
        customerList.Add(New Customer(2, "Ana Trujillo Emparedados y helados"))
        customerList.Add(New Customer(3, "Antonio Moreno Taquera"))
        customerList.Add(New Customer(4, "Around the Horn"))
        customerList.Add(New Customer(5, "Cactus Comidas para llevar"))

        ' in this case we just create a new customer, set properties
        ' then add it to our typed list
        Dim newCustomer As New Customer()
        newCustomer.Id = 6
        newCustomer.Name = "Ernst Handel"
        customerList.Add(newCustomer)
        Return customerList
    End Function
#End Region

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub

End Class
