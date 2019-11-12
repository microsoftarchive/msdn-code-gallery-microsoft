'****************************** Module Header ******************************'
' Module Name:  Solution1.vb
' Project:      VBAutomateExcel
' Copyright (c) Microsoft Corporation.
' 
' Solution1.AutomateExcel demonstrates automating Microsoft Excel application by 
' using Microsoft Excel Primary Interop Assembly (PIA) and explicitly assigning 
' each COM accessor object to a new variable that you would explicitly call 
' Marshal.FinalReleaseComObject to release it at the end. When you use this 
' solution, it is important to avoid calls that tunnel into the object model 
' because they will orphan Runtime Callable Wrapper (RCW) on the heap that you 
' will not be able to access in order to call Marshal.ReleaseComObject. You need 
' to be very careful. For example, 
' 
'   Dim oWB As Excel.Workbook = oXL.Workbooks.Add()
' 
' Calling oXL.Workbooks.Add creates an RCW for the Workbooks object. If you 
' invoke these accessors via tunneling as this code does, the RCW for Workbooks 
' is created on the GC heap, but the reference is created under the hood on the 
' stack and are then discarded. As such, there is no way to call 
' MarshalFinalReleaseComObject on this RCW. To get such kind of RCWs released, 
' you would either need to force a garbage collection as soon as the calling 
' function is off the stack (see Solution2.AutomateExcel), or you would need to 
' explicitly assign each accessor object to a variable and free it.
' 
'   Dim oWBs As Excel.Workbooks = oXL.Workbooks
'   Dim oWB As Excel.Workbook = oWBs.Add()
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************'

#Region "Imports directives"

Imports System.Reflection
Imports System.IO

Imports Excel = Microsoft.Office.Interop.Excel
Imports System.Runtime.InteropServices

#End Region


Class Solution1

    Public Shared Sub AutomateExcel()

        Dim oXL As Excel.Application = Nothing
        Dim oWBs As Excel.Workbooks = Nothing
        Dim oWB As Excel.Workbook = Nothing
        Dim oSheet As Excel.Worksheet = Nothing
        Dim oCells As Excel.Range = Nothing
        Dim oRng1 As Excel.Range = Nothing
        Dim oRng2 As Excel.Range = Nothing

        Try
            ' Create an instance of Microsoft Excel and make it invisible.

            oXL = New Excel.Application
            oXL.Visible = False
            Console.WriteLine("Excel.Application is started")

            ' Create a new workbook.

            oWBs = oXL.Workbooks
            oWB = oWBs.Add()
            Console.WriteLine("A new workbook is created")

            ' Get the active Worksheet and set its name.

            oSheet = oWB.ActiveSheet
            oSheet.Name = "Report"
            Console.WriteLine("The active worksheet is renamed as Report")

            ' Fill data into the worksheet's cells.

            Console.WriteLine("Filling data into the worksheet ...")

            ' Set the column header
            oCells = oSheet.Cells
            oCells(1, 1) = "First Name"
            oCells(1, 2) = "Last Name"
            oCells(1, 3) = "Full Name"

            ' Construct an array of user names
            Dim saNames(,) As String = {{"John", "Smith"}, _
                                        {"Tom", "Brown"}, _
                                        {"Sue", "Thomas"}, _
                                        {"Jane", "Jones"}, _
                                        {"Adam", "Johnson"}}

            ' Fill A2:B6 with an array of values (First and Last Names).
            oRng1 = oSheet.Range("A2", "B6")
            oRng1.Value2 = saNames

            ' Fill C2:C6 with a relative formula (=A2 & " " & B2).
            oRng2 = oSheet.Range("C2", "C6")
            oRng2.Formula = "=A2 & "" "" & B2"

            ' Save the workbook as a xlsx file and close it.

            Console.WriteLine("Save and close the workbook")

            Dim fileName As String = Path.GetDirectoryName( _
            Assembly.GetExecutingAssembly().Location) & "\Sample1.xlsx"
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
            Console.WriteLine("Solution1.AutomateExcel throws the error: {0}", _
                              ex.Message)
        Finally

            ' Clean up the unmanaged Excel COM resources by explicitly call 
            ' Marshal.FinalReleaseComObject on all accessor objects. 
            ' See http://support.microsoft.com/kb/317109.

            If Not oRng2 Is Nothing Then
                Marshal.FinalReleaseComObject(oRng2)
                oRng2 = Nothing
            End If
            If Not oRng1 Is Nothing Then
                Marshal.FinalReleaseComObject(oRng1)
                oRng1 = Nothing
            End If
            If Not oCells Is Nothing Then
                Marshal.FinalReleaseComObject(oCells)
                oCells = Nothing
            End If
            If Not oSheet Is Nothing Then
                Marshal.FinalReleaseComObject(oSheet)
                oSheet = Nothing
            End If
            If Not oWB Is Nothing Then
                Marshal.FinalReleaseComObject(oWB)
                oWB = Nothing
            End If
            If Not oWBs Is Nothing Then
                Marshal.FinalReleaseComObject(oWBs)
                oWBs = Nothing
            End If
            If Not oXL Is Nothing Then
                Marshal.FinalReleaseComObject(oXL)
                oXL = Nothing
            End If

        End Try

    End Sub

End Class
