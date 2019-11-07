namespace O365_SingleSignOn
{
    partial class DomainsList
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.DomainList = new System.Windows.Forms.DataGridView();
            this.ManageUsersSplitter = new System.Windows.Forms.SplitContainer();
            this.CreateDomain = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DomainList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ManageUsersSplitter)).BeginInit();
            this.ManageUsersSplitter.Panel1.SuspendLayout();
            this.ManageUsersSplitter.Panel2.SuspendLayout();
            this.ManageUsersSplitter.SuspendLayout();
            this.SuspendLayout();
            // 
            // DomainList
            // 
            this.DomainList.AllowUserToAddRows = false;
            this.DomainList.AllowUserToDeleteRows = false;
            this.DomainList.AllowUserToResizeRows = false;
            this.DomainList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DomainList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.DomainList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DomainList.DefaultCellStyle = dataGridViewCellStyle2;
            this.DomainList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DomainList.Location = new System.Drawing.Point(0, 0);
            this.DomainList.MultiSelect = false;
            this.DomainList.Name = "DomainList";
            this.DomainList.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.DomainList.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.DomainList.RowHeadersVisible = false;
            this.DomainList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DomainList.Size = new System.Drawing.Size(779, 278);
            this.DomainList.TabIndex = 1;
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
            this.ManageUsersSplitter.Panel1.Controls.Add(this.DomainList);
            // 
            // ManageUsersSplitter.Panel2
            // 
            this.ManageUsersSplitter.Panel2.Controls.Add(this.CreateDomain);
            this.ManageUsersSplitter.Size = new System.Drawing.Size(779, 349);
            this.ManageUsersSplitter.SplitterDistance = 278;
            this.ManageUsersSplitter.TabIndex = 3;
            // 
            // CreateDomain
            // 
            this.CreateDomain.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CreateDomain.Location = new System.Drawing.Point(321, 12);
            this.CreateDomain.Name = "CreateDomain";
            this.CreateDomain.Size = new System.Drawing.Size(136, 42);
            this.CreateDomain.TabIndex = 0;
            this.CreateDomain.Text = "Create Domain";
            this.CreateDomain.UseVisualStyleBackColor = true;
            this.CreateDomain.Click += new System.EventHandler(this.CreateDomain_Click);
            // 
            // DomainsList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 349);
            this.Controls.Add(this.ManageUsersSplitter);
            this.Name = "DomainsList";
            this.Text = "Domains List";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.DomainsList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.DomainList)).EndInit();
            this.ManageUsersSplitter.Panel1.ResumeLayout(false);
            this.ManageUsersSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ManageUsersSplitter)).EndInit();
            this.ManageUsersSplitter.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView DomainList;
        private System.Windows.Forms.SplitContainer ManageUsersSplitter;
        private System.Windows.Forms.Button CreateDomain;
    }
}