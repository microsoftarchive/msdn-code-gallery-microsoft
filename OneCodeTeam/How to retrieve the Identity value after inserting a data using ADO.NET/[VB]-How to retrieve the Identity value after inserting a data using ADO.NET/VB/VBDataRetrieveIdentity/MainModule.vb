'**************************** Module Header ******************************\
' Module Name:  MainModule.vb
' Project:      VBDataRetrieveIdentity
' Copyright (c) Microsoft Corporation.
' 
' We often set the column as identity when the values in the column must be unique. 
' And sometimes we need the identity value of new data. In this application, we 
' will demonstrate how to retrieve the identity values:
' 1. Create a stored procedure to insert data and return identity value;
' 2. Execute a command to insert the new data and display the result;
' 3. Use SqlDataAdapter to insert new data and display the result.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.Data.OleDb
Imports System.Data.SqlClient
Imports VBDataRetrieveIdentity.My

Namespace VBDataRetrieveIdentity
    Friend Class MainModule
        Shared Sub Main(ByVal args() As String)
            Dim SqlDbConnectionString As String = MySettings.Default.SqlDbConnectionString

            InsertPerson(SqlDbConnectionString, "Janice", "Galvin")
            Console.WriteLine()

            InsertPersonInAdapter(SqlDbConnectionString, "Peter", "Krebs")
            Console.WriteLine()

            Dim oledbConnectionString As String = MySettings.Default.OlelDbConnectionString
            InsertPersonInJet4Database(oledbConnectionString, "Janice", "Galvin")
            Console.WriteLine()

            Console.WriteLine("Please press any key to exit.....")
            Console.ReadKey()
        End Sub

        ''' <summary>
        ''' Using stored procedure to insert a new row and retrieve the identity value
        ''' </summary>
        Private Shared Sub InsertPerson(ByVal connectionString As String, ByVal firstName As String,
                                        ByVal lastName As String)
            Dim commandText As String = "dbo.InsertPerson"

            Using conn As New SqlConnection(connectionString)
                Using cmd As New SqlCommand(commandText, conn)
                    cmd.CommandType = CommandType.StoredProcedure

                    cmd.Parameters.Add(New SqlParameter("@FirstName", firstName))
                    cmd.Parameters.Add(New SqlParameter("@LastName", lastName))
                    Dim personId As New SqlParameter("@PersonID", SqlDbType.Int)
                    personId.Direction = ParameterDirection.Output
                    cmd.Parameters.Add(personId)

                    conn.Open()
                    cmd.ExecuteNonQuery()

                    Console.WriteLine("Person Id of new person:{0}", personId.Value)
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Using stored procedure in adapter to insert new rows and update the identity value.
        ''' </summary>
        Private Shared Sub InsertPersonInAdapter(ByVal connectionString As String,
                                                 ByVal firstName As String, ByVal lastName As String)
            Dim commandText As String = "dbo.InsertPerson"
            Using conn As New SqlConnection(connectionString)
                Dim mySchool As New SqlDataAdapter("Select PersonID,FirstName,LastName from [dbo].[Person]", conn)

                mySchool.InsertCommand = New SqlCommand(commandText, conn)
                mySchool.InsertCommand.CommandType = CommandType.StoredProcedure

                mySchool.InsertCommand.Parameters.Add(New SqlParameter("@FirstName", SqlDbType.NVarChar,
                                                                       50, "FirstName"))
                mySchool.InsertCommand.Parameters.Add(New SqlParameter("@LastName", SqlDbType.NVarChar,
                                                                       50, "LastName"))

                Dim personId As SqlParameter =
                    mySchool.InsertCommand.Parameters.Add(New SqlParameter("@PersonID", SqlDbType.Int, 0,
                                                                           "PersonID"))
                personId.Direction = ParameterDirection.Output

                Dim persons As New DataTable()
                mySchool.Fill(persons)

                Dim newPerson As DataRow = persons.NewRow()
                newPerson("FirstName") = firstName
                newPerson("LastName") = lastName
                persons.Rows.Add(newPerson)

                mySchool.Update(persons)
                Console.WriteLine("Show all persons:")
                ShowDataTable(persons, 14)
            End Using
        End Sub

        ''' <summary>
        ''' For a Jet 4.0 database, we need use the sigle statement and event handler to insert new rows 
        ''' and retrieve the identity value.
        ''' </summary>
        Private Shared Sub InsertPersonInJet4Database(ByVal connectionString As String,
                                                      ByVal firstName As String, ByVal lastName As String)
            Dim commandText As String = "Insert into Person(FirstName,LastName) Values(?,?)"
            Using conn As New OleDbConnection(connectionString)
                Dim mySchool As New OleDbDataAdapter("Select PersonID,FirstName,LastName from Person",
                                                     conn)

                ' Create Insert Command
                mySchool.InsertCommand = New OleDbCommand(commandText, conn)
                mySchool.InsertCommand.CommandType = CommandType.Text

                mySchool.InsertCommand.Parameters.Add(New OleDbParameter("@FirstName", OleDbType.VarChar,
                                                                         50, "FirstName"))
                mySchool.InsertCommand.Parameters.Add(New OleDbParameter("@LastName", OleDbType.VarChar,
                                                                         50, "LastName"))
                mySchool.InsertCommand.UpdatedRowSource = UpdateRowSource.Both

                Dim persons As DataTable = CreatePersonsTable()

                mySchool.Fill(persons)

                Dim newPerson As DataRow = persons.NewRow()
                newPerson("FirstName") = firstName
                newPerson("LastName") = lastName
                persons.Rows.Add(newPerson)

                Dim dataChanges As DataTable = persons.GetChanges()

                AddHandler mySchool.RowUpdated, AddressOf OnRowUpdated

                mySchool.Update(dataChanges)

                Console.WriteLine("Data before merging:")
                ShowDataTable(persons, 14)
                Console.WriteLine()

                persons.Merge(dataChanges)
                persons.AcceptChanges()

                Console.WriteLine("Data after merging")
                ShowDataTable(persons, 14)
            End Using
        End Sub

        Private Shared Sub OnRowUpdated(ByVal sender As Object, ByVal e As OleDbRowUpdatedEventArgs)
            If e.StatementType = StatementType.Insert Then
                ' Retrieve the identity value
                Dim cmdNewId As New OleDbCommand("Select @@IDENTITY", e.Command.Connection)
                e.Row("PersonID") = CInt(Fix(cmdNewId.ExecuteScalar()))

                ' After the status is changed, the original values in the row are preserved. And the 
                ' Merge method will be called to merge the new identity value into the original DataTable.
                e.Status = UpdateStatus.SkipCurrentRow
            End If
        End Sub

        ''' <summary>
        ''' Create the Persons table before filling.
        ''' </summary>
        Private Shared Function CreatePersonsTable() As DataTable
            Dim persons As New DataTable()

            Dim personId As New DataColumn()
            personId.DataType = Type.GetType("System.Int32")
            personId.ColumnName = "PersonID"
            personId.AutoIncrement = True
            personId.AutoIncrementSeed = 0
            personId.AutoIncrementStep = -1
            persons.Columns.Add(personId)

            Dim firstName As New DataColumn()
            firstName.DataType = Type.GetType("System.String")
            firstName.ColumnName = "FirstName"
            persons.Columns.Add(firstName)

            Dim lastName As New DataColumn()
            lastName.DataType = Type.GetType("System.String")
            lastName.ColumnName = "LastName"
            persons.Columns.Add(lastName)

            Dim pkey() As DataColumn = {personId}
            persons.PrimaryKey = pkey

            Return persons
        End Function

        Private Shared Sub ShowDataTable(ByVal table As DataTable, ByVal length As Int32)
            For Each col As DataColumn In table.Columns
                Console.Write("{0,-" & length & "}", col.ColumnName)
            Next col
            Console.WriteLine()

            For Each row As DataRow In table.Rows
                For Each col As DataColumn In table.Columns
                    If col.DataType.Equals(GetType(Date)) Then
                        Console.Write("{0,-" & length & ":d}", row(col))
                    ElseIf col.DataType.Equals(GetType(Decimal)) Then
                        Console.Write("{0,-" & length & ":C}", row(col))
                    Else
                        Console.Write("{0,-" & length & "}", row(col))
                    End If
                Next col

                Console.WriteLine()
            Next row
        End Sub
    End Class
End Namespace
