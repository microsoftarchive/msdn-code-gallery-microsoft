'**************************** Module Header ******************************\
' Module Name:  MainModule.vb
' Project:      VBDataAdapterOperations
' Copyright (c) Microsoft Corporation.
' 
' We can use DataAdapter to retrieve and update the data, and sometimes 
' the features of DataAdapter make some specific operations easier. In this 
' sample, we will demonstrate how to use DataAdapter to retrieve and update 
' the data:
' 1. Retrieve Data
' a. Use DataAdapter.AcceptChangesDuringFill Property to clone the data in 
' database.
'  If the property is set as false, AcceptChanges is not called when filling 
' the table, and the newly added rows are treated as inserted rows. So we can 
' use these rows to insert the new rows into the database.
' 
' b. Use DataAdapter.TableMappings Property to define the mapping between the 
' source table and DataTable.
' 
' c. Use DataAdapter.FillLoadOption Property to determine how the adapter fills 
' the DataTable from the DbDataReader.
' When we create a DataTable, we can only write the data from database to the 
' current version or the original version by setting the property as the 
' LoadOption.Upsert or the LoadOption.PreserveChanges.
' 
' 2. Update table
' Use DbDataAdapter.UpdateBatchSize Property to perform batch operations.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.Linq
Imports VBDataAdapterOperations.My

