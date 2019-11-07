/******************************** Module Header ********************************\
* Module Name:  Solution1.cs
* Project:      CSAutomateExcel
* Copyright (c) Microsoft Corporation.
* 
* Solution1.AutomateExcel demonstrates automating Microsoft Excel application by 
* using Microsoft Excel Primary Interop Assembly (PIA) and explicitly assigning 
* each COM accessor object to a new variable that you would explicitly call 
* Marshal.FinalReleaseComObject to release it at the end. When you use this 
* solution, it is important to avoid calls that tunnel into the object model 
* because they will orphan Runtime Callable Wrapper (RCW) on the heap that you 
* will not be able to access in order to call Marshal.ReleaseComObject. You need 
* to be very careful. For example, 
* 
*   Excel.Workbook oWB = oXL.Workbooks.Add(missing);
* 
* Calling oXL.Workbooks.Add creates an RCW for the Workbooks object. If you 
* invoke these accessors via tunneling as this code does, the RCW for Workbooks 
* is created on the GC heap, but the reference is created under the hood on the 
* stack and are then discarded. As such, there is no way to call 
* MarshalFinalReleaseComObject on this RCW. To get such kind of RCWs released, 
* you would either need to force a garbage collection as soon as the calling 
* function is off the stack (see Solution2.AutomateExcel), or you would need to 
* explicitly assign each accessor object to a variable and free it.
* 
*   Excel.Workbooks oWBs = oXL.Workbooks;
*   Excel.Workbook oWB = oWBs.Add(missing);
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*******************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
#endregion


namespace CSAutomateExcel
{
    static class Solution1
    {
        public static void AutomateExcel()
        {
            object missing = Type.Missing;
            Excel.Application oXL = null;
            Excel.Workbooks oWBs = null;
            Excel.Workbook oWB = null;
            Excel.Worksheet oSheet = null;
            Excel.Range oCells = null;
            Excel.Range oRng1 = null;
            Excel.Range oRng2 = null;

            try
            {
                // Create an instance of Microsoft Excel and make it invisible.

                oXL = new Excel.Application();
                oXL.Visible = false;
                Console.WriteLine("Excel.Application is started");

                // Create a new Workbook.

                oWBs = oXL.Workbooks;
                oWB = oWBs.Add(missing);
                Console.WriteLine("A new workbook is created");

                // Get the active Worksheet and set its name.
                oSheet = oWB.ActiveSheet as Excel.Worksheet;
                oSheet.Name = "Report";
                Console.WriteLine("The active worksheet is renamed as Report");

                // Fill data into the worksheet's cells.

                Console.WriteLine("Filling data into the worksheet ...");

                // Set the column header
                oCells = oSheet.Cells;
                oCells[1, 1] = "First Name";
                oCells[1, 2] = "Last Name";
                oCells[1, 3] = "Full Name";

                // Construct an array of user names
                string[,] saNames = new string[,] {
                {"John", "Smith"}, 
                {"Tom", "Brown"}, 
                {"Sue", "Thomas"}, 
                {"Jane", "Jones"}, 
                {"Adam", "Johnson"}};

                // Fill A2:B6 with an array of values (First and Last Names).
                oRng1 = oSheet.get_Range("A2", "B6");
                oRng1.Value2 = saNames;

                // Fill C2:C6 with a relative formula (=A2 & " " & B2).
                oRng2 = oSheet.get_Range("C2", "C6");
                oRng2.Formula = "=A2 & \" \" & B2";

                // Save the workbook as a xlsx file and close it.

                Console.WriteLine("Save and close the workbook");

                string fileName = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) + "\\Sample1.xlsx";
                oWB.SaveAs(fileName, Excel.XlFileFormat.xlOpenXMLWorkbook,
                    missing, missing, missing, missing,
                    Excel.XlSaveAsAccessMode.xlNoChange,
                    missing, missing, missing, missing, missing);
                oWB.Close(missing, missing, missing);

                // Quit the Excel application.

                Console.WriteLine("Quit the Excel application");

                // Excel will stick around after Quit if it is not under user 
                // control and there are outstanding references. When Excel is 
                // started or attached programmatically and 
                // Application.Visible = false, Application.UserControl is false. 
                // The UserControl property can be explicitly set to True which 
                // should force the application to terminate when Quit is called, 
                // regardless of outstanding references.
                oXL.UserControl = true;

                oXL.Quit();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Solution1.AutomateExcel throws the error: {0}",
                    ex.Message);
            }
            finally
            {
                // Clean up the unmanaged Excel COM resources by explicitly 
                // calling Marshal.FinalReleaseComObject on all accessor objects. 
                // See http://support.microsoft.com/kb/317109.

                if (oRng2 != null)
                {
                    Marshal.FinalReleaseComObject(oRng2);
                    oRng2 = null;
                }
                if (oRng1 != null)
                {
                    Marshal.FinalReleaseComObject(oRng1);
                    oRng1 = null;
                }
                if (oCells != null)
                {
                    Marshal.FinalReleaseComObject(oCells);
                    oCells = null;
                }
                if (oSheet != null)
                {
                    Marshal.FinalReleaseComObject(oSheet);
                    oSheet = null;
                }
                if (oWB != null)
                {
                    Marshal.FinalReleaseComObject(oWB);
                    oWB = null;
                }
                if (oWBs != null)
                {
                    Marshal.FinalReleaseComObject(oWBs);
                    oWBs = null;
                }
                if (oXL != null)
                {
                    Marshal.FinalReleaseComObject(oXL);
                    oXL = null;
                }
            }
        }
    }
}
