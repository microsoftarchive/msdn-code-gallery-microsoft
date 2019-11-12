namespace CSMonitorRegistryChange
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
            this.lbRegKeyPath = new System.Windows.Forms.Label();
            this.tbRegkeyPath = new System.Windows.Forms.TextBox();
            this.btnMonitor = new System.Windows.Forms.Button();
            this.lstChanges = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmbHives = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbRegKeyPath
            // 
            this.lbRegKeyPath.AutoSize = true;
            this.lbRegKeyPath.Location = new System.Drawing.Point(11, 21);
            this.lbRegKeyPath.Name = "lbRegKeyPath";
            this.lbRegKeyPath.Size = new System.Drawing.Size(91, 13);
            this.lbRegKeyPath.TabIndex = 0;
            this.lbRegKeyPath.Text = "Registry Key Path";
            // 
            // tbRegkeyPath
            // 
            this.tbRegkeyPath.Location = new System.Drawing.Point(264, 18);
            this.tbRegkeyPath.Name = "tbRegkeyPath";
            this.tbRegkeyPath.Size = new System.Drawing.Size(491, 20);
            this.tbRegkeyPath.TabIndex = 1;
            this.tbRegkeyPath.Text = "SOFTWARE\\\\Microsoft";
            // 
            // btnMonitor
            // 
            this.btnMonitor.Location = new System.Drawing.Point(761, 16);
            this.btnMonitor.Name = "btnMonitor";
            this.btnMonitor.Size = new System.Drawing.Size(78, 23);
            this.btnMonitor.TabIndex = 2;
            this.btnMonitor.Text = "Start Monitor";
            this.btnMonitor.UseVisualStyleBackColor = true;
            this.btnMonitor.Click += new System.EventHandler(this.btnMonitor_Click);
            // 
            // lstChanges
            // 
            this.lstChanges.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstChanges.FormattingEnabled = true;
            this.lstChanges.Location = new System.Drawing.Point(0, 0);
            this.lstChanges.Name = "lstChanges";
            this.lstChanges.Size = new System.Drawing.Size(851, 348);
            this.lstChanges.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmbHives);
            this.panel1.Controls.Add(this.lbRegKeyPath);
            this.panel1.Controls.Add(this.tbRegkeyPath);
            this.panel1.Controls.Add(this.btnMonitor);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(851, 43);
            this.panel1.TabIndex = 4;
            // 
            // cmbHives
            // 
            this.cmbHives.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHives.FormattingEnabled = true;
            this.cmbHives.Location = new System.Drawing.Point(108, 17);
            this.cmbHives.Name = "cmbHives";
            this.cmbHives.Size = new System.Drawing.Size(150, 21);
            this.cmbHives.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lstChanges);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 43);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(851, 348);
            this.panel2.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(851, 391);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(867, 429);
            this.Name = "MainForm";
            this.Text = "Registry Monitor";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lbRegKeyPath;
        private System.Windows.Forms.TextBox tbRegkeyPath;
        private System.Windows.Forms.Button btnMonitor;
        private System.Windows.Forms.ListBox lstChanges;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ComboBox cmbHives;
    }
}

