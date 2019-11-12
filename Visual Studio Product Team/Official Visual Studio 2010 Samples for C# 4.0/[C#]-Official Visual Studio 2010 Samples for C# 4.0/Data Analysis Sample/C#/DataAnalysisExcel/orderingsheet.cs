// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

using System;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Globalization;

namespace DataAnalysisExcel
{
    internal class OrderingSheet
    {
        object defaultParameter = System.Type.Missing;

        internal enum StatHeadings
        {
            DailySales = 0,
            Required,
            CurrentInventory,
            ProjectInventory,
            OrderQuanity
        }

        private string[] headers = {
            Properties.Resources.MaxDailySalesHeader, 
            Properties.Resources.ProjectedSalesHeader, 
            Properties.Resources.CurrentInventoryHeader,
            Properties.Resources.ProjectedInventoryHeader,
            Properties.Resources.OrderQuantityHeader
        };

        DateTime deliveryDate;

        DateTime nextScheduledDeliveryDate;

        DateTime orderDate;

        Excel.Worksheet worksheet;

        const string orderDateAddress = "$B$4";
        const string pivotTableAddress = "$B$10";

        internal OrderingSheet(bool isUnscheduled)
        {
            if (!Globals.DataSet.IsLastDayComplete())
            {
                throw new ApplicationException(Globals.ThisWorkbook.IncompleteDataMessage);
            }

            this.orderDate = Globals.DataSet.MaxDate;

            string worksheetName;

            if (isUnscheduled)
            {
                worksheetName = ExcelHelpers.CreateValidWorksheetName(
                    String.Format(
                        CultureInfo.CurrentUICulture, 
                        Properties.Resources.UnscheduledOrderSheetName,
                        this.orderDate.ToShortDateString()));
            }
            else
            {
                worksheetName = ExcelHelpers.CreateValidWorksheetName(
                    String.Format(
                        CultureInfo.CurrentUICulture,
                        Properties.Resources.WeeklyOrderSheetName,
                        this.orderDate.ToShortDateString()));
            }
            Excel.Worksheet worksheet = null;

            // Worksheet creation will throw an exception if the name already exists.
            try
            {
                worksheet = Globals.ThisWorkbook.CreateWorksheet(worksheetName);
            }
            catch (Exception ex)
            {
                string message;

                if (isUnscheduled)
                {
                    message = String.Format(
                        CultureInfo.CurrentUICulture,
                        Properties.Resources.UnscheduledOrderSheetCreationError,
                        worksheetName);
                }
                else
                {
                    message = String.Format(
                        CultureInfo.CurrentUICulture,
                        Properties.Resources.WeeklyOrderSheetCreationError,
                        worksheetName);
                }

                throw new ApplicationException(message, ex);
            }

            this.worksheet = worksheet;

            CreateOrder(isUnscheduled);
        }

        internal OrderingSheet(Excel.Worksheet worksheet, DateTime orderDate, bool isUnscheduled)
        {
            this.orderDate = orderDate;
            this.worksheet = worksheet;

            CreateOrder(isUnscheduled);
        }

        private DateTime ComputeUnscheduledDeliveryDate()
        {
            return this.orderDate.AddDays(1).Date;
        }

        private DateTime ComputeWeeklyDeliveryDate()
        {
            return Globals.DataSet.NextWeeklyDeliveryDate;
        }

        private void CreateOrder(bool isUnscheduled)
        {
            if (isUnscheduled)
            {
                this.deliveryDate = ComputeUnscheduledDeliveryDate();
                this.nextScheduledDeliveryDate = ComputeWeeklyDeliveryDate();
            }
            else
            {
                this.deliveryDate = ComputeWeeklyDeliveryDate();
                this.nextScheduledDeliveryDate = this.deliveryDate.AddDays(7);
            }

            // This creates a PivotTable with information regarding the 
            // amounts of ice cream sold.
            this.PopulateDateInformation(this.orderDate);

            Excel.PivotTable pivotTable = this.CreatePivotTable();

            this.AddCalculations(pivotTable);
        }

