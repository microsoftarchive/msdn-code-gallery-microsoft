namespace O365_ManageLicenses
{
    partial class ManageLicense
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UserLicenseList = new System.Windows.Forms.DataGridView();
            this.LicenseId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LicenseName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ManageUsersSplitter = new System.Windows.Forms.SplitContainer();
            this.RemoveLicense = new System.Windows.Forms.Button();
            this.AssignLicense = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.UserLicenseList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ManageUsersSplitter)).BeginInit();
            this.ManageUsersSplitter.Panel1.SuspendLayout();
            this.ManageUsersSplitter.Panel2.SuspendLayout();
            this.ManageUsersSplitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // UserLicenseList
            // 
            this.UserLicenseList.AllowUserToAddRows = false;
            this.UserLicenseList.AllowUserToDeleteRows = false;
            this.UserLicenseList.AllowUserToResizeRows = false;
            this.UserLicenseList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.UserLicenseList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.UserLicenseList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LicenseId,
            this.LicenseName});
            this.UserLicenseList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UserLicenseList.Location = new System.Drawing.Point(0, 0);
            this.UserLicenseList.MultiSelect = false;
            this.UserLicenseList.Name = "UserLicenseList";
            this.UserLicenseList.ReadOnly = true;
            this.UserLicenseList.RowHeadersVisible = false;
            this.UserLicenseList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.UserLicenseList.Size = new System.Drawing.Size(825, 275);
            this.UserLicenseList.TabIndex = 1;
            // 
            // LicenseId
            // 
            this.LicenseId.DataPropertyName = "SkuId";
            this.LicenseId.HeaderText = "License Id";
            this.LicenseId.Name = "LicenseId";
            this.LicenseId.ReadOnly = true;
            // 
            // LicenseName
            // 
            this.LicenseName.DataPropertyName = "AccountSkuId";
            this.LicenseName.HeaderText = "License Name";
            this.LicenseName.Name = "LicenseName";
            this.LicenseName.ReadOnly = true;
            // 
            // ManageUsersSplitter
            // 
            this.ManageUsersSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ManageUsersSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.ManageUsersSplitter.IsSplitterFixed = true;
            this.ManageUsersSplitter.Location = new System.Drawing.Point(0, 0);
            this.ManageUsersSplitter.Name = "ManageUsersSplitter";
            this.ManageUsersSplitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // ManageUsersSplitter.Panel1
            // 
            this.ManageUsersSplitter.Panel1.Controls.Add(this.UserLicenseList);
            // 
            // ManageUsersSplitter.Panel2
            // 
            this.ManageUsersSplitter.Panel2.Controls.Add(this.RemoveLicense);
            this.ManageUsersSplitter.Panel2.Controls.Add(this.AssignLicense);
            this.ManageUsersSplitter.Size = new System.Drawing.Size(825, 346);
            this.ManageUsersSplitter.SplitterDistance = 275;
            this.ManageUsersSplitter.TabIndex = 2;
            // 
            // RemoveLicense
            // 
            this.RemoveLicense.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.RemoveLicense.Location = new System.Drawing.Point(415, 12);
            this.RemoveLicense.Name = "RemoveLicense";
            this.RemoveLicense.Size = new System.Drawing.Size(104, 42);
            this.RemoveLicense.TabIndex = 1;
            this.RemoveLicense.Text = "Remove License";
            this.RemoveLicense.UseVisualStyleBackColor = true;
            this.RemoveLicense.Click += new System.EventHandler(this.RemoveLicense_Click);
            // 
            // AssignLicense
            // 
            this.AssignLicense.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AssignLicense.Location = new System.Drawing.Point(305, 12);
            this.AssignLicense.Name = "AssignLicense";
            this.AssignLicense.Size = new System.Drawing.Size(104, 42);
            this.AssignLicense.TabIndex = 0;
            this.AssignLicense.Text = "Assign License";
            this.AssignLicense.UseVisualStyleBackColor = true;
            this.AssignLicense.Click += new System.EventHandler(this.AssignLicense_Click);
            // 
            // ManageLicense
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(825, 346);
            this.Controls.Add(this.ManageUsersSplitter);
            this.Name = "ManageLicense";
            this.Text = "Manage Licenses";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ManageLicense_Load);
            ((System.ComponentModel.ISupportInitialize)(this.UserLicenseList)).EndInit();
            this.ManageUsersSplitter.Panel1.ResumeLayout(false);
            this.ManageUsersSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ManageUsersSplitter)).EndInit();
            this.ManageUsersSplitter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView UserLicenseList;
        private System.Windows.Forms.SplitContainer ManageUsersSplitter;
        private System.Windows.Forms.Button AssignLicense;
        private System.Windows.Forms.Button RemoveLicense;
        private System.Windows.Forms.DataGridViewTextBoxColumn LicenseId;
        private System.Windows.Forms.DataGridViewTextBoxColumn LicenseName;
    }
}