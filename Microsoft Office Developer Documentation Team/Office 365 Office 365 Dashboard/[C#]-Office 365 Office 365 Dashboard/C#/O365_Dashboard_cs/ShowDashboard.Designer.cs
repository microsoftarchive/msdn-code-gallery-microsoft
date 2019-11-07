namespace O365_Dashboard
{
    partial class ShowDashboard
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title3 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.DashboardMainTab = new System.Windows.Forms.TabControl();
            this.LyncSection = new System.Windows.Forms.TabPage();
            this.UserList = new System.Windows.Forms.DataGridView();
            this.colColorStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDisplayName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUserPrincipalName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OutlookSection = new System.Windows.Forms.TabPage();
            this.MailCountChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.MailboxUsageGroup = new System.Windows.Forms.GroupBox();
            this.TotalSizeHeading = new System.Windows.Forms.Label();
            this.SummaryInfo = new System.Windows.Forms.Label();
            this.TotalSizeInfo = new System.Windows.Forms.Label();
            this.UsedSizeInfo = new System.Windows.Forms.Label();
            this.UsedSizeHeading = new System.Windows.Forms.Label();
            this.DashboardMainTab.SuspendLayout();
            this.LyncSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UserList)).BeginInit();
            this.OutlookSection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MailCountChart)).BeginInit();
            this.MailboxUsageGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // DashboardMainTab
            // 
            this.DashboardMainTab.Controls.Add(this.LyncSection);
            this.DashboardMainTab.Controls.Add(this.OutlookSection);
            this.DashboardMainTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DashboardMainTab.Location = new System.Drawing.Point(0, 0);
            this.DashboardMainTab.Name = "DashboardMainTab";
            this.DashboardMainTab.SelectedIndex = 0;
            this.DashboardMainTab.Size = new System.Drawing.Size(765, 367);
            this.DashboardMainTab.TabIndex = 9;
            this.DashboardMainTab.SelectedIndexChanged += new System.EventHandler(this.DashboardMainTab_SelectedIndexChanged);
            // 
            // LyncSection
            // 
            this.LyncSection.Controls.Add(this.UserList);
            this.LyncSection.Location = new System.Drawing.Point(4, 22);
            this.LyncSection.Name = "LyncSection";
            this.LyncSection.Padding = new System.Windows.Forms.Padding(3);
            this.LyncSection.Size = new System.Drawing.Size(757, 341);
            this.LyncSection.TabIndex = 0;
            this.LyncSection.Text = "Online Users";
            this.LyncSection.UseVisualStyleBackColor = true;
            // 
            // UserList
            // 
            this.UserList.AllowUserToAddRows = false;
            this.UserList.AllowUserToDeleteRows = false;
            this.UserList.AllowUserToResizeRows = false;
            this.UserList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.UserList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.UserList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.UserList.ColumnHeadersVisible = false;
            this.UserList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colColorStatus,
            this.colStatus,
            this.colDisplayName,
            this.colUserPrincipalName});
            this.UserList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.UserList.Location = new System.Drawing.Point(3, 3);
            this.UserList.MultiSelect = false;
            this.UserList.Name = "UserList";
            this.UserList.ReadOnly = true;
            this.UserList.RowHeadersVisible = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.UserList.RowsDefaultCellStyle = dataGridViewCellStyle3;
            this.UserList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.UserList.Size = new System.Drawing.Size(751, 335);
            this.UserList.TabIndex = 2;
            // 
            // colColorStatus
            // 
            this.colColorStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colColorStatus.FillWeight = 20F;
            this.colColorStatus.HeaderText = "";
            this.colColorStatus.Name = "colColorStatus";
            this.colColorStatus.ReadOnly = true;
            this.colColorStatus.Width = 30;
            // 
            // colStatus
            // 
            this.colStatus.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.colStatus.DataPropertyName = "UserStatus";
            this.colStatus.FillWeight = 2.538071F;
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Visible = false;
            this.colStatus.Width = 5;
            // 
            // colDisplayName
            // 
            this.colDisplayName.DataPropertyName = "DisplayName";
            this.colDisplayName.FillWeight = 16.76647F;
            this.colDisplayName.HeaderText = "Display Name";
            this.colDisplayName.Name = "colDisplayName";
            this.colDisplayName.ReadOnly = true;
            // 
            // colUserPrincipalName
            // 
            this.colUserPrincipalName.DataPropertyName = "UserPrincipalName";
            this.colUserPrincipalName.FillWeight = 63.23354F;
            this.colUserPrincipalName.HeaderText = "User Principal Name";
            this.colUserPrincipalName.Name = "colUserPrincipalName";
            this.colUserPrincipalName.ReadOnly = true;
            // 
            // OutlookSection
            // 
            this.OutlookSection.Controls.Add(this.MailCountChart);
            this.OutlookSection.Controls.Add(this.MailboxUsageGroup);
            this.OutlookSection.Location = new System.Drawing.Point(4, 22);
            this.OutlookSection.Name = "OutlookSection";
            this.OutlookSection.Padding = new System.Windows.Forms.Padding(3);
            this.OutlookSection.Size = new System.Drawing.Size(757, 341);
            this.OutlookSection.TabIndex = 1;
            this.OutlookSection.Text = "Outlook Details";
            this.OutlookSection.UseVisualStyleBackColor = true;
            this.OutlookSection.Paint += new System.Windows.Forms.PaintEventHandler(this.OutlookSection_Paint);
            // 
            // MailCountChart
            // 
            this.MailCountChart.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.MailCountChart.BackSecondaryColor = System.Drawing.Color.White;
            this.MailCountChart.BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
            this.MailCountChart.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.MailCountChart.BorderlineWidth = 2;
            this.MailCountChart.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
            chartArea3.Area3DStyle.Rotation = 0;
            chartArea3.AxisX.IsLabelAutoFit = false;
            chartArea3.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chartArea3.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea3.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea3.AxisY.IsLabelAutoFit = false;
            chartArea3.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chartArea3.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea3.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea3.BackColor = System.Drawing.Color.Transparent;
            chartArea3.BackSecondaryColor = System.Drawing.Color.Transparent;
            chartArea3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea3.BorderWidth = 0;
            chartArea3.Name = "ChartArea1";
            chartArea3.ShadowColor = System.Drawing.SystemColors.Control;
            this.MailCountChart.ChartAreas.Add(chartArea3);
            legend3.Alignment = System.Drawing.StringAlignment.Center;
            legend3.BackColor = System.Drawing.Color.Transparent;
            legend3.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Bottom;
            legend3.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            legend3.IsTextAutoFit = false;
            legend3.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Row;
            legend3.Name = "Legend1";
            this.MailCountChart.Legends.Add(legend3);
            this.MailCountChart.Location = new System.Drawing.Point(8, 10);
            this.MailCountChart.Name = "MailCountChart";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
            series3.Legend = "Legend1";
            series3.Name = "srsMailCount";
            this.MailCountChart.Series.Add(series3);
            this.MailCountChart.Size = new System.Drawing.Size(300, 300);
            this.MailCountChart.TabIndex = 8;
            this.MailCountChart.Text = "Outlook Emails";
            title3.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Bold);
            title3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
            title3.Name = "Title1";
            title3.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            title3.ShadowOffset = 3;
            title3.Text = "Outlook Emails";
            this.MailCountChart.Titles.Add(title3);
            // 
            // MailboxUsageGroup
            // 
            this.MailboxUsageGroup.Controls.Add(this.TotalSizeHeading);
            this.MailboxUsageGroup.Controls.Add(this.SummaryInfo);
            this.MailboxUsageGroup.Controls.Add(this.TotalSizeInfo);
            this.MailboxUsageGroup.Controls.Add(this.UsedSizeInfo);
            this.MailboxUsageGroup.Controls.Add(this.UsedSizeHeading);
            this.MailboxUsageGroup.Location = new System.Drawing.Point(310, 10);
            this.MailboxUsageGroup.Name = "MailboxUsageGroup";
            this.MailboxUsageGroup.Size = new System.Drawing.Size(415, 300);
            this.MailboxUsageGroup.TabIndex = 7;
            this.MailboxUsageGroup.TabStop = false;
            this.MailboxUsageGroup.Text = "Mailbox Usage Statistics";
            // 
            // TotalSizeHeading
            // 
            this.TotalSizeHeading.AutoSize = true;
            this.TotalSizeHeading.Location = new System.Drawing.Point(15, 26);
            this.TotalSizeHeading.Name = "TotalSizeHeading";
            this.TotalSizeHeading.Size = new System.Drawing.Size(102, 13);
            this.TotalSizeHeading.TabIndex = 2;
            this.TotalSizeHeading.Text = "Mailbox Total Size : ";
            // 
            // SummaryInfo
            // 
            this.SummaryInfo.AutoSize = true;
            this.SummaryInfo.Location = new System.Drawing.Point(15, 74);
            this.SummaryInfo.Name = "SummaryInfo";
            this.SummaryInfo.Size = new System.Drawing.Size(0, 13);
            this.SummaryInfo.TabIndex = 6;
            // 
            // TotalSizeInfo
            // 
            this.TotalSizeInfo.AutoSize = true;
            this.TotalSizeInfo.Location = new System.Drawing.Point(124, 26);
            this.TotalSizeInfo.Name = "TotalSizeInfo";
            this.TotalSizeInfo.Size = new System.Drawing.Size(0, 13);
            this.TotalSizeInfo.TabIndex = 3;
            // 
            // UsedSizeInfo
            // 
            this.UsedSizeInfo.AutoSize = true;
            this.UsedSizeInfo.Location = new System.Drawing.Point(124, 50);
            this.UsedSizeInfo.Name = "UsedSizeInfo";
            this.UsedSizeInfo.Size = new System.Drawing.Size(0, 13);
            this.UsedSizeInfo.TabIndex = 5;
            // 
            // UsedSizeHeading
            // 
            this.UsedSizeHeading.AutoSize = true;
            this.UsedSizeHeading.Location = new System.Drawing.Point(15, 50);
            this.UsedSizeHeading.Name = "UsedSizeHeading";
            this.UsedSizeHeading.Size = new System.Drawing.Size(103, 13);
            this.UsedSizeHeading.TabIndex = 4;
            this.UsedSizeHeading.Text = "Mailbox Used Size : ";
            // 
            // ShowDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 367);
            this.Controls.Add(this.DashboardMainTab);
            this.Name = "ShowDashboard";
            this.Text = "O365 Dashboard";
            this.Load += new System.EventHandler(this.ShowDashboard_Load);
            this.DashboardMainTab.ResumeLayout(false);
            this.LyncSection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UserList)).EndInit();
            this.OutlookSection.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MailCountChart)).EndInit();
            this.MailboxUsageGroup.ResumeLayout(false);
            this.MailboxUsageGroup.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl DashboardMainTab;
        private System.Windows.Forms.TabPage LyncSection;
        private System.Windows.Forms.TabPage OutlookSection;
        private System.Windows.Forms.GroupBox MailboxUsageGroup;
        private System.Windows.Forms.Label TotalSizeHeading;
        private System.Windows.Forms.Label SummaryInfo;
        private System.Windows.Forms.Label TotalSizeInfo;
        private System.Windows.Forms.Label UsedSizeInfo;
        private System.Windows.Forms.Label UsedSizeHeading;
        private System.Windows.Forms.DataVisualization.Charting.Chart MailCountChart;
        private System.Windows.Forms.DataGridView UserList;
        private System.Windows.Forms.DataGridViewTextBoxColumn colColorStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDisplayName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUserPrincipalName;
    }
}

