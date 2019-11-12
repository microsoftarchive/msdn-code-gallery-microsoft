namespace CSEFAutoUpdate
{
    partial class AutoUpdate
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
            this.dgvWatch = new System.Windows.Forms.DataGridView();
            this.btnGetData = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label6 = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.proBar = new System.Windows.Forms.ProgressBar();
            this.txtInterval = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.rabtnRegUpdate = new System.Windows.Forms.RadioButton();
            this.rabtnImUpdate = new System.Windows.Forms.RadioButton();
            this.txtHighPrice = new System.Windows.Forms.TextBox();
            this.txtLowPrice = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.txtNewPrice = new System.Windows.Forms.TextBox();
            this.txtId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWatch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvWatch
            // 
            this.dgvWatch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWatch.Location = new System.Drawing.Point(12, 187);
            this.dgvWatch.Name = "dgvWatch";
            this.dgvWatch.Size = new System.Drawing.Size(443, 191);
            this.dgvWatch.TabIndex = 0;
            this.dgvWatch.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvWatch_CellMouseClick);
            // 
            // btnGetData
            // 
            this.btnGetData.Location = new System.Drawing.Point(109, 147);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(75, 23);
            this.btnGetData.TabIndex = 1;
            this.btnGetData.Text = "Get Data";
            this.btnGetData.UseVisualStyleBackColor = true;
            this.btnGetData.Click += new System.EventHandler(this.GetData_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            this.splitContainer1.Panel1.Controls.Add(this.btnStop);
            this.splitContainer1.Panel1.Controls.Add(this.proBar);
            this.splitContainer1.Panel1.Controls.Add(this.txtInterval);
            this.splitContainer1.Panel1.Controls.Add(this.label5);
            this.splitContainer1.Panel1.Controls.Add(this.rabtnRegUpdate);
            this.splitContainer1.Panel1.Controls.Add(this.rabtnImUpdate);
            this.splitContainer1.Panel1.Controls.Add(this.txtHighPrice);
            this.splitContainer1.Panel1.Controls.Add(this.txtLowPrice);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.dgvWatch);
            this.splitContainer1.Panel1.Controls.Add(this.btnGetData);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.btnUpdate);
            this.splitContainer1.Panel2.Controls.Add(this.txtNewPrice);
            this.splitContainer1.Panel2.Controls.Add(this.txtId);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(740, 414);
            this.splitContainer1.SplitterDistance = 463;
            this.splitContainer1.TabIndex = 4;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Enabled = false;
            this.label6.Location = new System.Drawing.Point(424, 52);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "(s)";
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(250, 147);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 11;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.StopSqlDependency);
            // 
            // proBar
            // 
            this.proBar.Enabled = false;
            this.proBar.Location = new System.Drawing.Point(12, 88);
            this.proBar.Name = "proBar";
            this.proBar.Size = new System.Drawing.Size(439, 23);
            this.proBar.TabIndex = 10;
            // 
            // txtInterval
            // 
            this.txtInterval.Enabled = false;
            this.txtInterval.Location = new System.Drawing.Point(376, 49);
            this.txtInterval.Name = "txtInterval";
            this.txtInterval.Size = new System.Drawing.Size(41, 20);
            this.txtInterval.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Enabled = false;
            this.label5.Location = new System.Drawing.Point(333, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Every";
            // 
            // rabtnRegUpdate
            // 
            this.rabtnRegUpdate.AutoSize = true;
            this.rabtnRegUpdate.Location = new System.Drawing.Point(337, 12);
            this.rabtnRegUpdate.Name = "rabtnRegUpdate";
            this.rabtnRegUpdate.Size = new System.Drawing.Size(107, 17);
            this.rabtnRegUpdate.TabIndex = 7;
            this.rabtnRegUpdate.Text = "Regularly Update";
            this.rabtnRegUpdate.UseVisualStyleBackColor = true;
            this.rabtnRegUpdate.CheckedChanged += new System.EventHandler(this.rabtnRegUpdate_CheckedChanged);
            // 
            // rabtnImUpdate
            // 
            this.rabtnImUpdate.AutoSize = true;
            this.rabtnImUpdate.Checked = true;
            this.rabtnImUpdate.Location = new System.Drawing.Point(201, 12);
            this.rabtnImUpdate.Name = "rabtnImUpdate";
            this.rabtnImUpdate.Size = new System.Drawing.Size(118, 17);
            this.rabtnImUpdate.TabIndex = 6;
            this.rabtnImUpdate.TabStop = true;
            this.rabtnImUpdate.Text = "Immediately Update";
            this.rabtnImUpdate.UseVisualStyleBackColor = true;
            // 
            // txtHighPrice
            // 
            this.txtHighPrice.Location = new System.Drawing.Point(84, 49);
            this.txtHighPrice.Name = "txtHighPrice";
            this.txtHighPrice.Size = new System.Drawing.Size(100, 20);
            this.txtHighPrice.TabIndex = 5;
            // 
            // txtLowPrice
            // 
            this.txtLowPrice.Location = new System.Drawing.Point(84, 13);
            this.txtLowPrice.Name = "txtLowPrice";
            this.txtLowPrice.Size = new System.Drawing.Size(100, 20);
            this.txtLowPrice.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "High Price:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Low Price:";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Enabled = false;
            this.btnUpdate.Location = new System.Drawing.Point(88, 88);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.Update_Click);
            // 
            // txtNewPrice
            // 
            this.txtNewPrice.CausesValidation = false;
            this.txtNewPrice.Enabled = false;
            this.txtNewPrice.Location = new System.Drawing.Point(129, 37);
            this.txtNewPrice.Name = "txtNewPrice";
            this.txtNewPrice.Size = new System.Drawing.Size(100, 20);
            this.txtNewPrice.TabIndex = 3;
            // 
            // txtId
            // 
            this.txtId.Enabled = false;
            this.txtId.Location = new System.Drawing.Point(129, 10);
            this.txtId.Name = "txtId";
            this.txtId.Size = new System.Drawing.Size(100, 20);
            this.txtId.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(99, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Product New Price:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Product Id:";
            // 
            // AutoUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 414);
            this.Controls.Add(this.splitContainer1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AutoUpdate";
            this.Text = "Auto Update";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AutoUpdate_FormClosed);
            this.Load += new System.EventHandler(this.AutoUpdate_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvWatch)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvWatch;
        private System.Windows.Forms.Button btnGetData;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtNewPrice;
        private System.Windows.Forms.TextBox txtId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ProgressBar proBar;
        private System.Windows.Forms.TextBox txtInterval;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton rabtnRegUpdate;
        private System.Windows.Forms.RadioButton rabtnImUpdate;
        private System.Windows.Forms.TextBox txtHighPrice;
        private System.Windows.Forms.TextBox txtLowPrice;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}

