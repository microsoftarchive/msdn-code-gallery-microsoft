namespace CSFTPDownload
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
            this.pnlFTPServer = new System.Windows.Forms.Panel();
            this.lbCurrentUrl = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.tbFTPServer = new System.Windows.Forms.TextBox();
            this.lbFTPServer = new System.Windows.Forms.Label();
            this.grpDownload = new System.Windows.Forms.GroupBox();
            this.btnDownload = new System.Windows.Forms.Button();
            this.btnBrowseDownloadPath = new System.Windows.Forms.Button();
            this.tbDownloadPath = new System.Windows.Forms.TextBox();
            this.lbDownloadPath = new System.Windows.Forms.Label();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.grpFileExplorer = new System.Windows.Forms.GroupBox();
            this.lstFileExplorer = new System.Windows.Forms.ListBox();
            this.btnNavigateParentFolder = new System.Windows.Forms.Button();
            this.pnlCurrentPath = new System.Windows.Forms.Panel();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.pnlFTPServer.SuspendLayout();
            this.grpDownload.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.grpFileExplorer.SuspendLayout();
            this.pnlCurrentPath.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlFTPServer
            // 
            this.pnlFTPServer.Controls.Add(this.btnConnect);
            this.pnlFTPServer.Controls.Add(this.tbFTPServer);
            this.pnlFTPServer.Controls.Add(this.lbFTPServer);
            this.pnlFTPServer.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFTPServer.Location = new System.Drawing.Point(0, 0);
            this.pnlFTPServer.Name = "pnlFTPServer";
            this.pnlFTPServer.Size = new System.Drawing.Size(1077, 33);
            this.pnlFTPServer.TabIndex = 0;
            // 
            // lbCurrentUrl
            // 
            this.lbCurrentUrl.AutoSize = true;
            this.lbCurrentUrl.Location = new System.Drawing.Point(106, 10);
            this.lbCurrentUrl.Name = "lbCurrentUrl";
            this.lbCurrentUrl.Size = new System.Drawing.Size(66, 13);
            this.lbCurrentUrl.TabIndex = 4;
            this.lbCurrentUrl.Text = "Current Path";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(440, 4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // tbFTPServer
            // 
            this.tbFTPServer.Location = new System.Drawing.Point(78, 8);
            this.tbFTPServer.Name = "tbFTPServer";
            this.tbFTPServer.Size = new System.Drawing.Size(355, 20);
            this.tbFTPServer.TabIndex = 1;
            this.tbFTPServer.Text = "ftp://localhost";
            // 
            // lbFTPServer
            // 
            this.lbFTPServer.AutoSize = true;
            this.lbFTPServer.Location = new System.Drawing.Point(13, 13);
            this.lbFTPServer.Name = "lbFTPServer";
            this.lbFTPServer.Size = new System.Drawing.Size(58, 13);
            this.lbFTPServer.TabIndex = 0;
            this.lbFTPServer.Text = "FTPServer";
            // 
            // grpDownload
            // 
            this.grpDownload.Controls.Add(this.btnDownload);
            this.grpDownload.Controls.Add(this.btnBrowseDownloadPath);
            this.grpDownload.Controls.Add(this.tbDownloadPath);
            this.grpDownload.Controls.Add(this.lbDownloadPath);
            this.grpDownload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDownload.Location = new System.Drawing.Point(524, 33);
            this.grpDownload.Name = "grpDownload";
            this.grpDownload.Size = new System.Drawing.Size(553, 341);
            this.grpDownload.TabIndex = 0;
            this.grpDownload.TabStop = false;
            this.grpDownload.Text = "Download";
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(91, 64);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(75, 23);
            this.btnDownload.TabIndex = 3;
            this.btnDownload.Text = "Download";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // btnBrowseDownloadPath
            // 
            this.btnBrowseDownloadPath.Location = new System.Drawing.Point(427, 25);
            this.btnBrowseDownloadPath.Name = "btnBrowseDownloadPath";
            this.btnBrowseDownloadPath.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseDownloadPath.TabIndex = 2;
            this.btnBrowseDownloadPath.Text = "Browse";
            this.btnBrowseDownloadPath.UseVisualStyleBackColor = true;
            this.btnBrowseDownloadPath.Click += new System.EventHandler(this.btnBrowseDownloadPath_Click);
            // 
            // tbDownloadPath
            // 
            this.tbDownloadPath.Location = new System.Drawing.Point(91, 26);
            this.tbDownloadPath.Name = "tbDownloadPath";
            this.tbDownloadPath.ReadOnly = true;
            this.tbDownloadPath.Size = new System.Drawing.Size(329, 20);
            this.tbDownloadPath.TabIndex = 1;
            // 
            // lbDownloadPath
            // 
            this.lbDownloadPath.AutoSize = true;
            this.lbDownloadPath.Location = new System.Drawing.Point(7, 30);
            this.lbDownloadPath.Name = "lbDownloadPath";
            this.lbDownloadPath.Size = new System.Drawing.Size(77, 13);
            this.lbDownloadPath.TabIndex = 0;
            this.lbDownloadPath.Text = "DownloadPath";
            // 
            // pnlStatus
            // 
            this.pnlStatus.Controls.Add(this.grpLog);
            this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlStatus.Location = new System.Drawing.Point(0, 374);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(1077, 199);
            this.pnlStatus.TabIndex = 1;
            // 
            // grpFileExplorer
            // 
            this.grpFileExplorer.Controls.Add(this.lstFileExplorer);
            this.grpFileExplorer.Controls.Add(this.pnlCurrentPath);
            this.grpFileExplorer.Dock = System.Windows.Forms.DockStyle.Left;
            this.grpFileExplorer.Location = new System.Drawing.Point(0, 33);
            this.grpFileExplorer.Name = "grpFileExplorer";
            this.grpFileExplorer.Size = new System.Drawing.Size(524, 341);
            this.grpFileExplorer.TabIndex = 2;
            this.grpFileExplorer.TabStop = false;
            this.grpFileExplorer.Text = "FTP File Explorer";
            // 
            // lstFileExplorer
            // 
            this.lstFileExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFileExplorer.FormattingEnabled = true;
            this.lstFileExplorer.Location = new System.Drawing.Point(3, 48);
            this.lstFileExplorer.Name = "lstFileExplorer";
            this.lstFileExplorer.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstFileExplorer.Size = new System.Drawing.Size(518, 290);
            this.lstFileExplorer.TabIndex = 0;
            this.lstFileExplorer.DoubleClick += new System.EventHandler(this.lstFileExplorer_DoubleClick);
            // 
            // btnNavigateParentFolder
            // 
            this.btnNavigateParentFolder.Location = new System.Drawing.Point(9, 4);
            this.btnNavigateParentFolder.Name = "btnNavigateParentFolder";
            this.btnNavigateParentFolder.Size = new System.Drawing.Size(91, 23);
            this.btnNavigateParentFolder.TabIndex = 5;
            this.btnNavigateParentFolder.Text = "Parent Folder";
            this.btnNavigateParentFolder.UseVisualStyleBackColor = true;
            this.btnNavigateParentFolder.Click += new System.EventHandler(this.btnNavigateParentFolder_Click);
            // 
            // pnlCurrentPath
            // 
            this.pnlCurrentPath.Controls.Add(this.lbCurrentUrl);
            this.pnlCurrentPath.Controls.Add(this.btnNavigateParentFolder);
            this.pnlCurrentPath.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCurrentPath.Location = new System.Drawing.Point(3, 16);
            this.pnlCurrentPath.Name = "pnlCurrentPath";
            this.pnlCurrentPath.Size = new System.Drawing.Size(518, 32);
            this.pnlCurrentPath.TabIndex = 6;
            // 
            // grpLog
            // 
            this.grpLog.Controls.Add(this.lstLog);
            this.grpLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLog.Location = new System.Drawing.Point(0, 0);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(1077, 199);
            this.grpLog.TabIndex = 0;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "Log";
            // 
            // lstLog
            // 
            this.lstLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstLog.FormattingEnabled = true;
            this.lstLog.Location = new System.Drawing.Point(3, 16);
            this.lstLog.Name = "lstLog";
            this.lstLog.Size = new System.Drawing.Size(1071, 180);
            this.lstLog.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1077, 573);
            this.Controls.Add(this.grpDownload);
            this.Controls.Add(this.grpFileExplorer);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.pnlFTPServer);
            this.Name = "MainForm";
            this.Text = "CSFTPDownload";
            this.pnlFTPServer.ResumeLayout(false);
            this.pnlFTPServer.PerformLayout();
            this.grpDownload.ResumeLayout(false);
            this.grpDownload.PerformLayout();
            this.pnlStatus.ResumeLayout(false);
            this.grpFileExplorer.ResumeLayout(false);
            this.pnlCurrentPath.ResumeLayout(false);
            this.pnlCurrentPath.PerformLayout();
            this.grpLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlFTPServer;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox tbFTPServer;
        private System.Windows.Forms.Label lbFTPServer;
        private System.Windows.Forms.GroupBox grpDownload;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.GroupBox grpFileExplorer;
        private System.Windows.Forms.ListBox lstFileExplorer;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.Button btnBrowseDownloadPath;
        private System.Windows.Forms.TextBox tbDownloadPath;
        private System.Windows.Forms.Label lbDownloadPath;
        private System.Windows.Forms.Label lbCurrentUrl;
        private System.Windows.Forms.Panel pnlCurrentPath;
        private System.Windows.Forms.Button btnNavigateParentFolder;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.ListBox lstLog;
    }
}

