// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;

namespace DocumentProtectionExcel
{
    public partial class ThisWorkbook
    {
        /// <summary>
        /// The DataGridView control on ActionsPane and the ListObject control 
        /// on Sheet1 share the same BindingSource. When value in DataGridView 
        /// changes, the ListObject value will change accordingly. However, 
        /// since Sheet1 is protected, unprotecting Sheet1 is needed for ListObject
        /// to change value.
        /// </summary>
        internal class CustomerBindingSource : BindingSource
        {
            protected override void OnListChanged(System.ComponentModel.ListChangedEventArgs e)
            {
                try
                {
                    try
                    {
                        // Unprotects Sheet1
                        Globals.Sheet1.UnprotectSheet();
                        base.OnListChanged(e);
                    }
                    finally
                    {
                        // Protects Sheet1
                        Globals.Sheet1.ProtectSheet();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,
                        "Error protecting or unprotecting sheet",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1);
                }
            }
        }

        /// <summary>
        /// User control which will be added to Actions Pane.
        /// </summary>
        private TechniqueUserControl techUserControl;
        /// <summary>
        /// A data set for storing data from ExcelSampleData xml file.
        /// </summary>
        internal DataSet customerDataSet = null;
        /// <summary>
        /// A binding source which is used for data binding to data set.
        /// </summary>
        internal CustomerBindingSource custBindingSource = null;

        #region Methods
        /// <summary>
        /// Loads DataSet with data from ExcelSampleData xml file.
        /// </summary>
        private void LoadDataSet()
        {
            try
            {
                if (customerDataSet == null)
                    customerDataSet = new DataSet();
                // Gets schema file location
                string schemaFileLocation = System.IO.Path.Combine(Path, "ExcelSampleData.xsd");
                // Gets xml file location
                string xmlFileLocation = System.IO.Path.Combine(Path, "ExcelSampleData.xml");

                // Reads data from schema and xml file
                customerDataSet.ReadXmlSchema(schemaFileLocation);
                customerDataSet.ReadXml(xmlFileLocation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                                "Error loading data set.",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error,
                                MessageBoxDefaultButton.Button1);
            }
        }
        #endregion

        /// <summary>
        /// Handles the Startup event for the workbook. When the event fires, 
        /// LoadDataSet method will be called to load data from XML file into
        /// customerDataSet, set customerBindingSource's DataSource property  
        /// to customerDataSet and its DataMember to "Customers". A TechniqueUserControl
        /// Should be created and attached to the ActionsPane.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void ThisWorkbook_Startup(object sender, System.EventArgs e)
        {
            // Loads data set from xml file
            LoadDataSet();

            // Creates BindingSource
            if (custBindingSource == null)
                custBindingSource = new CustomerBindingSource();
            custBindingSource.DataSource = customerDataSet;
            custBindingSource.DataMember = "Customer";

            // Adding rows in the data grid will not unprotect the sheet when the 
            // ListObject tries to change size, which will cause an exception. Set the
            // AllowNew property to false to force the data set to stay the same size.
            custBindingSource.AllowNew = false;

            // Adds user control to Actions Pane.
            techUserControl = new TechniqueUserControl();
            ActionsPane.Controls.Add(techUserControl);
        }

        private void ThisWorkbook_Shutdown(object sender, System.EventArgs e)
        {
        }
        #region VSTO Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisWorkbook_Startup);
            this.Shutdown += new System.EventHandler(ThisWorkbook_Shutdown);
        }

        #endregion

    }
}
