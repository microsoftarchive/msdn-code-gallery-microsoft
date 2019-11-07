'****************************** Module Header ******************************'
' Module Name:  Solution2.vb
' Project:      VBAutomateExcel
' Copyright (c) Microsoft Corporation.
' 
' Solution2.AutomateExcel demonstrates automating Microsoft Excel 
' application by using Microsoft Excel Primary Interop Assembly (PIA) and 
' forcing a garbage collection as soon as the automation function is off the 
' stack (at which point the Runtime Callable Wrapper (RCW) objects are no 
' longer rooted) to clean up RCWs and release COM objects.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

#Region "Import directives"

Imports System.IO
Imports System.Reflection

Imports Excel = Microsoft.Office.Interop.Excel

#End Region


Class Solution2

    Public Shared Sub AutomateExcel()

        AutomateExcelImpl()


        ' Clean up the unmanaged Excel COM resources by forcing a garbage 
        ' collection as soon as the calling function is off the stack (at 
        ' which point these objects are no longer rooted).

        GC.Collect()
        GC.WaitForPendingFinalizers()
        ' GC needs to be called twice in order to get the Finalizers called 
        ' - the first time in, it simply makes a list of what is to be 
        ' finalized, the second time in, it actually the finalizing. Only 
        ' then will the object do its automatic ReleaseComObject.
        GC.Collect()
        GC.WaitForPendingFinalizers()

    End Sub


    Private Shared Sub AutomateExcelImpl()

        Try
            ' Create an instance of Microsoft Excel and make it invisible.

            Dim oXL As New Excel.Application
            oXL.Visible = False
            Console.WriteLine("Excel.Application is started")

            ' Create a new workbook.

            Dim oWB As Excel.Workbook = oXL.Workbooks.Add()
            Console.WriteLine("A new workbook is created")

            ' Get the active Worksheet and set its name.

            Dim oSheet As Excel.Worksheet = oWB.ActiveSheet
            oSheet.Name = "Report"
            Console.WriteLine("The active worksheet is renamed as Report")

            ' Fill data into the worksheet's cells.

            Console.WriteLine("Filling data into the worksheet ...")

            ' Set the column header
            oSheet.Cells(1, 1) = "First Name"
            oSheet.Cells(1, 2) = "Last Name"
            oSheet.Cells(1, 3) = "Full Name"

            ' Construct an array of user names
            Dim saNames(,) As String = {{"John", "Smith"}, _
                                        {"Tom", "Brown"}, _
                                        {"Sue", "Thomas"}, _
                                        {"Jane", "Jones"}, _
                                        {"Adam", "Johnson"}}

            ' Fill A2:B6 with an array of values (First and Last Names).
            oSheet.Range("A2", "B6").Value2 = saNames

            ' Fill C2:C6 with a relative formula (=A2 & " " & B2).
            oSheet.Range("C2", "C6").Formula = "=A2 & "" "" & B2"

            ' Save the workbook as a xlsx file and close it.

            Console.WriteLine("Save and close the workbook")

            Dim fileName As String = Path.GetDirectoryName( _
            Assembly.GetExecutingAssembly().Location) & "\Sample2.xlsx"
            oWB.SaveAs(fileName, Excel.XlFileFormat.xlOpenXMLWorkbook)
            oWB.Close()

            ' Close the Excel application.

            Console.WriteLine("Quit the Excel application")

            ' Excel will stick around after Quit if it is not under user 
            ' control and there are outstanding references. When Excel is 
            ' started or attached programmatically and 
            ' Application.Visible = false, Application.UserControl is false. 
            ' The UserControl property can be explicitly set to True which 
            ' should force the application to terminate when Quit is called, 
            ' regardless of outstanding references.
            oXL.UserControl = True

            oXL.Quit()

        Catch ex As Exception
            Console.WriteLine("Solution2.AutomateExcel throws the error: {0}", _
                              ex.Message)
        End Try

    End Sub

End Class
