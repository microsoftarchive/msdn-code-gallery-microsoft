Imports System.Net
Imports Microsoft.Office.Interop
Module ExcelMail
    Function StartExcel(Optional ByVal IsVisible As Boolean = True) As Excel.Application
        Dim objExcel As New Excel.Application
        objExcel.Visible = IsVisible
        Return objExcel
    End Function
    Sub ForceExcelToQuit(ByVal objExcel As Excel.Application)
        Try
            objExcel.Quit()
        Catch ex As Exception
        End Try
    End Sub
    Sub DataTableToExcelSheet(ByVal dt As DataTable, ByVal objSheet As Excel.Worksheet, ByVal nStartRow As Integer, ByVal nStartCol As Integer)
        Dim nRow As Integer, nCol As Integer
        For nRow = 0 To dt.Rows.Count - 1
            For nCol = 0 To dt.Columns.Count - 1
                objSheet.Cells(nStartRow + nRow, nStartCol + nCol) = dt.Rows(nRow).Item(nCol)
            Next nCol
        Next nRow
    End Sub
    Function PopulateExcelWorkbook(ByVal strSaveFilename As String, ByVal blnIsVisible As Boolean) As DataTable
        Dim ds As New OrderSchema, dt As OrderSchema.OrderDataTable
        Dim objExcel As Excel.Application, objWorkbook As Excel.Workbook
        Dim objSheet As Excel.Worksheet
        Dim strFileName As String
        '
        'Load the order data from the XML file into a datatable
        ds.ReadXml(My.Application.Info.DirectoryPath & "\OrderData.xml")
        dt = ds.Tables("Order")
        '
        'Start Excel and create a new workbook from the template
        objExcel = StartExcel(blnIsVisible)
        Try
            strFileName = My.Application.Info.DirectoryPath & "\ReportTemplate.xlsx"
            objWorkbook = objExcel.Workbooks.Add(strFileName)
            objSheet = objWorkbook.Sheets("Report")
            '
            'Insert the DataTable into the Excel Spreadsheet
            DataTableToExcelSheet(dt, objSheet, 2, 1)
            'If Visible, then exit so user can see it, otherwise save and exit
            If blnIsVisible = False Then
                objWorkbook.SaveAs(strSaveFilename, Excel.XlFileFormat.xlWorkbookDefault)
                objWorkbook.Close(False)
                objExcel.Quit()
            End If
        Catch ex As Exception
            If blnIsVisible Then MsgBox(ex.ToString, MsgBoxStyle.Exclamation, "Error populating workbook")
            ForceExcelToQuit(objExcel)
        End Try
        Return dt
    End Function
    Function SendExcelMailViaSMTP(ByVal strToAddress As String, ByVal strFromAddress As String, ByVal strFilename As String, ByVal strSmtpHost As String, ByVal blnRemoveFileAfterwards As Boolean) As Boolean
        Try
            Dim objMessage As Mail.MailMessage
            Dim objEmailClient As New Mail.SmtpClient
            objMessage = New Mail.MailMessage(strFromAddress, strToAddress, "Excel Spreadsheet", "Excel Spreadsheet")
            objMessage.Attachments.Add(New Mail.Attachment(strFilename))
            objEmailClient.Host = strSmtpHost
            objEmailClient.Send(objMessage)
            objMessage.Dispose()
            objMessage = Nothing
            objEmailClient = Nothing

            If blnRemoveFileAfterwards = True Then My.Computer.FileSystem.DeleteFile(strFilename)
        Catch ex As Exception
            MsgBox("Please Fill in SMTP Host")
        End Try
        
    End Function
    Function SendExcelMailViaOutlook(ByVal strToAddress As String, ByVal strFromAddress As String, ByVal strFilename As String, ByVal strSmtpHost As String, ByVal blnRemoveFileAfterwards As Boolean) As Boolean
        Try
            Dim objOutlook As Outlook.Application
            Dim objMessage As Outlook.MailItem
            objOutlook = New Outlook.Application
            objMessage = objOutlook.CreateItem(Outlook.OlItemType.olMailItem)
            objMessage.Subject = "Excel Spreadsheet"
            objMessage.Body = "Excel Spreadsheet"
            objMessage.To = strToAddress
            objMessage.Attachments.Add(strFilename)
            objMessage.Send()
            MsgBox("File created and mailed")
            objMessage = Nothing
            objOutlook = Nothing
            If blnRemoveFileAfterwards = True Then My.Computer.FileSystem.DeleteFile(strFilename)
        Catch ex As Exception
            MsgBox("Please Fill in Email Address")
        End Try
        
    End Function

End Module
