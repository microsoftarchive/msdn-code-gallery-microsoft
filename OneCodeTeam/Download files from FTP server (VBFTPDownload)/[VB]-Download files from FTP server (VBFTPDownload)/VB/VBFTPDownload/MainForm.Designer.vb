
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
        Me.lbCurrentUrl = New Label()
        Me.btnConnect = New Button()
        Me.tbFTPServer = New TextBox()
        Me.lbFTPServer = New Label()
        Me.grpDownload = New GroupBox()
        Me.btnDownload = New Button()
        Me.btnBrowseDownloadPath = New Button()
        Me.tbDownloadPath = New TextBox()
        Me.lbDownloadPath = New Label()
        Me.pnlStatus = New Panel()
        Me.grpFileExplorer = New GroupBox()
        Me.lstFileExplorer = New ListBox()
        Me.btnNavigateParentFolder = New Button()
        Me.pnlCurrentPath = New Panel()
        Me.grpLog = New GroupBox()
        Me.lstLog = New ListBox()
        Me.pnlFTPServer.SuspendLayout()
        Me.grpDownload.SuspendLayout()
        Me.pnlStatus.SuspendLayout()
        Me.grpFileExplorer.SuspendLayout()
        Me.pnlCurrentPath.SuspendLayout()
        Me.grpLog.SuspendLayout()
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
        ' lbCurrentUrl
        ' 
        Me.lbCurrentUrl.AutoSize = True
        Me.lbCurrentUrl.Location = New Point(106, 10)
        Me.lbCurrentUrl.Name = "lbCurrentUrl"
        Me.lbCurrentUrl.Size = New Size(66, 13)
        Me.lbCurrentUrl.TabIndex = 4
        Me.lbCurrentUrl.Text = "Current Path"
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
        ' grpDownload
        ' 
        Me.grpDownload.Controls.Add(Me.btnDownload)
        Me.grpDownload.Controls.Add(Me.btnBrowseDownloadPath)
        Me.grpDownload.Controls.Add(Me.tbDownloadPath)
        Me.grpDownload.Controls.Add(Me.lbDownloadPath)
        Me.grpDownload.Dock = DockStyle.Fill
        Me.grpDownload.Location = New Point(524, 33)
        Me.grpDownload.Name = "grpDownload"
        Me.grpDownload.Size = New Size(553, 341)
        Me.grpDownload.TabIndex = 0
        Me.grpDownload.TabStop = False
        Me.grpDownload.Text = "Download"
        ' 
        ' btnDownload
        ' 
        Me.btnDownload.Location = New Point(91, 64)
        Me.btnDownload.Name = "btnDownload"
        Me.btnDownload.Size = New Size(75, 23)
        Me.btnDownload.TabIndex = 3
        Me.btnDownload.Text = "Download"
        Me.btnDownload.UseVisualStyleBackColor = True
        '			Me.btnDownload.Click += New System.EventHandler(Me.btnDownload_Click)
        ' 
        ' btnBrowseDownloadPath
        ' 
        Me.btnBrowseDownloadPath.Location = New Point(427, 25)
        Me.btnBrowseDownloadPath.Name = "btnBrowseDownloadPath"
        Me.btnBrowseDownloadPath.Size = New Size(75, 23)
        Me.btnBrowseDownloadPath.TabIndex = 2
        Me.btnBrowseDownloadPath.Text = "Browse"
        Me.btnBrowseDownloadPath.UseVisualStyleBackColor = True
        '			Me.btnBrowseDownloadPath.Click += New System.EventHandler(Me.btnBrowseDownloadPath_Click)
        ' 
        ' tbDownloadPath
        ' 
        Me.tbDownloadPath.Location = New Point(91, 26)
        Me.tbDownloadPath.Name = "tbDownloadPath"
        Me.tbDownloadPath.ReadOnly = True
        Me.tbDownloadPath.Size = New Size(329, 20)
        Me.tbDownloadPath.TabIndex = 1
        ' 
        ' lbDownloadPath
        ' 
        Me.lbDownloadPath.AutoSize = True
        Me.lbDownloadPath.Location = New Point(7, 30)
        Me.lbDownloadPath.Name = "lbDownloadPath"
        Me.lbDownloadPath.Size = New Size(77, 13)
        Me.lbDownloadPath.TabIndex = 0
        Me.lbDownloadPath.Text = "DownloadPath"
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
        ' MainForm
        ' 
        Me.AutoScaleDimensions = New SizeF(6.0F, 13.0F)
        Me.AutoScaleMode = AutoScaleMode.Font
        Me.ClientSize = New Size(1077, 573)
        Me.Controls.Add(Me.grpDownload)
        Me.Controls.Add(Me.grpFileExplorer)
        Me.Controls.Add(Me.pnlStatus)
        Me.Controls.Add(Me.pnlFTPServer)
        Me.Name = "MainForm"
        Me.Text = "VBFTPDownload"
        Me.pnlFTPServer.ResumeLayout(False)
        Me.pnlFTPServer.PerformLayout()
        Me.grpDownload.ResumeLayout(False)
        Me.grpDownload.PerformLayout()
        Me.pnlStatus.ResumeLayout(False)
        Me.grpFileExplorer.ResumeLayout(False)
        Me.pnlCurrentPath.ResumeLayout(False)
        Me.pnlCurrentPath.PerformLayout()
        Me.grpLog.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

    Private pnlFTPServer As Panel
    Private WithEvents btnConnect As Button
    Private tbFTPServer As TextBox
    Private lbFTPServer As Label
    Private grpDownload As GroupBox
    Private pnlStatus As Panel
    Private grpFileExplorer As GroupBox
    Private WithEvents lstFileExplorer As ListBox
    Private WithEvents btnDownload As Button
    Private WithEvents btnBrowseDownloadPath As Button
    Private tbDownloadPath As TextBox
    Private lbDownloadPath As Label
    Private lbCurrentUrl As Label
    Private pnlCurrentPath As Panel
    Private WithEvents btnNavigateParentFolder As Button
    Private grpLog As GroupBox
    Private lstLog As ListBox
End Class

