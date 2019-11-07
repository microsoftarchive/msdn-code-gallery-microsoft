// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

using System;
using Microsoft.Office.Core;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;

namespace DataAnalysisExcel
{
    /// <summary>
    /// Summary description for ExcelHelpers.
    /// </summary>
    internal class ExcelHelpers
    {
        #region "worksheet-related functions"

        /// <summary>
        /// Escapes special characters in a name and truncates it so that it
        /// could be used as a worksheet name in Excel. The name is truncated to 31
        /// characters; the characters ':', '\', '/', '?', '*', '[' and ']' are replaced
        /// with '_'.
        /// </summary>
        /// <param name="name">The original name.</param>
        /// <returns>The escaped name.</returns>
        static internal string CreateValidWorksheetName(string name)
        {
            // Worksheet name cannot be longer than 31 characters.
            System.Text.StringBuilder escapedString;

            if (name.Length <= 31)
            {
                escapedString = new System.Text.StringBuilder(name);
            }
            else
            {
                escapedString = new System.Text.StringBuilder(name, 0, 31, 31);
            }

            for (int i = 0; i < escapedString.Length; i++)
            {
                if (escapedString[i] == ':' ||
                    escapedString[i] == '\\' ||
                    escapedString[i] == '/' ||
                    escapedString[i] == '?' ||
                    escapedString[i] == '*' ||
                    escapedString[i] == '[' ||
                    escapedString[i] == ']')
                {
                    escapedString[i] = '_';
                }
            }

            return escapedString.ToString();
        }

        /// <summary>
        /// Returns the worksheet with the given name.
        /// </summary>
        /// <param name="workbook">The workbook containing the worksheet.</param>
        /// <param name="name">The name of the desired worksheet.</param>
        /// <returns>The worksheet from the workbook with the given name.</returns>
        static internal Excel.Worksheet GetWorksheet(Excel.Workbook workbook, string name)
        {
            return workbook.Worksheets[name] as Excel.Worksheet;
        }

        /// <summary>
        /// Returns the worksheet at the given index.
        /// </summary>
        /// <param name="workbook">The workbook containing the worksheet.</param>
        /// <param name="index">The index of the desired worksheet.</param>
        /// <returns>The worksheet from the workbook with the given name.</returns>
        static internal Excel.Worksheet GetWorksheet(Excel.Workbook workbook, int index)
        {
            return workbook.Worksheets[index] as Excel.Worksheet;
        }

        /// <summary>
        /// Returns the active worksheet from the workbook.
        /// </summary>
        /// <param name="workbook">The workbook containing the worksheet.</param>
        /// <returns>The active worksheet from the given workbook.</returns>
        static internal Excel.Worksheet GetActiveSheet(Excel.Workbook workbook)
        {
            return workbook.ActiveSheet as Excel.Worksheet;
        }

        /// <summary>
        /// Returns the worksheet or chart's name.
        /// </summary>
        /// <param name="item">Worksheet or chart.</param>
        /// <returns>The worksheet or chart's name.</returns>
        static internal string GetName(object item)
        {
            string itemName;

            Excel.Worksheet sheet = item as Excel.Worksheet;
            if (sheet != null)
            {
                itemName = sheet.Name;
            }
            else
            {
                Excel.Chart chart = item as Excel.Chart;

                if (chart != null)
                {
                    itemName = chart.Name;
                }
                else
                {
                    itemName = null;
                }
            }

            return itemName;
        }

      #endregion
        #region "range-related functions"

