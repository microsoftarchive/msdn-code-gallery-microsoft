// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using System.Globalization;

namespace DataAnalysisExcel
{
    /// <summary>
    /// This is the control that shows in the Actions Pane. It allows to create
    /// an unscheduled ice cream order and to see an ice cream's sales history.
    /// </summary>
    public partial class UnscheduledOrderControl : UserControl
    {
        /// <summary>
        /// The cost to place an unscheduled order.
        /// </summary>
        const double unscheduledDeliveryCost = 25;

        /// <summary>
        /// Gets or sets the current day's view.
        /// </summary>
        /// <value></value>
        internal OperationsData.OperationsView View
        {
            get
            {
                return this.view;
            }
            set
            {
                if (this.view != null)
                {
                    this.view.ListChanged -= new ListChangedEventHandler(view_ListChanged);
                }

                this.view = value;

                if (this.view != null)
                {
                    this.view.ListChanged += new ListChangedEventHandler(view_ListChanged);
                    UpdateRecommendationLabel();
                }
            }
        }

        public UnscheduledOrderControl()
        {
            this.InitializeComponent();

            System.ComponentModel.ComponentResourceManager resources = 
                new System.ComponentModel.ComponentResourceManager(typeof(UnscheduledOrderControl));
            resources.ApplyResources(this.selectorLabel, "selectorLabel", CultureInfo.CurrentUICulture);
            resources.ApplyResources(this.flavorComboBox, "flavorComboBox", CultureInfo.CurrentUICulture);
            resources.ApplyResources(this.highLabel, "highLabel", CultureInfo.CurrentUICulture);
            resources.ApplyResources(this.lowLabel, "lowLabel", CultureInfo.CurrentUICulture);
            resources.ApplyResources(this.highList, "highList", CultureInfo.CurrentUICulture);
            resources.ApplyResources(this.lowList, "lowList", CultureInfo.CurrentUICulture);
            resources.ApplyResources(this.recommendationGroup, "recommendationGroup", CultureInfo.CurrentUICulture);
            resources.ApplyResources(this.createOrderButton, "createOrderButton", CultureInfo.CurrentUICulture);
            resources.ApplyResources(this.orderLabel, "orderLabel", CultureInfo.CurrentUICulture);
            resources.ApplyResources(this.recommendationLabel, "recommendationLabel", CultureInfo.CurrentUICulture);
            resources.ApplyResources(this, "$this", CultureInfo.CurrentUICulture);

            // Data bind the flavor combo box to the pricing table.
            this.flavorComboBox.DataSource = Globals.DataSet.Pricing;
            this.flavorComboBox.DisplayMember = "Flavor";
        }

        /// <summary>
        /// Creates a new order worksheet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateOrderButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                new OrderingSheet(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Event handler for the flavor combo box's SelectedIndexChanged event.
        /// Have the flavor history worksheet display the selected flavor.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void flavorComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            DataRowView selectedItem = (DataRowView)((ComboBox)sender).SelectedItem;
            DisplayFlavorHistory(((OperationsBaseData.PricingRow)selectedItem.Row).Flavor);
        }

