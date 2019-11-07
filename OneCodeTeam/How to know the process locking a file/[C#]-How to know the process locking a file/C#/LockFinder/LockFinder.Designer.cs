namespace LockFinder
{
    partial class LockFinderForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LockFinderForm));
            this.tbFilePath = new System.Windows.Forms.TextBox();
            this.btnBrowseFile = new System.Windows.Forms.Button();
            this.tvwShowLockingProcess = new System.Windows.Forms.TreeView();
            this.tmrRefreshData = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tbFilePath
            // 
            this.tbFilePath.BackColor = System.Drawing.Color.Khaki;
            this.tbFilePath.Location = new System.Drawing.Point(13, 13);
            this.tbFilePath.Name = "tbFilePath";
            this.tbFilePath.ReadOnly = true;
            this.tbFilePath.Size = new System.Drawing.Size(460, 20);
            this.tbFilePath.TabIndex = 0;
            this.tbFilePath.TextChanged += new System.EventHandler(this.tbFilePath_TextChanged);
            // 
            // btnBrowseFile
            // 
            this.btnBrowseFile.BackColor = System.Drawing.Color.Transparent;
            this.btnBrowseFile.BackgroundImage = global::LockFinder.Properties.Resources.ApplicationIcon;
            this.btnBrowseFile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnBrowseFile.FlatAppearance.BorderSize = 0;
            this.btnBrowseFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseFile.Location = new System.Drawing.Point(475, 8);
            this.btnBrowseFile.Name = "btnBrowseFile";
            this.btnBrowseFile.Size = new System.Drawing.Size(33, 26);
            this.btnBrowseFile.TabIndex = 1;
            this.btnBrowseFile.UseVisualStyleBackColor = false;
            this.btnBrowseFile.Click += new System.EventHandler(this.btnBrowseFile_Click);
            this.btnBrowseFile.MouseEnter += new System.EventHandler(this.btnBrowseFile_MouseEnter);
            // 
            // tvwShowLockingProcess
            // 
            this.tvwShowLockingProcess.BackColor = System.Drawing.Color.Lavender;
            this.tvwShowLockingProcess.Location = new System.Drawing.Point(13, 49);
            this.tvwShowLockingProcess.Name = "tvwShowLockingProcess";
            this.tvwShowLockingProcess.Size = new System.Drawing.Size(491, 321);
            this.tvwShowLockingProcess.TabIndex = 2;
            // 
            // LockFinderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(516, 382);
            this.Controls.Add(this.tvwShowLockingProcess);
            this.Controls.Add(this.btnBrowseFile);
            this.Controls.Add(this.tbFilePath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "LockFinderForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Lock Finder";
            this.Load += new System.EventHandler(this.LockFinderForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbFilePath;
        private System.Windows.Forms.Button btnBrowseFile;
        private System.Windows.Forms.TreeView tvwShowLockingProcess;
        private System.Windows.Forms.Timer tmrRefreshData;
    }
}

