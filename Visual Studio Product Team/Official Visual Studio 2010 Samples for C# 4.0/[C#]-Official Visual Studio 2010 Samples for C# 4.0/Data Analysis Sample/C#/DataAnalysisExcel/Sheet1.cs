// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using Excel = Microsoft.Office.Interop.Excel;

namespace DataAnalysisExcel
{
    public partial class Sheet1
    {
        /// <summary>
        /// This is the location where the pivotTable will be created.
        /// </summary>        
        private const string pivotTableAddress = "$B$22";

        /// <summary>
        /// The data source for the sales list. This view is based on the Sales table of 
        /// Globals.DataSet, filtered to display one day's worth of data.
        /// </summary>
        private OperationsData.OperationsView dayView;

        /// <summary>
        /// The PivotTable with sales statistics.
        /// </summary>
        private Excel.PivotTable pivotTable;

        /// <summary>
        /// When the date currently selected is the last one for which data is available,
        /// two additional columns are shown: "Estimated Inventory" and "Recommendation"
        /// and columnsAdded is set to true. Otherwise it is false.
        /// </summary>
        private bool columnsAdded;

        private void Sheet1_Startup(object sender, System.EventArgs e)
        {

            try
            {
                this.Sheet1_TitleLabel.Value2 = Properties.Resources.Sheet1Title;
                this.Name = Properties.Resources.Sheet1Name;

                this.newDateButton.Text = Properties.Resources.AddNewDateButton;
                this.saveButton.Text = Properties.Resources.SaveDataButton;
   
                this.dayView = Globals.DataSet.CreateView();

                if (Globals.DataSet.Sales.Count != 0)
                {
                    this.DateSelector.MinDate = Globals.DataSet.MinDate;
                    this.DateSelector.MaxDate = Globals.DataSet.MaxDate;
                    this.DateSelector.Value = this.DateSelector.MaxDate;
                }

                using (TextFileGenerator textFile = new TextFileGenerator(Globals.DataSet.Sales))
                {
                    this.pivotTable = CreatePivotTable(textFile.FullPath);
                }

                Globals.DataSet.Sales.SalesRowChanged += new OperationsBaseData.SalesRowChangeEventHandler(Sales_SalesRowChanged);
                UnscheduledOrderControl smartPaneControl = new UnscheduledOrderControl();
                smartPaneControl.Dock = DockStyle.Fill;
                smartPaneControl.View = this.dayView;

                Globals.ThisWorkbook.ActionsPane.Controls.Add(smartPaneControl);

                this.Activate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }

        private void Sheet1_Shutdown(object sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// ValueChanged event handler for the DateTimePicker. Changes the
        /// dateView to show the selected date.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void DateSelector_ValueChanged(object sender, EventArgs e)
        {

            try
            {
                DateTimePicker control = (DateTimePicker)sender;

                dayView.Date = control.Value;

                DateTime lastDay = control.MaxDate;

                if (control.Value == lastDay)
                {
                    AddColumns();
                }
                else
                {
                    RemoveColumns();
                }

            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Add columns "Estimated Inventory" and "Recommendation" to the list
        /// object on the worksheet.
        /// </summary>
        private void AddColumns()
        {
            if (!columnsAdded)
            {
                dayView.BindToProtectedList(this.DayInventory, "Flavor", "Inventory", "Sold", "Profit", "Estimated Inventory", "Recommendation");

                SetLocalizedColumnNames();
                columnsAdded = true;
            }
        }

        /// <summary>
        /// Remove columns "Estimated Inventory" and "Recommendation" from the list
        /// object on the worksheet.
        /// </summary>
        private void RemoveColumns()
        {
            if (columnsAdded)
            {
                dayView.BindToProtectedList(this.DayInventory, "Flavor", "Inventory", "Sold", "Profit");
                SetLocalizedColumnNames();
                columnsAdded = false;
            }
        }

        /// <summary>
        /// Create a PivotTable with data from a tab-delimiter text file.
        /// </summary>
        /// <param name="filePath">Text file location.</param>
        /// <returns>Created PivotTable.</returns>
        private Excel.PivotTable CreatePivotTable(string filePath)
        {
            // If the table is already there,
            // return the existing table.
            string tableName = Properties.Resources.AveragesPivotTableName;            
            Excel.PivotTables tables = (Excel.PivotTables)this.PivotTables(missing);
            System.Collections.Generic.Queue<double> savedWidths = new System.Collections.Generic.Queue<double>();
            
            if (tables != null)
            { 
                int count = tables.Count;

                for (int i = 1; i <= count; i++)
                {
                    Excel.PivotTable table = tables.Item(i);

                    if (table.Name == tableName)
                    {
                        return table;
                    }
                }
            }
            
            
            try
            {
                // AddFields will resize the columns. Save the columns' widths
                // for restoring them after pivot fields are added
                foreach (Excel.Range column in DayInventory.HeaderRowRange.Cells)
                {
                    savedWidths.Enqueue((double)column.ColumnWidth);
                }
                
                // PivotTable creation requires that protection be off.
                Globals.ThisWorkbook.MakeReadWrite();
               
                Excel.PivotTable table = Globals.ThisWorkbook.CreateSalesPivotTable(this.get_Range(pivotTableAddress, missing), filePath);
                table.Name = tableName;

                // Adds the desired rows and columns within 
                // the PivotTable.
                table.AddFields("Flavor", missing, missing, missing);
                
                Excel.PivotField soldField = table.AddDataField(table.PivotFields("Sold"), Properties.Resources.AverageSold, Excel.XlConsolidationFunction.xlAverage);

                // Sets the view of data desired within the PivotTable.
                // Format "0.0" - one decimal place.
                soldField.NumberFormat = "0.0";

                Excel.PivotField profitField = table.AddDataField(table.PivotFields("Profit"), Properties.Resources.AverageProfit, Excel.XlConsolidationFunction.xlAverage);

                // Sets the view of data desired within the PivotTable.
                // Format "0.0" - two decimal places.
                profitField.NumberFormat = "0.00";

                // Hiding the two floating bars that get added when a PivotTable is created.
                Globals.ThisWorkbook.ShowPivotTableFieldList = false;
                Globals.ThisWorkbook.Application.CommandBars["PivotTable"].Visible = false;

                return table;
            }
            finally
            {
                // AddFields will have resized the columns. Restore the columns' widths.
                foreach (Excel.Range column in DayInventory.HeaderRowRange.Cells)
                {
                    column.ColumnWidth = savedWidths.Dequeue();
                }
                Globals.ThisWorkbook.MakeReadOnly();
            }
        }

        /// <summary>
        /// Set the list object's column headers to values from the resource table.
        /// </summary>
        private void SetLocalizedColumnNames()
        {
            string[] localizedInventoryColumns = {
                Properties.Resources.IceCreamHeader, 
                Properties.Resources.EodInventoryHeader, 
                Properties.Resources.UnitsSoldHeader, 
                Properties.Resources.NetGainHeader, 
                Properties.Resources.EstimatedInventoryHeader, 
                Properties.Resources.RecommendationHeader
            };

            try
            {
                Globals.ThisWorkbook.MakeReadWrite();
                this.DayInventory.HeaderRowRange.Value2 = localizedInventoryColumns;
            }
            finally
            {
                Globals.ThisWorkbook.MakeReadOnly();
            }
        }

        /// <summary>
        /// Click event handler for newDateButton. Inserts new rows into the Sales table for
        /// for a new date and sets up the list object for new data entry.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void newDateButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Globals.DataSet.IsLastDayComplete())
                {
                    MessageBox.Show(Globals.ThisWorkbook.IncompleteDataMessage);
                    return;
                }

                DateTime nextDate = Globals.DataSet.MaxDate.AddDays(1);

                foreach (OperationsBaseData.PricingRow row in Globals.DataSet.Pricing)
                {
                    OperationsBaseData.SalesRow newRow = (OperationsBaseData.SalesRow)this.dayView.Table.NewRow();
                    newRow.Flavor = row.Flavor;
                    newRow.Date = nextDate;
                    this.dayView.Table.AddSalesRow(newRow);
                }

                this.DateSelector.MaxDate = nextDate;
                this.DateSelector.Value = nextDate;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Click event handler for saveButton. Writes data back into XML files.
        /// </summary>
        /// <param name="sender">Sender of the event.</param>
        /// <param name="e">Event arguments.</param>
        private void saveButton_Click(object sender, EventArgs e)
        {
            Globals.DataSet.Save();
        }

        void Sales_SalesRowChanged(object sender, OperationsBaseData.SalesRowChangeEvent e)
        {
            if (e.Action == DataRowAction.Change)
            {
                Globals.ThisWorkbook.UpdateSalesPivotTable(this.pivotTable);
            }
        }

        #region VSTO Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.DateSelector.ValueChanged += new System.EventHandler(this.DateSelector_ValueChanged);
            this.newDateButton.Click += new System.EventHandler(this.newDateButton_Click);
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            this.Shutdown += new System.EventHandler(this.Sheet1_Shutdown);
            this.Startup += new System.EventHandler(this.Sheet1_Startup);

        }

        #endregion

    }
}
