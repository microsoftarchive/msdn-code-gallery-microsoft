namespace O365_ManageUsers
{
    partial class ManageUsers
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
            this.UserList = new System.Windows.Forms.DataGridView();
            this.colDisplayName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFirstName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colLastName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUserPrincipalName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDepartment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCountry = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEdit = new System.Windows.Forms.DataGridViewButtonColumn();
            this.ManageUsersSplitter = new System.Windows.Forms.SplitContainer();
            this.AddUser = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.UserList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ManageUsersSplitter)).BeginInit();
            this.ManageUsersSplitter.Panel1.SuspendLayout();
            this.ManageUsersSplitter.Panel2.SuspendLayout();
            this.ManageUsersSplitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // UserList
            // 
            this.UserList.AllowUserToAddRows = false;
            this.UserList.AllowUserToDeleteRows = false;
            this.UserList.AllowUserToResizeRows = false;
            this.UserList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.UserList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.UserList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDisplayName,
            this.colFirstName,
            this.colLastName,
            this.colUserPrincipalName,
            this.colDepartment,
            this.colCountry,
            this.colEdit});
            this.UserList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UserList.Location = new System.Drawing.Point(0, 0);
            this.UserList.MultiSelect = false;
            this.UserList.Name = "UserList";
            this.UserList.ReadOnly = true;
            this.UserList.RowHeadersVisible = false;
            this.UserList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.UserList.Size = new System.Drawing.Size(825, 275);
            this.UserList.TabIndex = 1;
            this.UserList.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.UserList_CellClick);
            // 
            // colDisplayName
            // 
            this.colDisplayName.DataPropertyName = "DisplayName";
            this.colDisplayName.FillWeight = 10F;
            this.colDisplayName.HeaderText = "DisplayName";
            this.colDisplayName.Name = "colDisplayName";
            this.colDisplayName.ReadOnly = true;
            // 
            // colFirstName
            // 
            this.colFirstName.DataPropertyName = "FirstName";
            this.colFirstName.FillWeight = 10F;
            this.colFirstName.HeaderText = "First Name";
            this.colFirstName.Name = "colFirstName";
            this.colFirstName.ReadOnly = true;
            // 
            // colLastName
            // 
            this.colLastName.DataPropertyName = "LastName";
            this.colLastName.FillWeight = 10F;
            this.colLastName.HeaderText = "Last Name";
            this.colLastName.Name = "colLastName";
            this.colLastName.ReadOnly = true;
            // 
            // colUserPrincipalName
            // 
            this.colUserPrincipalName.DataPropertyName = "UserPrincipalName";
            this.colUserPrincipalName.FillWeight = 40F;
            this.colUserPrincipalName.HeaderText = "UserPrincipanlName";
            this.colUserPrincipalName.Name = "colUserPrincipalName";
            this.colUserPrincipalName.ReadOnly = true;
            // 
            // colDepartment
            // 
            this.colDepartment.DataPropertyName = "Department";
            this.colDepartment.FillWeight = 10F;
            this.colDepartment.HeaderText = "Department";
            this.colDepartment.Name = "colDepartment";
            this.colDepartment.ReadOnly = true;
            // 
            // colCountry
            // 
            this.colCountry.DataPropertyName = "Country";
            this.colCountry.FillWeight = 10F;
            this.colCountry.HeaderText = "Country";
            this.colCountry.Name = "colCountry";
            this.colCountry.ReadOnly = true;
            // 
            // colEdit
            // 
            this.colEdit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colEdit.FillWeight = 10F;
            this.colEdit.HeaderText = "Edit";
            this.colEdit.MinimumWidth = 70;
            this.colEdit.Name = "colEdit";
            this.colEdit.ReadOnly = true;
            this.colEdit.Text = "Edit";
            this.colEdit.UseColumnTextForButtonValue = true;
            this.colEdit.Width = 70;
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
            this.ManageUsersSplitter.Panel1.Controls.Add(this.UserList);
            // 
            // ManageUsersSplitter.Panel2
            // 
            this.ManageUsersSplitter.Panel2.Controls.Add(this.AddUser);
            this.ManageUsersSplitter.Size = new System.Drawing.Size(825, 346);
            this.ManageUsersSplitter.SplitterDistance = 275;
            this.ManageUsersSplitter.TabIndex = 2;
            // 
            // AddUser
            // 
            this.AddUser.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AddUser.Location = new System.Drawing.Point(344, 12);
            this.AddUser.Name = "AddUser";
            this.AddUser.Size = new System.Drawing.Size(136, 42);
            this.AddUser.TabIndex = 0;
            this.AddUser.Text = "Add User";
            this.AddUser.UseVisualStyleBackColor = true;
            this.AddUser.Click += new System.EventHandler(this.AddUser_Click);
            // 
            // ManageUsers
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(825, 346);
            this.Controls.Add(this.ManageUsersSplitter);
            this.Name = "ManageUsers";
            this.Text = "Manage Users";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ManageUsers_Load);
            ((System.ComponentModel.ISupportInitialize)(this.UserList)).EndInit();
            this.ManageUsersSplitter.Panel1.ResumeLayout(false);
            this.ManageUsersSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ManageUsersSplitter)).EndInit();
            this.ManageUsersSplitter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView UserList;
        private System.Windows.Forms.SplitContainer ManageUsersSplitter;
        private System.Windows.Forms.Button AddUser;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisplayName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFirstName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colLastName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUserPrincipalName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDepartment;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCountry;
        private System.Windows.Forms.DataGridViewButtonColumn colEdit;
    }
}