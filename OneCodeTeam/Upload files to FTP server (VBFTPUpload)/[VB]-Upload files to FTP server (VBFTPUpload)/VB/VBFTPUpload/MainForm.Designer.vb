
Partial Public Class MainForm
    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    ''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso (components IsNot Nothing) Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.pnlFTPServer = New Panel()
        Me.btnConnect = New Button()
        Me.tbFTPServer = New TextBox()
        Me.lbFTPServer = New Label()
        Me.lbCurrentUrl = New Label()
        Me.grpUploadFolder = New GroupBox()
        Me.chkCreateFolder = New CheckBox()
        Me.btnUploadFolder = New Button()
        Me.btnBrowseLocalFolder = New Button()
        Me.tbLocalFolder = New TextBox()
        Me.lbLocalFolder = New Label()
        Me.pnlStatus = New Panel()
        Me.grpLog = New GroupBox()
        Me.lstLog = New ListBox()
        Me.grpFileExplorer = New GroupBox()
        Me.lstFileExplorer = New ListBox()
        Me.pnlCurrentPath = New Panel()
        Me.btnNavigateParentFolder = New Button()
        Me.grpUploadFile = New GroupBox()
        Me.btnUploadFile = New Button()
        Me.btnBrowseLocalFile = New Button()
        Me.tbLocalFile = New TextBox()
        Me.lbLocalFile = New Label()
        Me.groupBox1 = New GroupBox()
        Me.btnDelete = New Button()
        Me.lbDelete = New Label()
        Me.pnlFTPServer.SuspendLayout()
        Me.grpUploadFolder.SuspendLayout()
        Me.pnlStatus.SuspendLayout()
        Me.grpLog.SuspendLayout()
        Me.grpFileExplorer.SuspendLayout()
        Me.pnlCurrentPath.SuspendLayout()
        Me.grpUploadFile.SuspendLayout()
        Me.groupBox1.SuspendLayout()
        Me.SuspendLayout()
        ' 
        ' pnlFTPServer
        ' 
        Me.pnlFTPServer.Controls.Add(Me.btnConnect)
        Me.pnlFTPServer.Controls.Add(Me.tbFTPServer)
        Me.pnlFTPServer.Controls.Add(Me.lbFTPServer)
        Me.pnlFTPServer.Dock = DockStyle.Top
        Me.pnlFTPServer.Location = New Point(0, 0)
        Me.pnlFTPServer.Name = "pnlFTPServer"
        Me.pnlFTPServer.Size = New Size(1077, 33)
        Me.pnlFTPServer.TabIndex = 0
        ' 
        ' btnConnect
        ' 
        Me.btnConnect.Location = New Point(440, 4)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New Size(75, 23)
        Me.btnConnect.TabIndex = 2
        Me.btnConnect.Text = "Connect"
        Me.btnConnect.UseVisualStyleBackColor = True
        '			Me.btnConnect.Click += New System.EventHandler(Me.btnConnect_Click)
        ' 
        ' tbFTPServer
        ' 
        Me.tbFTPServer.Location = New Point(78, 8)
        Me.tbFTPServer.Name = "tbFTPServer"
        Me.tbFTPServer.Size = New Size(355, 20)
        Me.tbFTPServer.TabIndex = 1
        Me.tbFTPServer.Text = "ftp://localhost"
        ' 
        ' lbFTPServer
        ' 
        Me.lbFTPServer.AutoSize = True
        Me.lbFTPServer.Location = New Point(13, 13)
        Me.lbFTPServer.Name = "lbFTPServer"
        Me.lbFTPServer.Size = New Size(58, 13)
        Me.lbFTPServer.TabIndex = 0
        Me.lbFTPServer.Text = "FTPServer"
        ' 
        ' lbCurrentUrl
        ' 
        Me.lbCurrentUrl.AutoSize = True
        Me.lbCurrentUrl.Location = New Point(106, 10)
        Me.lbCurrentUrl.Name = "lbCurrentUrl"
        Me.lbCurrentUrl.Size = New Size(66, 13)
        Me.lbCurrentUrl.TabIndex = 4
        Me.lbCurrentUrl.Text = "Current Path"
        ' 
        ' grpUploadFolder
        ' 
        Me.grpUploadFolder.Controls.Add(Me.chkCreateFolder)
        Me.grpUploadFolder.Controls.Add(Me.btnUploadFolder)
        Me.grpUploadFolder.Controls.Add(Me.btnBrowseLocalFolder)
        Me.grpUploadFolder.Controls.Add(Me.tbLocalFolder)
        Me.grpUploadFolder.Controls.Add(Me.lbLocalFolder)
        Me.grpUploadFolder.Dock = DockStyle.Top
        Me.grpUploadFolder.Location = New Point(524, 33)
        Me.grpUploadFolder.Name = "grpUploadFolder"
        Me.grpUploadFolder.Size = New Size(553, 111)
        Me.grpUploadFolder.TabIndex = 0
        Me.grpUploadFolder.TabStop = False
        Me.grpUploadFolder.Text = "Upload Folder"
        ' 
        ' chkCreateFolder
        ' 
        Me.chkCreateFolder.AutoSize = True
        Me.chkCreateFolder.Location = New Point(91, 54)
        Me.chkCreateFolder.Name = "chkCreateFolder"
        Me.chkCreateFolder.Size = New Size(165, 17)
        Me.chkCreateFolder.TabIndex = 4
        Me.chkCreateFolder.Text = "Create a folder on FTP server"
        Me.chkCreateFolder.UseVisualStyleBackColor = True
        ' 
        ' btnUploadFolder
        ' 
        Me.btnUploadFolder.Enabled = False
        Me.btnUploadFolder.Location = New Point(91, 77)
        Me.btnUploadFolder.Name = "btnUploadFolder"
        Me.btnUploadFolder.Size = New Size(98, 23)
        Me.btnUploadFolder.TabIndex = 3
        Me.btnUploadFolder.Text = "Upload Folder"
        Me.btnUploadFolder.UseVisualStyleBackColor = True
        '			Me.btnUploadFolder.Click += New System.EventHandler(Me.btnUploadFolder_Click)
        ' 
        ' btnBrowseLocalFolder
        ' 
        Me.btnBrowseLocalFolder.Location = New Point(427, 25)
        Me.btnBrowseLocalFolder.Name = "btnBrowseLocalFolder"
        Me.btnBrowseLocalFolder.Size = New Size(75, 23)
        Me.btnBrowseLocalFolder.TabIndex = 2
        Me.btnBrowseLocalFolder.Text = "Browse"
        Me.btnBrowseLocalFolder.UseVisualStyleBackColor = True
        '			Me.btnBrowseLocalFolder.Click += New System.EventHandler(Me.btnBrowseLocalFolder_Click)
        ' 
        ' tbLocalFolder
        ' 
        Me.tbLocalFolder.Location = New Point(91, 26)
        Me.tbLocalFolder.Name = "tbLocalFolder"
        Me.tbLocalFolder.ReadOnly = True
        Me.tbLocalFolder.Size = New Size(329, 20)
        Me.tbLocalFolder.TabIndex = 1
        ' 
        ' lbLocalFolder
        ' 
        Me.lbLocalFolder.AutoSize = True
        Me.lbLocalFolder.Location = New Point(7, 30)
        Me.lbLocalFolder.Name = "lbLocalFolder"
        Me.lbLocalFolder.Size = New Size(65, 13)
        Me.lbLocalFolder.TabIndex = 0
        Me.lbLocalFolder.Text = "Local Folder"
        ' 
        ' pnlStatus
        ' 
        Me.pnlStatus.Controls.Add(Me.grpLog)
        Me.pnlStatus.Dock = DockStyle.Bottom
        Me.pnlStatus.Location = New Point(0, 374)
        Me.pnlStatus.Name = "pnlStatus"
        Me.pnlStatus.Size = New Size(1077, 199)
        Me.pnlStatus.TabIndex = 1
        ' 
        ' grpLog
        ' 
        Me.grpLog.Controls.Add(Me.lstLog)
        Me.grpLog.Dock = DockStyle.Fill
        Me.grpLog.Location = New Point(0, 0)
        Me.grpLog.Name = "grpLog"
        Me.grpLog.Size = New Size(1077, 199)
        Me.grpLog.TabIndex = 0
        Me.grpLog.TabStop = False
        Me.grpLog.Text = "Log"
        ' 
        ' lstLog
        ' 
        Me.lstLog.Dock = DockStyle.Fill
        Me.lstLog.FormattingEnabled = True
        Me.lstLog.Location = New Point(3, 16)
        Me.lstLog.Name = "lstLog"
        Me.lstLog.Size = New Size(1071, 180)
        Me.lstLog.TabIndex = 0
        ' 
        ' grpFileExplorer
        ' 
        Me.grpFileExplorer.Controls.Add(Me.lstFileExplorer)
        Me.grpFileExplorer.Controls.Add(Me.pnlCurrentPath)
        Me.grpFileExplorer.Dock = DockStyle.Left
        Me.grpFileExplorer.Location = New Point(0, 33)
        Me.grpFileExplorer.Name = "grpFileExplorer"
        Me.grpFileExplorer.Size = New Size(524, 341)
        Me.grpFileExplorer.TabIndex = 2
        Me.grpFileExplorer.TabStop = False
        Me.grpFileExplorer.Text = "FTP File Explorer"
        ' 
        ' lstFileExplorer
        ' 
        Me.lstFileExplorer.Dock = DockStyle.Fill
        Me.lstFileExplorer.FormattingEnabled = True
        Me.lstFileExplorer.Location = New Point(3, 48)
        Me.lstFileExplorer.Name = "lstFileExplorer"
        Me.lstFileExplorer.SelectionMode = SelectionMode.MultiExtended
        Me.lstFileExplorer.Size = New Size(518, 290)
        Me.lstFileExplorer.TabIndex = 0
        '			Me.lstFileExplorer.DoubleClick += New System.EventHandler(Me.lstFileExplorer_DoubleClick)
        ' 
        ' pnlCurrentPath
        ' 
        Me.pnlCurrentPath.Controls.Add(Me.lbCurrentUrl)
        Me.pnlCurrentPath.Controls.Add(Me.btnNavigateParentFolder)
        Me.pnlCurrentPath.Dock = DockStyle.Top
        Me.pnlCurrentPath.Location = New Point(3, 16)
        Me.pnlCurrentPath.Name = "pnlCurrentPath"
        Me.pnlCurrentPath.Size = New Size(518, 32)
        Me.pnlCurrentPath.TabIndex = 6
        ' 
        ' btnNavigateParentFolder
        ' 
        Me.btnNavigateParentFolder.Location = New Point(9, 4)
        Me.btnNavigateParentFolder.Name = "btnNavigateParentFolder"
        Me.btnNavigateParentFolder.Size = New Size(91, 23)
        Me.btnNavigateParentFolder.TabIndex = 5
        Me.btnNavigateParentFolder.Text = "Parent Folder"
        Me.btnNavigateParentFolder.UseVisualStyleBackColor = True
        '			Me.btnNavigateParentFolder.Click += New System.EventHandler(Me.btnNavigateParentFolder_Click)
        ' 
        ' grpUploadFile
        ' 
        Me.grpUploadFile.Controls.Add(Me.btnUploadFile)
        Me.grpUploadFile.Controls.Add(Me.btnBrowseLocalFile)
        Me.grpUploadFile.Controls.Add(Me.tbLocalFile)
        Me.grpUploadFile.Controls.Add(Me.lbLocalFile)
        Me.grpUploadFile.Dock = DockStyle.Top
        Me.grpUploadFile.Location = New Point(524, 144)
        Me.grpUploadFile.Name = "grpUploadFile"
        Me.grpUploadFile.Size = New Size(553, 96)
        Me.grpUploadFile.TabIndex = 3
        Me.grpUploadFile.TabStop = False
        Me.grpUploadFile.Text = "Upload Files"
        ' 
        ' btnUploadFile
        ' 
        Me.btnUploadFile.Enabled = False
        Me.btnUploadFile.Location = New Point(91, 52)
        Me.btnUploadFile.Name = "btnUploadFile"
        Me.btnUploadFile.Size = New Size(98, 23)
        Me.btnUploadFile.TabIndex = 3
        Me.btnUploadFile.Text = "Upload Files"
        Me.btnUploadFile.UseVisualStyleBackColor = True
        '			Me.btnUploadFile.Click += New System.EventHandler(Me.btnUploadFile_Click)
        ' 
        ' btnBrowseLocalFile
        ' 
        Me.btnBrowseLocalFile.Location = New Point(427, 25)
        Me.btnBrowseLocalFile.Name = "btnBrowseLocalFile"
        Me.btnBrowseLocalFile.Size = New Size(75, 23)
        Me.btnBrowseLocalFile.TabIndex = 2
        Me.btnBrowseLocalFile.Text = "Browse"
        Me.btnBrowseLocalFile.UseVisualStyleBackColor = True
        '			Me.btnBrowseLocalFile.Click += New System.EventHandler(Me.btnBrowseLocalFile_Click)
        ' 
        ' tbLocalFile
        ' 
        Me.tbLocalFile.Location = New Point(91, 26)
        Me.tbLocalFile.Name = "tbLocalFile"
        Me.tbLocalFile.ReadOnly = True
        Me.tbLocalFile.Size = New Size(329, 20)
        Me.tbLocalFile.TabIndex = 1
        ' 
        ' lbLocalFile
        ' 
        Me.lbLocalFile.AutoSize = True
        Me.lbLocalFile.Location = New Point(7, 30)
        Me.lbLocalFile.Name = "lbLocalFile"
        Me.lbLocalFile.Size = New Size(57, 13)
        Me.lbLocalFile.TabIndex = 0
        Me.lbLocalFile.Text = "Local Files"
        ' 
        ' groupBox1
        ' 
        Me.groupBox1.Controls.Add(Me.lbDelete)
        Me.groupBox1.Controls.Add(Me.btnDelete)
        Me.groupBox1.Dock = DockStyle.Top
        Me.groupBox1.Location = New Point(524, 240)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New Size(553, 96)
        Me.groupBox1.TabIndex = 4
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "Delete Folders / Files"
        ' 
        ' btnDelete
        ' 
        Me.btnDelete.Enabled = False
        Me.btnDelete.Location = New Point(91, 19)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New Size(98, 23)
        Me.btnDelete.TabIndex = 3
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = True
        '			Me.btnDelete.Click += New System.EventHandler(Me.btnDelete_Click)
        ' 
        ' lbDelete
        ' 
        Me.lbDelete.AutoSize = True
        Me.lbDelete.Location = New Point(88, 56)
        Me.lbDelete.Name = "lbDelete"
        Me.lbDelete.Size = New Size(238, 13)
        Me.lbDelete.TabIndex = 4
        Me.lbDelete.Text = "Delete the selected items in the FTP File Explorer"
        ' 
        ' MainForm
        ' 
        Me.AutoScaleDimensions = New SizeF(6.0F, 13.0F)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(1077, 573)
        Me.Controls.Add(Me.groupBox1)
        Me.Controls.Add(Me.grpUploadFile)
        Me.Controls.Add(Me.grpUploadFolder)
        Me.Controls.Add(Me.grpFileExplorer)
        Me.Controls.Add(Me.pnlStatus)
        Me.Controls.Add(Me.pnlFTPServer)
        Me.Name = "MainForm"
        Me.Text = "VBFTPUpload"
        Me.pnlFTPServer.ResumeLayout(False)
        Me.pnlFTPServer.PerformLayout()
        Me.grpUploadFolder.ResumeLayout(False)
        Me.grpUploadFolder.PerformLayout()
        Me.pnlStatus.ResumeLayout(False)
        Me.grpLog.ResumeLayout(False)
        Me.grpFileExplorer.ResumeLayout(False)
        Me.pnlCurrentPath.ResumeLayout(False)
        Me.pnlCurrentPath.PerformLayout()
        Me.grpUploadFile.ResumeLayout(False)
        Me.grpUploadFile.PerformLayout()
        Me.groupBox1.ResumeLayout(False)
        Me.groupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private pnlFTPServer As Panel
    Private WithEvents btnConnect As Button
    Private tbFTPServer As TextBox
    Private lbFTPServer As Label
    Private grpUploadFolder As GroupBox
    Private pnlStatus As Panel
    Private grpFileExplorer As GroupBox
    Private WithEvents lstFileExplorer As ListBox
    Private WithEvents btnUploadFolder As Button
    Private WithEvents btnBrowseLocalFolder As Button
    Private tbLocalFolder As TextBox
    Private lbLocalFolder As Label
    Private lbCurrentUrl As Label
    Private pnlCurrentPath As Panel
    Private WithEvents btnNavigateParentFolder As Button
    Private grpLog As GroupBox
    Private lstLog As ListBox
    Private chkCreateFolder As CheckBox
    Private grpUploadFile As GroupBox
    Private WithEvents btnUploadFile As Button
    Private WithEvents btnBrowseLocalFile As Button
    Private tbLocalFile As TextBox
    Private lbLocalFile As Label
    Private groupBox1 As GroupBox
    Private lbDelete As Label
    Private WithEvents btnDelete As Button
End Class