        private void AddCalculations(Excel.PivotTable pivotTable)
        {
            // Gets the ranges that make up the PivotTable.
            Excel.Range tableRange = pivotTable.TableRange1;
            Excel.Range dataRange = pivotTable.DataBodyRange;

            // Gets each of the columns that needs to be added after the
            // PivotTable.
            System.Array values = Enum.GetValues(typeof(StatHeadings));

            // Determines upper left cell of the PivotTable.
            Excel.Range tableStartCell = ExcelHelpers.GetCellFromRange(tableRange, 1, 1);

            // Gets the first available cell in the appropriate row at the
            // end of the PivotTable.
            Excel.Range nextHeader = tableStartCell.get_End(Excel.XlDirection.xlDown).get_End(Excel.XlDirection.xlToRight).get_End(Excel.XlDirection.xlUp).Next;

            // Determines the boundary cells that make up the calculated fields
            // for the current column.
            Excel.Range colStart = ExcelHelpers.GetCellFromRange(nextHeader, 2, 1);

            Excel.Range colEnd = colStart.get_Offset(dataRange.Rows.Count - 1, 0);

            // For each of the columns that need to be added,
            // populates its stats and headings.

            foreach (int i in values)
            {
                nextHeader.Value2 = this.headers[i];
                this.PopulateStatColumn(i, colStart, colEnd);
                nextHeader = nextHeader.Next;
                colStart = colStart.Next;
                colEnd = colEnd.Next;
            }
        }