        /// <summary>
        /// Returns the union of the ranges.
        /// </summary>
        /// <param name="range1">The first range to union.</param>
        /// <param name="range2">The second range to union.</param>
        /// <param name="ranges">An array of ranges to union.</param>
        /// <returns>Returns a range containing the union of all the ranges passed in.</returns>
        static internal Excel.Range Union(Excel.Range range1,
           Excel.Range range2,
           params Excel.Range[] ranges)
        {
            // All the ranges except the first two.
            object[] overflowParameters = new object[28];


            ranges.CopyTo(overflowParameters, 0);

            for (int i = ranges.Length;
               i < overflowParameters.Length;
               i++)
            {
                overflowParameters[i] = Type.Missing;
            }

            return range1.Application.Union(
               range1,
               range2,
               overflowParameters[0],
               overflowParameters[1],
               overflowParameters[2],
               overflowParameters[3],
               overflowParameters[4],
               overflowParameters[5],
               overflowParameters[6],
               overflowParameters[7],
               overflowParameters[8],
               overflowParameters[9],
               overflowParameters[10],
               overflowParameters[11],
               overflowParameters[12],
               overflowParameters[13],
               overflowParameters[14],
               overflowParameters[15],
               overflowParameters[16],
               overflowParameters[17],
               overflowParameters[18],
               overflowParameters[19],
               overflowParameters[20],
               overflowParameters[21],
               overflowParameters[22],
               overflowParameters[23],
               overflowParameters[24],
               overflowParameters[25],
               overflowParameters[26],
               overflowParameters[27]
               );
        }


        /// <summary>
        /// Returns the intersection of the ranges.
        /// </summary>
        /// <param name="range1">The first range to intersect.</param>
        /// <param name="range2">The second range to intersect.</param>
        /// <param name="ranges">An array of ranges to intersect.</param>
        /// <returns>Returns a range containing the intersect of all the ranges passed in.</returns>
        static internal Excel.Range Intersect(Excel.Range range1,
           Excel.Range range2,
         params Excel.Range[] ranges)
        {
            // All the ranges except the first two.
            object[] overflowParameters = new object[28];


            ranges.CopyTo(overflowParameters, 0);

            for (int i = ranges.Length;
               i < overflowParameters.Length;
               i++)
            {
                overflowParameters[i] = Type.Missing;
            }

            return range1.Application.Intersect(
               range1,
               range2,
               overflowParameters[0],
               overflowParameters[1],
               overflowParameters[2],
               overflowParameters[3],
               overflowParameters[4],
               overflowParameters[5],
               overflowParameters[6],
               overflowParameters[7],
               overflowParameters[8],
               overflowParameters[9],
               overflowParameters[10],
               overflowParameters[11],
               overflowParameters[12],
               overflowParameters[13],
               overflowParameters[14],
               overflowParameters[15],
               overflowParameters[16],
               overflowParameters[17],
               overflowParameters[18],
               overflowParameters[19],
               overflowParameters[20],
               overflowParameters[21],
               overflowParameters[22],
               overflowParameters[23],
               overflowParameters[24],
               overflowParameters[25],
               overflowParameters[26],
               overflowParameters[27]
               );
        }

        /// <summary>
        /// Returns the range with the given name from the workbook.
        /// </summary>
        /// <param name="workbook">The workbook containing the named range.</param>
        /// <param name="name">The name of the desired range.</param>
        /// <returns>The range with the given name from the workbook.</returns>
        internal static Excel.Range GetNamedRange(Excel.Workbook workbook, string name)
        {
            Excel.Name nameObject = workbook.Names.Item(
               name,
               Type.Missing,
               Type.Missing);

            return nameObject.RefersToRange;
        }

        /// <summary>
        /// Returns the range with the given name from the given worksheet.
        /// </summary>
        /// <param name="worksheet">The worksheet containing the named range.</param>
        /// <param name="name">The name of the desired range.</param>
        /// <returns>The range with the given name from the given worksheet.</returns>
        internal static Excel.Range GetNamedRange(Excel.Worksheet worksheet, string name)
        {
            return worksheet.get_Range(name, Type.Missing);
        }

        /// <summary>
        /// Returns a range with the column at the specified index of the range.
        /// </summary>
        /// <param name="rowRange">The range containing the desired column.</param>
        /// <param name="column">The index of the desired column from the range.</param>
        /// <returns>The range containing the specified column from the given range.</returns>
        internal static Excel.Range GetColumnFromRange(Excel.Range rowRange, int column)
        {
            return rowRange.Columns[column, Type.Missing] as Excel.Range;
        }

