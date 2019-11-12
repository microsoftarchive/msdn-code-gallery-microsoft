/****************************** Module Header ******************************\
* Module Name: CreateSpreadSheetProvider.cs
* Project:     CSOpenXmlExportImportExcel
* Copyright(c) Microsoft Corporation.
* 
* The class is used to create spreadsheet document. 
* We can create spreadsheet document structure and content by using strongly-typed 
* classes that correspond to SpreadsheetMl elements in Open XML SDK 2.0.
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
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace CSOpenXmlExportImportExcel
{
    public class CreateSpreadSheetProvider
    {
        // The collection of Excel columns
        private static string[] excelColumns ={"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        private List<string> cellHeaders = null;

        public CreateSpreadSheetProvider()
        {
            this.InitilizeCellHeaders();
        }

        /// <summary>
        ///  Add the item into List
        /// </summary>
        private void InitilizeCellHeaders()
        {
            cellHeaders = new List<string>();
            foreach (string sItem in excelColumns)
                cellHeaders.Add(sItem);
        }

        /// <summary>
        /// Returns the column caption for the given row & column index.
        /// </summary>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="columnIndex">Index of the column.</param>
        /// <returns></returns>
        private string Get(int columnIndex,int rowIndex)
        {
            return this.cellHeaders.ElementAt(columnIndex) + (rowIndex + 1).ToString();
        }

        /// <summary>
        ///  Generate an excel file with data in GridView control
        /// </summary>
        /// <param name="datatable">DataTable object</param>
        /// <param name="filepath">The Path of exported excel file</param>
        public void ExportToExcel(DataTable datatable, string filepath)
        {
            // Initialize an instance of  SpreadSheet Document 
            using(SpreadsheetDocument spreadsheetDocument =SpreadsheetDocument.Create(filepath,SpreadsheetDocumentType.Workbook))
            {
                CreateExcelFile(spreadsheetDocument,datatable);
            }
        }

        /// <summary>
        ///  Create SpreadSheet Document and Fill datas
        /// </summary>
        /// <param name="spreadsheetdoc">SpreadSheet Document</param>
        /// <param name="table">DataTable Object</param>
        private void CreateExcelFile(SpreadsheetDocument spreadsheetdoc, DataTable table)
        {
            // Initialize an instance of WorkbookPart
            WorkbookPart workBookPart = spreadsheetdoc.AddWorkbookPart();

            // Create WorkBook 
            CreateWorkBookPart(workBookPart);

            // Add WorkSheetPart into WorkBook
            WorksheetPart worksheetPart1 = workBookPart.AddNewPart<WorksheetPart>("rId1");
            CreateWorkSheetPart(worksheetPart1, table);

            // Add SharedStringTable Part into WorkBook
            SharedStringTablePart sharedStringTablePart = workBookPart.AddNewPart<SharedStringTablePart>("rId2");
            CreateSharedStringTablePart(sharedStringTablePart, table);

            // Add WorkbookStyles Part into Workbook
            WorkbookStylesPart workbookStylesPart = workBookPart.AddNewPart<WorkbookStylesPart>("rId3");
            CreateWorkBookStylesPart(workbookStylesPart);

            // Save workbook
            workBookPart.Workbook.Save();
        }

        /// <summary>
        /// Create an Workbook instance and add its children
        /// </summary>
        /// <param name="workbookPart">WorkbookPart Object</param>
        private void CreateWorkBookPart(WorkbookPart workbookPart)
        {
            Workbook workbook = new Workbook();
            Sheets sheets = new Sheets();

            // Initilize an instance of Sheet Object
            Sheet sheet1 = new Sheet()
            {
                Name = "Sheet1",
                SheetId = Convert.ToUInt32(1),
                Id = "rId1"
            };

            // Add the sheet into sheets collection
            sheets.Append(sheet1);

            CalculationProperties calculationProperties1 = new CalculationProperties()
            {
                CalculationId = (UInt32Value)111222U
            };

            // Add elements into workbook
            workbook.Append(sheets);
            workbook.Append(calculationProperties1);
            workbookPart.Workbook = workbook;
        }

        /// <summary>
        ///  Generates content of worksheetPart
        /// </summary>
        /// <param name="worksheetPart">WorksheetPart Object</param>
        /// <param name="table">DataTable Object</param>
        private void CreateWorkSheetPart(WorksheetPart worksheetPart, DataTable table)
        {
            // Initialize worksheet and set the properties
            Worksheet worksheet1 = new Worksheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            worksheet1.AddNamespaceDeclaration("r", "http://schemas.openxmlformats.org/officeDocument/2006/relationships");
            worksheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            worksheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");
            
            SheetViews sheetViews1 = new SheetViews();

            // Initialize an instance of the sheetview class
            SheetView sheetView1 = new SheetView()
            {
                WorkbookViewId = (UInt32Value)0U
            };

            Selection selection = new Selection() { ActiveCell = "A1" };
            sheetView1.Append(selection);

            sheetViews1.Append(sheetView1);
            SheetFormatProperties sheetFormatProperties1 = new SheetFormatProperties()
            {
                DefaultRowHeight = 15D,
                DyDescent = 0.25D
            };

            SheetData sheetData1 = new SheetData();
            UInt32Value rowIndex = 1U;
            PageMargins pageMargins1 = new PageMargins() 
            {
                Left = 0.7D,
                Right = 0.7D, 
                Top = 0.75D,
                Bottom = 0.75D, 
                Header = 0.3D, 
                Footer = 0.3D 
            };
     
            Row row1 = new Row() 
            { 
                RowIndex = rowIndex++, 
                Spans = new ListValue<StringValue>() { InnerText = "1:3" },
                DyDescent = 0.25D 
            };

            // Add columns in DataTable to columns collection of SpreadSheet Document 
            for (int columnindex = 0; columnindex < table.Columns.Count; columnindex++)
            {
                Cell cell = new Cell() 
                { 
                    CellReference = new CreateSpreadSheetProvider().Get(columnindex, (Convert.ToInt32((UInt32)rowIndex) - 2)), DataType = CellValues.String 
                };

                // Get Value of DataTable and append the value to cell of spreadsheet document
                CellValue cellValue = new CellValue();
                cellValue.Text = table.Columns[columnindex].ColumnName.ToString();
                cell.Append(cellValue);

                row1.Append(cell);
            }

            // Add row to sheet
            sheetData1.Append(row1);

            // Add rows in DataTable to rows collection of SpreadSheet Document 
            for (int rIndex = 0; rIndex < table.Rows.Count; rIndex++)
            {
                Row row = new Row()
                {
                    RowIndex = rowIndex++,
                    Spans = new ListValue<StringValue>() { InnerText = "1:3" },
                    DyDescent = 0.25D
                };

                for (int cIndex = 0; cIndex < table.Columns.Count; cIndex++)
                {
                    Cell cell = new Cell() 
                    { 
                        CellReference = new CreateSpreadSheetProvider().Get(cIndex, (Convert.ToInt32((UInt32)rowIndex) - 2)), DataType = CellValues.String
                    };

                    CellValue cellValue = new CellValue();
                    cellValue.Text = table.Rows[rIndex][cIndex].ToString();
                    cell.Append(cellValue);
                    row.Append(cell);
                }

                // Add row to Sheet Data
                sheetData1.Append(row);
            }

            // Add elements to worksheet
            worksheet1.Append(sheetViews1);
            worksheet1.Append(sheetFormatProperties1);
            worksheet1.Append(sheetData1);
            worksheet1.Append(pageMargins1);

            worksheetPart.Worksheet = worksheet1;
        }

        /// <summary>
        /// Generates content of sharedStringTablePart
        /// </summary>
        /// <param name="sharedStringTablePart">SharedStringTablePart Object</param>
        /// <param name="table">DataTable Object</param>
        private void CreateSharedStringTablePart(SharedStringTablePart sharedStringTablePart, DataTable table)
        {
            UInt32Value stringCount = Convert.ToUInt32(table.Rows.Count) + Convert.ToUInt32(table.Columns.Count);

            // Initialize an instance of SharedString Table
            SharedStringTable sharedStringTable = new SharedStringTable()
            {
                Count = stringCount,
                UniqueCount = stringCount
            };

            // Add columns of DataTable to sharedString iteam
            for (int columnIndex = 0; columnIndex < table.Columns.Count; columnIndex++)
            {
                SharedStringItem sharedStringItem = new SharedStringItem();
                Text text = new Text();
                text.Text = table.Columns[columnIndex].ColumnName;
                sharedStringItem.Append(text);

                // Add sharedstring item to sharedstring Table
                sharedStringTable.Append(sharedStringItem);
            }

            // Add rows of DataTable to sharedString iteam
            for (int rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
            {
                SharedStringItem sharedStringItem = new SharedStringItem();
                Text text = new Text();
                text.Text = table.Rows[rowIndex][0].ToString();
                sharedStringItem.Append(text);
                sharedStringTable.Append(sharedStringItem);
            }

            sharedStringTablePart.SharedStringTable = sharedStringTable;
        }
      
        /// <summary>
        /// Generates content of workbookStylesPart
        /// </summary>
        /// <param name="workbookStylesPart">WorkbookStylesPart Object</param>
        private void CreateWorkBookStylesPart(WorkbookStylesPart workbookStylesPart)
        {
            // Define Style of Sheet in workbook
            Stylesheet stylesheet1 = new Stylesheet() { MCAttributes = new MarkupCompatibilityAttributes() { Ignorable = "x14ac" } };
            stylesheet1.AddNamespaceDeclaration("mc", "http://schemas.openxmlformats.org/markup-compatibility/2006");
            stylesheet1.AddNamespaceDeclaration("x14ac", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/ac");

            // Initialize  an instance of fonts
            Fonts fonts = new Fonts() { Count = (UInt32Value)2U, KnownFonts = true };

            // Initialize  an instance of font,fontsize,color
            Font font = new Font();     
            FontSize fontSize = new FontSize() { Val = 14D };
            Color color = new Color() { Theme = (UInt32Value)1U };
            FontName fontName = new FontName() { Val = "Calibri" };
            FontFamilyNumbering fontFamilyNumbering = new FontFamilyNumbering() { Val = 2 };
            FontScheme fontScheme = new FontScheme() { Val = FontSchemeValues.Minor };

            // Add elements to font
            font.Append(fontSize);
            font.Append(color);
            font.Append(fontName);
            font.Append(fontFamilyNumbering);
            font.Append(fontScheme);

            fonts.Append(font);

            // Define the StylesheetExtensionList Class. When the object is serialized out as xml, its qualified name is x:extLst
            StylesheetExtensionList stylesheetExtensionList1 = new StylesheetExtensionList();

            // Define the StylesheetExtension Class
            StylesheetExtension stylesheetExtension1 = new StylesheetExtension() { Uri = "{EB79DEF2-80B8-43e5-95BD-54CBDDF9020C}" };
            stylesheetExtension1.AddNamespaceDeclaration("x14", "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main");
            DocumentFormat.OpenXml.Office2010.Excel.SlicerStyles slicerStyles1 = new DocumentFormat.OpenXml.Office2010.Excel.SlicerStyles() { DefaultSlicerStyle = "SlicerStyleLight1" };
            
            stylesheetExtension1.Append(slicerStyles1);
            stylesheetExtensionList1.Append(stylesheetExtension1);

            // Add elements to stylesheet
            stylesheet1.Append(fonts);
            stylesheet1.Append(stylesheetExtensionList1);
         
            // Set the style of workbook
            workbookStylesPart.Stylesheet = stylesheet1;
        }
    }
}