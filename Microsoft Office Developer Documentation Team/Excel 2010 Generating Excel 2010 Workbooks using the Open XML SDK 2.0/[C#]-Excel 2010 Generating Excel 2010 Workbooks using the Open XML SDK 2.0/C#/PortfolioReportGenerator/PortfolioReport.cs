using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace PortfolioReportGenerator
{
    class PortfolioReport
    {
        string path = "c:\\example\\";
        string templateName = "PortfolioReport.xlsx";
        WorkbookPart wbPart = null;
        SpreadsheetDocument document = null;
        Portfolio portfolio = null;

        public PortfolioReport(string client)
        {
            string newFileName = path + client + ".xlsx";
            CopyFile(path + templateName, newFileName);
            document = SpreadsheetDocument.Open(newFileName, true);
            wbPart = document.WorkbookPart;
            portfolio = new Portfolio(client);
        }

        // Create a new Portfolio report
        public void CreateReport()
        {
            string wsName = "Portfolio Summary";

            UpdateValue(wsName, "J2", "Prepared for " + portfolio.Name, 0, true);
            UpdateValue(wsName, "J3", "Account # " + portfolio.AccountNumber.ToString(), 0, true);
            UpdateValue(wsName, "D9", portfolio.BeginningValueQTR.ToString(), 0, false);
            UpdateValue(wsName, "E9", portfolio.BeginningValueYTD.ToString(), 0, false);
            UpdateValue(wsName, "D11", portfolio.ContributionsQTR.ToString(), 0, false);
            UpdateValue(wsName, "E11", portfolio.ContributionsYTD.ToString(), 0, false);
            UpdateValue(wsName, "D12", portfolio.WithdrawalsQTR.ToString(), 0, false);
            UpdateValue(wsName, "E12", portfolio.WithdrawalsYTD.ToString(), 0, false);
            UpdateValue(wsName, "D13", portfolio.DistributionsQTR.ToString(), 0, false);
            UpdateValue(wsName, "E13", portfolio.DistributionsYTD.ToString(), 0, false);
            UpdateValue(wsName, "D14", portfolio.FeesQTR.ToString(), 0, false);
            UpdateValue(wsName, "E14", portfolio.FeesYTD.ToString(), 0, false);
            UpdateValue(wsName, "D15", portfolio.GainLossQTR.ToString(), 0, false);
            UpdateValue(wsName, "E15", portfolio.GainLossYTD.ToString(), 0, false);

            int row = 7;
            wsName = "Portfolio Holdings";

            UpdateValue(wsName, "J2", "Prepared for " + portfolio.Name, 0, true);
            UpdateValue(wsName, "J3", "Account # " + portfolio.AccountNumber.ToString(), 0, true);
            foreach (PortfolioItem item in portfolio.Holdings)
            {
                UpdateValue(wsName, "B" + row.ToString(), item.Description, 3, true);
                UpdateValue(wsName, "D" + row.ToString(), item.CurrentPrice.ToString(), 24, false);
                UpdateValue(wsName, "E" + row.ToString(), item.SharesHeld.ToString(), 27, false);
                UpdateValue(wsName, "F" + row.ToString(), item.MarketValue.ToString(), 24, false);
                UpdateValue(wsName, "G" + row.ToString(), item.Cost.ToString(), 24, false);
                UpdateValue(wsName, "H" + row.ToString(), item.High52Week.ToString(), 28, false);
                UpdateValue(wsName, "I" + row.ToString(), item.Low52Week.ToString(), 28, false);
                UpdateValue(wsName, "J" + row.ToString(), item.Ticker, 11, true);
                row++;
            }

            // Force re-calc when the workbook is opened
            this.RemoveCellValue("Portfolio Summary", "D17");
            this.RemoveCellValue("Portfolio Summary", "E17");

            // All done! Close and save the document.
            document.Close();
        }

        private string CopyFile(string source, string dest)
        {
            string result = "Copied file";
            try
            {
                // Overwrites existing files
                File.Copy(source, dest, true);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        // Given a Worksheet and an address (like "AZ254"), either return a cell reference, or 
        // create the cell reference and return it.
        private Cell InsertCellInWorksheet(Worksheet ws, string addressName)
        {
            SheetData sheetData = ws.GetFirstChild<SheetData>();
            Cell cell = null;

            UInt32 rowNumber = GetRowIndex(addressName);
            Row row = GetRow(sheetData, rowNumber);

            // If the cell you need already exists, return it.
            // If there is not a cell with the specified column name, insert one.  
            Cell refCell = row.Elements<Cell>().
                Where(c => c.CellReference.Value == addressName).FirstOrDefault();
            if (refCell != null)
            {
                cell = refCell;
            }
            else
            {
                cell = CreateCell(row, addressName);
            }
            return cell;
        }

        private Cell CreateCell(Row row, String address)
        {
            Cell cellResult;
            Cell refCell = null;

            // Cells must be in sequential order according to CellReference. Determine where to insert the new cell.
            foreach (Cell cell in row.Elements<Cell>())
            {
                if (string.Compare(cell.CellReference.Value, address, true) > 0)
                {
                    refCell = cell;
                    break;
                }
            }

            cellResult = new Cell();
            cellResult.CellReference = address;

            row.InsertBefore(cellResult, refCell);
            return cellResult;
        }

        private Row GetRow(SheetData wsData, UInt32 rowIndex)
        {
            var row = wsData.Elements<Row>().
            Where(r => r.RowIndex.Value == rowIndex).FirstOrDefault();
            if (row == null)
            {
                row = new Row();
                row.RowIndex = rowIndex;
                wsData.Append(row);
            }
            return row;
        }

        private UInt32 GetRowIndex(string address)
        {
            string rowPart;
            UInt32 l;
            UInt32 result = 0;

            for (int i = 0; i < address.Length; i++)
            {
                if (UInt32.TryParse(address.Substring(i, 1), out l))
                {
                    rowPart = address.Substring(i, address.Length - i);
                    if (UInt32.TryParse(rowPart, out l))
                    {
                        result = l;
                        break;
                    }
                }
            }
            return result;
        }

        public bool UpdateValue(string sheetName, string addressName, string value, UInt32Value styleIndex, bool isString)
        {
            // Assume failure.
            bool updated = false;

            Sheet sheet = wbPart.Workbook.Descendants<Sheet>().Where((s) => s.Name == sheetName).FirstOrDefault();

            if (sheet != null)
            {
                Worksheet ws = ((WorksheetPart)(wbPart.GetPartById(sheet.Id))).Worksheet;
                Cell cell = InsertCellInWorksheet(ws, addressName);

                if (isString)
                {
                    // Either retrieve the index of an existing string,
                    // or insert the string into the shared string table
                    // and get the index of the new item.
                    int stringIndex = InsertSharedStringItem(wbPart, value);

                    cell.CellValue = new CellValue(stringIndex.ToString());
                    cell.DataType = new EnumValue<CellValues>(CellValues.SharedString);
                }
                else
                {
                    cell.CellValue = new CellValue(value);
                    cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                }

                if (styleIndex > 0)
                    cell.StyleIndex = styleIndex;
                
                // Save the worksheet.
                ws.Save();
                updated = true;
            }

            return updated;
        }

        // Given the main workbook part, and a text value, insert the text into the shared
        // string table. Create the table if necessary. If the value already exists, return
        // its index. If it doesn't exist, insert it and return its new index.
        private int InsertSharedStringItem(WorkbookPart wbPart, string value)
        {
            int index = 0;
            bool found = false;
            var stringTablePart = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

            // If the shared string table is missing, something's wrong.
            // Just return the index that you found in the cell.
            // Otherwise, look up the correct text in the table.
            if (stringTablePart == null)
            {
                // Create it.
                stringTablePart = wbPart.AddNewPart<SharedStringTablePart>();
            }

            var stringTable = stringTablePart.SharedStringTable;
            if (stringTable == null)
            {
                stringTable = new SharedStringTable();
            }

            // Iterate through all the items in the SharedStringTable. If the text already exists, return its index.
            foreach (SharedStringItem item in stringTable.Elements<SharedStringItem>())
            {
                if (item.InnerText == value)
                {
                    found = true;
                    break;
                }
                index += 1;
            }

            if (!found)
            {
                stringTable.AppendChild(new SharedStringItem(new Text(value)));
                stringTable.Save();
            }

            return index;
        }

        // Used to force a recalc of cells containing formulas. The
        // CellValue has a cached value of the evaluated formula. This
        // will prevent Excel from recalculating the cell even if 
        // calculation is set to automatic.
        private bool RemoveCellValue(string sheetName, string addressName)
        {
            bool returnValue = false;

            Sheet sheet = wbPart.Workbook.Descendants<Sheet>().
                Where(s => s.Name == sheetName).FirstOrDefault();
            if (sheet != null)
            {
                Worksheet ws = ((WorksheetPart)(wbPart.GetPartById(sheet.Id))).Worksheet;
                Cell cell = InsertCellInWorksheet(ws, addressName);

                // If there is a cell value, remove it to force a recalc
                // on this cell.
                if (cell.CellValue != null)
                {
                    cell.CellValue.Remove();
                }

                // Save the worksheet.
                ws.Save();
                returnValue = true;
            }

            return returnValue;
        }
    }
}
