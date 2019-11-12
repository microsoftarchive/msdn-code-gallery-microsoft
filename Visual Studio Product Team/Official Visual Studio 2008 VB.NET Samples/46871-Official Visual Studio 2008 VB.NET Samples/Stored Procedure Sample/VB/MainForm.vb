' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Data
Imports System.Data.SqlClient
Imports System.Text
Imports System.Text.RegularExpressions
Public Class MainForm
    Private _hasCreatedSprocs As Boolean = False
    Private Function ParseScriptToCommands(ByVal script As String) As String()
        Dim commands() As String
        commands = Regex.Split(script, _
                                "GO\r\n", _
                                RegexOptions.IgnoreCase)
        Return commands
    End Function

#Region "Create database, tables, and procedures"
    ' This subroutine handles the click event for the Create Sprocs button, found 
    ' on the Create Sprocs tab. This routine uses classes from the 
    ' System.Data.SqlClient namespace to execute SQL statements that drop a stored 
    ' procedure (if it exists) and then create it.
    Private Sub btnCreateSprocs_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateDatabase.Click

        Me.Cursor = Cursors.WaitCursor
        Try
            ' Open the connection and leave it open until last SQL statement
            ' is executed.
            Using conn As New SqlConnection(My.Settings.MasterInstanceConnectionString)
                conn.Open()
                CreateDatabase(conn)
                btnCreateDatabase.Enabled = False
            End Using
        Catch sqlExc As SqlException
            MsgBox(sqlExc.ToString, MsgBoxStyle.OkOnly, "SQL Exception Error")
        Catch ex As Exception
            MsgBox("To run this sample, you must have SQL Express or SQL Server " & _
                        "and the Northwind database installed." & vbCrLf & _
                        ex.ToString(), MsgBoxStyle.Critical, "Connection failed.")
        Finally
            Me.Cursor = Cursors.Default
        End Try
    End Sub
#End Region

#Region "Create database and table"
    ' This routine executes a SQL statement that drops the database (if it exists) 
    ' and then creates it. 
    Private Sub CreateDatabase(ByVal connection As SqlConnection)

        ' A SqlCommand object is used to execute the SQL commands.
        Using cmd As New SqlCommand()
            cmd.Connection = connection

            cmd.CommandText = My.Resources.DataBaseCreationScripts.CreateDatabase
            cmd.ExecuteNonQuery()
            ' Get a collection of commands to execute against the database
            Dim commands() As String = Nothing
            commands = ParseScriptToCommands(My.Resources.DataBaseCreationScripts.CreateEmployeesTable)
            For Each command As String In commands
                cmd.CommandText = command
                cmd.ExecuteNonQuery()
            Next

            commands = ParseScriptToCommands(My.Resources.DataBaseCreationScripts.CreateSprocs)
            ' Open the connection, execute the command, and close the 
            ' connection. It is more efficient to ExecuteNonQuery when data is 
            ' not being returned.
            ' Since the commands are more script related as they perform several tasks
            ' Leverage My.Resources to store a script for each commandText
            ' This enables debugging of creation scripts, 
            ' but(isn) 't recomended for typcial query commands
            For Each command As String In commands
                If command.Trim().Length > 0 Then
                    cmd.CommandText = command
                    cmd.ExecuteNonQuery()
                End If
            Next
        End Using

        MsgBox("Database 'StoredProceduresDemo' successfully created.", _
            MsgBoxStyle.OkOnly, "Database Creation Status")
        _hasCreatedSprocs = True
    End Sub
#End Region

