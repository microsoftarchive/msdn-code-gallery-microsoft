'************************************* Module Header **************************************\
' Module Name:  JustInTimeDataLoading.Cache 
' Project:      VBWinFormDataGridView
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates how to use virtual mode in the DataGridView control 
' with a data cache that loads data from a server only when it is needed. 
' This kind of data loading is called "Just-in-time data loading". 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**********************************************************************************/


Imports System.Data.SqlClient

Namespace VBWinFormDataGridView.JustInTimeDataLoading

#Region "Cache Class"
    Public Class Cache
        Private Shared RowsPerPage As Integer

        Public Structure DataPage
            Public table As DataTable
            Private lowestIndexValue As Integer
            Private highestIndexValue As Integer

            Public Sub New(ByVal table As DataTable, ByVal rowIndex As Integer)
                Me.table = table
                lowestIndexValue = MapToLowerBoundary(rowIndex)
                highestIndexValue = MapToUpperBoundary(rowIndex)
                System.Diagnostics.Debug.Assert(lowestIndexValue >= 0)
                System.Diagnostics.Debug.Assert(highestIndexValue >= 0)
            End Sub

            Public ReadOnly Property LowestIndex() As Integer
                Get
                    Return lowestIndexValue
                End Get
            End Property

            Public ReadOnly Property HighestIndex() As Integer
                Get
                    Return highestIndexValue
                End Get
            End Property

            Public Shared Function MapToLowerBoundary(ByVal rowIndex As Integer) As Integer
                ' Return the lowest index of a page containing the given index.
                Return (rowIndex \ RowsPerPage) * RowsPerPage
            End Function

            Private Shared Function MapToUpperBoundary(ByVal rowIndex) As Integer
                ' Return the highest index of a page containing the given index.
                Return MapToLowerBoundary(rowIndex) + RowsPerPage - 1
            End Function
        End Structure

        Private cachePages As DataPage()
        Private dataSupply As IDataPageRetriever

        Public Sub New(ByVal dataSupplier As IDataPageRetriever, ByVal rowsPerPage As Integer)
            dataSupply = dataSupplier
            Cache.RowsPerPage = rowsPerPage
            LoadFirstTwoPages()
        End Sub

        Private Function IfPageCached_ThenSetElement(ByVal rowIndex As Integer, ByVal columnIndex As Integer, ByRef element As String) As Boolean

            If IsRowCachedInPage(0, rowIndex) Then
                element = cachePages(0).table.Rows(rowIndex Mod RowsPerPage)(columnIndex).ToString()
                Return True
            ElseIf (IsRowCachedInPage(1, rowIndex)) Then
                element = cachePages(1).table.Rows(rowIndex Mod RowsPerPage)(columnIndex).ToString()
                Return True
            End If
            Return False
        End Function

        Public Function RetrieveElement(ByVal rowIndex As Integer, ByVal columnIndex As Integer) As String
            Dim element As String = Nothing
            If IfPageCached_ThenSetElement(rowIndex, columnIndex, element) Then
                Return element
            Else
                Return RetrieveData_CacheIt_ThenReturnElement(rowIndex, columnIndex)
            End If
        End Function

        Private Sub LoadFirstTwoPages()
            cachePages = New DataPage() {
                New DataPage(dataSupply.SupplyPageOfData(DataPage.MapToLowerBoundary(0), RowsPerPage), 0), _
                                         New DataPage(dataSupply.SupplyPageOfData(DataPage.MapToLowerBoundary(RowsPerPage), _
                                         RowsPerPage), RowsPerPage)}
        End Sub

        Private Function RetrieveData_CacheIt_ThenReturnElement(ByVal rowIndex As Integer, ByVal columnIndex As Integer)
            ' Retrieve a page worth of data containing the requested value.
            Dim table As DataTable = dataSupply.SupplyPageOfData(DataPage.MapToLowerBoundary(rowIndex), RowsPerPage)
            ' Replace the cached page furthest from the requested cell
            ' with a new page containing the newly retrieved data.
            cachePages(GetIndexToUnusedPage(rowIndex)) = New DataPage(table, rowIndex)
            Return RetrieveElement(rowIndex, columnIndex)
        End Function

        Private Function GetIndexToUnusedPage(ByVal rowIndex As Integer) As Integer
            If rowIndex > cachePages(0).HighestIndex AndAlso rowIndex > cachePages(1).HighestIndex Then
                Dim offsetFromPage0 As Integer = rowIndex - cachePages(0).HighestIndex
                Dim offsetFromPage1 As Integer = rowIndex - cachePages(1).HighestIndex
                If offsetFromPage0 < offsetFromPage1 Then
                    Return 1
                End If
                Return 0
            Else
                Dim offsetFromPage0 As Integer = cachePages(0).LowestIndex - rowIndex
                Dim offsetFromPage1 As Integer = cachePages(1).LowestIndex - rowIndex
                If offsetFromPage0 < offsetFromPage1 Then
                    Return 1
                End If
                Return 0
            End If
        End Function

        Private Function IsRowCachedInPage(ByVal pageNumber As Integer, ByVal rowIndex As Integer) As Boolean
            Return ((rowIndex <= cachePages(pageNumber).HighestIndex) AndAlso (rowIndex >= cachePages(pageNumber).LowestIndex))
        End Function
    End Class
