'/****************************** Module Header ******************************\
' Module Name:  ConvertExcelToXml.vb
' Project:      VBOpenXmlExcelToXml
' Copyright(c)  Microsoft Corporation.
' 
' This class is used to convert excel data to XML format string using Open XMl
' Firstly, we use OpenXML API to get data from Excel to DataTable
' Then we Load the DataTable to Dataset.
' At Last,we call DataSet.GetXml() to get XML format string 
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/

Imports System.IO
Imports System.Text.RegularExpressions
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Spreadsheet


Public Class ConvertExcelToXml

    ''' <summary>
    '''  Read Data from selected excel file into DataTable
    ''' </summary>
    ''' <param name="filename">Excel File Path</param>
    ''' <returns></returns>
    Private Function ReadExcelFile(filename As String) As DataTable
        ' Initialize an instance of DataTable
        Dim dt As New DataTable()

        Try
            ' Use SpreadSheetDocument class of Open XML SDK to open excel file
            Using spreadsheetDocument__1 As SpreadsheetDocument = SpreadsheetDocument.Open(filename, False)
                ' Get Workbook Part of Spread Sheet Document
                Dim workbookPart As WorkbookPart = spreadsheetDocument__1.WorkbookPart

                ' Get all sheets in spread sheet document 
                Dim sheetcollection As IEnumerable(Of Sheet) = spreadsheetDocument__1.WorkbookPart.Workbook.GetFirstChild(Of Sheets)().Elements(Of Sheet)()

                ' Get relationship Id
                Dim relationshipId As String = sheetcollection.First().Id.Value

                ' Get sheet1 Part of Spread Sheet Document
                Dim worksheetPart As WorksheetPart = DirectCast(spreadsheetDocument__1.WorkbookPart.GetPartById(relationshipId), WorksheetPart)

                ' Get Data in Excel file
                Dim sheetData As SheetData = worksheetPart.Worksheet.Elements(Of SheetData)().First()
                Dim rowcollection As IEnumerable(Of Row) = sheetData.Descendants(Of Row)()

                If rowcollection.Count() = 0 Then
                    Return dt
                End If

                ' Add columns
                For Each cell As Cell In rowcollection.ElementAt(0)
                    dt.Columns.Add(GetValueOfCell(spreadsheetDocument__1, cell))
                Next

                ' Add rows into DataTable
                For Each row As Row In rowcollection
                    Dim temprow As DataRow = dt.NewRow()
                    Dim columnIndex As Integer = 0
                    For Each cell As Cell In row.Descendants(Of Cell)()
                        ' Get Cell Column Index
                        Dim cellColumnIndex As Integer = GetColumnIndex(GetColumnName(cell.CellReference))

                        If columnIndex < cellColumnIndex Then
                            Do
                                temprow(columnIndex) = String.Empty
                                columnIndex += 1

                            Loop While columnIndex < cellColumnIndex
                        End If

                        temprow(columnIndex) = GetValueOfCell(spreadsheetDocument__1, cell)
                        columnIndex += 1
                    Next

                    ' Add the row to DataTable
                    ' the rows include header row
                    dt.Rows.Add(temprow)
                Next
            End Using

            ' Here remove header row
            dt.Rows.RemoveAt(0)
            Return dt
        Catch ex As IOException
            Throw New IOException(ex.Message)
        End Try
    End Function

    ''' <summary>
    '''  Get Value of Cell 
    ''' </summary>
    ''' <param name="spreadsheetdocument">SpreadSheet Document Object</param>
    ''' <param name="cell">Cell Object</param>
    ''' <returns>The Value in Cell</returns>
    Private Shared Function GetValueOfCell(spreadsheetdocument As SpreadsheetDocument, cell As Cell) As String
        ' Get value in Cell
        Dim sharedString As SharedStringTablePart = spreadsheetdocument.WorkbookPart.SharedStringTablePart
        If cell.CellValue Is Nothing Then
            Return String.Empty
        End If

        Dim cellValue As String = cell.CellValue.InnerText

        ' The condition that the Cell DataType is SharedString
        If cell.DataType IsNot Nothing AndAlso cell.DataType.Value = CellValues.SharedString Then
            Return sharedString.SharedStringTable.ChildElements(Integer.Parse(cellValue)).InnerText
        Else
            Return cellValue
        End If
    End Function

    ''' <summary>
    ''' Get Column Name From given cell name
    ''' </summary>
    ''' <param name="cellReference">Cell Name(For example,A1)</param>
    ''' <returns>Column Name(For example, A)</returns>
    Private Function GetColumnName(cellReference As String) As String
        ' Create a regular expression to match the column name of cell
        Dim regex As New Regex("[A-Za-z]+")
        Dim match As Match = regex.Match(cellReference)
        Return match.Value
    End Function

    ''' <summary>
    ''' Get Index of Column from given column name
    ''' </summary>
    ''' <param name="columnName">Column Name(For Example,A or AA)</param>
    ''' <returns>Column Index</returns>
    Private Function GetColumnIndex(columnName As String) As Integer
        Dim columnIndex As Integer = 0
        Dim factor As Integer = 1

        ' From right to left
        For position As Integer = columnName.Length - 1 To 0 Step -1
            ' For letters
            If [Char].IsLetter(columnName(position)) Then
                columnIndex += factor * ((AscW(columnName(position)) - AscW("A"c)) + 1) - 1

                factor *= 26
            End If
        Next

        Return columnIndex
    End Function

    ''' <summary>
    ''' Convert DataTable to Xml format
    ''' </summary>
    ''' <param name="filename">Excel File Path</param>
    ''' <returns>Xml format string</returns>
    Public Function GetXML(filename As String) As String
        Using ds As New DataSet()
            ds.Tables.Add(Me.ReadExcelFile(filename))
            Return ds.GetXml()
        End Using
    End Function

End Class
