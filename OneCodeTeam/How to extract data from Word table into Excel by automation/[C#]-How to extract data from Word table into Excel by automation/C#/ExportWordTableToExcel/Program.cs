//****************************** Module Header ******************************\
//Module Name:    Program.cs
//Project:        ExportWordTableToExcel
//Copyright (c) Microsoft Corporation

// The project illustrates how to extract data from Word table into Excel by automation

//This source is subject to the Microsoft Public License.
//See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
//All other rights reserved.

//*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop;
using System.IO;
using System.Data;

namespace ExportWordTableToExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                string excelFile = appPath + "\\TestExcel.xlsx";
                string wordFile = appPath + "\\TestDoc.docx";

                // Delete the excel file if already exists
                if (File.Exists(excelFile))
                {
                    File.Delete(excelFile);
                }

                Microsoft.Office.Interop.Word._Application objWordApp = new Microsoft.Office.Interop.Word.Application();

                if (objWordApp == null)
                {
                    Console.WriteLine("Word could not be started. Check that your office installation and project references are correct.");
                    return;
                }

                objWordApp.Visible = false;

                Microsoft.Office.Interop.Word._Document objDoc = objWordApp.Documents.Open(wordFile);

                if (objDoc.Tables.Count == 0)
                {
                    Console.WriteLine("This document contains no tables");
                    return;
                }

                Microsoft.Office.Interop.Excel._Application objExcelApp = new Microsoft.Office.Interop.Excel.Application();

                objExcelApp.Visible = false;

                Microsoft.Office.Interop.Excel._Workbook workbook = objExcelApp.Workbooks.Add(1);

                Microsoft.Office.Interop.Excel._Worksheet worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];

                if (worksheet == null)
                {
                    Console.WriteLine("Worksheet could not be created. Check that your office installation and project references are correct.");
                    return;
                }

                foreach (Microsoft.Office.Interop.Word.Table table in objDoc.Tables)
                {
                    for (int row = 1; row <= table.Rows.Count; row++)
                    {
                        for (int col = 1; col <= table.Columns.Count; col++)
                        {
                            worksheet.Cells[row, col] = objExcelApp.WorksheetFunction.Clean(table.Cell(row, col).Range.Text);
                        }
                    }
                }

                // Save the excel file
                workbook.SaveAs(excelFile, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookDefault);
                objExcelApp.Workbooks.Close();
                objExcelApp.Quit();

                objWordApp.Documents.Close();
                objWordApp.Application.Quit();

                Console.WriteLine("Word document table contents exported to excel file:" + excelFile);
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