        /// <summary>
        /// Event handler for the view's ListChanged event. When the view changes to display
        /// a date that is not the most recent one, the only controls showing are the flavor
        /// combo box and its label. When inventory data changes in the view, three controls are updated:
        /// the low inventory list, the high inventory list and the recommendation label.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void view_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (this.View.Date == Globals.DataSet.MaxDate)
            {
                if (e.ListChangedType == ListChangedType.Reset)
                {
                    ShowLastDayControls(true);
                }
                else if (e.ListChangedType == ListChangedType.ItemChanged)
                {
                    double estimatedInventory = (double)this.View[e.NewIndex]["Estimated Inventory"];
                    string flavor = (string)this.View[e.NewIndex]["Flavor"];

                    if (estimatedInventory < 0)
                    {
                        ShowAsLowInventory(flavor);
                    }
                    else
                    {
                        double todaysInventory;
                        todaysInventory = ((OperationsBaseData.SalesRow)this.view[e.NewIndex].Row).Inventory;

                        double idealInventory;
                        idealInventory = todaysInventory - estimatedInventory;

                        // if more than 10 percent more than warranted by previous sales
                        if (todaysInventory > idealInventory * 1.1)
                        {
                            ShowAsHighInventory(flavor);
                        }
                        else
                        {
                            ShowAsAdequateInventory(flavor);
                        }
                    }

                    UpdateRecommendationLabel();
                }
            }
            else
            {
                if (e.ListChangedType == ListChangedType.Reset)
                {
                    ShowLastDayControls(false);
                }
            }
        }

        /// <summary>
        /// Shows a flavor as a low inventory item.
        /// </summary>
        /// <param name="flavor">Flavor to show.</param>
        void ShowAsLowInventory(string flavor)
        {
            if (!this.lowList.Items.Contains(flavor))
            {
                this.lowList.Items.Add(flavor);

                if (this.highList.Items.Contains(flavor))
                {
                    this.highList.Items.Remove(flavor);
                }
            }
        }

        /// <summary>
        /// Shows a flavor as a high inventory item.
        /// </summary>
        /// <param name="flavor">Flavor to show.</param>
        void ShowAsHighInventory(string flavor)
        {
            if (!this.highList.Items.Contains(flavor))
            {
                this.highList.Items.Add(flavor);

                if (this.lowList.Items.Contains(flavor))
                {
                    this.lowList.Items.Remove(flavor);
                }
            }
        }

        /// <summary>
        /// Removes a flavor from high- and low inventory lists.
        /// </summary>
        /// <param name="flavor">Flavor to remove from lists.</param>
        void ShowAsAdequateInventory(string flavor)
        {
            if (this.highList.Items.Contains(flavor))
            {
                this.highList.Items.Remove(flavor);
            }

            if (this.lowList.Items.Contains(flavor))
            {
                this.lowList.Items.Remove(flavor);
            }
        }

        /// <summary>
        /// Show or hide the low/high inventory list boxes, 
        /// their labels and the recommendation group box.
        /// </summary>
        /// <param name="show">True to show, false to hide.</param>
        private void ShowLastDayControls(bool show)
        {
            Control[] dynamicControls = new Control[] {
                this.highLabel, this.highList, this.lowLabel, this.lowList, this.recommendationGroup
            };

            if (show)
            {
                foreach (Control c in dynamicControls)
                {
                    c.Show();
                }
            }
            else
            {
                foreach (Control c in dynamicControls)
                {
                    c.Hide();
                }
            }
        }

        /// <summary>
        /// Click event handler for the high and low inventory lists. Displays sales history
        /// for the flavor being clicked on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inventoryList_Click(object sender, EventArgs e)
        {
            string flavor = (string)(((ListBox)sender).SelectedItem);
            if (flavor != null)
            {
                DisplayFlavorHistory(flavor);
            }
        }

        /// <summary>
        /// Shows a flavor's sales history in the Flavor History worksheet.
        /// </summary>
        /// <param name="flavor">Flavor for which history is displayed.</param>
        private static void DisplayFlavorHistory(string flavor)
        {
            Globals.Sheet2.Flavor = flavor;
            Globals.Sheet2.Activate();
        }

        /// <summary>
        /// Calculates potential profit from ordering ice cream.
        /// </summary>
        /// <returns>Potential profit.</returns>
        private double CalculatePotentialProfit()
        {
            double profit = 0 - unscheduledDeliveryCost;

            foreach (DataRowView rowView in this.view)
            {
                OperationsBaseData.SalesRow row = (OperationsBaseData.SalesRow)rowView.Row;

                if (!row.IsEstimated_InventoryNull() && row.Estimated_Inventory < 0)
                {
                    OperationsBaseData.PricingRow pricing = (OperationsBaseData.PricingRow)row.GetParentRow("Pricing_Sales");
                    double flavorProfit = (pricing.Price - pricing.Cost) * (0 - row.Estimated_Inventory);

                    profit += flavorProfit;
                }
            }

            return profit;
        }

        /// <summary>
        /// Compute and display ordering recommendation in recommendationLabel.
        /// </summary>
        void UpdateRecommendationLabel()
        {
            double profit = CalculatePotentialProfit();

            if (profit > 0)
            {
                this.recommendationLabel.Text = string.Format(
                    CultureInfo.CurrentUICulture,
                    Properties.Resources.UnscheduledOrderRecommended, 
                    profit);
            }
            else
            {
                this.recommendationLabel.Text = string.Format(
                    CultureInfo.CurrentUICulture,
                    Properties.Resources.UnscheduledOrderNotRecommended, 
                    0 - profit);
            }
        }
    }
}
