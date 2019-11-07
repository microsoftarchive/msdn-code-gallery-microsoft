' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Data.SqlClient
Imports System.Data

Public Class Form1

    ' Initialize constants for connecting to the database
    ' and displaying a connection error to the user.
    Protected Const SqlConnectionString As String = _
        "Server=(local);" & _
        "DataBase=;" & _
        "Integrated Security=SSPI"

    Protected Const ConnectionErrorMessage As String = _
        "To run this sample, you must have SQL " & _
        "installed.  For " & _
        "instructions on installing SQL, view the documentation file."

    Protected didPreviouslyConnect As Boolean = False
    Protected didCreateTable As Boolean = False
    Protected connectionString As String = SqlConnectionString

#Region "Create database"
    ' Handles the click event for the Create Database button.
    Private Sub btnCreateDB_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateDB.Click

        ' If the "Create Table" button is enabled this means the user is trying to
        ' recreate the database again in the same application run. Warn the user of
        ' how this will affect things, and if they want to proceed, reset the UI to
        ' the initial state so that errors are not induced if currently enabled 
        ' buttons are clicked out of order.
        If btnCreateTable.Enabled Then
            Dim dr As DialogResult = MessageBox.Show("Recreating the database " & _
                "will undo the other database object creation steps you have " & _
                "made so far. Do you wish to proceed?", "Database Re-creation " & _
                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

            If dr = Windows.Forms.DialogResult.Yes Then
                ResetUI()
                CreateDatabase()
            End If
        Else
            CreateDatabase()
        End If

    End Sub


    ' This routine executes a SQL statement that deletes the database (if it exists) 
    ' and then creates it. 
    Private Sub CreateDatabase()
        Dim sqlStatement As String = _
            "IF EXISTS (" & _
            "SELECT * " & _
            "FROM master..sysdatabases " & _
            "WHERE Name = 'HowToDemo')" & vbCrLf & _
            "DROP DATABASE HowToDemo" & vbCrLf & _
            "CREATE DATABASE HowToDemo"

        ' Display a status message saying that we're attempting to connect.
        ' This only needs to be done the very first time a connection is
        ' attempted.  After we've determined that MSDE or SQL Server is
        ' installed, this message no longer needs to be displayed.
        Dim statusForm As New Status()
        If Not didPreviouslyConnect Then
            statusForm.Show("Connecting to SQL Server")
        End If

        ' Attempt to connect to the SQL server instance.  
        Try
            ' The SqlConnection class allows you to communicate with SQL Server.
            ' The constructor accepts a connection string as an argument.  This
            ' connection string uses Integrated Security, which means that you 
            ' must have a login in SQL Server, or be part of the Administrators
            ' group for this to work.
            Dim connection As New SqlConnection(connectionString)

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(sqlStatement, connection)

            ' Open the connection, execute the command, and close the 
            ' connection. It is more efficient to ExecuteNonQuery when data is 
            ' not being returned.
            connection.Open()
            cmd.ExecuteNonQuery()
            connection.Close()

            ' Data has been successfully submitted.
            didPreviouslyConnect = True
            didCreateTable = True
            statusForm.Close()

            ' Show the controls for the next step.
            lblArrow1.Visible = True
            lblStep2.Enabled = True
            btnCreateTable.Enabled = True

            MsgBox("Database 'HowToDemo' successfully created.", MsgBoxStyle.OKOnly, "Database Creation Status")
        Catch sqlExc As SqlException
            MsgBox(sqlExc.Message, MsgBoxStyle.OKOnly, "SQL Exception Error")
        Catch exc As Exception
            ' Unable to connect to SQL Server or MSDE
            statusForm.Close()
            MsgBox(exc.Message, MsgBoxStyle.OKOnly, "Connection failed.")
        End Try
    End Sub
#End Region

#Region "Reset UI"
    ' Sets up the user interface so that the user proceeds in proper order
    ' through the steps.
    Private Sub ResetUI()
        lblArrow1.Visible = False
        lblStep2.Enabled = False
        btnCreateTable.Enabled = False
        lblArrow2.Visible = False
        lblStep3.Enabled = False
        btnCreateSP.Enabled = False
        lblArrow3.Visible = False
        lblStep4.Enabled = False
        btnCreateView.Enabled = False
        lblArrow4.Visible = False
        lblStep5.Enabled = False
        btnPopulate.Enabled = False
        lblArrow5.Visible = False
        lblStep6.Enabled = False
        btnDisplay.Enabled = False

        With DataGridView1
            .Visible = False
            .DataSource = Nothing
        End With
    End Sub
#End Region

#Region "Create table"
    ' Handles the click event for the Create Table button.
    Private Sub btnCreateTable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateTable.Click

        Dim strSQL As String = _
            "USE HowToDemo" & vbCrLf & _
            "IF EXISTS (" & _
            "SELECT * " & _
            "FROM HowToDemo.dbo.sysobjects " & _
            "WHERE Name = 'Contact' " & _
            "AND TYPE = 'u')" & vbCrLf & _
            "BEGIN" & vbCrLf & _
            "DROP TABLE HowToDemo.dbo.Contact" & vbCrLf & _
            "END" & vbCrLf & _
            "CREATE TABLE Contact (" & _
            "ContactID Int NOT NULL," & _
            "FirstName NVarChar(40) NOT NULL," & _
            "LastName NVarChar(40) NOT NULL" & ")"

        Try
            ' The SqlConnection class allows you to communicate with SQL Server.
            ' The constructor accepts a connection string as an argument.  This
            ' connection string uses Integrated Security, which means that you 
            ' must have a login in SQL Server, or be part of the Administrators
            ' group for this to work.
            Dim dbConnection As New SqlConnection(connectionString)

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)

            ' Open the connection, execute the command, and close the connection.
            ' It is more efficient to ExecuteNonQuery when data is not being 
            ' returned.
            dbConnection.Open()
            cmd.ExecuteNonQuery()
            dbConnection.Close()

            ' Show the controls for the next step.
            lblArrow2.Visible = True
            lblStep3.Enabled = True
            btnCreateSP.Enabled = True

            MessageBox.Show("Table 'Contact' successfully created.", _
                "Table Creation Status", _
                MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
#End Region

#Region "Create stored procedure"
    ' Handles the click event for the Create Procedure button. This handler executes
    ' two SQL statements, one that drops the Procedure (if it exists) and another 
    ' that creates the Procedure. The statements are broken up in this manner 
    ' because SQL Server doesn't allow them to be combined in one batch. (You can 
    ' combine them when using the GO keyword and executing from SQL Query Analyzer 
    ' or another tool, but not from code.)
    Private Sub btnCreateSP_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateSP.Click
        ' The SqlConnection class allows you to communicate with SQL Server.
        ' The constructor accepts a connection string as an argument.  This
        ' connection string uses Integrated Security, which means that you 
        ' must have a login in SQL Server, or be part of the Administrators
        ' group for this to work.
        Dim dbConnection As New SqlConnection(connectionString)

        Dim strSQL As String = _
            "USE HowToDemo" & vbCrLf & _
            "IF EXISTS (" & _
            "SELECT * " & _
            "FROM HowToDemo.dbo.sysobjects " & _
            "WHERE Name = 'AddContacts' " & _
            "AND TYPE = 'p')" & vbCrLf & _
            "BEGIN" & vbCrLf & _
            "DROP PROCEDURE AddContacts" & vbCrLf & _
            "END"

        ' A SqlCommand object is used to execute the SQL commands.
        Dim cmd As New SqlCommand(strSQL, dbConnection)

        Try
            ' Open the connection, execute the command, and close the connection.
            ' It is more efficient to ExecuteNonQuery when data is not being 
            ' returned.
            dbConnection.Open()
            cmd.ExecuteNonQuery()

        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Try
            cmd.CommandText = _
                "CREATE PROCEDURE AddContacts AS" & vbCrLf & _
                "INSERT INTO Contact" & vbCrLf & _
                "(ContactID, FirstName, LastName)" & _
                "SELECT EmployeeID, FirstName, LastName " & _
                "FROM Northwind.dbo.Employees"

            cmd.ExecuteNonQuery()
            dbConnection.Close()

            ' Show the controls for the next step.
            lblArrow3.Visible = True
            lblStep4.Enabled = True
            btnCreateView.Enabled = True

            MessageBox.Show("Stored Procedure 'AddContacts' successfully " & _
                "created.", "SPROC Creation Status", _
                MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
#End Region

#Region "Create view"
    ' Handles the click event for the Create View button. This handler executes
    ' two SQL statements, one that drops the View (if it exists) and another 
    ' that creates the View. The statements are broken up in this manner because
    ' SQL Server doesn't allow them to be combined in one batch. (You can combine
    ' them when using the GO keyword and executing from SQL Query Analyzer or 
    ' another tool, but not from code.)
    Private Sub btnCreateView_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateView.Click
        ' The SqlConnection class allows you to communicate with SQL Server.
        ' The constructor accepts a connection string as an argument.  This
        ' connection string uses Integrated Security, which means that you 
        ' must have a login in SQL Server, or be part of the Administrators
        ' group for this to work.
        Dim dbConnection As New SqlConnection(connectionString)

        Dim strSQL As String = _
            "USE HowToDemo" & vbCrLf & _
            "IF EXISTS (" & _
            "SELECT * " & _
            "FROM HowToDemo.dbo.sysobjects " & _
            "WHERE Name = 'GetContacts' " & _
            "AND TYPE = 'v')" & vbCrLf & _
            "BEGIN" & vbCrLf & _
            "DROP VIEW GetContacts" & vbCrLf & _
            "END"

        ' A SqlCommand object is used to execute the SQL commands.
        Dim cmd As New SqlCommand(strSQL, dbConnection)

        Try
            ' Open the connection, execute the command. Do not close the
            ' connection yet as it will be used in the next Try...Catch blocl.
            dbConnection.Open()
            cmd.ExecuteNonQuery()

        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        Try
            ' Change the SQL statement for the next query.
            cmd.CommandText = _
                "CREATE VIEW GetContacts AS " & _
                "SELECT * " & _
                "FROM Contact"

            cmd.ExecuteNonQuery()
            dbConnection.Close()

            ' Show the controls for the next step.
            lblArrow4.Visible = True
            lblStep5.Enabled = True
            btnPopulate.Enabled = True

            MessageBox.Show("View 'GetContacts' successfully " & _
                "created.", "SQL View Creation Status", _
                MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
#End Region

#Region "Populate table"
    ' Handles the click event for the Populate button. This handler executes the
    ' stored procedure that fills the Contact table with product info from the
    ' AdventureWorks database Contact table.
    Private Sub btnPopulate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPopulate.Click

        Dim strSQL As String = "EXECUTE HowToDemo.dbo.AddContacts"

        Try
            ' The SqlConnection class allows you to communicate with SQL Server.
            ' The constructor accepts a connection string as an argument.  This
            ' connection string uses Integrated Security, which means that you 
            ' must have a login in SQL Server, or be part of the Administrators
            ' group for this to work.
            Dim dbConnection As New SqlConnection(connectionString)

            ' A SqlCommand object is used to execute the SQL commands.
            Dim cmd As New SqlCommand(strSQL, dbConnection)

            ' Open the connection, execute the command, and close the connection.
            ' It is more efficient to ExecuteNonQuery when data is not being 
            ' returned.
            dbConnection.Open()
            cmd.ExecuteNonQuery()
            dbConnection.Close()

            ' Show the controls for the next step.
            lblArrow5.Visible = True
            lblStep6.Enabled = True
            btnDisplay.Enabled = True

            MessageBox.Show("Table successfully populated.", _
                "Data Addition Status", _
                MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch sqlExc As SqlException
            MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
#End Region

#Region "Display data"
    ' Handles the click event for the Display button. This handler gets the product
    ' information from the Contact table puts it into a DataSet which is used to
    ' bind to a DataGrid for display. Custom style objects are used to give the 
    ' DataGrid a nice appearance.
    Private Sub btnDisplay_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDisplay.Click

        If IsNothing(DataGridView1.DataSource) Then

            Dim strSQL As String = _
                "USE HowToDemo" & vbCrLf & _
                "SELECT * " & _
                "FROM GetContacts"

            Try
                ' The SqlConnection class allows you to communicate with SQL Server.
                ' The constructor accepts a connection string as an argument.  This
                ' connection string uses Integrated Security, which means that you 
                ' must have a login in SQL Server, or be part of the Administrators
                ' group for this to work.
                Dim dbConnection As New SqlConnection(connectionString)

                ' A SqlCommand object is used to execute the SQL commands.
                Dim cmd As New SqlCommand(strSQL, dbConnection)

                ' The SqlDataAdapter is responsible for using a SqlCommand object to 
                ' fill a DataSet.
                Dim da As New SqlDataAdapter(cmd)
                Dim dsContacts As New DataSet()
                da.Fill(dsContacts, "Contact")

                With Me.DataGridView1
                    .Visible = True
                    .AutoGenerateColumns = False
                    .AlternatingRowsDefaultCellStyle.BackColor = Color.Lavender
                    .BackColor = Color.WhiteSmoke
                    .ForeColor = Color.MidnightBlue
                    .CellBorderStyle = DataGridViewCellBorderStyle.None
                    .ColumnHeadersDefaultCellStyle.Font = New Font("Tahoma", 8.0!, FontStyle.Bold)
                    .ColumnHeadersDefaultCellStyle.BackColor = Color.MidnightBlue
                    .ColumnHeadersDefaultCellStyle.ForeColor = Color.WhiteSmoke
                    .DefaultCellStyle.ForeColor = Color.MidnightBlue
                    .DefaultCellStyle.BackColor = Color.WhiteSmoke
                End With


                Me.DataGridView1.DataSource = dsContacts.Tables(0)
                Dim newColumn As Integer = Me.DataGridView1.Columns.Add("ContactID", "Contact ID")
                Me.DataGridView1.Columns(newColumn).DataPropertyName = "ContactID"

                newColumn = Me.DataGridView1.Columns.Add("FirstName", "First Name")
                Me.DataGridView1.Columns(newColumn).DataPropertyName = "FirstName"

                newColumn = Me.DataGridView1.Columns.Add("LastName", "Last Name")
                Me.DataGridView1.Columns(newColumn).DataPropertyName = "LastName"
            Catch sqlExc As SqlException
                MessageBox.Show(sqlExc.ToString, "SQL Exception Error!", _
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If

    End Sub
#End Region

#Region "Form load"
    ' Handles the Form load event.
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ResetUI()
    End Sub
#End Region

    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
