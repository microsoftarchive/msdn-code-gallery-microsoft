//****************************** Module Header ******************************\
//Module Name:    Program.cs
//Project:        ExportWordTableToExcel
//Copyright (c) Microsoft Corporation

//The project illustrates how to convert Word table into Excel using OpenXML

//This source is subject to the Microsoft Public License.
//See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
//All other rights reserved.

//*****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using System.Data;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using System.IO;

namespace ExportWordTableToExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string wordFile = appPath + "\\TestDoc.docx";

            DataTable dataTable = ReadWordTable(wordFile);

            if (dataTable != null)
            {
                string sFile = appPath + "\\ExportExcel.xlsx";

                ExportDataTableToExcel(dataTable, sFile);

                Console.WriteLine("Contents of word table exported to excel spreadsheet");

                Console.ReadKey();
            }
        }

        /// <summary>
        /// This method reads the contents of table using openxml sdk
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static DataTable ReadWordTable(string fileName)
        {
            DataTable table;

            try
            {
                using (var document = WordprocessingDocument.Open(fileName, false))
                {
                    var docPart = document.MainDocumentPart;
                    var doc = docPart.Document;

                    DocumentFormat.OpenXml.Wordprocessing.Table myTable = doc.Body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Table>().First();

                    List<List<string>> totalRows = new List<List<string>>();
                    int maxCol = 0;

                    foreach (TableRow row in myTable.Elements<TableRow>())
                    {
                        List<string> tempRowValues = new List<string>();
                        foreach (TableCell cell in row.Elements<TableCell>())
                        {
                            tempRowValues.Add(cell.InnerText);
                        }

                        maxCol = ProcessList(tempRowValues, totalRows, maxCol);
                    }

                    table = ConvertListListStringToDataTable(totalRows, maxCol);
                }

                return table;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }

        /// <summary>
        /// Add each row to the totalRows.
        /// </summary>
        /// <param name="tempRows"></param>
        /// <param name="totalRows"></param>
        /// <param name="MaxCol">the max column number in rows of the totalRows</param>
        /// <returns></returns>
        private static int ProcessList(List<string> tempRows, List<List<string>> totalRows, int MaxCol)
        {
            if (tempRows.Count > MaxCol)
            {
                MaxCol = tempRows.Count;
            }

            totalRows.Add(tempRows);
            return MaxCol;
        }

        /// <summary>
        /// This method converts list data to a data table
        /// </summary>
        /// <param name="totalRows"></param>
        /// <param name="maxCol"></param>
        /// <returns>returns datatable object</returns>
        private static DataTable ConvertListListStringToDataTable(List<List<string>> totalRows, int maxCol)
        {
            DataTable table = new DataTable();
            for (int i = 0; i < maxCol; i++)
            {
                table.Columns.Add();
            }
            foreach (List<string> row in totalRows)
            {
                while (row.Count < maxCol)
                {
                    row.Add("");
                }
                table.Rows.Add(row.ToArray());
            }
            return table;
        }

        /// <summary>
        /// This method exports datatable to a excel file
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="exportFile">Excel file name</param>
        private static void ExportDataTableToExcel(DataTable table, string exportFile)
        {
            try
            {
                // Create a spreadsheet document by supplying the filepath.
                SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.
                    Create(exportFile, SpreadsheetDocumentType.Workbook);

                // Add a WorkbookPart to the document.
                WorkbookPart workbookpart = spreadsheetDocument.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                // Add a WorksheetPart to the WorkbookPart.
                WorksheetPart worksheetPart = workbookpart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                // Add Sheets to the Workbook.
                Sheets sheets = spreadsheetDocument.WorkbookPart.Workbook.
                    AppendChild<Sheets>(new Sheets());

                // Append a new worksheet and associate it with the workbook.
                Sheet sheet = new Sheet()
                {
                    Id = spreadsheetDocument.WorkbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "mySheet"
                };
                sheets.Append(sheet);

                SheetData data = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                //add column names to the first row  
                Row header = new Row();
                header.RowIndex = (UInt32)1;

                foreach (DataColumn column in table.Columns)
                {
                    Cell headerCell = createTextCell(
                        table.Columns.IndexOf(column) + 1,
                        1,
                        column.ColumnName);

                    header.AppendChild(headerCell);
                }
                data.AppendChild(header);

                //loop through each data row  
                DataRow contentRow;
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    contentRow = table.Rows[i];
                    data.AppendChild(createContentRow(contentRow, i + 2));
                }

                workbookpart.Workbook.Save();

                // Close the document.
                spreadsheetDocument.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        /// <summary>
        /// This method creates text cell
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="rowIndex"></param>
        /// <param name="cellValue"></param>
        /// <returns></returns>
        private static Cell createTextCell( int columnIndex, int rowIndex, object cellValue)
        {
            Cell cell = new Cell();

            cell.DataType = CellValues.InlineString;
            cell.CellReference = getColumnName(columnIndex) + rowIndex;

            InlineString inlineString = new InlineString();
            DocumentFormat.OpenXml.Spreadsheet.Text t = new DocumentFormat.OpenXml.Spreadsheet.Text();

            t.Text = cellValue.ToString();
            inlineString.AppendChild(t);
            cell.AppendChild(inlineString);

            return cell;
        }

        /// <summary>
        /// This method creates content row
        /// </summary>
        /// <param name="dataRow"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        private static Row createContentRow( DataRow dataRow, int rowIndex)
        {
            Row row = new Row
            {
                RowIndex = (UInt32)rowIndex
            };

            for (int i = 0; i < dataRow.Table.Columns.Count; i++)
            {
                Cell dataCell = createTextCell(i + 1, rowIndex, dataRow[i]);
                row.AppendChild(dataCell);
            }
            return row;
        }

        /// <summary>
        /// Formates or gets column name
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <returns></returns>
        private static string getColumnName(int columnIndex)
        {
            int dividend = columnIndex;
            string columnName = String.Empty;
            int modifier;

            while (dividend > 0)
            {
                modifier = (dividend - 1) % 26;
                columnName =
                    Convert.ToChar(65 + modifier).ToString() + columnName;
                dividend = (int)((dividend - modifier) / 26);
            }

            return columnName;
        }
    }
}
