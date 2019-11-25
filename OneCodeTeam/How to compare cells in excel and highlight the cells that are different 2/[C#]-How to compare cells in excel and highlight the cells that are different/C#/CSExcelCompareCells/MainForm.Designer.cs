namespace CSExcelCompareCells
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.txbExcelPath = new System.Windows.Forms.TextBox();
            this.btnSelect = new System.Windows.Forms.Button();
            this.grbCompareCol = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txbSourceCol = new System.Windows.Forms.TextBox();
            this.txbTargetCol = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.grbCompareSheets = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txbTargetSheet = new System.Windows.Forms.TextBox();
            this.txbSourceSheet = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCompareCol = new System.Windows.Forms.Button();
            this.btnCompareSheet = new System.Windows.Forms.Button();
            this.grbCompareCol.SuspendLayout();
            this.grbCompareSheets.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Source Excel File:";
            // 
            // txbExcelPath
            // 
            this.txbExcelPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbExcelPath.Location = new System.Drawing.Point(116, 10);
            this.txbExcelPath.Name = "txbExcelPath";
            this.txbExcelPath.Size = new System.Drawing.Size(475, 20);
            this.txbExcelPath.TabIndex = 1;
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelect.Location = new System.Drawing.Point(597, 8);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 2;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // grbCompareCol
            // 
            this.grbCompareCol.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grbCompareCol.Controls.Add(this.btnCompareCol);
            this.grbCompareCol.Controls.Add(this.label5);
            this.grbCompareCol.Controls.Add(this.txbTargetCol);
            this.grbCompareCol.Controls.Add(this.txbSourceCol);
            this.grbCompareCol.Controls.Add(this.label4);
            this.grbCompareCol.Location = new System.Drawing.Point(12, 49);
            this.grbCompareCol.Name = "grbCompareCol";
            this.grbCompareCol.Size = new System.Drawing.Size(321, 128);
            this.grbCompareCol.TabIndex = 4;
            this.grbCompareCol.TabStop = false;
            this.grbCompareCol.Text = "Comprare Columns";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Source Column:";
            // 
            // txbSourceCol
            // 
            this.txbSourceCol.Location = new System.Drawing.Point(94, 25);
            this.txbSourceCol.Name = "txbSourceCol";
            this.txbSourceCol.Size = new System.Drawing.Size(60, 20);
            this.txbSourceCol.TabIndex = 2;
            // 
            // txbTargetCol
            // 
            this.txbTargetCol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txbTargetCol.Location = new System.Drawing.Point(242, 25);
            this.txbTargetCol.Name = "txbTargetCol";
            this.txbTargetCol.Size = new System.Drawing.Size(60, 20);
            this.txbTargetCol.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(160, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Target Column:";
            // 
            // grbCompareSheets
            // 
            this.grbCompareSheets.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbCompareSheets.Controls.Add(this.btnCompareSheet);
            this.grbCompareSheets.Controls.Add(this.label2);
            this.grbCompareSheets.Controls.Add(this.txbTargetSheet);
            this.grbCompareSheets.Controls.Add(this.txbSourceSheet);
            this.grbCompareSheets.Controls.Add(this.label3);
            this.grbCompareSheets.Location = new System.Drawing.Point(339, 49);
            this.grbCompareSheets.Name = "grbCompareSheets";
            this.grbCompareSheets.Size = new System.Drawing.Size(333, 128);
            this.grbCompareSheets.TabIndex = 5;
            this.grbCompareSheets.TabStop = false;
            this.grbCompareSheets.Text = "Comprare Sheets";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(153, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Target Sheet:";
            // 
            // txbTargetSheet
            // 
            this.txbTargetSheet.Location = new System.Drawing.Point(231, 25);
            this.txbTargetSheet.Name = "txbTargetSheet";
            this.txbTargetSheet.Size = new System.Drawing.Size(56, 20);
            this.txbTargetSheet.TabIndex = 3;
            // 
            // txbSourceSheet
            // 
            this.txbSourceSheet.Location = new System.Drawing.Point(87, 25);
            this.txbSourceSheet.Name = "txbSourceSheet";
            this.txbSourceSheet.Size = new System.Drawing.Size(60, 20);
            this.txbSourceSheet.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Source Sheet:";
            // 
            // btnCompareCol
            // 
            this.btnCompareCol.Location = new System.Drawing.Point(9, 58);
            this.btnCompareCol.Name = "btnCompareCol";
            this.btnCompareCol.Size = new System.Drawing.Size(145, 23);
            this.btnCompareCol.TabIndex = 5;
            this.btnCompareCol.Text = "Compare Columns";
            this.btnCompareCol.UseVisualStyleBackColor = true;
            this.btnCompareCol.Click += new System.EventHandler(this.btnCompareCol_Click);
            // 
            // btnCompareSheet
            // 
            this.btnCompareSheet.Location = new System.Drawing.Point(9, 58);
            this.btnCompareSheet.Name = "btnCompareSheet";
            this.btnCompareSheet.Size = new System.Drawing.Size(138, 23);
            this.btnCompareSheet.TabIndex = 5;
            this.btnCompareSheet.Text = "Compare Sheets";
            this.btnCompareSheet.UseVisualStyleBackColor = true;
            this.btnCompareSheet.Click += new System.EventHandler(this.btnCompareSheet_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 189);
            this.Controls.Add(this.grbCompareSheets);
            this.Controls.Add(this.grbCompareCol);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.txbExcelPath);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(670, 190);
            this.Name = "MainForm";
            this.Text = "Compare Cells";
            this.grbCompareCol.ResumeLayout(false);
            this.grbCompareCol.PerformLayout();
            this.grbCompareSheets.ResumeLayout(false);
            this.grbCompareSheets.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txbExcelPath;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.GroupBox grbCompareCol;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txbTargetCol;
        private System.Windows.Forms.TextBox txbSourceCol;
        private System.Windows.Forms.GroupBox grbCompareSheets;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txbTargetSheet;
        private System.Windows.Forms.TextBox txbSourceSheet;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCompareCol;
        private System.Windows.Forms.Button btnCompareSheet;
    }
}

