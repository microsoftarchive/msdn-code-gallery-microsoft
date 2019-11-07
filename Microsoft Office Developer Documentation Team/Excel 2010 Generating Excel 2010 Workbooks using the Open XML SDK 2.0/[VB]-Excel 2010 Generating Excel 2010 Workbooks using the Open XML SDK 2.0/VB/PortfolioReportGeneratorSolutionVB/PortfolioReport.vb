Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.IO
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Spreadsheet
Imports DocumentFormat.OpenXml

Namespace PortfolioReportGenerator
    Class PortfolioReport
        Private path As String = "c:\example\"
        Private templateName As String = "PortfolioReport.xlsx"
        Private wbPart As WorkbookPart = Nothing
        Private document As SpreadsheetDocument = Nothing
        Private portfolio As Portfolio = Nothing

        Public Sub New(ByVal client As String)
            Dim newFileName As String = path + client + ".xlsx"
            CopyFile(path + templateName, newFileName)
            document = SpreadsheetDocument.Open(newFileName, True)
            wbPart = document.WorkbookPart
            portfolio = New Portfolio(client)
        End Sub

        ' Create a new Portfolio report
        Public Sub CreateReport()
            Dim wsName As String = "Portfolio Summary"

            UpdateValue(wsName, "J2", "Prepared for " + portfolio.Name, 0, True)
            UpdateValue(wsName, "J3", "Account # " + portfolio.AccountNumber.ToString(), 0, True)
            UpdateValue(wsName, "D9", portfolio.BeginningValueQTR.ToString(), 0, False)
            UpdateValue(wsName, "E9", portfolio.BeginningValueYTD.ToString(), 0, False)
            UpdateValue(wsName, "D11", portfolio.ContributionsQTR.ToString(), 0, False)
            UpdateValue(wsName, "E11", portfolio.ContributionsYTD.ToString(), 0, False)
            UpdateValue(wsName, "D12", portfolio.WithdrawalsQTR.ToString(), 0, False)
            UpdateValue(wsName, "E12", portfolio.WithdrawalsYTD.ToString(), 0, False)
            UpdateValue(wsName, "D13", portfolio.DistributionsQTR.ToString(), 0, False)
            UpdateValue(wsName, "E13", portfolio.DistributionsYTD.ToString(), 0, False)
            UpdateValue(wsName, "D14", portfolio.FeesQTR.ToString(), 0, False)
            UpdateValue(wsName, "E14", portfolio.FeesYTD.ToString(), 0, False)
            UpdateValue(wsName, "D15", portfolio.GainLossQTR.ToString(), 0, False)
            UpdateValue(wsName, "E15", portfolio.GainLossYTD.ToString(), 0, False)

            Dim row As Integer = 7
            wsName = "Portfolio Holdings"

            UpdateValue(wsName, "J2", "Prepared for " + portfolio.Name, 0, True)
            UpdateValue(wsName, "J3", "Account # " + portfolio.AccountNumber.ToString(), 0, True)
            For Each item As PortfolioItem In portfolio.Holdings
                UpdateValue(wsName, "B" + row.ToString(), item.Description, 3, True)
                UpdateValue(wsName, "D" + row.ToString(), item.CurrentPrice.ToString(), 24, False)
                UpdateValue(wsName, "E" + row.ToString(), item.SharesHeld.ToString(), 27, False)
                UpdateValue(wsName, "F" + row.ToString(), item.MarketValue.ToString(), 24, False)
                UpdateValue(wsName, "G" + row.ToString(), item.Cost.ToString(), 24, False)
                UpdateValue(wsName, "H" + row.ToString(), item.High52Week.ToString(), 28, False)
                UpdateValue(wsName, "I" + row.ToString(), item.Low52Week.ToString(), 28, False)
                UpdateValue(wsName, "J" + row.ToString(), item.Ticker, 11, True)
                row += 1
            Next

            ' Force re-calc when the workbook is opened
            Me.RemoveCellValue("Portfolio Summary", "D17")
            Me.RemoveCellValue("Portfolio Summary", "E17")

            ' All done! Close and save the document.
            document.Close()
        End Sub

        Private Function CopyFile(ByVal source As String, ByVal dest As String) As String
            Dim result As String = "Copied file"
            Try
                ' Overwrites existing files
                File.Copy(source, dest, True)
            Catch ex As Exception
                result = ex.Message
            End Try
            Return result
        End Function

        ' Given a Worksheet and an address (like "AZ254"), either return a cell reference, or 
        ' create the cell reference and return it.
        Private Function InsertCellInWorksheet(ByVal ws As Worksheet, ByVal addressName As String) As Cell
            Dim sheetData As SheetData = ws.GetFirstChild(Of SheetData)()
            Dim cell As Cell = Nothing

            Dim rowNumber As UInt32 = GetRowIndex(addressName)
            Dim row As Row = GetRow(sheetData, rowNumber)

            ' If the cell you need already exists, return it.
            ' If there is not a cell with the specified column name, insert one.  
            Dim refCell As Cell = row.Elements(Of Cell)().Where(Function(c) c.CellReference.Value = addressName).FirstOrDefault()
            If refCell IsNot Nothing Then
                cell = refCell
            Else
                cell = CreateCell(row, addressName)
            End If
            Return cell
        End Function

        Private Function CreateCell(ByVal row As Row, ByVal address As [String]) As Cell
            Dim cellResult As Cell
            Dim refCell As Cell = Nothing

            ' Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
            For Each cell As Cell In row.Elements(Of Cell)()
                If String.Compare(cell.CellReference.Value, address, True) > 0 Then
                    refCell = cell
                    Exit For
                End If
            Next

            cellResult = New Cell()
            cellResult.CellReference = address

            row.InsertBefore(cellResult, refCell)
            Return cellResult
        End Function

        Private Function GetRow(ByVal wsData As SheetData, ByVal rowIndex As UInt32) As Row
            Dim row = wsData.Elements(Of Row)().Where(Function(r) r.RowIndex.Value = rowIndex).FirstOrDefault()
            If row Is Nothing Then
                row = New Row()
                row.RowIndex = rowIndex
                wsData.Append(row)
            End If
            Return row
        End Function

        Private Function GetRowIndex(ByVal address As String) As UInt32
            Dim rowPart As String
            Dim l As UInt32
            Dim result As UInt32 = 0

            For i As Integer = 0 To address.Length - 1
                If UInt32.TryParse(address.Substring(i, 1), l) Then
                    rowPart = address.Substring(i, address.Length - i)
                    If UInt32.TryParse(rowPart, l) Then
                        result = l
                        Exit For
                    End If
                End If
            Next
            Return result
        End Function

        Public Function UpdateValue(ByVal sheetName As String, ByVal addressName As String, ByVal value As String, ByVal styleIndex As Integer, ByVal isString As Boolean) As Boolean
            ' Assume failure.
            Dim updated As Boolean = False

            Dim sheet As Sheet = wbPart.Workbook.Descendants(Of Sheet)().Where(Function(s) s.Name = sheetName).FirstOrDefault()

            If sheet IsNot Nothing Then
                Dim ws As Worksheet = DirectCast(wbPart.GetPartById(sheet.Id), WorksheetPart).Worksheet
                Dim cell As Cell = InsertCellInWorksheet(ws, addressName)

                If isString Then
                    ' Either retrieve the index of an existing string,
                    ' or insert the string into the shared string table
                    ' and get the index of the new item.
                    Dim stringIndex As Integer = InsertSharedStringItem(wbPart, value)

                    cell.CellValue = New CellValue(stringIndex.ToString())
                    cell.DataType = New EnumValue(Of CellValues)(CellValues.SharedString)
                Else
                    cell.CellValue = New CellValue(value)
                    cell.DataType = New EnumValue(Of CellValues)(CellValues.Number)
                End If

                'If styleIndex > 0 Then
                '    cell.StyleIndex = styleIndex
                'End If

                If styleIndex > 0 Then
                    cell.StyleIndex = styleIndex
                End If

                ' Save the worksheet.
                ws.Save()
                updated = True
            End If

            Return updated
        End Function

        ' Given the main workbook part, and a text value, insert the text into the shared
        ' string table. Create the table if necessary. If the value already exists, return
        ' its index. If it doesn't exist, insert it and return its new index.
        Private Function InsertSharedStringItem(ByVal wbPart As WorkbookPart, ByVal value As String) As Integer
            Dim index As Integer = 0
            Dim found As Boolean = False
            Dim stringTablePart = wbPart.GetPartsOfType(Of SharedStringTablePart)().FirstOrDefault()

            ' If the shared string table is missing, something's wrong.
            ' Just return the index that you found in the cell.
            ' Otherwise, look up the correct text in the table.
            If stringTablePart Is Nothing Then
                ' Create it.
                stringTablePart = wbPart.AddNewPart(Of SharedStringTablePart)()
            End If

            Dim stringTable = stringTablePart.SharedStringTable
            If stringTable Is Nothing Then
                stringTable = New SharedStringTable()
            End If

            ' Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            For Each item As SharedStringItem In stringTable.Elements(Of SharedStringItem)()
                If item.InnerText = value Then
                    found = True
                    Exit For
                End If
                index += 1
            Next

            If Not found Then
                stringTable.AppendChild(New SharedStringItem(New Text(value)))
                stringTable.Save()
            End If

            Return index
        End Function

        ' Used to force a recalc of cells containing formulas. The
        ' CellValue has a cached value of the evaluated formula. This
        ' will prevent Excel from recalculating the cell even if 
        ' calculation is set to automatic.
        Private Function RemoveCellValue(ByVal sheetName As String, ByVal addressName As String) As Boolean
            Dim returnValue As Boolean = False

            Dim sheet As Sheet = wbPart.Workbook.Descendants(Of Sheet)().Where(Function(s) s.Name = sheetName).FirstOrDefault()
            If sheet IsNot Nothing Then
                Dim ws As Worksheet = DirectCast(wbPart.GetPartById(sheet.Id), WorksheetPart).Worksheet
                Dim cell As Cell = InsertCellInWorksheet(ws, addressName)

                ' If there is a cell value, remove it to force a recalc
                ' on this cell.
                If cell.CellValue IsNot Nothing Then
                    cell.CellValue.Remove()
                End If

                ' Save the worksheet.
                ws.Save()
                returnValue = True
            End If

            Return returnValue
        End Function
    End Class
End Namespace
