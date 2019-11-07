﻿/****************************** Module Header ******************************\ 
* Module Name:   CompareHelper.cs
* Project:       CSExcelCompareCells
* Copyright (c)  Microsoft Corporation. 
*  
* The Class is used to compare cells in excel file
* 
*  
* This source is subject to the Microsoft Public License. 
* See http://www.microsoft.com/en-us/openness/licenses.aspx. 
* All other rights reserved. 
*  
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,  
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED  
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
\***************************************************************************/

using System;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace CSExcelCompareCells
{
   public class CompareHelper
    {
       // Define Variables
       Excel.Application excelApp;
       Excel.Workbook excelWorkbook;
       Excel.Worksheet excelWorkSheet1;
       Excel.Worksheet excelWorkSheet2;
       int lastLine;

       /// <summary>
       /// Compare Cells in different columns in the same sheet of an excel file
       /// </summary>
       /// <param name="sourceCol">Source Column</param>
       /// <param name="targetCol">Taget Column</param>
       /// <param name="excelFile">The Path of Excel File</param>
       public void CompareColumns(string sourceCol, string targetCol, string excelFile)
       {
           try
           {
               // Create an instance of Microsoft Excel and make it invisible
               excelApp = new Excel.Application();
               excelApp.Visible = false;
              
               // open a Workbook and get the active Worksheet
               excelWorkbook = excelApp.Workbooks.Open(excelFile);
               excelWorkSheet1 = excelWorkbook.ActiveSheet;

               // maximum Row
               lastLine = excelWorkSheet1.UsedRange.Rows.Count;
               for (int row = 1; row <= lastLine; row++)
               {
                   // Compare cell value
                   if (excelWorkSheet1.Range[sourceCol + row.ToString()].Value != excelWorkSheet1.Range[targetCol + row.ToString()].Value)
                   {
                       excelWorkSheet1.Range[sourceCol + row.ToString()].Interior.Color = 255;
                       excelWorkSheet1.Range[targetCol + row.ToString()].Interior.Color = 5296274;
                   }
               }

               // Save WorkBook and close
               excelWorkbook.Save();
               excelWorkbook.Close();

               // Quit Excel Application
               excelApp.Quit();
           }
           catch
           {
               throw ;
           }
       }

       /// <summary>
       /// Compare Cells in different sheets of an excel file
       /// </summary>
       /// <param name="sourceSheetNum">Source Sheet Number</param>
       /// <param name="targetSheetNum">Target Sheet Number</param>
       /// <param name="excelFile">Path of Excel File</param>
       public void CompareSheets(int sourceSheetNum,int targetSheetNum,string excelFile)
       {
           try
           {
               // Create an instance of Microsoft Excel and make it invisible
               excelApp = new Excel.Application();
               excelApp.Visible = false;
               excelWorkbook = excelApp.Workbooks.Open(excelFile);
               excelWorkSheet1 = excelWorkbook.Sheets[sourceSheetNum];
               excelWorkSheet2 = excelWorkbook.Sheets[targetSheetNum];
               lastLine = Math.Max(
                   excelWorkSheet1.UsedRange.Rows.Count,
                   excelWorkSheet2.UsedRange.Rows.Count);
               for (int row = 1; row <= lastLine; row++)
               {
                   // maximum column 
                   int lastCol = Math.Max(
                       excelWorkSheet1.UsedRange.Columns.Count,
                       excelWorkSheet2.UsedRange.Columns.Count);
                   for (int column = 1; column <= lastCol; column++)
                   {
                       if (excelWorkSheet1.Cells[row, column].Value != excelWorkSheet2.Cells[row, column].Value)
                       {
                           ((Excel.Range)excelWorkSheet1.Cells[row, column]).Interior.Color = 255;
                           ((Excel.Range)excelWorkSheet2.Cells[row, column]).Interior.Color = 5296274;
                       }
                   }
               }

               // Save WorkBook and close
               excelWorkbook.Save();
               excelWorkbook.Close();

               // Quit Excel Application
               excelApp.Quit();
           }
           catch
           {
               throw;
           }
       }
    }
}
