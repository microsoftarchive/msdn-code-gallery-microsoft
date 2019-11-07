using System.Security.Permissions;
namespace CSDetectWindowsSessionState
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
       [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
        protected override void Dispose(bool disposing)
        {
            if (disposing && session != null)
            {
                session.Dispose();
            }

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
            this.pnlState = new System.Windows.Forms.Panel();
            this.chkEnableTimer = new System.Windows.Forms.CheckBox();
            this.lbState = new System.Windows.Forms.Label();
            this.pnlList = new System.Windows.Forms.Panel();
            this.lstRecord = new System.Windows.Forms.ListBox();
            this.pnlState.SuspendLayout();
            this.pnlList.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlState
            // 
            this.pnlState.Controls.Add(this.chkEnableTimer);
            this.pnlState.Controls.Add(this.lbState);
            this.pnlState.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlState.Location = new System.Drawing.Point(0, 0);
            this.pnlState.Name = "pnlState";
            this.pnlState.Size = new System.Drawing.Size(783, 29);
            this.pnlState.TabIndex = 0;
            // 
            // chkEnableTimer
            // 
            this.chkEnableTimer.AutoSize = true;
            this.chkEnableTimer.Location = new System.Drawing.Point(468, 7);
            this.chkEnableTimer.Name = "chkEnableTimer";
            this.chkEnableTimer.Size = new System.Drawing.Size(304, 17);
            this.chkEnableTimer.TabIndex = 1;
            this.chkEnableTimer.Text = "Enable a timer to detect the session state every 5 seconds ";
            this.chkEnableTimer.UseVisualStyleBackColor = true;
            this.chkEnableTimer.CheckedChanged += new System.EventHandler(this.chkEnableTimer_CheckedChanged);
            // 
            // lbState
            // 
            this.lbState.AutoSize = true;
            this.lbState.Location = new System.Drawing.Point(13, 8);
            this.lbState.Name = "lbState";
            this.lbState.Size = new System.Drawing.Size(69, 13);
            this.lbState.TabIndex = 0;
            this.lbState.Text = "Current State";
            // 
            // pnlList
            // 
            this.pnlList.Controls.Add(this.lstRecord);
            this.pnlList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlList.Location = new System.Drawing.Point(0, 29);
            this.pnlList.Name = "pnlList";
            this.pnlList.Size = new System.Drawing.Size(783, 149);
            this.pnlList.TabIndex = 1;
            // 
            // lstRecord
            // 
            this.lstRecord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstRecord.FormattingEnabled = true;
            this.lstRecord.Location = new System.Drawing.Point(0, 0);
            this.lstRecord.Name = "lstRecord";
            this.lstRecord.Size = new System.Drawing.Size(783, 149);
            this.lstRecord.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 178);
            this.Controls.Add(this.pnlList);
            this.Controls.Add(this.pnlState);
            this.Name = "MainForm";
            this.Text = "DetectWindowsSessionState";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.pnlState.ResumeLayout(false);
            this.pnlState.PerformLayout();
            this.pnlList.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlState;
        private System.Windows.Forms.Label lbState;
        private System.Windows.Forms.Panel pnlList;
        private System.Windows.Forms.ListBox lstRecord;
        private System.Windows.Forms.CheckBox chkEnableTimer;
    }
}

