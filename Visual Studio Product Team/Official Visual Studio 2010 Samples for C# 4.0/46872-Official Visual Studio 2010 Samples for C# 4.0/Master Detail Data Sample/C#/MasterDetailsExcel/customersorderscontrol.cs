// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace MasterDetailsRelationships
{
    /// <summary>
    /// Summary description for ActionsPaneControl.
    /// </summary>
    public class CustomersOrdersControl : System.Windows.Forms.UserControl
    {
        private System.Windows.Forms.Label selectCustomerLabel;
        private System.Windows.Forms.Panel divider1;
        private System.Windows.Forms.ComboBox selectCustomer;
        private System.Windows.Forms.Label contactNameLabel;
        private System.Windows.Forms.Label contactName;
        private System.Windows.Forms.Label phoneNumberLabel;
        private System.Windows.Forms.Label phoneNumber;
        private System.Windows.Forms.Label faxNumber;
        private System.Windows.Forms.Label faxNumberLabel;
        private System.Windows.Forms.Panel divider2;
        private System.Windows.Forms.Label unfulfilledOrdersLabel;
        private System.Windows.Forms.ListBox unfulfilledOrders;

        public CustomersOrdersControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.selectCustomer.DataSource = Globals.ThisWorkbook.CustomersBindingSource;
            this.selectCustomer.DisplayMember = "ContactName";
            this.selectCustomer.ValueMember = "CustomerID";

            this.contactName.DataBindings.Add("Text", Globals.ThisWorkbook.CustomersBindingSource, "ContactName");
            this.phoneNumber.DataBindings.Add("Text", Globals.ThisWorkbook.CustomersBindingSource, "Phone");
            this.faxNumber.DataBindings.Add("Text", Globals.ThisWorkbook.CustomersBindingSource, "Fax");

            this.unfulfilledOrders.DataSource = Globals.ThisWorkbook.CustomerOrdersBindingSource;
            this.unfulfilledOrders.DisplayMember = "OrderID";
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.selectCustomerLabel = new System.Windows.Forms.Label();
            this.divider1 = new System.Windows.Forms.Panel();
            this.selectCustomer = new System.Windows.Forms.ComboBox();
            this.contactNameLabel = new System.Windows.Forms.Label();
            this.contactName = new System.Windows.Forms.Label();
            this.phoneNumberLabel = new System.Windows.Forms.Label();
            this.phoneNumber = new System.Windows.Forms.Label();
            this.faxNumber = new System.Windows.Forms.Label();
            this.faxNumberLabel = new System.Windows.Forms.Label();
            this.divider2 = new System.Windows.Forms.Panel();
            this.unfulfilledOrdersLabel = new System.Windows.Forms.Label();
            this.unfulfilledOrders = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // selectCustomerLabel
            // 
            this.selectCustomerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectCustomerLabel.Location = new System.Drawing.Point(4, 34);
            this.selectCustomerLabel.Name = "selectCustomerLabel";
            this.selectCustomerLabel.Size = new System.Drawing.Size(143, 18);
            this.selectCustomerLabel.TabIndex = 1;
            this.selectCustomerLabel.Text = "Select a Customer";
            // 
            // divider1
            // 
            this.divider1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.divider1.Location = new System.Drawing.Point(4, 55);
            this.divider1.Name = "divider1";
            this.divider1.Size = new System.Drawing.Size(143, 2);
            this.divider1.TabIndex = 2;
            // 
            // selectCustomer
            // 
            this.selectCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.selectCustomer.FormattingEnabled = true;
            this.selectCustomer.Location = new System.Drawing.Point(4, 60);
            this.selectCustomer.Name = "selectCustomer";
            this.selectCustomer.Size = new System.Drawing.Size(143, 21);
            this.selectCustomer.TabIndex = 3;
            // 
            // contactNameLabel
            // 
            this.contactNameLabel.AutoSize = true;
            this.contactNameLabel.Location = new System.Drawing.Point(4, 88);
            this.contactNameLabel.Name = "contactNameLabel";
            this.contactNameLabel.Size = new System.Drawing.Size(76, 14);
            this.contactNameLabel.TabIndex = 4;
            this.contactNameLabel.Text = "Contact Name";
            // 
            // contactName
            // 
            this.contactName.AutoSize = true;
            this.contactName.Location = new System.Drawing.Point(4, 109);
            this.contactName.Name = "contactName";
            this.contactName.Size = new System.Drawing.Size(81, 14);
            this.contactName.TabIndex = 5;
            this.contactName.Text = "customerName";
            // 
            // phoneNumberLabel
            // 
            this.phoneNumberLabel.AutoSize = true;
            this.phoneNumberLabel.Location = new System.Drawing.Point(4, 146);
            this.phoneNumberLabel.Name = "phoneNumberLabel";
            this.phoneNumberLabel.Size = new System.Drawing.Size(80, 14);
            this.phoneNumberLabel.TabIndex = 6;
            this.phoneNumberLabel.Text = "Phone Number";
            // 
            // phoneNumber
            // 
            this.phoneNumber.AutoSize = true;
            this.phoneNumber.Location = new System.Drawing.Point(4, 166);
            this.phoneNumber.Name = "phoneNumber";
            this.phoneNumber.Size = new System.Drawing.Size(76, 14);
            this.phoneNumber.TabIndex = 7;
            this.phoneNumber.Text = "phoneNumber";
            // 
            // faxNumber
            // 
            this.faxNumber.AutoSize = true;
            this.faxNumber.Location = new System.Drawing.Point(4, 225);
            this.faxNumber.Name = "faxNumber";
            this.faxNumber.Size = new System.Drawing.Size(60, 14);
            this.faxNumber.TabIndex = 9;
            this.faxNumber.Text = "faxNumber";
            // 
            // faxNumberLabel
            // 
            this.faxNumberLabel.AutoSize = true;
            this.faxNumberLabel.Location = new System.Drawing.Point(4, 204);
            this.faxNumberLabel.Name = "faxNumberLabel";
            this.faxNumberLabel.Size = new System.Drawing.Size(66, 14);
            this.faxNumberLabel.TabIndex = 8;
            this.faxNumberLabel.Text = "Fax Number";
            // 
            // divider2
            // 
            this.divider2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.divider2.Location = new System.Drawing.Point(4, 278);
            this.divider2.Name = "divider2";
            this.divider2.Size = new System.Drawing.Size(141, 2);
            this.divider2.TabIndex = 3;
            // 
            // unfulfilledOrdersLabel
            // 
            this.unfulfilledOrdersLabel.Location = new System.Drawing.Point(4, 260);
            this.unfulfilledOrdersLabel.Name = "unfulfilledOrdersLabel";
            this.unfulfilledOrdersLabel.TabIndex = 1;
            this.unfulfilledOrdersLabel.Text = "Unfulfilled Orders";
            // 
            // unfulfilledOrders
            // 
            this.unfulfilledOrders.FormattingEnabled = true;
            this.unfulfilledOrders.Location = new System.Drawing.Point(4, 287);
            this.unfulfilledOrders.Name = "unfulfilledOrders";
            this.unfulfilledOrders.Size = new System.Drawing.Size(143, 186);
            this.unfulfilledOrders.TabIndex = 11;

            // 
            // CustomersOrdersControl
            // 
            this.Controls.Add(this.unfulfilledOrders);
            this.Controls.Add(this.phoneNumberLabel);
            this.Controls.Add(this.phoneNumber);
            this.Controls.Add(this.contactNameLabel);
            this.Controls.Add(this.selectCustomer);
            this.Controls.Add(this.divider1);
            this.Controls.Add(this.selectCustomerLabel);
            this.Controls.Add(this.contactName);
            this.Controls.Add(this.faxNumber);
            this.Controls.Add(this.faxNumberLabel);
            this.Controls.Add(this.divider2);
            this.Controls.Add(this.unfulfilledOrdersLabel);
            this.Name = "CustomersOrdersControl";
            this.Size = new System.Drawing.Size(264, 584);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