#Region "Get Count of Output Params Tab"

    ' This subroutine handles the Click event for the Get Count 
    ' button, found on the All Types tab. The stored procedure used
    ' here requires a single input parameter and sends back both an output
    ' parameter value as well as a return value. After the sproc is executed a 
    ' Label is used to dislay the results.
    Private Sub btnGetCountInCountry_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetCountInCountry.Click

        Dim dbConnection As New SqlConnection(My.Settings.NorthwindConnectionString)
        Dim command As New SqlCommand("CountPeopleInCountry", dbConnection)
        Dim adapter As New SqlDataAdapter(command)
        Dim dsEmployees As New DataSet()

        command.CommandType = CommandType.StoredProcedure

        ' Add the parameter required by the stored procedure. Set the input
        ' parameter's value to that of the ComboBox control's selected value.
        ' Set the Direction of the output parameter. Finally, add a paramter
        ' to capture the return value, i.e., the value sent back from the 
        ' stored procedure by the RETURN statement (or, if RETURN is not 
        ' explicitly used in the stored procedure, the default success/fail
        ' code returned by SQL Server).
        With command.Parameters
            .Add(New SqlParameter("@Country", SqlDbType.NVarChar)).Value = _
                cboCountries.SelectedValue
            ' By default, the value of the Direction property is "Input",
            ' so only in the next two parameters does Direction need to be
            ' explicitly set.
            .Add(New SqlParameter("@CountInCountry", _
                SqlDbType.Money)).Direction = ParameterDirection.Output
            ' Instead of adding a SqlParameter to obtain the Return value you
            ' can also just declare an integer variable and initialize it to
            ' the value returned by the SqlDataAdapter Fill method, or 
            ' whatever method you are using to execute the SQL statement
            ' (as was done with the CREATE "GetEmployees" stored procedure,
            ' above).
            .Add(New SqlParameter("ReturnValue", _
                SqlDbType.Int)).Direction = ParameterDirection.ReturnValue
        End With

        Try
            ' For comments on this line of code see the Try...Catch block
            ' in the frmMain Load event handler.
            adapter.Fill(dsEmployees, "Employee")

        Catch expSQL As SqlException
            MsgBox(expSQL.ToString, MsgBoxStyle.OkOnly, "SQL Exception")
            Exit Sub
        End Try

        ' Display the results.
        lblProductCountAndAvgPrice.Text = _
            String.Format("There are {0} employees in {1}.", _
                command.Parameters("ReturnValue").Value, _
                cboCountries.Text)
    End Sub
#End Region