#End Region

#Region "IDataPageRetriever Interface"
    Public Interface IDataPageRetriever
        Function SupplyPageOfData(ByVal lowerPageBoundary As Integer, ByVal rowsPerPage As Integer) As DataTable
    End Interface
#End Region

#Region "DataRetriever Class"
    Public Class DataRetriever
        Implements IDataPageRetriever

        Private tableName As String
        Private command As SqlCommand

        Public Sub New(ByVal connectionString As String, ByVal tableName As String)
            Dim connection As SqlConnection = New SqlConnection(connectionString)
            connection.Open()
            command = connection.CreateCommand()
            Me.tableName = tableName
        End Sub

        Private rowCountValue As Integer = -1

        Public ReadOnly Property RowCount() As Integer
            Get
                ' Return the existing value if it has already been determined.
                If rowCountValue <> -1 Then
                    Return rowCountValue
                End If
                ' Retrieve the row count from the database.
                command.CommandText = "SELECT COUNT(*) FROM " & tableName
                rowCountValue = CType(command.ExecuteScalar(), Integer)
                Return rowCountValue
            End Get
        End Property

        Private columnsValue As DataColumnCollection

        Public ReadOnly Property Columns() As DataColumnCollection
            Get
                ' Return the existing value if it has already been determined.
                If columnsValue IsNot Nothing Then
                    Return columnsValue
                End If

                ' Retrieve the column information from the database.
                command.CommandText = "SELECT * FROM " & tableName
                Dim adapter As SqlDataAdapter = New SqlDataAdapter()
                adapter.SelectCommand = command
                Dim table As DataTable = New DataTable()
                table.Locale = System.Globalization.CultureInfo.InvariantCulture
                adapter.FillSchema(table, SchemaType.Source)
                columnsValue = table.Columns
                Return columnsValue
            End Get
        End Property

        Private commaSeparatedListOfColumnNamesValue As String = Nothing

        Private ReadOnly Property CommaSeparatedListOfColumnNames() As String
            Get
                ' Return the existing value if it has already been determined.

                If commaSeparatedListOfColumnNamesValue IsNot Nothing Then
                    Return commaSeparatedListOfColumnNamesValue
                End If

                ' Store a list of column names for use in the
                ' SupplyPageOfData method.
                Dim commaSeparatedColumnNames As System.Text.StringBuilder = New System.Text.StringBuilder()
                Dim firstColumn As Boolean = True
                For Each column As DataColumn In Columns
                    If Not firstColumn Then
                        commaSeparatedColumnNames.Append(", ")
                    End If
                    commaSeparatedColumnNames.Append(column.ColumnName)
                    firstColumn = False
                Next

                commaSeparatedListOfColumnNamesValue = commaSeparatedColumnNames.ToString()
                Return commaSeparatedListOfColumnNamesValue
            End Get
        End Property

        ' Declare variables to be reused by the SupplyPageOfData method.
        Private columnToSortBy As String
        Private adapter As SqlDataAdapter = New SqlDataAdapter()

        Public Function SupplyPageOfData(ByVal lowerPageBoundary As Integer, ByVal rowsPerPage As Integer) As System.Data.DataTable Implements IDataPageRetriever.SupplyPageOfData
            ' Store the name of the ID column. This column must contain unique
            ' values so the SQL below will work properly.
            If columnToSortBy Is Nothing Then
                columnToSortBy = Me.Columns(0).ColumnName
            End If

            If Not Me.Columns(columnToSortBy).Unique Then
                Throw New InvalidOperationException(String.Format("Column {0} must contain unique values.", columnToSortBy))
            End If

            ' Retrieve the specified number of rows from the database, starting
            ' with the row specified by the lowerPageBoundary parameter.
            command.CommandText = "Select Top " & rowsPerPage & " " & _
            CommaSeparatedListOfColumnNames & " From " & tableName & _
                " WHERE " & columnToSortBy & " NOT IN (SELECT TOP " & _
                lowerPageBoundary & " " & columnToSortBy & " From " & _
                tableName & " Order By " & columnToSortBy & _
                ") Order By " & columnToSortBy

            adapter.SelectCommand = command

            Dim table As DataTable = New DataTable()
            table.Locale = System.Globalization.CultureInfo.InvariantCulture
            adapter.Fill(table)
            Return table
        End Function
    End Class
#End Region

End Namespace
