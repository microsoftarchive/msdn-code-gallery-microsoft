namespace CSEFDeepCloneObject
{
    partial class EmployeeList
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
            this.employeeGridView = new System.Windows.Forms.DataGridView();
            this.btnCreate = new System.Windows.Forms.Button();
            this.lblEmployee = new System.Windows.Forms.Label();
            this.lblEmpAddress = new System.Windows.Forms.Label();
            this.empAddressGridView = new System.Windows.Forms.DataGridView();
            this.btnSales = new System.Windows.Forms.Button();
            this.lblBasicSalesInfo = new System.Windows.Forms.Label();
            this.bsInfoGridView = new System.Windows.Forms.DataGridView();
            this.lblDsInfo = new System.Windows.Forms.Label();
            this.dsInfoGridView = new System.Windows.Forms.DataGridView();
            this.btnClone = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.employeeGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.empAddressGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsInfoGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsInfoGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // employeeGridView
            // 
            this.employeeGridView.AllowDrop = true;
            this.employeeGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.employeeGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.employeeGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.employeeGridView.Location = new System.Drawing.Point(29, 22);
            this.employeeGridView.MultiSelect = false;
            this.employeeGridView.Name = "employeeGridView";
            this.employeeGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.employeeGridView.Size = new System.Drawing.Size(554, 141);
            this.employeeGridView.TabIndex = 0;
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(28, 169);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(75, 23);
            this.btnCreate.TabIndex = 1;
            this.btnCreate.Text = "Create";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // lblEmployee
            // 
            this.lblEmployee.AutoSize = true;
            this.lblEmployee.Location = new System.Drawing.Point(26, 6);
            this.lblEmployee.Name = "lblEmployee";
            this.lblEmployee.Size = new System.Drawing.Size(53, 13);
            this.lblEmployee.TabIndex = 2;
            this.lblEmployee.Text = "Employee";
            // 
            // lblEmpAddress
            // 
            this.lblEmpAddress.AutoSize = true;
            this.lblEmpAddress.Location = new System.Drawing.Point(26, 195);
            this.lblEmpAddress.Name = "lblEmpAddress";
            this.lblEmpAddress.Size = new System.Drawing.Size(66, 13);
            this.lblEmpAddress.TabIndex = 3;
            this.lblEmpAddress.Text = "EmpAddress";
            // 
            // empAddressGridView
            // 
            this.empAddressGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.empAddressGridView.Location = new System.Drawing.Point(29, 211);
            this.empAddressGridView.Name = "empAddressGridView";
            this.empAddressGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.empAddressGridView.Size = new System.Drawing.Size(554, 106);
            this.empAddressGridView.TabIndex = 4;
            // 
            // btnSales
            // 
            this.btnSales.Location = new System.Drawing.Point(124, 169);
            this.btnSales.Name = "btnSales";
            this.btnSales.Size = new System.Drawing.Size(102, 23);
            this.btnSales.TabIndex = 6;
            this.btnSales.Text = "CreateSalesInfo";
            this.btnSales.UseVisualStyleBackColor = true;
            this.btnSales.Click += new System.EventHandler(this.btnSales_Click);
            // 
            // lblBasicSalesInfo
            // 
            this.lblBasicSalesInfo.AutoSize = true;
            this.lblBasicSalesInfo.Location = new System.Drawing.Point(26, 320);
            this.lblBasicSalesInfo.Name = "lblBasicSalesInfo";
            this.lblBasicSalesInfo.Size = new System.Drawing.Size(77, 13);
            this.lblBasicSalesInfo.TabIndex = 7;
            this.lblBasicSalesInfo.Text = "BasicSalesInfo";
            // 
            // bsInfoGridView
            // 
            this.bsInfoGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.bsInfoGridView.Location = new System.Drawing.Point(29, 336);
            this.bsInfoGridView.Name = "bsInfoGridView";
            this.bsInfoGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.bsInfoGridView.Size = new System.Drawing.Size(554, 110);
            this.bsInfoGridView.TabIndex = 8;
            // 
            // lblDsInfo
            // 
            this.lblDsInfo.AutoSize = true;
            this.lblDsInfo.Location = new System.Drawing.Point(26, 449);
            this.lblDsInfo.Name = "lblDsInfo";
            this.lblDsInfo.Size = new System.Drawing.Size(78, 13);
            this.lblDsInfo.TabIndex = 9;
            this.lblDsInfo.Text = "DetailSalesInfo";
            // 
            // dsInfoGridView
            // 
            this.dsInfoGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dsInfoGridView.Location = new System.Drawing.Point(29, 465);
            this.dsInfoGridView.Name = "dsInfoGridView";
            this.dsInfoGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dsInfoGridView.Size = new System.Drawing.Size(554, 146);
            this.dsInfoGridView.TabIndex = 10;
            // 
            // btnClone
            // 
            this.btnClone.Location = new System.Drawing.Point(248, 169);
            this.btnClone.Name = "btnClone";
            this.btnClone.Size = new System.Drawing.Size(75, 23);
            this.btnClone.TabIndex = 11;
            this.btnClone.Text = "Clone";
            this.btnClone.UseVisualStyleBackColor = true;
            this.btnClone.Click += new System.EventHandler(this.btnClone_Click);
            // 
            // EmployeeList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(619, 623);
            this.Controls.Add(this.btnClone);
            this.Controls.Add(this.dsInfoGridView);
            this.Controls.Add(this.lblDsInfo);
            this.Controls.Add(this.bsInfoGridView);
            this.Controls.Add(this.lblBasicSalesInfo);
            this.Controls.Add(this.btnSales);
            this.Controls.Add(this.empAddressGridView);
            this.Controls.Add(this.lblEmpAddress);
            this.Controls.Add(this.lblEmployee);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.employeeGridView);
            this.Name = "EmployeeList";
            this.Text = "EmployeeList";
            this.Load += new System.EventHandler(this.EmployeeList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.employeeGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.empAddressGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsInfoGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsInfoGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView employeeGridView;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Label lblEmployee;
        private System.Windows.Forms.Label lblEmpAddress;
        private System.Windows.Forms.DataGridView empAddressGridView;
        private System.Windows.Forms.Button btnSales;
        private System.Windows.Forms.Label lblBasicSalesInfo;
        private System.Windows.Forms.DataGridView bsInfoGridView;
        private System.Windows.Forms.Label lblDsInfo;
        private System.Windows.Forms.DataGridView dsInfoGridView;
        private System.Windows.Forms.Button btnClone;
    }
}

