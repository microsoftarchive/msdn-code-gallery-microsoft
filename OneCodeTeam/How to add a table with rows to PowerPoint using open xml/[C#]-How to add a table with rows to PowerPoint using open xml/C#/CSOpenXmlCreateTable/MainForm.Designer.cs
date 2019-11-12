namespace CSOpenXmlCreateTable
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
            this.lblSource = new System.Windows.Forms.Label();
            this.txbSource = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnInserTable = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblSource
            // 
            this.lblSource.AutoSize = true;
            this.lblSource.Location = new System.Drawing.Point(12, 44);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(101, 13);
            this.lblSource.TabIndex = 0;
            this.lblSource.Text = "Source PowerPoint:";
            // 
            // txbSource
            // 
            this.txbSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txbSource.Location = new System.Drawing.Point(119, 42);
            this.txbSource.Name = "txbSource";
            this.txbSource.Size = new System.Drawing.Size(137, 20);
            this.txbSource.TabIndex = 1;
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpen.Location = new System.Drawing.Point(262, 39);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(75, 23);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnInserTable
            // 
            this.btnInserTable.Location = new System.Drawing.Point(15, 78);
            this.btnInserTable.Name = "btnInserTable";
            this.btnInserTable.Size = new System.Drawing.Size(75, 23);
            this.btnInserTable.TabIndex = 3;
            this.btnInserTable.Text = "Insert Table";
            this.btnInserTable.UseVisualStyleBackColor = true;
            this.btnInserTable.Click += new System.EventHandler(this.btnInserTable_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 243);
            this.Controls.Add(this.btnInserTable);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.txbSource);
            this.Controls.Add(this.lblSource);
            this.MinimumSize = new System.Drawing.Size(300, 200);
            this.Name = "MainForm";
            this.Text = "Create Table in PowerPoint";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSource;
        private System.Windows.Forms.TextBox txbSource;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnInserTable;
    }
}