Namespace VBDataAdapterOperations
    Friend Class MainModule
        Shared Sub Main(ByVal args() As String)
            Dim settings As New MySettings()

            ' Copy the data from the database.
            ' Get the table Department and Course from the database.
            Dim selectString As String = "SELECT [DepartmentID],[Name],[Budget],[StartDate],[Administrator]" &
                ControlChars.CrLf & " FROM [MySchool].[dbo].[Department];" &
                ControlChars.CrLf & ControlChars.CrLf & " SELECT [CourseID],@Year as [Year],Max([Title]) as [Title]," &
                ControlChars.CrLf & " Max([Credits]) as [Credits],Max([DepartmentID]) as [DepartmentID]" &
                ControlChars.CrLf & " FROM [MySchool].[dbo].[Course]" & ControlChars.CrLf & "                                   Group by [CourseID]"

            Dim mySchool As New DataSet()

            Dim selectCommand As New SqlCommand(selectString)
            Dim parameter As SqlParameter = selectCommand.Parameters.Add("@Year", SqlDbType.SmallInt, 2)
            parameter.Value = New Random(Date.Now.Millisecond).Next(9999)

            ' Use DataTableMapping to map the source tables and the destination tables.
            Dim tableMappings() As DataTableMapping = {New DataTableMapping("Table", "Department"),
                                                       New DataTableMapping("Table1", "Course")}
            CopyData(mySchool, settings.MySchoolConnectionString, selectCommand, tableMappings)

            Console.WriteLine("The following tables are from the database.")
            For Each table As DataTable In mySchool.Tables
                Console.WriteLine(table.TableName)
                ShowDataTable(table)
            Next table


            ' Roll back the changes
            Dim department As DataTable = mySchool.Tables("Department")
            Dim course As DataTable = mySchool.Tables("Course")

            department.Rows(0)("Name") = "New" & department.Rows(0)(1)
            course.Rows(0)("Title") = "New" & course.Rows(0)("Title")
            course.Rows(0)("Credits") = 10

            Console.WriteLine("After we changed the tables:")
            For Each table As DataTable In mySchool.Tables
                Console.WriteLine(table.TableName)
                ShowDataTable(table)
            Next table

            department.RejectChanges()
            Console.WriteLine("After use the RejectChanges method in Department table to roll back the changes:")
            ShowDataTable(department)

            Dim primaryColumns() As DataColumn = {course.Columns("CourseID")}
            Dim resetColumns() As DataColumn = {course.Columns("Title")}
            ResetCourse(course, settings.MySchoolConnectionString, primaryColumns, resetColumns)
            Console.WriteLine("After use the ResetCourse method in Course table to roll back the changes:")
            ShowDataTable(course)

            ' Batch update the table.
            Dim insertString As String = "Insert into [MySchool].[dbo].[Course]([CourseID],[Year],[Title]," &
                ControlChars.CrLf & " [Credits],[DepartmentID]) " & ControlChars.CrLf &
                " values (@CourseID,@Year,@Title,@Credits,@DepartmentID)"
            Dim insertCommand As New SqlCommand(insertString)
            insertCommand.Parameters.Add("@CourseID", SqlDbType.NVarChar, 10, "CourseID")
            insertCommand.Parameters.Add("@Year", SqlDbType.SmallInt, 2, "Year")
            insertCommand.Parameters.Add("@Title", SqlDbType.NVarChar, 100, "Title")
            insertCommand.Parameters.Add("@Credits", SqlDbType.Int, 4, "Credits")
            insertCommand.Parameters.Add("@DepartmentID", SqlDbType.Int, 4, "DepartmentID")

            Const batchSize As Int32 = 10
            BatchInsertUpdate(course, settings.MySchoolConnectionString, insertCommand, batchSize)

            Console.WriteLine()
            Console.WriteLine("Press any key to exit.....")
            Console.ReadKey()
        End Sub

        ''' <summary>
        ''' Copy the Data from the database.
        ''' </summary>
        Private Shared Sub CopyData(ByVal dataSet As DataSet, ByVal connectionString As String,
                                    ByVal selectCommand As SqlCommand, ByVal tableMappings() As DataTableMapping)
            Using connection As New SqlConnection(connectionString)
                selectCommand.Connection = connection

                connection.Open()

                Using adapter As New SqlDataAdapter(selectCommand)
                    adapter.TableMappings.AddRange(tableMappings)
                    ' If set the AcceptChangesDuringFill as the false, AcceptChanges will not be 
                    ' called on a DataRow after it is added to the DataTable during any of the 
                    ' Fill operations.
                    adapter.AcceptChangesDuringFill = False

                    adapter.Fill(dataSet)
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Roll back only one column or serveral columns data of the Course table by calling 
        ''' ResetDataTable method.
        ''' </summary>
        ''' <param name="table">the Course table</param>
        ''' <param name="connectionString">the connection string</param>
        ''' <param name="primaryColumns">the primary columns of table</param>
        ''' <param name="resetColumns">the columns whose data need to be reset</param>
        Private Shared Sub ResetCourse(ByVal table As DataTable, ByVal connectionString As String,
                                       ByVal primaryColumns() As DataColumn, ByVal resetColumns() As DataColumn)
            table.PrimaryKey = primaryColumns

            ' Build the query string
            Dim primaryCols As String = String.Join(",", primaryColumns.Select(Function(col) col.ColumnName))
            Dim resetCols As String = String.Join(",", resetColumns.Select(
                                                  Function(col) "Max(" & col.ColumnName & ") as " & col.ColumnName))

            Dim selectString As String = String.Format(
                "Select {0},{1} from Course Group by {0}", primaryCols, resetCols)

            Dim selectCommand As New SqlCommand(selectString)

            ResetDataTable(table, connectionString, selectCommand)
        End Sub

        ''' <summary>
        ''' RejectChanges will roll back all changes that have been made to the table since it was loaded, 
        ''' or the last time AcceptChanges was called. If we want to copy all the data from the database, 
        ''' we may lose all the data after call the RejectChanges method. The ResetDataTable method can 
        ''' just roll back one or more columns of data.
        ''' </summary>
        Private Shared Sub ResetDataTable(ByVal table As DataTable, ByVal connectionString As String,
                                          ByVal selectCommand As SqlCommand)
            Using connection As New SqlConnection(connectionString)
                selectCommand.Connection = connection

                connection.Open()

                Using adapter As New SqlDataAdapter(selectCommand)
                    ' The incoming values for this row will be written to the current version of each 
                    ' column. The original version of each column's data will not be changed.
                    adapter.FillLoadOption = LoadOption.Upsert

                    adapter.Fill(table)
                End Using
            End Using
        End Sub

        Private Shared Sub BatchInsertUpdate(ByVal table As DataTable, ByVal connectionString As String,
                                             ByVal insertCommand As SqlCommand, ByVal batchSize As Int32)
            Using connection As New SqlConnection(connectionString)
                insertCommand.Connection = connection
                ' When setting UpdateBatchSize to a value other than 1, all the commands 
                ' associated with the SqlDataAdapter have to have their UpdatedRowSource 
                ' property set to None or OutputParameters. An exception is thrown otherwise.
                insertCommand.UpdatedRowSource = UpdateRowSource.None

                connection.Open()

                Using adapter As New SqlDataAdapter()
                    adapter.InsertCommand = insertCommand
                    ' Gets or sets the number of rows that are processed in each round-trip to the server.
                    ' Setting it to 1 disables batch updates, as rows are sent one at a time.
                    adapter.UpdateBatchSize = batchSize

                    adapter.Update(table)

                    Console.WriteLine("Successfully to update the table.")
                End Using
            End Using
        End Sub

        Private Shared Sub ShowDataTable(ByVal table As DataTable)
            For Each col As DataColumn In table.Columns
                Console.Write("{0,-14}", col.ColumnName)
            Next col
            Console.WriteLine("{0,-14}", "RowState")

            For Each row As DataRow In table.Rows
                For Each col As DataColumn In table.Columns
                    If col.DataType.Equals(GetType(Date)) Then
                        Console.Write("{0,-14:d}", row(col))
                    ElseIf col.DataType.Equals(GetType(Decimal)) Then
                        Console.Write("{0,-14:C}", row(col))
                    Else
                        Console.Write("{0,-14}", row(col))
                    End If
                Next col
                Console.WriteLine("{0,-14}", row.RowState)
            Next row
        End Sub
    End Class
End Namespace
