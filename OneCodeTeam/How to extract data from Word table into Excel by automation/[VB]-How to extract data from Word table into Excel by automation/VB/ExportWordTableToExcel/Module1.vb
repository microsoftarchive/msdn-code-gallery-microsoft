Module Module1

    Sub Main()
        Try
            Dim appPath As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)

            Dim excelFile As String = appPath & Convert.ToString("\TestExcel.xlsx")
            Dim wordFile As String = appPath & Convert.ToString("\TestDoc.docx")

            ' Delete the excel file if already exists
            If File.Exists(excelFile) Then
                File.Delete(excelFile)
            End If

            Dim objWordApp As Microsoft.Office.Interop.Word._Application = New Microsoft.Office.Interop.Word.Application()

            If objWordApp Is Nothing Then
                Console.WriteLine("Word could not be started. Check that your office installation and project references are correct.")
                Return
            End If

            objWordApp.Visible = False

            Dim objDoc As Microsoft.Office.Interop.Word._Document = objWordApp.Documents.Open(wordFile)

            If objDoc.Tables.Count = 0 Then
                Console.WriteLine("This document contains no tables")
                Return
            End If

            Dim objExcelApp As Microsoft.Office.Interop.Excel._Application = New Microsoft.Office.Interop.Excel.Application()

            objExcelApp.Visible = False

            Dim workbook As Microsoft.Office.Interop.Excel._Workbook = objExcelApp.Workbooks.Add(1)

            Dim worksheet As Microsoft.Office.Interop.Excel._Worksheet = DirectCast(workbook.Sheets(1), Microsoft.Office.Interop.Excel.Worksheet)

            If worksheet Is Nothing Then
                Console.WriteLine("Worksheet could not be created. Check that your office installation and project references are correct.")
                Return
            End If

            For Each table As Microsoft.Office.Interop.Word.Table In objDoc.Tables
                For row As Integer = 1 To table.Rows.Count
                    For col As Integer = 1 To table.Columns.Count
                        worksheet.Cells(row, col) = objExcelApp.WorksheetFunction.Clean(table.Cell(row, col).Range.Text)
                    Next
                Next
            Next

            ' Save the excel file
            workbook.SaveAs(excelFile, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault)
            objExcelApp.Workbooks.Close()
            objExcelApp.Quit()

            objWordApp.Documents.Close()
            objWordApp.Application.Quit()

            Console.WriteLine(Convert.ToString("Word document table contents exported to excel file:") & excelFile)

            Console.ReadLine()
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub

End Module
