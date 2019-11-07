// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using System.Diagnostics;

namespace MasterDetailsRelationships
{
    public partial class Sheet1
    {
        private string[] productListColumnHeaders = { "ProductName", "Quantity", "Inventory" };

        private const int productNameColumn = 1;
        private const int quantityOrderedColumn = 2;
        private const int currentInventoryColumn = 3;
        private const string quantityOrderedChartSeries = "=\"Quantity Ordered\"";
        private const string inventoryChartSeries = "=\"Inventory\"";
        private const string noOrderSelectedTitle = "No Order Selected";
        private const string canFulfillOrderTitle = "Order Can Be Fulfilled";
        private const string cannotFulfillOrderTitle = "Order Not Ready for Fulfillment";

        private void Sheet1_Startup(object sender, System.EventArgs e)
        {
            // Set the column headers for the ProductList.
            this.ProductList.HeaderRowRange.Value2 = productListColumnHeaders;

            // Set the captions for the Chart.
            ((Excel.Series)this.OrdersChart.SeriesCollection(1)).Name = quantityOrderedChartSeries;
            ((Excel.Series)this.OrdersChart.SeriesCollection(2)).Name = inventoryChartSeries;
            this.OrdersChart.ChartTitle.Text = noOrderSelectedTitle;

            // Bind the ProductList to the order details of the currently selected order.
            this.ProductList.SetDataBinding(Globals.ThisWorkbook.OrderDetailsBindingSource,
                null, productListColumnHeaders);

            // Bind the Status named range to the status of the currently selected order.
            this.Status.DataBindings.Add("Value2", Globals.ThisWorkbook.StatusBindingSource, "Status");
        }

        void ProductList_Change(Microsoft.Office.Interop.Excel.Range targetRange, Microsoft.Office.Tools.Excel.ListRanges changedRanges)
        {
            this.UpdateChart();
        }

        /// <summary>
        /// Updates the title of the chart based on the details of the currently 
        /// selected order.
        /// </summary>
        private void UpdateChart()
        {
            if (Globals.ThisWorkbook.CustomerOrdersBindingSource.Count == 0)
                this.OrdersChart.ChartTitle.Text = noOrderSelectedTitle;
            else if (this.CanFulfillOrder())
                this.OrdersChart.ChartTitle.Text = canFulfillOrderTitle;
            else
                this.OrdersChart.ChartTitle.Text = cannotFulfillOrderTitle;
        }

        /// <summary>
        /// Determines if there is enough inventory of the products 
        /// for the currently selected order.
        /// </summary>
        /// <returns></returns>
        private bool CanFulfillOrder()
        {
            Excel.Range listRange = this.ProductList.DataBodyRange;

            for (int i = 1; i <= listRange.Rows.Count; i++)
            {
                // Get the values within the ListRow.
                object[,] values = (object[,])((Excel.Range)listRange.Rows[i, missing]).Value2;

                // Determine what product the row is representing.
                if (values[1, productNameColumn] == null)
                    continue;
                string product = values[1, productNameColumn].ToString();

                // If there is a product in this row, determine the available quantity of that product.
                if (!String.IsNullOrEmpty(product))
                {
                    int quantity = Convert.ToInt32(values[1, quantityOrderedColumn]);
                    CompanyData.ProductsRow productRow = Globals.ThisWorkbook.CurrentCompanyData.Products.FindByName(product);

                    // Check to see if there is enough inventory for the quantity being ordered.
                    if ((productRow.Inventory - quantity) < 0)
                        return false;
                }
            }

            return true;
        }

        private void Status_Change(Microsoft.Office.Interop.Excel.Range Target)
        {
            // Get the StatusID for the Status just set on the Status named range.
            Debug.Assert((Globals.ThisWorkbook.CustomerOrdersBindingSource.Current as DataRowView) != null);
            DataRowView currentRow = (DataRowView)Globals.ThisWorkbook.CustomerOrdersBindingSource.Current;
            Debug.Assert((currentRow.Row as CompanyData.OrdersRow) != null);
            CompanyData.OrdersRow orderRow = (CompanyData.OrdersRow)currentRow.Row;
            int newStatus = Globals.ThisWorkbook.CurrentCompanyData.Status.FindByStatus(
                this.Status.Value2.ToString()).StatusID;

            // Check to see if the status was set to Fulfilled when it could not
            // actually be fulfilled.  If so, alert the user that the order cannot be fulfilled.
            if (newStatus == 0 && orderRow.StatusID !=0 && !this.CanFulfillOrder())
            {
                MessageBox.Show("Order cannot be fulfilled with current inventory levels.");
                this.Status.Value2 = orderRow.StatusRow.Status;
                return;
            }
            else if (newStatus == 0 && orderRow.StatusID != 0)
            {
                // The order was changed to be fulfilled, so the inventory needs
                // to be updated to remove the quantities that were shipped.
                this.UpdateInventory();
            }

            // Update the order to reflect the new status.
            orderRow.StatusID = newStatus;
        }

        /// <summary>
        /// Updates the available inventory of products based on the current order being fulfilled.
        /// </summary>
        private void UpdateInventory()
        {
            Excel.Range listRange = this.ProductList.DataBodyRange;

            for (int i = 1; i <= listRange.Rows.Count; i++)
            {
                // Get the values within the ListRow.
                object[,] values = (object[,])((Excel.Range)listRange.Rows[i, missing]).Value2;
                if (values[1, productNameColumn] == null)
                    continue;

                // Determine what product the row is representing.
                string product = values[1, productNameColumn].ToString();

                // If there is a product in this row, determine the available quantity of that product.
                if (!String.IsNullOrEmpty(product))
                {
                    int quantity = Convert.ToInt32(values[1, quantityOrderedColumn]);
                    CompanyData.ProductsRow productRow = Globals.ThisWorkbook.CurrentCompanyData.Products.FindByName(product);

                    // Update the ProductRow to reflect the new inventory level.
                    productRow.Inventory = productRow.Inventory - quantity;
                }
            }

            // Save changes to the DataSet.
            Globals.ThisWorkbook.CurrentCompanyData.AcceptChanges();
        }


        private void Sheet1_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO Designer generated code


        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Shutdown += new System.EventHandler(this.Sheet1_Shutdown);
            this.Startup += new System.EventHandler(this.Sheet1_Startup);
            this.ProductList.Change += new Microsoft.Office.Tools.Excel.ListObjectChangeHandler(ProductList_Change);
            this.Status.Change += new Microsoft.Office.Interop.Excel.DocEvents_ChangeEventHandler(Status_Change);
        }
        #endregion
    }
}
