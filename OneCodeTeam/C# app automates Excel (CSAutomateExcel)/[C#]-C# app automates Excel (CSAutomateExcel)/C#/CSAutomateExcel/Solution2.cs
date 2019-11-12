/******************************** Module Header ********************************\
* Module Name:  Solution2.cs
* Project:      CSAutomateExcel
* Copyright (c) Microsoft Corporation.
* 
* Solution2.AutomateExcel demonstrates automating Microsoft Excel application by 
* using Microsoft Excel Primary Interop Assembly (PIA) and forcing a garbage 
* collection as soon as the automation function is off the stack (at which point 
* the Runtime Callable Wrapper (RCW) objects are no longer rooted) to clean up 
* RCWs and release COM objects.
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
#endregion


namespace CSAutomateExcel
{
    static class Solution2
    {
        public static void AutomateExcel()
        {
            AutomateExcelImpl();


            // Clean up the unmanaged Excel COM resources by forcing a garbage 
            // collection as soon as the calling function is off the stack (at 
            // which point these objects are no longer rooted).

            GC.Collect();
            GC.WaitForPendingFinalizers();
            // GC needs to be called twice in order to get the Finalizers called 
            // - the first time in, it simply makes a list of what is to be 
            // finalized, the second time in, it actually is finalizing. Only 
            // then will the object do its automatic ReleaseComObject.
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private static void AutomateExcelImpl()
        {
            object missing = Type.Missing;

            try
            {
                // Create an instance of Microsoft Excel and make it invisible.

                Excel.Application oXL = new Excel.Application();
                oXL.Visible = false;
                Console.WriteLine("Excel.Application is started");

                // Create a new Workbook.

                Excel.Workbook oWB = oXL.Workbooks.Add(missing);
                Console.WriteLine("A new workbook is created");

                // Get the active Worksheet and set its name.

                Excel.Worksheet oSheet = oWB.ActiveSheet as Excel.Worksheet;
                oSheet.Name = "Report";
                Console.WriteLine("The active worksheet is renamed as Report");

                // Fill data into the worksheet's cells.

                Console.WriteLine("Filling data into the worksheet ...");

                // Set the column header
                oSheet.Cells[1, 1] = "First Name";
                oSheet.Cells[1, 2] = "Last Name";
                oSheet.Cells[1, 3] = "Full Name";

                // Construct an array of user names
                string[,] saNames = new string[,] {
                {"John", "Smith"}, 
                {"Tom", "Brown"}, 
                {"Sue", "Thomas"}, 
                {"Jane", "Jones"}, 
                {"Adam", "Johnson"}};

                // Fill A2:B6 with an array of values (First and Last Names).
                oSheet.get_Range("A2", "B6").Value2 = saNames;

                // Fill C2:C6 with a relative formula (=A2 & " " & B2).
                oSheet.get_Range("C2", "C6").Formula = "=A2 & \" \" & B2";

                // Save the workbook as a xlsx file and close it.

                Console.WriteLine("Save and close the workbook");

                string fileName = Path.GetDirectoryName(
                    Assembly.GetExecutingAssembly().Location) + "\\Sample2.xlsx";
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
                Console.WriteLine("Solution2.AutomateExcel throws the error: {0}",
                    ex.Message);
            }
        }
    }
}