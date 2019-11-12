namespace CSFTPUpload
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
            this.btnConnect = new System.Windows.Forms.Button();
            this.tbFTPServer = new System.Windows.Forms.TextBox();
            this.lbFTPServer = new System.Windows.Forms.Label();
            this.lbCurrentUrl = new System.Windows.Forms.Label();
            this.grpUploadFolder = new System.Windows.Forms.GroupBox();
            this.chkCreateFolder = new System.Windows.Forms.CheckBox();
            this.btnUploadFolder = new System.Windows.Forms.Button();
            this.btnBrowseLocalFolder = new System.Windows.Forms.Button();
            this.tbLocalFolder = new System.Windows.Forms.TextBox();
            this.lbLocalFolder = new System.Windows.Forms.Label();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.lstLog = new System.Windows.Forms.ListBox();
            this.grpFileExplorer = new System.Windows.Forms.GroupBox();
            this.lstFileExplorer = new System.Windows.Forms.ListBox();
            this.pnlCurrentPath = new System.Windows.Forms.Panel();
            this.btnNavigateParentFolder = new System.Windows.Forms.Button();
            this.grpUploadFile = new System.Windows.Forms.GroupBox();
            this.btnUploadFile = new System.Windows.Forms.Button();
            this.btnBrowseLocalFile = new System.Windows.Forms.Button();
            this.tbLocalFile = new System.Windows.Forms.TextBox();
            this.lbLocalFile = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lbDelete = new System.Windows.Forms.Label();
            this.pnlFTPServer.SuspendLayout();
            this.grpUploadFolder.SuspendLayout();
            this.pnlStatus.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.grpFileExplorer.SuspendLayout();
            this.pnlCurrentPath.SuspendLayout();
            this.grpUploadFile.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            // lbCurrentUrl
            // 
            this.lbCurrentUrl.AutoSize = true;
            this.lbCurrentUrl.Location = new System.Drawing.Point(106, 10);
            this.lbCurrentUrl.Name = "lbCurrentUrl";
            this.lbCurrentUrl.Size = new System.Drawing.Size(66, 13);
            this.lbCurrentUrl.TabIndex = 4;
            this.lbCurrentUrl.Text = "Current Path";
            // 
            // grpUploadFolder
            // 
            this.grpUploadFolder.Controls.Add(this.chkCreateFolder);
            this.grpUploadFolder.Controls.Add(this.btnUploadFolder);
            this.grpUploadFolder.Controls.Add(this.btnBrowseLocalFolder);
            this.grpUploadFolder.Controls.Add(this.tbLocalFolder);
            this.grpUploadFolder.Controls.Add(this.lbLocalFolder);
            this.grpUploadFolder.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpUploadFolder.Location = new System.Drawing.Point(524, 33);
            this.grpUploadFolder.Name = "grpUploadFolder";
            this.grpUploadFolder.Size = new System.Drawing.Size(553, 111);
            this.grpUploadFolder.TabIndex = 0;
            this.grpUploadFolder.TabStop = false;
            this.grpUploadFolder.Text = "Upload Folder";
            // 
            // chkCreateFolder
            // 
            this.chkCreateFolder.AutoSize = true;
            this.chkCreateFolder.Location = new System.Drawing.Point(91, 54);
            this.chkCreateFolder.Name = "chkCreateFolder";
            this.chkCreateFolder.Size = new System.Drawing.Size(165, 17);
            this.chkCreateFolder.TabIndex = 4;
            this.chkCreateFolder.Text = "Create a folder on FTP server";
            this.chkCreateFolder.UseVisualStyleBackColor = true;
            // 
            // btnUploadFolder
            // 
            this.btnUploadFolder.Enabled = false;
            this.btnUploadFolder.Location = new System.Drawing.Point(91, 77);
            this.btnUploadFolder.Name = "btnUploadFolder";
            this.btnUploadFolder.Size = new System.Drawing.Size(98, 23);
            this.btnUploadFolder.TabIndex = 3;
            this.btnUploadFolder.Text = "Upload Folder";
            this.btnUploadFolder.UseVisualStyleBackColor = true;
            this.btnUploadFolder.Click += new System.EventHandler(this.btnUploadFolder_Click);
            // 
            // btnBrowseLocalFolder
            // 
            this.btnBrowseLocalFolder.Location = new System.Drawing.Point(427, 25);
            this.btnBrowseLocalFolder.Name = "btnBrowseLocalFolder";
            this.btnBrowseLocalFolder.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseLocalFolder.TabIndex = 2;
            this.btnBrowseLocalFolder.Text = "Browse";
            this.btnBrowseLocalFolder.UseVisualStyleBackColor = true;
            this.btnBrowseLocalFolder.Click += new System.EventHandler(this.btnBrowseLocalFolder_Click);
            // 
            // tbLocalFolder
            // 
            this.tbLocalFolder.Location = new System.Drawing.Point(91, 26);
            this.tbLocalFolder.Name = "tbLocalFolder";
            this.tbLocalFolder.ReadOnly = true;
            this.tbLocalFolder.Size = new System.Drawing.Size(329, 20);
            this.tbLocalFolder.TabIndex = 1;
            // 
            // lbLocalFolder
            // 
            this.lbLocalFolder.AutoSize = true;
            this.lbLocalFolder.Location = new System.Drawing.Point(7, 30);
            this.lbLocalFolder.Name = "lbLocalFolder";
            this.lbLocalFolder.Size = new System.Drawing.Size(65, 13);
            this.lbLocalFolder.TabIndex = 0;
            this.lbLocalFolder.Text = "Local Folder";
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
            // grpUploadFile
            // 
            this.grpUploadFile.Controls.Add(this.btnUploadFile);
            this.grpUploadFile.Controls.Add(this.btnBrowseLocalFile);
            this.grpUploadFile.Controls.Add(this.tbLocalFile);
            this.grpUploadFile.Controls.Add(this.lbLocalFile);
            this.grpUploadFile.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpUploadFile.Location = new System.Drawing.Point(524, 144);
            this.grpUploadFile.Name = "grpUploadFile";
            this.grpUploadFile.Size = new System.Drawing.Size(553, 96);
            this.grpUploadFile.TabIndex = 3;
            this.grpUploadFile.TabStop = false;
            this.grpUploadFile.Text = "Upload Files";
            // 
            // btnUploadFile
            // 
            this.btnUploadFile.Enabled = false;
            this.btnUploadFile.Location = new System.Drawing.Point(91, 52);
            this.btnUploadFile.Name = "btnUploadFile";
            this.btnUploadFile.Size = new System.Drawing.Size(98, 23);
            this.btnUploadFile.TabIndex = 3;
            this.btnUploadFile.Text = "Upload Files";
            this.btnUploadFile.UseVisualStyleBackColor = true;
            this.btnUploadFile.Click += new System.EventHandler(this.btnUploadFile_Click);
            // 
            // btnBrowseLocalFile
            // 
            this.btnBrowseLocalFile.Location = new System.Drawing.Point(427, 25);
            this.btnBrowseLocalFile.Name = "btnBrowseLocalFile";
            this.btnBrowseLocalFile.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseLocalFile.TabIndex = 2;
            this.btnBrowseLocalFile.Text = "Browse";
            this.btnBrowseLocalFile.UseVisualStyleBackColor = true;
            this.btnBrowseLocalFile.Click += new System.EventHandler(this.btnBrowseLocalFile_Click);
            // 
            // tbLocalFile
            // 
            this.tbLocalFile.Location = new System.Drawing.Point(91, 26);
            this.tbLocalFile.Name = "tbLocalFile";
            this.tbLocalFile.ReadOnly = true;
            this.tbLocalFile.Size = new System.Drawing.Size(329, 20);
            this.tbLocalFile.TabIndex = 1;
            // 
            // lbLocalFile
            // 
            this.lbLocalFile.AutoSize = true;
            this.lbLocalFile.Location = new System.Drawing.Point(7, 30);
            this.lbLocalFile.Name = "lbLocalFile";
            this.lbLocalFile.Size = new System.Drawing.Size(57, 13);
            this.lbLocalFile.TabIndex = 0;
            this.lbLocalFile.Text = "Local Files";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbDelete);
            this.groupBox1.Controls.Add(this.btnDelete);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(524, 240);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(553, 96);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Delete Folders / Files";
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(91, 19);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(98, 23);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lbDelete
            // 
            this.lbDelete.AutoSize = true;
            this.lbDelete.Location = new System.Drawing.Point(88, 56);
            this.lbDelete.Name = "lbDelete";
            this.lbDelete.Size = new System.Drawing.Size(238, 13);
            this.lbDelete.TabIndex = 4;
            this.lbDelete.Text = "Delete the selected items in the FTP File Explorer";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1077, 573);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grpUploadFile);
            this.Controls.Add(this.grpUploadFolder);
            this.Controls.Add(this.grpFileExplorer);
            this.Controls.Add(this.pnlStatus);
            this.Controls.Add(this.pnlFTPServer);
            this.Name = "MainForm";
            this.Text = "CSFTPUpload";
            this.pnlFTPServer.ResumeLayout(false);
            this.pnlFTPServer.PerformLayout();
            this.grpUploadFolder.ResumeLayout(false);
            this.grpUploadFolder.PerformLayout();
            this.pnlStatus.ResumeLayout(false);
            this.grpLog.ResumeLayout(false);
            this.grpFileExplorer.ResumeLayout(false);
            this.pnlCurrentPath.ResumeLayout(false);
            this.pnlCurrentPath.PerformLayout();
            this.grpUploadFile.ResumeLayout(false);
            this.grpUploadFile.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlFTPServer;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox tbFTPServer;
        private System.Windows.Forms.Label lbFTPServer;
        private System.Windows.Forms.GroupBox grpUploadFolder;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.GroupBox grpFileExplorer;
        private System.Windows.Forms.ListBox lstFileExplorer;
        private System.Windows.Forms.Button btnUploadFolder;
        private System.Windows.Forms.Button btnBrowseLocalFolder;
        private System.Windows.Forms.TextBox tbLocalFolder;
        private System.Windows.Forms.Label lbLocalFolder;
        private System.Windows.Forms.Label lbCurrentUrl;
        private System.Windows.Forms.Panel pnlCurrentPath;
        private System.Windows.Forms.Button btnNavigateParentFolder;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.ListBox lstLog;
        private System.Windows.Forms.CheckBox chkCreateFolder;
        private System.Windows.Forms.GroupBox grpUploadFile;
        private System.Windows.Forms.Button btnUploadFile;
        private System.Windows.Forms.Button btnBrowseLocalFile;
        private System.Windows.Forms.TextBox tbLocalFile;
        private System.Windows.Forms.Label lbLocalFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbDelete;
        private System.Windows.Forms.Button btnDelete;
    }
}

