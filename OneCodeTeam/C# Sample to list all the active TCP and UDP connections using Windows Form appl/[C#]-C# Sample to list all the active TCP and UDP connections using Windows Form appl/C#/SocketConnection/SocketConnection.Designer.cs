namespace SocketConnection
{
    partial class SocketConnectionForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SocketConnectionForm));
            this.gdvSocketConnections = new System.Windows.Forms.DataGridView();
            this.tmrDataRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.tsActionPanel = new System.Windows.Forms.ToolStrip();
            this.tscProtocolType = new System.Windows.Forms.ToolStripComboBox();
            this.tssActionSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.tsbStartCapture = new System.Windows.Forms.ToolStripButton();
            this.tsbStopCapture = new System.Windows.Forms.ToolStripButton();
            this.tslTotalRecords = new System.Windows.Forms.ToolStripLabel();
            this.tsbCopyData = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.gdvSocketConnections)).BeginInit();
            this.tsActionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // gdvSocketConnections
            // 
            this.gdvSocketConnections.AllowUserToAddRows = false;
            this.gdvSocketConnections.AllowUserToDeleteRows = false;
            this.gdvSocketConnections.AllowUserToOrderColumns = true;
            this.gdvSocketConnections.AllowUserToResizeColumns = false;
            this.gdvSocketConnections.AllowUserToResizeRows = false;
            this.gdvSocketConnections.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gdvSocketConnections.BackgroundColor = System.Drawing.SystemColors.Control;
            this.gdvSocketConnections.BorderStyle = System.Windows.Forms.BorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gdvSocketConnections.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gdvSocketConnections.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightGoldenrodYellow;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gdvSocketConnections.DefaultCellStyle = dataGridViewCellStyle2;
            this.gdvSocketConnections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gdvSocketConnections.Location = new System.Drawing.Point(0, 0);
            this.gdvSocketConnections.Name = "gdvSocketConnections";
            this.gdvSocketConnections.ReadOnly = true;
            this.gdvSocketConnections.RowHeadersVisible = false;
            this.gdvSocketConnections.Size = new System.Drawing.Size(708, 486);
            this.gdvSocketConnections.TabIndex = 0;
            this.gdvSocketConnections.SelectionChanged += new System.EventHandler(this.gdvConnections_SelectionChanged);
            // 
            // tmrDataRefreshTimer
            // 
            this.tmrDataRefreshTimer.Interval = 1;
            this.tmrDataRefreshTimer.Tick += new System.EventHandler(this.tmrDataRefreshTimer_Tick);
            // 
            // tsActionPanel
            // 
            this.tsActionPanel.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsActionPanel.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tscProtocolType,
            this.tssActionSeparator,
            this.tsbStartCapture,
            this.tsbStopCapture,
            this.tslTotalRecords,
            this.tsbCopyData});
            this.tsActionPanel.Location = new System.Drawing.Point(0, 0);
            this.tsActionPanel.Name = "tsActionPanel";
            this.tsActionPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsActionPanel.Size = new System.Drawing.Size(708, 36);
            this.tsActionPanel.TabIndex = 5;
            // 
            // tscProtocolType
            // 
            this.tscProtocolType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tscProtocolType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.tscProtocolType.Items.AddRange(new object[] {
            "TCP",
            "UDP"});
            this.tscProtocolType.Name = "tscProtocolType";
            this.tscProtocolType.Size = new System.Drawing.Size(121, 36);
            this.tscProtocolType.ToolTipText = "Select protocol";
            this.tscProtocolType.SelectedIndexChanged += new System.EventHandler(this.tscProtocolType_SelectedIndexChanged);
            // 
            // tssActionSeparator
            // 
            this.tssActionSeparator.Name = "tssActionSeparator";
            this.tssActionSeparator.Size = new System.Drawing.Size(6, 36);
            // 
            // tsbStartCapture
            // 
            this.tsbStartCapture.AutoSize = false;
            this.tsbStartCapture.BackColor = System.Drawing.SystemColors.Control;
            this.tsbStartCapture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStartCapture.Image = ((System.Drawing.Image)(resources.GetObject("tsbStartCapture.Image")));
            this.tsbStartCapture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStartCapture.Name = "tsbStartCapture";
            this.tsbStartCapture.Size = new System.Drawing.Size(28, 33);
            this.tsbStartCapture.ToolTipText = "Start Capture";
            this.tsbStartCapture.Click += new System.EventHandler(this.tsbStartCapture_Click);
            // 
            // tsbStopCapture
            // 
            this.tsbStopCapture.AutoSize = false;
            this.tsbStopCapture.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbStopCapture.Image = global::SocketConnection.Properties.Resources.StopCapture;
            this.tsbStopCapture.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbStopCapture.Name = "tsbStopCapture";
            this.tsbStopCapture.Size = new System.Drawing.Size(28, 33);
            this.tsbStopCapture.ToolTipText = "Stop Capture";
            this.tsbStopCapture.Click += new System.EventHandler(this.tsbStopCapture_Click);
            // 
            // tslTotalRecords
            // 
            this.tslTotalRecords.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tslTotalRecords.Name = "tslTotalRecords";
            this.tslTotalRecords.Size = new System.Drawing.Size(0, 33);
            // 
            // tsbCopyData
            // 
            this.tsbCopyData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbCopyData.Image = ((System.Drawing.Image)(resources.GetObject("tsbCopyData.Image")));
            this.tsbCopyData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCopyData.Name = "tsbCopyData";
            this.tsbCopyData.Size = new System.Drawing.Size(23, 33);
            this.tsbCopyData.Text = "&Copy";
            this.tsbCopyData.ToolTipText = "Copy data in clipboard";
            this.tsbCopyData.Click += new System.EventHandler(this.tsbCopyData_Click);
            // 
            // SocketConnectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(708, 486);
            this.Controls.Add(this.tsActionPanel);
            this.Controls.Add(this.gdvSocketConnections);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SocketConnectionForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Socket Connection";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.SocketConnection_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gdvSocketConnections)).EndInit();
            this.tsActionPanel.ResumeLayout(false);
            this.tsActionPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView gdvSocketConnections;
        private System.Windows.Forms.Timer tmrDataRefreshTimer;
        private System.Windows.Forms.ToolStrip tsActionPanel;
        private System.Windows.Forms.ToolStripComboBox tscProtocolType;
        private System.Windows.Forms.ToolStripSeparator tssActionSeparator;
        private System.Windows.Forms.ToolStripButton tsbStartCapture;
        private System.Windows.Forms.ToolStripButton tsbStopCapture;
        private System.Windows.Forms.ToolStripLabel tslTotalRecords;
        private System.Windows.Forms.ToolStripButton tsbCopyData;


    }
}

