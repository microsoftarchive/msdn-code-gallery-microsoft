namespace CSTaskScheduler.Views
{
    partial class ScheduledTasksView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.scheduledTasksText = new System.Windows.Forms.Label();
            this.ScheduledTasksViewer = new System.Windows.Forms.DataGridView();
            this.ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.DeleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ContainerPanel = new System.Windows.Forms.Panel();
            this.RefreshScheduledTasksAction = new System.Windows.Forms.Button();
            this.CreateTaskAction = new System.Windows.Forms.Button();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.ToolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.ErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.ScheduledTasksViewer)).BeginInit();
            this.ContextMenuStrip.SuspendLayout();
            this.ContainerPanel.SuspendLayout();
            this.StatusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // scheduledTasksText
            // 
            this.scheduledTasksText.AutoSize = true;
            this.scheduledTasksText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scheduledTasksText.Location = new System.Drawing.Point(12, 73);
            this.scheduledTasksText.Name = "scheduledTasksText";
            this.scheduledTasksText.Size = new System.Drawing.Size(109, 13);
            this.scheduledTasksText.TabIndex = 0;
            this.scheduledTasksText.Text = "Scheduled Tasks:";
            // 
            // ScheduledTasksViewer
            // 
            this.ScheduledTasksViewer.AllowUserToAddRows = false;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ScheduledTasksViewer.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.ScheduledTasksViewer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ScheduledTasksViewer.DefaultCellStyle = dataGridViewCellStyle6;
            this.ScheduledTasksViewer.GridColor = System.Drawing.SystemColors.ControlLight;
            this.ScheduledTasksViewer.Location = new System.Drawing.Point(12, 101);
            this.ScheduledTasksViewer.Name = "ScheduledTasksViewer";
            this.ScheduledTasksViewer.ReadOnly = true;
            this.ScheduledTasksViewer.RowTemplate.ContextMenuStrip = this.ContextMenuStrip;
            this.ScheduledTasksViewer.Size = new System.Drawing.Size(762, 197);
            this.ScheduledTasksViewer.TabIndex = 2;
            // 
            // ContextMenuStrip
            // 
            this.ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DeleteToolStripMenuItem});
            this.ContextMenuStrip.Name = "ContextMenuStrip";
            this.ContextMenuStrip.Size = new System.Drawing.Size(108, 26);
            // 
            // DeleteToolStripMenuItem
            // 
            this.DeleteToolStripMenuItem.Name = "DeleteToolStripMenuItem";
            this.DeleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.DeleteToolStripMenuItem.Text = "Delete";
            this.DeleteToolStripMenuItem.ToolTipText = "Delete a task";
            // 
            // ContainerPanel
            // 
            this.ContainerPanel.Controls.Add(this.RefreshScheduledTasksAction);
            this.ContainerPanel.Controls.Add(this.CreateTaskAction);
            this.ContainerPanel.Controls.Add(this.scheduledTasksText);
            this.ContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContainerPanel.Location = new System.Drawing.Point(0, 0);
            this.ContainerPanel.Name = "ContainerPanel";
            this.ContainerPanel.Size = new System.Drawing.Size(801, 626);
            this.ContainerPanel.TabIndex = 4;
            // 
            // RefreshScheduledTasksAction
            // 
            this.RefreshScheduledTasksAction.Location = new System.Drawing.Point(649, 68);
            this.RefreshScheduledTasksAction.Name = "RefreshScheduledTasksAction";
            this.RefreshScheduledTasksAction.Size = new System.Drawing.Size(125, 23);
            this.RefreshScheduledTasksAction.TabIndex = 5;
            this.RefreshScheduledTasksAction.Text = "Refresh";
            this.RefreshScheduledTasksAction.UseVisualStyleBackColor = true;
            // 
            // CreateTaskAction
            // 
            this.CreateTaskAction.Location = new System.Drawing.Point(15, 396);
            this.CreateTaskAction.Name = "CreateTaskAction";
            this.CreateTaskAction.Size = new System.Drawing.Size(125, 23);
            this.CreateTaskAction.TabIndex = 4;
            this.CreateTaskAction.Text = "Create a Task";
            this.CreateTaskAction.UseVisualStyleBackColor = true;
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(32, 19);
            // 
            // StatusStrip
            // 
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripProgressBar});
            this.StatusStrip.Location = new System.Drawing.Point(0, 604);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Size = new System.Drawing.Size(801, 22);
            this.StatusStrip.TabIndex = 5;
            this.StatusStrip.Text = "StatusStrip";
            // 
            // ToolStripProgressBar
            // 
            this.ToolStripProgressBar.Name = "ToolStripProgressBar";
            this.ToolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            this.ToolStripProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // ErrorProvider
            // 
            this.ErrorProvider.ContainerControl = this;
            // 
            // ScheduledTasksView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(801, 626);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.ScheduledTasksViewer);
            this.Controls.Add(this.ContainerPanel);
            this.MaximizeBox = false;
            this.Name = "ScheduledTasksView";
            this.Text = "ScheduledTasksView";
            ((System.ComponentModel.ISupportInitialize)(this.ScheduledTasksViewer)).EndInit();
            this.ContextMenuStrip.ResumeLayout(false);
            this.ContainerPanel.ResumeLayout(false);
            this.ContainerPanel.PerformLayout();
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label scheduledTasksText;
        private System.Windows.Forms.DataGridView ScheduledTasksViewer;
        private System.Windows.Forms.Button RefreshScheduledTasksAction;
        private System.Windows.Forms.Panel ContainerPanel;
        private new System.Windows.Forms.ContextMenuStrip ContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem DeleteToolStripMenuItem;
        private System.Windows.Forms.StatusStrip StatusStrip;
        private System.Windows.Forms.ToolStripProgressBar ToolStripProgressBar;
        private System.Windows.Forms.ErrorProvider ErrorProvider;
        private System.Windows.Forms.Button CreateTaskAction;
    }
}