#Region "Get Employees of Input Param tab"
    ''' <summary>
    ''' This subroutine handles the Click event for the Get Employees button,
    ''' found on the Input Param tab. The "GetEmployees" stored procedure requires
    ''' a single input parameter. The sproc is executed and a DataGrid used to
    ''' dislay the results.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub btnGetEmployees_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetEmployees.Click
        Dim employeesTableAdapter As New StoredProceduresDemoDataSetTableAdapters.GetEmployeesByNameTableAdapter()
        Dim typedDataset As New StoredProceduresDemoDataSet()

        ' Add the parameter required by the stored procedure. Set the input
        ' parameter's value to that of the ComboBox control's selected value.
        ' The following syntax is the long way of adding a SqlParameter and
        ' setting its properties. Look at btnGetProductCountAndAvgPrice_Click
        ' event handler for a much shorter syntax.
        Try
            ' Fill the DataSet using the TableAdapter typed methods
            employeesTableAdapter.Fill(typedDataset.GetEmployeesByName, CStr(cboCategoriesInputParam.SelectedValue))
        Catch expSQL As SqlException
            MsgBox(expSQL.ToString, MsgBoxStyle.OkOnly, "SQL Exception")
            Exit Sub
        End Try

        ' Bind the DataGridView to the desired table in the DataSet. This
        ' will cause the product information to display.
        productsGrid.DataSource = typedDataset.GetEmployeesByName

    End Sub
#End Region

#Region "Get First Names of No Params tab"
    ' This subroutine handles the Click event for the List of First Names button,
    ' found on the No Params tab. The stored procedure used here requires no  
    ' input parameters.
    Private Sub getListOfFirstNames_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles getListOfFirstNames.Click

        Dim sb As New StringBuilder()
        Using conn As New SqlConnection(My.Settings.NorthwindConnectionString)
            ' The SqlCommand object is responsible for executing SQL statements
            ' and managing the associated SQL objects, like parameters. With ad
            ' hoc SQL you would pass the SQL statement as the first argument to 
            ' the constructor. But when using stored procedures, you simply pass
            ' the name of the stored procedure and set the CommandType property 
            ' to the CommandType.StoredProcedure enum.
            Dim scmd As New SqlCommand("GetFirstNames", conn)
            scmd.CommandType = CommandType.StoredProcedure

            ' Open the connection to the database and execute the command, also passing 
            ' in an enum value that immediately closes the connection. This is an option
            ' to explicitly calling dbConnection.Close().
            conn.Open()
            Using dataReader As SqlDataReader = scmd.ExecuteReader(CommandBehavior.CloseConnection)
                ' Instantiate a StringBuilder to concatenate the strings displayed in the
                ' TextBox. The StringBuilder class is optimized for concatenation and is
                ' to be preferred over the traditional &= approach.
                sb.AppendLine("First Name")
                sb.AppendLine("========================")
                ' Loop through the contents of the SqlDataReader object.
                While dataReader.Read
                    sb.AppendLine(dataReader.GetString(0))
                End While
            End Using
        End Using

        ' Display the results.
        txtTenMostExpProds.Text = sb.ToString
    End Sub
#End Region

#Region "Tab changed event"
    ' This subroutine handles the SelectedIndexChanged for the TabControl. 
    ' It ensures that the user cannot run any of the examples prior to creating 
    ' the stored procedures. If the sprocs have been created, the "GetEmployees" 
    ' sproc is executed to fill the product Categories ComboBox controls on the 
    ' Input Param and All Types tab pages.
    Private Sub tabApp_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles tabApp.SelectedIndexChanged
        If Not _hasCreatedSprocs AndAlso tabApp.SelectedTab.TabIndex > 0 Then
            MessageBox.Show("You must first create the required stored procedures.", _
                Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Stop)
            tabApp.SelectedIndex = 0
        ElseIf _hasCreatedSprocs AndAlso cboCategoriesInputParam.Items.Count = 0 Then
            Me.FillFirstNames()
            Me.FillCountries()
        End If
    End Sub

    Sub FillFirstNames()
        ' Fill ComboBox controls only if they are empty and the sprocs have 
        ' been created.

        ' See other event handlers for comments about the following
        ' lines of code.
        Using conn As New SqlConnection(My.Settings.NorthwindConnectionString)
            Dim sql As String = _
                "GetFirstNames"
            Dim scmd As New SqlCommand(sql, conn)
            Dim sda As New SqlDataAdapter(scmd)
            Dim dsEmployees As New DataSet
            scmd.CommandType = CommandType.StoredProcedure

            Try
                ' Fill the DataSet.
                sda.Fill(dsEmployees)
            Catch expSql As SqlException
                MsgBox(expSql.ToString, MsgBoxStyle.OkOnly, "SQL Exception")
            End Try

            ' Bind the data to the ComboBox controls
            With cboCategoriesInputParam
                .DataSource = dsEmployees.Tables(0)
                .DisplayMember = "FirstName"
                .ValueMember = "FirstName"
            End With
        End Using

    End Sub

    Sub FillCountries()
        ' Fill ComboBox controls only if they are empty and the sprocs have 
        ' been created.
        Using conn As New SqlConnection(My.Settings.NorthwindConnectionString)
            Dim sql As String = _
                "GetCountryNames"
            Dim scmd As New SqlCommand(sql, conn)
            Dim sda As New SqlDataAdapter(scmd)
            Dim dsEmployees As New DataSet
            scmd.CommandType = CommandType.StoredProcedure

            Try
                ' Fill the DataSet.
                sda.Fill(dsEmployees)
            Catch expSql As SqlException
                MsgBox(expSql.ToString, MsgBoxStyle.OkOnly, "SQL Exception")
                Exit Sub
            End Try

            ' Bind the data to the ComboBox controls
            With cboCountries
                .DataSource = dsEmployees.Tables(0)
                .DisplayMember = "Country"
                .ValueMember = "Country"
            End With
        End Using

    End Sub
#End Region

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class