'****************************** Module Header ******************************\
'Module Name:    WebForm1.aspx.vb
'Project:        VBNETWebExportDataTabletoExcel
'Copyright (c) Microsoft Corporation

'The project illustrates how to export data from DataSet into an Excel workbook in ASP.NET

'This source is subject to the Microsoft Public License.
'See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
'All other rights reserved.

'*****************************************************************************/

Public Class WebForm1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            ' Construct the datatable programetically
            Dim datatable As DataTable = MakeDataTable()

            Dim appPath As String = AppDomain.CurrentDomain.BaseDirectory

            Dim excelFileName As String = appPath & Convert.ToString("\TestFile.xlsx")

            ExportDataSet(datatable, excelFileName)

            Response.Write(vbLf & vbLf & "Datatable exported to Excel file successfully")
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub

    Private Function MakeDataTable() As DataTable
        ' Create a new DataTable.
        Dim table As System.Data.DataTable = New DataTable("ParentTable")
        ' Declare variables for DataColumn and DataRow objects.
        Dim column As DataColumn
        Dim row As DataRow

        ' Create new DataColumn, set DataType, 
        ' ColumnName and add to DataTable.    
        column = New DataColumn()
        column.DataType = System.Type.[GetType]("System.Int32")
        column.ColumnName = "id"
        column.[ReadOnly] = True
        column.Unique = True
        ' Add the Column to the DataColumnCollection.
        table.Columns.Add(column)

        ' Create second column.
        column = New DataColumn()
        column.DataType = System.Type.[GetType]("System.String")
        column.ColumnName = "ParentItem"
        column.AutoIncrement = False
        column.Caption = "ParentItem"
        column.[ReadOnly] = False
        column.Unique = False
        ' Add the column to the table.
        table.Columns.Add(column)

        ' Make the ID column the primary key column.
        Dim PrimaryKeyColumns As DataColumn() = New DataColumn(0) {}
        PrimaryKeyColumns(0) = table.Columns("id")
        table.PrimaryKey = PrimaryKeyColumns

        ' Create three new DataRow objects and add 
        ' them to the DataTable
        For i As Integer = 0 To 2
            row = table.NewRow()
            row("id") = i
            row("ParentItem") = "ParentItem " + i.ToString()
            table.Rows.Add(row)
        Next

        Return table
    End Function


    Private Sub ExportDataSet(table As DataTable, excelFileName As String)
        Using workbook = SpreadsheetDocument.Create(excelFileName, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook)
            Dim workbookPart = workbook.AddWorkbookPart()

            workbook.WorkbookPart.Workbook = New DocumentFormat.OpenXml.Spreadsheet.Workbook()

            workbook.WorkbookPart.Workbook.Sheets = New DocumentFormat.OpenXml.Spreadsheet.Sheets()

            Dim sheetPart = workbook.WorkbookPart.AddNewPart(Of WorksheetPart)()
            Dim sheetData = New DocumentFormat.OpenXml.Spreadsheet.SheetData()
            sheetPart.Worksheet = New DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData)

            Dim sheets As DocumentFormat.OpenXml.Spreadsheet.Sheets = workbook.WorkbookPart.Workbook.GetFirstChild(Of DocumentFormat.OpenXml.Spreadsheet.Sheets)()
            Dim relationshipId As String = workbook.WorkbookPart.GetIdOfPart(sheetPart)

            Dim sheetId As UInteger = 1
            If sheets.Elements(Of DocumentFormat.OpenXml.Spreadsheet.Sheet)().Count() > 0 Then
                sheetId = sheets.Elements(Of DocumentFormat.OpenXml.Spreadsheet.Sheet)().[Select](Function(s) s.SheetId.Value).Max() + 1
            End If

            Dim sheet As New DocumentFormat.OpenXml.Spreadsheet.Sheet() With { _
                 .Id = relationshipId, _
                 .SheetId = sheetId, _
                .Name = table.TableName _
            }
            sheets.Append(sheet)

            Dim headerRow As New DocumentFormat.OpenXml.Spreadsheet.Row()


            ' Construct column names
            Dim columns As List(Of [String]) = New List(Of String)()
            For Each column As System.Data.DataColumn In table.Columns
                columns.Add(column.ColumnName)

                Dim cell As New DocumentFormat.OpenXml.Spreadsheet.Cell()
                cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.[String]
                cell.CellValue = New DocumentFormat.OpenXml.Spreadsheet.CellValue(column.ColumnName)
                headerRow.AppendChild(cell)
            Next

            ' Add the row values to the excel sheet
            sheetData.AppendChild(headerRow)

            For Each dsrow As System.Data.DataRow In table.Rows
                Dim newRow As New DocumentFormat.OpenXml.Spreadsheet.Row()
                For Each col As [String] In columns
                    Dim cell As New DocumentFormat.OpenXml.Spreadsheet.Cell()
                    cell.DataType = DocumentFormat.OpenXml.Spreadsheet.CellValues.[String]
                    cell.CellValue = New DocumentFormat.OpenXml.Spreadsheet.CellValue(dsrow(col).ToString())
                    newRow.AppendChild(cell)
                Next

                sheetData.AppendChild(newRow)
            Next
        End Using
    End Sub
End Class