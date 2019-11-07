'**************************** Module Header ******************************\
' Module Name:  MainModule.vb
' Project:      VBDataTableModify
' Copyright (c) Microsoft Corporation.
' 
' We have several ways to modify the data in DataTable. In this application, 
' we will demonstrate how to use different ways to modify data in DataTable 
' and update to the source.
' 1. We use SqlDataAdapter to fill the DataTables.
' 2. We set  DataTable Constraints in DataTables.
' 4. We use DataTable Edits to modify data.
' 3. We use DataRow.Delete Method and DataRowCollection.Remove Method to delete 
' the rows, and then compare them.
' 5. We use SqlDataAdapter to update the datasource.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/


Imports System.Data.SqlClient

Namespace VBDataTableModify
    Friend Class MainModule
        Shared Sub Main(ByVal args() As String)
            Dim settings As New My.MySettings()

            ' Get Data
            Dim selectString As String =
                "Select [CourseID],[Year],[Title],[Credits],[DepartmentID] From [dbo].[Course];" & ControlChars.CrLf &
                "               Select [DepartmentID],[Name],[Budget],[StartDate],[Administrator] From [dbo].[Department] "

            Dim dataSet As New DataSet()
            Dim course As DataTable = dataSet.Tables.Add("Course")
            Dim department As DataTable = dataSet.Tables.Add("Department")

            Console.WriteLine("Get data from database:")
            GetDataTables(settings.MySchoolConnectionString, selectString, dataSet, course, department)
            Console.WriteLine()


            ' Use DataTable Edits to edit the data
            Dim updateString As String =
                "Update [dbo].[Course] Set [Credits]=@Credits Where [CourseID]=@CourseID;"

            AddHandler course.ColumnChanged, AddressOf OnColumnChanged

            ' Set the Credits of first row is negative value, and set the Credits of second row is plus.
            ChangeCredits(course, course.Rows(0), -1)
            ChangeCredits(course, course.Rows(1), 11)

            UpdateDataTables(settings.MySchoolConnectionString, updateString, dataSet, "Course",
                             New SqlParameter("@CourseID", SqlDbType.NVarChar, 10, "CourseID"),
                             New SqlParameter("@Credits", SqlDbType.Int, 4, "Credits"))
            Console.WriteLine("Only the Credits of second row is changed.")
            ShowDataTable(course)
            Console.WriteLine()

            ' Delete and Remove from DataTable
            ' Create the foreign key constraint, and set the DeleteRule with Cascade.
            Dim courseDepartFK As New ForeignKeyConstraint("CourseDepartFK",
                                                           department.Columns("DepartmentID"),
                                                           course.Columns("DepartmentID"))
            courseDepartFK.DeleteRule = Rule.Cascade
            courseDepartFK.UpdateRule = Rule.Cascade
            courseDepartFK.AcceptRejectRule = AcceptRejectRule.None
            course.Constraints.Add(courseDepartFK)

            Dim deleteString As String = "Delete From [dbo].[Course] Where [CourseID]=@CourseID;"

            department.Rows(0).Delete()
            Console.WriteLine("If One row in Department table is deleted, the related rows " &
                              "in Course table will also be deleted.")
            Console.WriteLine("Department DataTable:")
            ShowDataTable(department)
            Console.WriteLine()
            Console.WriteLine("Course DataTable:")
            ShowDataTable(course)
            Console.WriteLine()
            ' Update the delete operation
            DeleteDataTables(settings.MySchoolConnectionString, deleteString, dataSet, "Course",
                             New SqlParameter("@CourseID", SqlDbType.NVarChar, 10, "CourseID"))
            Console.WriteLine("After delete operation:")
            Console.WriteLine("Course DataTable:")
            ShowDataTable(course)
            Console.WriteLine()

            course.Rows.RemoveAt(0)
            Console.WriteLine("Now we remove one row from Course:")
            ShowDataTable(course)
            DeleteDataTables(settings.MySchoolConnectionString, deleteString, dataSet, "Course",
                             New SqlParameter("@CourseID", SqlDbType.NVarChar, 10, "CourseID"))

            Console.WriteLine("Please press any key to exit......")
            Console.ReadLine()
        End Sub

        ''' <summary>
        ''' Use SqlDataAdapter to get data.
        ''' </summary>
        Private Shared Sub GetDataTables(ByVal connectionString As String, ByVal selectString As String,
                                         ByVal dataSet As DataSet, ByVal ParamArray tables() As DataTable)
            Using adapter As New SqlDataAdapter()
                adapter.SelectCommand = New SqlCommand(selectString)
                adapter.SelectCommand.Connection = New SqlConnection(connectionString)

                adapter.Fill(0, 0, tables)

                For Each table As DataTable In dataSet.Tables
                    Console.WriteLine("Data in {0}:", table.TableName)
                    ShowDataTable(table)
                    Console.WriteLine()
                Next table
            End Using
        End Sub

        ''' <summary>
        ''' Use SqlDataAdapter to update the updata operation.
        ''' </summary>
        Private Shared Sub UpdateDataTables(ByVal connectionString As String,
                                            ByVal updateString As String,
                                            ByVal dataSet As DataSet,
                                            ByVal tableName As String,
                                            ByVal ParamArray parameters() As SqlParameter)
            Using adapter As New SqlDataAdapter()
                adapter.UpdateCommand = New SqlCommand(updateString)
                adapter.UpdateCommand.Parameters.AddRange(parameters)
                adapter.UpdateCommand.Connection = New SqlConnection(connectionString)

                adapter.Update(dataSet, tableName)
            End Using
        End Sub

        ''' <summary>
        ''' Use SqlDataAdapter to update delete operation.
        ''' </summary>
        Private Shared Sub DeleteDataTables(ByVal connectionString As String,
                                            ByVal deleteString As String,
                                            ByVal dataSet As DataSet,
                                            ByVal tableName As String,
                                            ByVal ParamArray parameters() As SqlParameter)
            Using adapter As New SqlDataAdapter()
                adapter.DeleteCommand = New SqlCommand(deleteString)
                adapter.DeleteCommand.Parameters.AddRange(parameters)
                adapter.DeleteCommand.Connection = New SqlConnection(connectionString)

                adapter.Update(dataSet, tableName)
            End Using
        End Sub

        ''' <summary>
        ''' Use DataTable Edits to modify the data.
        ''' </summary>
        Private Shared Sub ChangeCredits(ByVal table As DataTable, ByVal row As DataRow,
                                         ByVal credits As Int32)
            row.BeginEdit()
            Console.WriteLine("We change row {0}", table.Rows.IndexOf(row))
            row("Credits") = credits
            row.EndEdit()
        End Sub

        ''' <summary>
        ''' The method will be invoked when the value in DataTable is changed.
        ''' </summary>
        Private Shared Sub OnColumnChanged(ByVal sender As Object,
                                           ByVal args As DataColumnChangeEventArgs)
            Dim credits As Int32 = 0
            ' If Credits is changed and the value is negative, we'll cancel the edit.
            If (args.Column.ColumnName = "Credits") AndAlso
                ((Not Int32.TryParse(args.ProposedValue.ToString(), credits)) OrElse credits < 0) Then
                Console.WriteLine("The value of Credits is invalid. Edit canceled.")
                args.Row.CancelEdit()
            End If
        End Sub

        ''' <summary>
        ''' Display the column and value of DataTable.
        ''' </summary>
        Private Shared Sub ShowDataTable(ByVal table As DataTable)
            For Each col As DataColumn In table.Columns
                Console.Write("{0,-14}", col.ColumnName)
            Next col
            Console.WriteLine("{0,-14}", "RowState")

            For Each row As DataRow In table.Rows
                If row.RowState = DataRowState.Deleted Then
                    For Each col As DataColumn In table.Columns
                        If col.DataType.Equals(GetType(Date)) Then
                            Console.Write("{0,-14:d}", row(col, DataRowVersion.Original))
                        ElseIf col.DataType.Equals(GetType(Decimal)) Then
                            Console.Write("{0,-14:C}", row(col, DataRowVersion.Original))
                        Else
                            Console.Write("{0,-14}", row(col, DataRowVersion.Original))
                        End If
                    Next col
                Else
                    For Each col As DataColumn In table.Columns
                        If col.DataType.Equals(GetType(Date)) Then
                            Console.Write("{0,-14:d}", row(col))
                        ElseIf col.DataType.Equals(GetType(Decimal)) Then
                            Console.Write("{0,-14:C}", row(col))
                        Else
                            Console.Write("{0,-14}", row(col))
                        End If
                    Next col
                End If
                Console.WriteLine("{0,-14}", row.RowState)
            Next row
        End Sub
    End Class
End Namespace
