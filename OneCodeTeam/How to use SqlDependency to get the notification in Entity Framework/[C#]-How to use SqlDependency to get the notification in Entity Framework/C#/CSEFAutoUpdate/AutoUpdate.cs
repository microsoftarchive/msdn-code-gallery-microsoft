/****************************** Module Header ******************************\
Module Name:  AutoUpdate.cs
Project:      CSEFAutoUpdate
Copyright (c) Microsoft Corporation.

We can use the Sqldependency to get the notification when the data is changed 
in database, but EF doesn’t have the same feature. In this sample, we will 
demonstrate how to automatically update by Sqldependency in Entity Framework.
In this sample, we will demonstrate two ways that use SqlDependency to get the 
change notification to auto update data:
1. Get the notification of changes immediately.
2. Get the notification of changes regularly.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Security;

namespace CSEFAutoUpdate
{
    public partial class AutoUpdate : Form
    {
        private WarehouseContext warehouse = new WarehouseContext();
        private IQueryable iquery = null;
        private ImmediateNotificationRegister<Product> notification = null;
        private RegularlyNotificationRegister<Product> regularNotificaton = null;
        private Timer formTimer = null;
        private Int32 interval;
        private Int32 count;

        private bool CanRequestNotifications()
        {
            // In order to use the callback feature of the
            // SqlDependency, the application must have
            // the SqlClientPermission permission.
            try
            {
                SqlClientPermission perm =
                    new SqlClientPermission(
                    PermissionState.Unrestricted);

                perm.Demand();

                return true;
            }
            catch (SecurityException se)
            {
                MessageBox.Show(se.Message, "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
        }
        }

        public AutoUpdate()
        {
            InitializeComponent();
        }

        private void AutoUpdate_Load(object sender, EventArgs e)
        {
            btnGetData.Enabled = CanRequestNotifications();

            CreateDatabaseIfNotExist();

            // start immediate notification register
            ImmediateNotificationRegister<Product>.StartMonitor(warehouse);
            RegularlyNotificationRegister<Product>.StartMonitor(warehouse);
        }


        /// <summary>
        /// Create the database and insert the data if there's no database
        /// </summary>
        private void CreateDatabaseIfNotExist()
        {
            if (warehouse != null && !warehouse.Database.Exists())
            {
                Product[] products = 
                { 
                  new Product { Name = "Red Bicycle",Price=(Decimal)805.5,Amount=1050 },
                  new Product { Name = "White Bicycle",Price=(Decimal)1049.9,Amount=312 },
                  new Product { Name = "Black Bicycle",Price=(Decimal)888.8,Amount=965 }
                };

                foreach (Product product in products)
                {
                    warehouse.Products.Add(product);
                }

                warehouse.SaveChanges();
            }
        }

        private void GetData_Click(object sender, EventArgs e)
        {
            Decimal lowPrice, highPrice;

            if (!Decimal.TryParse(txtLowPrice.Text, out lowPrice))
            {
                lowPrice = 0;
            }

            if (!Decimal.TryParse(txtHighPrice.Text, out highPrice))
            {
                highPrice = Decimal.MaxValue;
            }


            // Create the query.
            iquery = from p in warehouse.Products
                     where p.Price >= lowPrice && p.Price <= highPrice
                     select p;

            if (this.rabtnImUpdate.Checked)
            {
                // If need to update immediately, use ImmediateNotificationRegister to register 
                // SqlDependency.
                notification = new ImmediateNotificationRegister<Product>(warehouse, iquery);
                notification.OnChanged += NotificationOnChanged;
            }
            else
            {
                // We can use RegularlyNotificationRegister to implement update regularly.
                if (Int32.TryParse(this.txtInterval.Text, out interval))
                {
                    regularNotificaton = new RegularlyNotificationRegister<Product>(warehouse, iquery, interval * 1000);
                    regularNotificaton.OnChanged += NotificationOnChanged;

                    // Only for displaying the progress
                    this.proBar.Value = 100 / interval;
                    count = 1;
                    formTimer = new Timer();
                    formTimer.Interval = 1000;
                    formTimer.Tick += formTimer_Tick;
                    formTimer.Start();
                }
                else
                {
                    return;
                }
            }

            GetData();

            ChangeButtonState();
        }

        /// <summary>
        /// Display progress
        /// </summary>
        void formTimer_Tick(object sender, EventArgs e)
        {
            count = (count) % interval + 1;
            this.proBar.Value = (100 * count) / interval;
        }

        /// <summary>
        /// When changed the data, the method will be invoked.
        /// </summary>
        void NotificationOnChanged(object sender, EventArgs e)
        {
            // If InvokeRequired returns True, the code
            // is executing on a worker thread.
            if (this.InvokeRequired)
            {
                // Create a delegate to perform the thread switch.
                this.BeginInvoke(new Action(GetData), null);

                return;
            }

            GetData();
        }

        /// <summary>
        /// Display the data in the DataGridView
        /// </summary>
        void GetData()
        {
            dgvWatch.DataSource = (iquery as DbQuery<Product>).ToList();
        }

        /// <summary>
        /// Update the new price.
        /// </summary>
        private void Update_Click(object sender, EventArgs e)
        {
            Int32 id;
            Decimal newPrice;

            if (Int32.TryParse(txtId.Text, out id) && Decimal.TryParse(txtNewPrice.Text, out newPrice))
            {
                Product product = (from p in warehouse.Products
                                   where p.Id == id
                                   select p).FirstOrDefault();

                if (product != null)
                {
                    product.Price = newPrice;
                    warehouse.SaveChanges();
                }
            }
            else
            {
                MessageBox.Show("Please input the valid values.");
            }
        }


        private void dgvWatch_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView datagrid = (DataGridView)sender;
            List<Product> products = (List<Product>)datagrid.DataSource;

            if (e.RowIndex >= 0 && e.RowIndex < products.Count)
            {
                Product product = products[e.RowIndex];

                txtId.Text = product.Id.ToString();
                txtNewPrice.Text = product.Price.ToString();
            }
        }

        /// <summary>
        /// Stop SqlDependency.
        /// </summary>
        private void StopSqlDependency(object sender, EventArgs e)
        {
            try
            {
                if (notification != null)
                {
                    notification.Dispose();
                    notification = null;
                }

                if (regularNotificaton != null)
                {
                    regularNotificaton.Dispose();
                    regularNotificaton = null;
                }

                if (formTimer != null)
                {
                    formTimer.Stop();
                    formTimer.Dispose();
                    formTimer = null;
                    this.proBar.Value = 0;
                }

                ChangeButtonState();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message, "Paramter Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    MessageBox.Show(ex.Message + "(" + ex.InnerException.Message + ")", "Failed to Stop SqlDependency", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(ex.Message, "Failed to Stop SqlDependency", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ChangeButtonState()
        {
            Boolean state = !this.btnGetData.Enabled;

            this.btnGetData.Enabled = state;
            this.txtLowPrice.Enabled = state;
            this.txtHighPrice.Enabled = state;
            this.rabtnImUpdate.Enabled = state;
            this.rabtnRegUpdate.Enabled = state;
            ChangeStateByRadioButton();

            this.txtId.Enabled = !state;
            this.txtNewPrice.Enabled = !state;
            this.btnUpdate.Enabled = !state;

            this.btnStop.Enabled = !state;
        }

        private void ChangeStateByRadioButton()
        {
            Boolean state = this.rabtnRegUpdate.Enabled && this.rabtnRegUpdate.Checked;

            this.txtInterval.Enabled = state;
            this.proBar.Enabled = state;
        }

        private void rabtnRegUpdate_CheckedChanged(object sender, EventArgs e)
        {
            ChangeStateByRadioButton();
        }

        private void AutoUpdate_FormClosed(object sender, FormClosedEventArgs e)
        {
            // stop notification monitor
            ImmediateNotificationRegister<Product>.StopMonitor(warehouse);
            RegularlyNotificationRegister<Product>.StopMonitor(warehouse);
        }
    }
}
