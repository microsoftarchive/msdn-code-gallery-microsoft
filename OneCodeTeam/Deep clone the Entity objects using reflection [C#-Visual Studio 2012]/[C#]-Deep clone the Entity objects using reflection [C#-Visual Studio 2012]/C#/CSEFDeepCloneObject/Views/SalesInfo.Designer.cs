namespace CSEFDeepCloneObject
{
    partial class SalesInfo
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
            this.components = new System.ComponentModel.Container();
            this.lblEmpId = new System.Windows.Forms.Label();
            this.lblSales = new System.Windows.Forms.Label();
            this.tbxSales = new System.Windows.Forms.TextBox();
            this.lblYear = new System.Windows.Forms.Label();
            this.cbxYear = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.cbxEmpId = new System.Windows.Forms.ComboBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // lblEmpId
            // 
            this.lblEmpId.AutoSize = true;
            this.lblEmpId.Location = new System.Drawing.Point(13, 29);
            this.lblEmpId.Name = "lblEmpId";
            this.lblEmpId.Size = new System.Drawing.Size(37, 13);
            this.lblEmpId.TabIndex = 0;
            this.lblEmpId.Text = "EmpId";
            // 
            // lblSales
            // 
            this.lblSales.AutoSize = true;
            this.lblSales.Location = new System.Drawing.Point(13, 70);
            this.lblSales.Name = "lblSales";
            this.lblSales.Size = new System.Drawing.Size(33, 13);
            this.lblSales.TabIndex = 2;
            this.lblSales.Text = "Sales";
            // 
            // tbxSales
            // 
            this.tbxSales.Location = new System.Drawing.Point(57, 70);
            this.tbxSales.Name = "tbxSales";
            this.tbxSales.Size = new System.Drawing.Size(100, 20);
            this.tbxSales.TabIndex = 3;
            this.tbxSales.Validating += new System.ComponentModel.CancelEventHandler(tbxSales_Validating);
            this.tbxSales.Validated += new System.EventHandler(tbxSales_Validated);
            // 
            // lblYear
            // 
            this.lblYear.AutoSize = true;
            this.lblYear.Location = new System.Drawing.Point(13, 107);
            this.lblYear.Name = "lblYear";
            this.lblYear.Size = new System.Drawing.Size(29, 13);
            this.lblYear.TabIndex = 4;
            this.lblYear.Text = "Year";
            // 
            // cbxYear
            // 
            this.cbxYear.FormattingEnabled = true;
            this.cbxYear.Location = new System.Drawing.Point(57, 107);
            this.cbxYear.Name = "cbxYear";
            this.cbxYear.Size = new System.Drawing.Size(121, 21);
            this.cbxYear.TabIndex = 5;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(12, 144);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cbxEmpId
            // 
            this.cbxEmpId.FormattingEnabled = true;
            this.cbxEmpId.Location = new System.Drawing.Point(57, 29);
            this.cbxEmpId.Name = "cbxEmpId";
            this.cbxEmpId.Size = new System.Drawing.Size(121, 21);
            this.cbxEmpId.TabIndex = 7;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // SalesInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 197);
            this.Controls.Add(this.cbxEmpId);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.cbxYear);
            this.Controls.Add(this.lblYear);
            this.Controls.Add(this.tbxSales);
            this.Controls.Add(this.lblSales);
            this.Controls.Add(this.lblEmpId);
            this.Name = "SalesInfo";
            this.Text = "SalesInfo";
            this.Load += new System.EventHandler(this.SalesInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblEmpId;
        private System.Windows.Forms.Label lblSales;
        private System.Windows.Forms.TextBox tbxSales;
        private System.Windows.Forms.Label lblYear;
        private System.Windows.Forms.ComboBox cbxYear;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cbxEmpId;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}