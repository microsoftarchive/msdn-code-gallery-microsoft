'**************************** Module Header ******************************\
' Module Name:  MainModule.vb
' Project:      VBDataGetSchema
' Copyright (c) Microsoft Corporation.
' 
' Sometimes we may need the schema information of the database, tables or some 
' columns. In this application, we will demonstrate how to get schema information 
' from database:
' 1. In this application, we will use the SqlConnection.GetSchema Method.
' 2. We will use the Schema Restrictions to get the specified information.
' 3. We will get the schema information of the database, tables, some columns 
' and so on.
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

Namespace VBDataGetSchema
    Friend Class MainModule
        Shared Sub Main(ByVal args() As String)
            Dim settings As New My.MySettings()

            Using conn As New SqlConnection(settings.VBDataGetSchema)
                conn.Open()

                ' Get the Meta Data for Supported Schema Collections
                Dim metaDataTable As DataTable = conn.GetSchema("MetaDataCollections")

                Console.WriteLine("Meta Data for Supported Schema Collections:")
                ShowDataTable(metaDataTable, 25)
                Console.WriteLine()


                ' Get the schema information of Databases in your instance
                Dim databasesSchemaTable As DataTable = conn.GetSchema("Databases")

                Console.WriteLine("Schema Information of Databases:")
                ShowDataTable(databasesSchemaTable, 25)
                Console.WriteLine()


                ' Get the schema information of Tables
                ' First, get schema information of all the tables in current database;
                Dim allTablesSchemaTable As DataTable = conn.GetSchema("Tables")

                Console.WriteLine("Schema Information of All Tables:")
                ShowDataTable(allTablesSchemaTable, 20)
                Console.WriteLine()

                ' You can specify the Catalog, Schema, Table Name, Table Type to get 
                ' the specified table(s).
                ' You can use four restrictions for Table, so you should create a 4 members array.
                Dim tableRestrictions(3) As String

                ' For the array, 0-member represents Catalog; 1-member represents Schema; 
                ' 2-member represents Table Name; 3-member represents Table Type. 
                ' Now we specify the Table Name of the table what we want to get schema information.
                tableRestrictions(2) = "Course"

                Dim courseTableSchemaTable As DataTable = conn.GetSchema("Tables", tableRestrictions)

                Console.WriteLine("Schema Information of Course Tables:")
                ShowDataTable(courseTableSchemaTable, 20)
                Console.WriteLine()

                ' Get the schema information of Columns
                ' First, get schema information of all the columns in current database.
                Dim allColumnsSchemaTable As DataTable = conn.GetSchema("Columns")

                Console.WriteLine("Schema Information of All Columns:")
                ShowColumns(allColumnsSchemaTable)
                Console.WriteLine()

                ' You can specify the Catalog, Schema, Table Name, Column Name to get the specified column(s).
                ' You can use four restrictions for Column, so you should create a 4 members array.
                Dim columnRestrictions(3) As String

                ' For the array, 0-member represents Catalog; 1-member represents Schema; 
                ' 2-member represents Table Name; 3-member represents Column Name. 
                ' Now we specify the Table_Name and Column_Name of the columns what we want to get 
                ' schema information.
                columnRestrictions(2) = "Course"
                columnRestrictions(3) = "DepartmentID"

                Dim departmentIDSchemaTable As DataTable = conn.GetSchema("Columns", columnRestrictions)

                Console.WriteLine("Schema Information of DepartmentID Column in Course Table:")
                ShowColumns(departmentIDSchemaTable)
                Console.WriteLine()
  
                ' Get the schema information of IndexColumns
                ' First, get schema information of all the IndexColumns in current database
                Dim allIndexColumnsSchemaTable As DataTable = conn.GetSchema("IndexColumns")

                Console.WriteLine("Schema Information of All IndexColumns:")
                ShowIndexColumns(allIndexColumnsSchemaTable)
                Console.WriteLine()

                ' You can specify the Catalog, Schema, Table Name, Constraint Name, Column Name to 
                ' get the specified column(s).
                ' You can use five restrictions for Column, so you should create a 5 members array.
                Dim indexColumnsRestrictions(4) As String

                ' For the array, 0-member represents Catalog; 1-member represents Schema; 
                ' 2-member represents Table Name; 3-member represents Constraint Name;4-member represents 
                ' Column Name. 
                ' Now we specify the Table_Name and Column_Name of the columns what we want to get 
                ' schema information.
                indexColumnsRestrictions(2) = "Course"
                indexColumnsRestrictions(4) = "CourseID"

                Dim courseIdIndexSchemaTable As DataTable = conn.GetSchema("IndexColumns",
                                                                           indexColumnsRestrictions)

                Console.WriteLine("Index Schema Information of CourseID Column in Course Table:")
                ShowIndexColumns(courseIdIndexSchemaTable)
                Console.WriteLine()

            End Using

            Console.WriteLine("Please press any key to exit...")
            Console.ReadKey()
        End Sub

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

        Private Shared Sub ShowDataTable(ByVal table As DataTable)
            ShowDataTable(table, 14)
        End Sub

        Private Shared Sub ShowColumns(ByVal columnsTable As DataTable)
            Dim selectedRows = From info In columnsTable.AsEnumerable()
                               Select New With {Key .TableCatalog = info("TABLE_CATALOG"),
                                                Key .TableSchema = info("TABLE_SCHEMA"),
                                                Key .TableName = info("TABLE_NAME"),
                                                Key .ColumnName = info("COLUMN_NAME"),
                                                Key .DataType = info("DATA_TYPE")}

            Console.WriteLine("{0,-15}{1,-15}{2,-15}{3,-15}{4,-15}", "TableCatalog", "TABLE_SCHEMA",
                              "TABLE_NAME", "COLUMN_NAME", "DATA_TYPE")
            For Each row In selectedRows
                Console.WriteLine("{0,-15}{1,-15}{2,-15}{3,-15}{4,-15}", row.TableCatalog,
                                  row.TableSchema, row.TableName, row.ColumnName, row.DataType)
            Next row
        End Sub

        Private Shared Sub ShowIndexColumns(ByVal indexColumnsTable As DataTable)
            Dim selectedRows = From info In indexColumnsTable.AsEnumerable()
                               Select New With {Key .TableSchema = info("table_schema"),
                                                Key .TableName = info("table_name"),
                                                Key .ColumnName = info("column_name"),
                                                Key .ConstraintSchema = info("constraint_schema"),
                                                Key .ConstraintName = info("constraint_name"),
                                                Key .KeyType = info("KeyType")}

            Console.WriteLine("{0,-14}{1,-11}{2,-14}{3,-18}{4,-16}{5,-8}", "table_schema", "table_name",
                              "column_name", "constraint_schema", "constraint_name", "KeyType")
            For Each row In selectedRows
                Console.WriteLine("{0,-14}{1,-11}{2,-14}{3,-18}{4,-16}{5,-8}", row.TableSchema,
                                  row.TableName, row.ColumnName, row.ConstraintSchema,
                                  row.ConstraintName, row.KeyType)
            Next row
        End Sub
    End Class
End Namespace
