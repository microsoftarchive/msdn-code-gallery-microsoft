/****************************** Module Header ******************************\
* Module Name: ExportAndImportExcel.cs
* Project:     CSOpenXmlExportImportExcel
* Copyright(c) Microsoft Corporation.
* 
* The Main form.
* Users can use this form to export Data in GridView control to excel 
* and Import data from Excel document into GridView control.
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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text;

namespace CSOpenXmlExportImportExcel
{
    public partial class ExportAndImportExcel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Disable Export button
            this.btnExport.Enabled = false;
        }

        #region Import operation

        /// <summary>
        ///  Import Excel Data into GridView Control 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImport_Click(object sender, EventArgs e)
        {
            // The condition that FileUpload control contains a file 
            if (FileUpload1.HasFile)
            {
                // Get selected file name
                string filename = FileUpload1.PostedFile.FileName;

                // Get the extension of the selected file
                string fileExten = Path.GetExtension(filename);

                // The condition that the extension is not xlsx
                if (!fileExten.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                {
                    Response.Write("<script language=\"javascript\">alert('The extension of selected file is incorrect ,please select again');</script>");
                    return;
                }

                // Read Data in excel file
                try
                {
                    DataTable dt = ReadExcelFile(filename);

                    if (dt.Rows.Count == 0)
                    {
                        Response.Write("<script language=\"javascript\">alert('The first sheet is empty.');</script>");
                        return;
                    }

                    // Bind Datasource
                    this.gridViewTest.DataSource = dt;
                    this.gridViewTest.DataBind();

                    // Enable Export button
                    this.btnExport.Enabled = true;
                }
                catch (IOException ex)
                {
                    string exceptionmessage = ex.Message;
                    Response.Write("<script language=\"javascript\">alert(\""+exceptionmessage+"\");</script>");
                }
            }
            else
            {
                Response.Write("<script language=\"javascript\">alert('You did not specify a file to import');</script>");
            }
        }

        /// <summary>
        ///  Read Data from selected excel file on client
        /// </summary>
        /// <param name="filename">File Path</param>
        /// <returns></returns>
        private DataTable ReadExcelFile(string filename)
        {
            // Initializate an instance of DataTable
            DataTable dt = new DataTable();

            try
            {
                // Use SpreadSheetDocument class of Open XML SDK to open excel file
                using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filename, false))
                {
                    // Get Workbook Part of Spread Sheet Document
                    WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;

                    // Get all sheets in spread sheet document 
                    IEnumerable<Sheet> sheetcollection = spreadsheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();

                    // Get relationship Id
                    string relationshipId = sheetcollection.First().Id.Value;

                    // Get sheet1 Part of Spread Sheet Document
                    WorksheetPart worksheetPart = (WorksheetPart)spreadsheetDocument.WorkbookPart.GetPartById(relationshipId);

                    // Get Data in Excel file
                    SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();
                    IEnumerable<Row> rowcollection = sheetData.Descendants<Row>();

                    if (rowcollection.Count() == 0)
                    {
                        return dt;
                    }

                    // Add columns
                    foreach (Cell cell in rowcollection.ElementAt(0))
                    {
                        dt.Columns.Add(GetValueOfCell(spreadsheetDocument, cell));
                    }

                    // Add rows into DataTable
                    foreach (Row row in rowcollection)
                    {
                        DataRow temprow = dt.NewRow();
                        for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                        {
                            temprow[i] = GetValueOfCell(spreadsheetDocument, row.Descendants<Cell>().ElementAt(i));
                        }

                        // Add the row to DataTable
                        // the rows include header row
                        dt.Rows.Add(temprow);
                    }
                }

                // Here remove header row
                dt.Rows.RemoveAt(0);
                return dt;
            }
            catch(IOException ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        ///  Get Value in Cell 
        /// </summary>
        /// <param name="spreadsheetdocument">SpreadSheet Document</param>
        /// <param name="cell">Cell in SpreadSheet Document</param>
        /// <returns>The value in cell</returns>
        private static string GetValueOfCell(SpreadsheetDocument spreadsheetdocument, Cell cell)
        {
            // Get value in Cell
            SharedStringTablePart sharedString =spreadsheetdocument.WorkbookPart.SharedStringTablePart;
            if (cell.CellValue == null)
            {
                return string.Empty;
            }

            string cellValue = cell.CellValue.InnerText;
            
            // The condition that the Cell DataType is SharedString
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return sharedString.SharedStringTable.ChildElements[int.Parse(cellValue)].InnerText;
            }
            else
            {
                return cellValue;
            }
        }

        #endregion Import operation

        #region Export operation

        /// <summary>
        ///  Export Data in GridView control to Excel file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnExport_Click(object sender, EventArgs e)
        {
            // Initialize an instance of DataTable
            DataTable dt = CreateDataTable(this.gridViewTest);

            // Save the exported file 
            string appPath = Request.PhysicalApplicationPath;
            string filename = Guid.NewGuid().ToString() + ".xlsx";
            string filePath = appPath+ filename;

            new CreateSpreadSheetProvider().ExportToExcel(dt, filePath);

            string savefilepath = "Export Excel file successfully, the exported excel file is placed in: " + filePath;
            Response.Write("<script language='javascript'>alert('"+savefilepath.Replace("\\","\\\\")+"');</script>");
        }

        /// <summary>
        ///  Create DataTable from GridView Control
        /// </summary>
        /// <param name="girdViewtest">GridView Control</param>
        /// <returns>An instance of DataTable Object</returns>
        private DataTable CreateDataTable(GridView girdViewtest)
        {
            DataTable dt = new DataTable();

            // Get columns from GridView
            // Add value of columns to DataTable 
            for (int i = 0; i < gridViewTest.HeaderRow.Cells.Count; i++)
            {
                dt.Columns.Add(gridViewTest.HeaderRow.Cells[i].Text);
            }

            // Get rows from GridView
            foreach (GridViewRow row in gridViewTest.Rows)
            {
                DataRow datarow = dt.NewRow();
                for (int i = 0; i < row.Cells.Count; i++)
                {
                    datarow[i] = row.Cells[i].Text.Replace("&nbsp;", " ");
                }

                // Add rows to DataTable
                dt.Rows.Add(datarow);
            }

            return dt;
        }

        #endregion Export operation
    }
}