        /// <summary>
        /// Returns a range with the row at the specified index of the range.
        /// </summary>
        /// <param name="columnRange">The range containing the desired row.</param>
        /// <param name="row">The index of the desired row from the range.</param>
        /// <returns>The range containing the specified row from the given range.</returns>
        internal static Excel.Range GetRowFromRange(Excel.Range columnRange, int row)
        {
            return columnRange.Rows[row, Type.Missing] as Excel.Range;
        }

        /// <summary>
        /// Returns a range consisting of the cell at the specified row and column.
        /// </summary>
        /// <param name="range">The range containing the desired cell.</param>
        /// <param name="row">The index of the row containing the desired cell.</param>
        /// <param name="column">The index of the column containing the desired cell.</param>
        /// <returns></returns>
        internal static Excel.Range GetCellFromRange(Excel.Range range, int row, int column)
        {
            return range.Cells[row, column] as Excel.Range;
        }

        /// <summary>
        /// Returns the value of the given range as an object.
        /// </summary>
        /// <param name="range">The range from which the value will be obtained.</param>
        /// <param name="address">The local address of the subrange from which to pull the value.</param>
        /// <returns>Returns the value of the cell in the subrange specified by the address.</returns>
        internal static Object GetValue(Excel.Range range, string address)
        {
            return range.get_Range(address, Type.Missing).Value2;
        }

        /// <summary>
        /// Returns the value of the given range as a double.
        /// </summary>
        /// <param name="range">The range from which the value will be obtained.</param>
        /// <returns>Returns the value of the range as a double.</returns>
        internal static double GetValueAsDouble(Excel.Range range)
        {
            if (range.Value2 is double)
            {
                return (double)range.Value2;
            }

            return double.NaN;
        }

        /// <summary>
        /// Returns the value of the cell at the specified indexes as a double.
        /// </summary>
        /// <param name="sheet">The worksheet containing the desired cell.</param>
        /// <param name="row">The row of the worksheet containing the cell.</param>
        /// <param name="column">The column of the worksheet containing the cell.</param>
        /// <returns>Returns the value of the cell at the given indexes as a double.</returns>
        internal static double GetValueAsDouble(Excel.Worksheet sheet, int row, int column)
        {
            Excel.Range subRange = ((Excel.Range)sheet.Cells[row, column]);

            return GetValueAsDouble(subRange);
        }

        /// <summary>
        /// Returns the value of the cell at the specified indexes as a double.
        /// </summary>
        /// <param name="range">The range containing the desired cell.</param>
        /// <param name="row">The row of the range containing the cell.</param>
        /// <param name="column">The column of the range containing the cell.</param>
        /// <returns>Returns the value of the cell at the specified indexes as a double.</returns>
        internal static double GetValueAsDouble(Excel.Range range, int row, int column)
        {
            Excel.Range subRange = ((Excel.Range)range.Cells[row, column]);

            return GetValueAsDouble(subRange);
        }

        /// <summary>
        /// Returns the value of the given range as a string.
        /// </summary>
        /// <param name="range">The range from which the value will be obtained.</param>
        /// <returns>Returns the value of the given range as a string.</returns>
        internal static string GetValueAsString(Excel.Range range)
        {
            if (!(range.Value2 == null))
            {
                return range.Value2.ToString();
            }

            return null;
        }

        /// <summary>
        /// Returns the value of the cell at the specified indexes as a string.
        /// </summary>
        /// <param name="range">The range containing the desired cell.</param>
        /// <param name="row">The row of the range containing the cell.</param>
        /// <param name="column">The column of the range containing the cell.</param>
        /// <returns>Returns the value of the cell at the specified indexes as a string.</returns>
        internal static string GetValueAsString(Excel.Range range, int row, int column)
        {
            Excel.Range subRange = ((Excel.Range)range.Cells[row, column]);

            return GetValueAsString(subRange);
        }