        private void PopulateStatColumn(int column, Excel.Range start, Excel.Range end)
        {
            try
            {
                // Determines the range that needs to get filled with data.
                Excel.Range twoLines = start.get_Resize(2, 1);

                twoLines.Merge(System.Type.Missing);

                Excel.Range fillRange = this.worksheet.get_Range(start, end);
                end.Select();

                switch (column)
                {
                    case (int)StatHeadings.DailySales:
                        // Fills in the daily sales column.
                        // Gets the addresses of the cells containing the
                        // standard deviation and average.
                        Excel.Range average = start.Previous;
                        string averageAddress = average.get_Address(false, false, Excel.XlReferenceStyle.xlA1, defaultParameter, defaultParameter);
                        Excel.Range standardDev = average.get_Offset(1, 0);
                        string standardDevAddress = standardDev.get_Address(false, false, Excel.XlReferenceStyle.xlA1, defaultParameter, defaultParameter);

                        // Sets the formulas for the column.
                        start.Formula = "=" + averageAddress + "+ (2*" + standardDevAddress + ")";

                        // Format "0.00" - two decimal places.
                        start.NumberFormat = "0.00";
                        twoLines.AutoFill(fillRange, Excel.XlAutoFillType.xlFillDefault);
                        break;

                    case (int)StatHeadings.Required:
                        // Fills in the required column.
                        // Determines the address for the cell containing
                        // the expected sales.
                        Excel.Range expectedSales = start.Previous;
                        string salesAddress = expectedSales.get_Address(false, false, Excel.XlReferenceStyle.xlA1, defaultParameter, defaultParameter);

                        // Determines how much inventory is required 
                        // until delivery.
                        // Determines the number of days until delivery.
                        int waitDays = this.GetDaysToDelivery();

                        start.Formula = "=" + waitDays + "*" + salesAddress;

                        // Format "0.00" - two decimal places.
                        start.NumberFormat = "0.00";
                        twoLines.AutoFill(fillRange, Excel.XlAutoFillType.xlFillDefault);
                        break;

                    case (int)StatHeadings.CurrentInventory:
                        // Fills in the current inventory column.
                        // Gets the range for the last day from the journal.
                        int count = (end.Row - start.Row + 1) / 2;
                        Excel.Range currentCell = start;

                        for (int row = 0; row < count; row += 1)
                        {
                            Excel.Range flavorCell = currentCell.get_Offset(0, 0 - 5);


                            string flavor = ExcelHelpers.GetValueAsString(flavorCell);
                            int inventory = Globals.DataSet.Sales.FindByDateFlavor(Globals.DataSet.MaxDate, flavor).Inventory;

                            currentCell.Value2 = inventory;

                            if (row != 0)
                            {
                                Excel.Range twoCells = currentCell.get_Resize(2, 1);

                                twoCells.Merge(System.Type.Missing);
                                currentCell = twoCells;
                            }

                            currentCell = currentCell.get_Offset(1, 0);
                        }

                        break;

                    case (int)StatHeadings.ProjectInventory:

                        // Gets the addresses for the projected sales and
                        // current inventory.
                        Excel.Range currentInventory = start.Previous;
                        Excel.Range required = currentInventory.Previous;
                        string currentInventoryAddress = currentInventory.get_Address(false, false, Excel.XlReferenceStyle.xlA1, defaultParameter, defaultParameter);
                        string requiredAddress = required.get_Address(false, false, Excel.XlReferenceStyle.xlA1, defaultParameter, defaultParameter);

                        // Determines the inventory expected on the 
                        // delivery date.
                        start.Formula = "=MAX(0," + currentInventoryAddress + "-" + requiredAddress + ")";

                        // Format "0.00" - two decimal places.
                        start.NumberFormat = "0.00";
                        twoLines.AutoFill(fillRange, Excel.XlAutoFillType.xlFillDefault);
                        break;

                    case (int)StatHeadings.OrderQuanity:
                        // Determines the addresses for the projected inventory
                        // and the required amounts.
                        Excel.Range projectedInventory = start.Previous;
                        Excel.Range needed = projectedInventory.Previous.Previous;
                        string projectedInventoryAddress = projectedInventory.get_Address(false, false, Excel.XlReferenceStyle.xlA1, defaultParameter, defaultParameter);
                        string neededAddress = needed.get_Address(false, false, Excel.XlReferenceStyle.xlA1, defaultParameter, defaultParameter);

                        // Determines the order size needed for each item.
                        start.Formula = "=" + neededAddress + "-" + projectedInventoryAddress;

                        // Format "0.00" - two decimal places.
                        start.NumberFormat = "0.00";
                        twoLines.AutoFill(fillRange, Excel.XlAutoFillType.xlFillDefault);
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw;
            }
        }

        private int GetDaysToDelivery()
        {
            // This method determines the number of days
            // till the next scheduled delivery.
            // This is needed to estimate for how many days the order is made.

            TimeSpan difference = this.nextScheduledDeliveryDate - this.deliveryDate;

            return difference.Days;
        }

        Excel.PivotTable CreatePivotTable()
        {
            TextFileGenerator generator = new TextFileGenerator(Globals.DataSet.Sales);

            try
            {
                Excel.Range destination = this.worksheet.get_Range(pivotTableAddress, defaultParameter);
                Excel.PivotTable pivotTable;

                pivotTable = Globals.ThisWorkbook.CreateSalesPivotTable(destination, generator.FullPath);

                // Adjusts the properties of the new PivotTable 
                pivotTable.ColumnGrand = false;
                pivotTable.RowGrand = false;

                // Adds the desired rows and columns within 
                // the PivotTable.
                pivotTable.AddFields("Flavor", defaultParameter, defaultParameter, defaultParameter);

                Excel.PivotField soldField = pivotTable.AddDataField(pivotTable.PivotFields("Sold"), Properties.Resources.AverageOfUnitsSold, Excel.XlConsolidationFunction.xlAverage);

                // Sets the view of data desired within the PivotTable.
                // Format "0.0" - one decimal place.
                soldField.NumberFormat = "0.0";

                Excel.PivotField profitField = pivotTable.AddDataField(pivotTable.PivotFields("Sold"), Properties.Resources.StdDevOfUnitsSold, Excel.XlConsolidationFunction.xlStDev);

                // Sets the view of data desired within the PivotTable.
                // Format "0.00" - two decimal places.
                profitField.NumberFormat = "0.00";

                // Hiding the two floating bars that get added when a PivotTable is created.
                Globals.ThisWorkbook.ShowPivotTableFieldList = false;
                Globals.ThisWorkbook.Application.CommandBars["PivotTable"].Visible = false;

                return pivotTable;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                throw;
            }
            finally
            {
                generator.Dispose();
            }

        }


        private void PopulateDateInformation(DateTime selectedDate)
        {
            // This method populates the worksheet with the next order 
            // date and its corresponding delivery date.
            // Gets the next order date and populates it.
            Excel.Range orderDateCell = worksheet.get_Range(orderDateAddress, defaultParameter);

            orderDateCell.Value2 = Properties.Resources.OrderDateHeader;
            orderDateCell.Next.Value2 = selectedDate.ToShortDateString();

            Excel.Range deliveryDateCell = ExcelHelpers.GetCellFromRange(orderDateCell, 2, 1);

            deliveryDateCell.Value2 = Properties.Resources.DeliveryDateHeader;
            deliveryDateCell.Next.Value2 = this.deliveryDate.ToShortDateString();
        }

    }
}
