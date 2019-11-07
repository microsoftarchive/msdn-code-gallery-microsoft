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
    public partial class ThisWorkbook
    {
        private CustomersOrdersControl customerOrdersControl;
        private CompanyData currentCompanyData;
        private BindingSource customersBindingSource;
        private BindingSource customerOrdersBindingSource;
        private BindingSource orderDetailsBindingSource;
        private BindingSource statusBindingSource;

        private void ThisWorkbook_Startup(object sender, System.EventArgs e)
        {
            LoadCompanyData();
            customerOrdersControl = new CustomersOrdersControl();
            this.ActionsPane.Controls.Add(customerOrdersControl);
        }

        private void ThisWorkbook_Shutdown(object sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// CurrentCompanyData accessor; ensures data is loaded for sample and returns it 
        /// via a CompanyData object
        /// </summary>
        internal CompanyData CurrentCompanyData
        {
            get
            {
                Debug.Assert(this.currentCompanyData != null);
                return currentCompanyData;
            }
        }

        internal BindingSource CustomersBindingSource
        {
            get
            {
                if (this.customersBindingSource == null)
                {
                    this.customersBindingSource = new BindingSource();
                    this.customersBindingSource.DataSource = this.currentCompanyData;
                    this.customersBindingSource.DataMember = "Customers";
                }
                return this.customersBindingSource;
            }
        }

        internal BindingSource CustomerOrdersBindingSource
        {
            get
            {
                if (this.customerOrdersBindingSource == null)
                {
                    this.customerOrdersBindingSource = new BindingSource();
                    this.customerOrdersBindingSource.DataSource = this.CustomersBindingSource;
                    this.customerOrdersBindingSource.DataMember = "FK_Customers_Orders";
                    this.customerOrdersBindingSource.Filter = "StatusID <> 0";
                }
                return this.customerOrdersBindingSource;
            }
        }

        internal BindingSource OrderDetailsBindingSource
        {
            get
            {
                if (this.orderDetailsBindingSource == null)
                {
                    this.orderDetailsBindingSource = new BindingSource();
                    this.orderDetailsBindingSource.DataSource = this.CustomerOrdersBindingSource;
                    this.orderDetailsBindingSource.DataMember = "FK_Orders_OrderDetails";
                }
                return this.orderDetailsBindingSource;
            }
        }

        internal BindingSource StatusBindingSource
        {
            get
            {
                if (this.statusBindingSource == null)
                {
                    this.statusBindingSource = new BindingSource();
                    this.statusBindingSource.DataSource = this.CustomerOrdersBindingSource;
                    this.statusBindingSource.DataMember = "Orders_Status";
                }
                return this.statusBindingSource;
            }
        }

        /// <summary>
        /// Ensures data is read in from data source so the sample will run properly.
        /// </summary>
        private void LoadCompanyData()
        {
            Debug.Assert(this.currentCompanyData == null);
            this.currentCompanyData = new CompanyData();
            this.currentCompanyData.DataSetName = "CompanyData";
            this.currentCompanyData.Locale = new System.Globalization.CultureInfo("en-US");
            this.currentCompanyData.RemotingFormat = System.Data.SerializationFormat.Xml;

            string fileLocation = System.IO.Path.Combine(this.Path,"data.xml");
            Debug.Assert(System.IO.File.Exists(fileLocation),
                String.Format("Sample data file {0} does not exist.", fileLocation));

            this.currentCompanyData.ReadXml(fileLocation, XmlReadMode.IgnoreSchema);
            this.currentCompanyData.AcceptChanges();
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
