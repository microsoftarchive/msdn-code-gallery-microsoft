'****************************** Module Header ******************************\
' Module Name:  Form1.vb
' Project:      VBDGViewToExcel
' Copyright (c) Microsoft Corporation.
'
' The sample demostrates how to export DataGridView to Excel.
'
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/
Public Class Form1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PopulateRows()
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        ExportToExcel()
    End Sub

    Private Sub PopulateRows()
        For i As Integer = 1 To 10
            Dim row As DataGridViewRow = DirectCast(dgvCityDetails.RowTemplate.Clone(), DataGridViewRow)

            row.CreateCells(dgvCityDetails, String.Format("City{0}", i), String.Format("State{0}", i), String.Format("Country{0}", i))


            dgvCityDetails.Rows.Add(row)
        Next
    End Sub

    Private Sub ExportToExcel()
        ' Creating a Excel object.
        Dim excel As Microsoft.Office.Interop.Excel._Application = New Microsoft.Office.Interop.Excel.Application()
        Dim workbook As Microsoft.Office.Interop.Excel._Workbook = excel.Workbooks.Add(Type.Missing)
        Dim worksheet As Microsoft.Office.Interop.Excel._Worksheet = Nothing

        Try

            worksheet = workbook.ActiveSheet

            worksheet.Name = "ExportedFromDatGrid"

            Dim cellRowIndex As Integer = 1
            Dim cellColumnIndex As Integer = 1

            'Loop through each row and read value from each column.
            For i As Integer = 0 To dgvCityDetails.Rows.Count - 2
                For j As Integer = 0 To dgvCityDetails.Columns.Count - 1
                    ' Excel index starts from 1,1. As first Row would have the Column headers, adding a condition check.
                    If cellRowIndex = 1 Then
                        worksheet.Cells(cellRowIndex, cellColumnIndex) = dgvCityDetails.Columns(j).HeaderText
                    Else
                        worksheet.Cells(cellRowIndex, cellColumnIndex) = dgvCityDetails.Rows(i).Cells(j).Value.ToString()
                    End If
                    cellColumnIndex += 1
                Next
                cellColumnIndex = 1
                cellRowIndex += 1
            Next

            'Getting the location and file name of the excel to save from user.
            Dim saveDialog As New SaveFileDialog()
            saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*"
            saveDialog.FilterIndex = 2

            If saveDialog.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
                workbook.SaveAs(saveDialog.FileName)
                MessageBox.Show("Export Successful")
            End If
        Catch ex As System.Exception
            MessageBox.Show(ex.Message)
        Finally
            excel.Quit()
            workbook = Nothing
            excel = Nothing
        End Try

    End Sub

End Class
