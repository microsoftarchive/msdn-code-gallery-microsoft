// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace DataAnalysisExcel
{
    /// <summary>
    /// This worksheet displays historic sales data for an ice cream flavor.
    /// </summary>
    public partial class Sheet2
    {
        /// <summary>
        /// The data view built on the Sales table and filtered by flavor.
        /// </summary>
        private OperationsData.OperationsView view;

        /// <summary>
        /// The flavor for which history is displayed.
        /// </summary>
        private string flavor = null;

        /// <summary>
        /// Accessor and mutator for the flavor field. When
        /// property is modified, changes the view accordingly.
        /// </summary>
        /// <value>The current flavor.</value>
        public string Flavor
        {
            get
            {
                return flavor;
            }
            set
            {
                flavor = value;

                if (FlavorChanged != null)
                {
                    FlavorChanged(this, new System.ComponentModel.PropertyChangedEventArgs("Flavor"));
                }

                if (view != null)
                {
                    view.Flavor = flavor;
                }
            }
        }

        /// <summary>
        /// Event raised when Flavor is changed. When the Flavor property is
        /// used for data binding, the PropertyManager listens for this event.
        /// </summary>
        public event EventHandler FlavorChanged;

        private void Sheet2_Startup(object sender, System.EventArgs e)
        {
            this.Sheet2_TitleLabel.Value2 = Properties.Resources.Sheet2Title;
            this.Name = Properties.Resources.Sheet2Name;
            this.IceCreamLabel.Value2 = Properties.Resources.IceCreamHeader;
            
            this.Chart_1.ChartTitle.Text = Properties.Resources.ProfitHeader;
            ((Excel.Axis)this.Chart_1.Axes(Excel.XlAxisType.xlValue, Excel.XlAxisGroup.xlPrimary)).AxisTitle.Text = Properties.Resources.ProfitHeader;
            ((Excel.Axis)this.Chart_1.Axes(Excel.XlAxisType.xlCategory, Excel.XlAxisGroup.xlPrimary)).AxisTitle.Text = Properties.Resources.DateHeader;

            this.view = Globals.DataSet.CreateView();

            if (this.Flavor != null)
            {
                view.Flavor = this.Flavor;
            }
            else if (view.Count != 0)
            {
                this.Flavor = (string)view[0].Row["Flavor"];
            }

            this.FlavorNamedRange.DataBindings.Add("Value2", this, "Flavor");

            this.History.SetDataBinding(view, "", "Date", "Inventory", "Sold", "Profit");

            this.History.ListColumns[1].Name = Properties.Resources.DateHeader;
            this.History.ListColumns[2].Name = Properties.Resources.InventoryHeader;
            this.History.ListColumns[3].Name = Properties.Resources.SoldHeader;
            this.History.ListColumns[4].Name = Properties.Resources.ProfitHeader;            
        }

        private void Sheet2_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(Sheet2_Startup);
            this.Shutdown += new System.EventHandler(Sheet2_Shutdown);
        }

        #endregion

    }
}