        /// <summary>
        /// Returns the value of the cell at the specified indexes as a string.
        /// </summary>
        /// <param name="sheet">The worksheet containing the desired cell.</param>
        /// <param name="row">The row of the worksheet containing the cell.</param>
        /// <param name="column">The column of the worksheet containing the cell.</param>
        /// <returns>Returns the value of the cell at the given indexes as a string.</returns>
        internal static string GetValueAsString(Excel.Worksheet sheet, int row, int column)
        {
            Excel.Range subRange = ((Excel.Range)sheet.Cells[row, column]);

            return GetValueAsString(subRange);
        }

      #endregion
        #region "shapes-related functions"
        /// <summary>
        /// Gets the shape with the given name from the active worksheet.
        /// </summary>
        /// <param name="workbook">The workbook containing the shape.</param>
        /// <param name="name">The name of the shape.</param>
        /// <returns>Returns the shape with the given name from the active worksheet.</returns>
        static internal Excel.Shape GetShape(Excel.Workbook workbook, string name)
        {
            return GetShape(GetActiveSheet(workbook), name);
        }

        /// <summary>
        /// Gets the shape at the given index from the active worksheet.
        /// </summary>
        /// <param name="workbook">The workbook containing the shape.</param>
        /// <param name="index">The index of the shape.</param>
        /// <returns>Returns the shape at the given index from the active worksheet.</returns>
        static internal Excel.Shape GetShape(Excel.Workbook workbook, int index)
        {
            return GetShape(GetActiveSheet(workbook), index);
        }

        /// <summary>
        /// Gets the shape with the given name from the given worksheet.
        /// </summary>
        /// <param name="worksheet">The worksheet containing the shape.</param>
        /// <param name="name">The name of the shape.</param>
        /// <returns>Returns the shape with the given name from the given worksheet.</returns>
        static internal Excel.Shape GetShape(Excel.Worksheet worksheet, string name)
        {
            return worksheet.Shapes._Default(name);
        }

        /// <summary>
        /// Gets the shape at the given index from the given worksheet.
        /// </summary>
        /// <param name="worksheet">The worksheet containing the shape.</param>
        /// <param name="index">The index of the shape.</param>
        /// <returns>Returns the shape at the given index from the given worksheet.</returns>
        static internal Excel.Shape GetShape(Excel.Worksheet worksheet, int index)
        {
            return worksheet.Shapes._Default(index);
        }
      #endregion
        #region "date-related functions"
        // Dates in Excel are based on January 1, 1900.
        // There are two reasons for using December 30th, 1899.
        // One reason is that 29/2/1900 is valid in excel (in
        // reality, it is not valid date: 1900 is not a leap year);
        // the other is that 0 in Excel corresponds to January 0.
        private readonly static DateTime timeOrigin =
           new DateTime(1899, 12, 30, 0, 0, 0, 0);

        /// <summary>
        /// Returns the date as the decimal equivalent for Excel.
        /// </summary>
        /// <param name="dateValue">The date to convert.</param>
        /// <returns>A decimal representation of the date for Excel.</returns>
        internal static double GetSerialDate(DateTime dateValue)
        {
            TimeSpan since1900 = dateValue - timeOrigin;

            return since1900.Days;
        }

        /// <summary>
        /// Returns a DateTime from the decimal representation of a date in Excel.
        /// </summary>
        /// <param name="serial">The decimal date value from Excel.</param>
        /// <returns>A DateTime equivalent of the decimal representation of a date in Excel.</returns>
        internal static DateTime GetDateTime(double serial)
        {
            TimeSpan since1900 = new TimeSpan((int)serial, 0, 0, 0);

            return timeOrigin.Add(since1900);
        }

      #endregion


        public static stdole.IPictureDisp Convert(System.Drawing.Image image)
        {
            return ImageToPictureConverter.Convert(image);
        }

        /// <summary>
        /// Class for exposing protected method GetIPictureDispFromPicture
        /// of AxHost.
        /// </summary>
        sealed private class ImageToPictureConverter : System.Windows.Forms.AxHost
        {
            private ImageToPictureConverter()
                : base(null)
            {
            }

            public static stdole.IPictureDisp Convert(System.Drawing.Image image)
            {
                return (stdole.IPictureDisp)System.Windows.Forms.AxHost.GetIPictureDispFromPicture(image);
            }
        }
    }
}
