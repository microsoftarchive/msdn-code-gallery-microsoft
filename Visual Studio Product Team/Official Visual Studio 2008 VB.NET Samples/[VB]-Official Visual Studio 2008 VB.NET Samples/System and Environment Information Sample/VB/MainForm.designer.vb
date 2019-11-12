' Copyright (c) Microsoft Corporation. All rights reserved.
Partial Public Class MainForm
    Inherits System.Windows.Forms.Form

    <System.Diagnostics.DebuggerNonUserCode()> _
    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

    End Sub

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblSystemFolder = New System.Windows.Forms.Label
        Me.pgeSpecialFolders = New System.Windows.Forms.TabPage
        Me.btnGetSystemFolder = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblSpecialFolder = New System.Windows.Forms.Label
        Me.lstFolders = New System.Windows.Forms.ListBox
        Me.btnRefreshTickCount = New System.Windows.Forms.Button
        Me.Label12 = New System.Windows.Forms.Label
        Me.lstCommandLineArgs = New System.Windows.Forms.ListBox
        Me.lblWorkingSet = New System.Windows.Forms.Label
        Me.Label17 = New System.Windows.Forms.Label
        Me.btnExit = New System.Windows.Forms.Button
        Me.lblUserDomainName = New System.Windows.Forms.Label
        Me.Label15 = New System.Windows.Forms.Label
        Me.Label13 = New System.Windows.Forms.Label
        Me.pgeProperties = New System.Windows.Forms.TabPage
        Me.btnStackTrace = New System.Windows.Forms.Button
        Me.Label21 = New System.Windows.Forms.Label
        Me.lblVersion = New System.Windows.Forms.Label
        Me.Label19 = New System.Windows.Forms.Label
        Me.lblUserName = New System.Windows.Forms.Label
        Me.lblOSVersion = New System.Windows.Forms.Label
        Me.lblMachineName = New System.Windows.Forms.Label
        Me.Label11 = New System.Windows.Forms.Label
        Me.lblTickCount = New System.Windows.Forms.Label
        Me.Label9 = New System.Windows.Forms.Label
        Me.lblSystemDirectory = New System.Windows.Forms.Label
        Me.Label7 = New System.Windows.Forms.Label
        Me.lblCurrentDirectory = New System.Windows.Forms.Label
        Me.Label5 = New System.Windows.Forms.Label
        Me.lblCommandLine = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.Label10 = New System.Windows.Forms.Label
        Me.nudExitCode = New System.Windows.Forms.NumericUpDown
        Me.Label4 = New System.Windows.Forms.Label
        Me.pgeMethods = New System.Windows.Forms.TabPage
        Me.lstLogicalDrives = New System.Windows.Forms.ListBox
        Me.grpMethods = New System.Windows.Forms.GroupBox
        Me.btnExpand = New System.Windows.Forms.Button
        Me.lblExpandResults = New System.Windows.Forms.Label
        Me.Label8 = New System.Windows.Forms.Label
        Me.Label6 = New System.Windows.Forms.Label
        Me.txtExpand = New System.Windows.Forms.TextBox
        Me.lstEnvironmentVariables = New System.Windows.Forms.ListBox
        Me.pgeSystemInformation = New System.Windows.Forms.TabPage
        Me.lvwSystemInformation = New System.Windows.Forms.ListView
        Me.colProperty = New System.Windows.Forms.ColumnHeader
        Me.colValue = New System.Windows.Forms.ColumnHeader
        Me.tabExplore = New System.Windows.Forms.TabControl
        Me.pgeEnvironmentVariables = New System.Windows.Forms.TabPage
        Me.lblTEMP = New System.Windows.Forms.Label
        Me.btnGetEnvironmentVariable = New System.Windows.Forms.Button
        Me.Label2 = New System.Windows.Forms.Label
        Me.lblEnvironmentVariable = New System.Windows.Forms.Label
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.FileToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.pgeSpecialFolders.SuspendLayout()
        Me.pgeProperties.SuspendLayout()
        CType(Me.nudExitCode, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pgeMethods.SuspendLayout()
        Me.grpMethods.SuspendLayout()
        Me.pgeSystemInformation.SuspendLayout()
        Me.tabExplore.SuspendLayout()
        Me.pgeEnvironmentVariables.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblSystemFolder
        '
        Me.lblSystemFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSystemFolder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblSystemFolder.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblSystemFolder.Location = New System.Drawing.Point(224, 168)
        Me.lblSystemFolder.Name = "lblSystemFolder"
        Me.lblSystemFolder.Size = New System.Drawing.Size(660, 23)
        Me.lblSystemFolder.TabIndex = 4
        Me.lblSystemFolder.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pgeSpecialFolders
        '
        Me.pgeSpecialFolders.Controls.Add(Me.lblSystemFolder)
        Me.pgeSpecialFolders.Controls.Add(Me.btnGetSystemFolder)
        Me.pgeSpecialFolders.Controls.Add(Me.Label1)
        Me.pgeSpecialFolders.Controls.Add(Me.lblSpecialFolder)
        Me.pgeSpecialFolders.Controls.Add(Me.lstFolders)
        Me.pgeSpecialFolders.Location = New System.Drawing.Point(4, 22)
        Me.pgeSpecialFolders.Name = "pgeSpecialFolders"
        Me.pgeSpecialFolders.Size = New System.Drawing.Size(546, 396)
        Me.pgeSpecialFolders.TabIndex = 0
        Me.pgeSpecialFolders.Text = "Special Folders"
        Me.pgeSpecialFolders.UseVisualStyleBackColor = False
        Me.pgeSpecialFolders.Visible = False
        '
        'btnGetSystemFolder
        '
        Me.btnGetSystemFolder.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnGetSystemFolder.Location = New System.Drawing.Point(224, 144)
        Me.btnGetSystemFolder.Name = "btnGetSystemFolder"
        Me.btnGetSystemFolder.Size = New System.Drawing.Size(152, 23)
        Me.btnGetSystemFolder.TabIndex = 3
        Me.btnGetSystemFolder.Text = "&Get System Folder"
        '
        'Label1
        '
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(224, 8)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(200, 23)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Special Folder Path:"
        '
        'lblSpecialFolder
        '
        Me.lblSpecialFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSpecialFolder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblSpecialFolder.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblSpecialFolder.Location = New System.Drawing.Point(224, 32)
        Me.lblSpecialFolder.Name = "lblSpecialFolder"
        Me.lblSpecialFolder.Size = New System.Drawing.Size(660, 96)
        Me.lblSpecialFolder.TabIndex = 2
        '
        'lstFolders
        '
        Me.lstFolders.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lstFolders.FormattingEnabled = True
        Me.lstFolders.Location = New System.Drawing.Point(8, 8)
        Me.lstFolders.Name = "lstFolders"
        Me.lstFolders.Size = New System.Drawing.Size(208, 537)
        Me.lstFolders.TabIndex = 0
        '
        'btnRefreshTickCount
        '
        Me.btnRefreshTickCount.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnRefreshTickCount.Location = New System.Drawing.Point(285, 129)
        Me.btnRefreshTickCount.Name = "btnRefreshTickCount"
        Me.btnRefreshTickCount.Size = New System.Drawing.Size(75, 23)
        Me.btnRefreshTickCount.TabIndex = 21
        Me.btnRefreshTickCount.Text = "&Refresh"
        '
        'Label12
        '
        Me.Label12.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label12.Location = New System.Drawing.Point(8, 128)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(120, 23)
        Me.Label12.TabIndex = 4
        Me.Label12.Text = "GetLogicalDrives"
        '
        'lstCommandLineArgs
        '
        Me.lstCommandLineArgs.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lstCommandLineArgs.FormattingEnabled = True
        Me.lstCommandLineArgs.HorizontalScrollbar = True
        Me.lstCommandLineArgs.Location = New System.Drawing.Point(144, 152)
        Me.lstCommandLineArgs.Name = "lstCommandLineArgs"
        Me.lstCommandLineArgs.Size = New System.Drawing.Size(391, 95)
        Me.lstCommandLineArgs.TabIndex = 7
        '
        'lblWorkingSet
        '
        Me.lblWorkingSet.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblWorkingSet.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblWorkingSet.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblWorkingSet.Location = New System.Drawing.Point(144, 224)
        Me.lblWorkingSet.Name = "lblWorkingSet"
        Me.lblWorkingSet.Size = New System.Drawing.Size(394, 23)
        Me.lblWorkingSet.TabIndex = 19
        Me.lblWorkingSet.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label17
        '
        Me.Label17.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label17.Location = New System.Drawing.Point(8, 176)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(136, 23)
        Me.Label17.TabIndex = 14
        Me.Label17.Text = "UserName"
        Me.Label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnExit
        '
        Me.btnExit.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnExit.Location = New System.Drawing.Point(8, 8)
        Me.btnExit.Name = "btnExit"
        Me.btnExit.Size = New System.Drawing.Size(88, 24)
        Me.btnExit.TabIndex = 0
        Me.btnExit.Text = "E&xit"
        '
        'lblUserDomainName
        '
        Me.lblUserDomainName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblUserDomainName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblUserDomainName.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblUserDomainName.Location = New System.Drawing.Point(144, 152)
        Me.lblUserDomainName.Name = "lblUserDomainName"
        Me.lblUserDomainName.Size = New System.Drawing.Size(394, 23)
        Me.lblUserDomainName.TabIndex = 13
        Me.lblUserDomainName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label15
        '
        Me.Label15.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label15.Location = New System.Drawing.Point(8, 152)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(136, 23)
        Me.Label15.TabIndex = 12
        Me.Label15.Text = "UserDomainName"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label13
        '
        Me.Label13.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label13.Location = New System.Drawing.Point(8, 80)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(136, 23)
        Me.Label13.TabIndex = 10
        Me.Label13.Text = "OSVersion"
        Me.Label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pgeProperties
        '
        Me.pgeProperties.Controls.Add(Me.btnRefreshTickCount)
        Me.pgeProperties.Controls.Add(Me.btnStackTrace)
        Me.pgeProperties.Controls.Add(Me.lblWorkingSet)
        Me.pgeProperties.Controls.Add(Me.Label21)
        Me.pgeProperties.Controls.Add(Me.lblVersion)
        Me.pgeProperties.Controls.Add(Me.Label19)
        Me.pgeProperties.Controls.Add(Me.lblUserName)
        Me.pgeProperties.Controls.Add(Me.Label17)
        Me.pgeProperties.Controls.Add(Me.lblUserDomainName)
        Me.pgeProperties.Controls.Add(Me.Label15)
        Me.pgeProperties.Controls.Add(Me.lblOSVersion)
        Me.pgeProperties.Controls.Add(Me.Label13)
        Me.pgeProperties.Controls.Add(Me.lblMachineName)
        Me.pgeProperties.Controls.Add(Me.Label11)
        Me.pgeProperties.Controls.Add(Me.lblTickCount)
        Me.pgeProperties.Controls.Add(Me.Label9)
        Me.pgeProperties.Controls.Add(Me.lblSystemDirectory)
        Me.pgeProperties.Controls.Add(Me.Label7)
        Me.pgeProperties.Controls.Add(Me.lblCurrentDirectory)
        Me.pgeProperties.Controls.Add(Me.Label5)
        Me.pgeProperties.Controls.Add(Me.lblCommandLine)
        Me.pgeProperties.Controls.Add(Me.Label3)
        Me.pgeProperties.Location = New System.Drawing.Point(4, 22)
        Me.pgeProperties.Name = "pgeProperties"
        Me.pgeProperties.Size = New System.Drawing.Size(546, 396)
        Me.pgeProperties.TabIndex = 2
        Me.pgeProperties.Text = "Properties"
        Me.pgeProperties.UseVisualStyleBackColor = False
        '
        'btnStackTrace
        '
        Me.btnStackTrace.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnStackTrace.Location = New System.Drawing.Point(144, 256)
        Me.btnStackTrace.Name = "btnStackTrace"
        Me.btnStackTrace.Size = New System.Drawing.Size(224, 23)
        Me.btnStackTrace.TabIndex = 20
        Me.btnStackTrace.Text = "&Display Current Stack Trace"
        '
        'Label21
        '
        Me.Label21.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label21.Location = New System.Drawing.Point(8, 224)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(136, 23)
        Me.Label21.TabIndex = 18
        Me.Label21.Text = "WorkingSet"
        Me.Label21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblVersion
        '
        Me.lblVersion.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblVersion.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblVersion.Location = New System.Drawing.Point(144, 200)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(394, 23)
        Me.lblVersion.TabIndex = 17
        Me.lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label19
        '
        Me.Label19.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label19.Location = New System.Drawing.Point(8, 200)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(136, 23)
        Me.Label19.TabIndex = 16
        Me.Label19.Text = "Version"
        Me.Label19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblUserName
        '
        Me.lblUserName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblUserName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblUserName.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblUserName.Location = New System.Drawing.Point(144, 176)
        Me.lblUserName.Name = "lblUserName"
        Me.lblUserName.Size = New System.Drawing.Size(394, 23)
        Me.lblUserName.TabIndex = 15
        Me.lblUserName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblOSVersion
        '
        Me.lblOSVersion.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblOSVersion.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblOSVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblOSVersion.Location = New System.Drawing.Point(144, 80)
        Me.lblOSVersion.Name = "lblOSVersion"
        Me.lblOSVersion.Size = New System.Drawing.Size(394, 23)
        Me.lblOSVersion.TabIndex = 11
        Me.lblOSVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMachineName
        '
        Me.lblMachineName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblMachineName.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblMachineName.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblMachineName.Location = New System.Drawing.Point(144, 56)
        Me.lblMachineName.Name = "lblMachineName"
        Me.lblMachineName.Size = New System.Drawing.Size(394, 23)
        Me.lblMachineName.TabIndex = 9
        Me.lblMachineName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label11
        '
        Me.Label11.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label11.Location = New System.Drawing.Point(8, 56)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(136, 23)
        Me.Label11.TabIndex = 8
        Me.Label11.Text = "MachineName"
        Me.Label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblTickCount
        '
        Me.lblTickCount.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTickCount.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblTickCount.Location = New System.Drawing.Point(144, 128)
        Me.lblTickCount.Name = "lblTickCount"
        Me.lblTickCount.Size = New System.Drawing.Size(128, 23)
        Me.lblTickCount.TabIndex = 7
        Me.lblTickCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label9
        '
        Me.Label9.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label9.Location = New System.Drawing.Point(8, 128)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(136, 23)
        Me.Label9.TabIndex = 6
        Me.Label9.Text = "TickCount"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblSystemDirectory
        '
        Me.lblSystemDirectory.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSystemDirectory.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblSystemDirectory.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblSystemDirectory.Location = New System.Drawing.Point(144, 104)
        Me.lblSystemDirectory.Name = "lblSystemDirectory"
        Me.lblSystemDirectory.Size = New System.Drawing.Size(394, 23)
        Me.lblSystemDirectory.TabIndex = 5
        Me.lblSystemDirectory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label7.Location = New System.Drawing.Point(8, 104)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(136, 23)
        Me.Label7.TabIndex = 4
        Me.Label7.Text = "SystemDirectory"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblCurrentDirectory
        '
        Me.lblCurrentDirectory.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblCurrentDirectory.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCurrentDirectory.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblCurrentDirectory.Location = New System.Drawing.Point(144, 32)
        Me.lblCurrentDirectory.Name = "lblCurrentDirectory"
        Me.lblCurrentDirectory.Size = New System.Drawing.Size(394, 23)
        Me.lblCurrentDirectory.TabIndex = 3
        Me.lblCurrentDirectory.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label5
        '
        Me.Label5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label5.Location = New System.Drawing.Point(8, 32)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(136, 23)
        Me.Label5.TabIndex = 2
        Me.Label5.Text = "CurrentDirectory"
        Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblCommandLine
        '
        Me.lblCommandLine.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblCommandLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblCommandLine.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblCommandLine.Location = New System.Drawing.Point(144, 8)
        Me.lblCommandLine.Name = "lblCommandLine"
        Me.lblCommandLine.Size = New System.Drawing.Size(394, 23)
        Me.lblCommandLine.TabIndex = 1
        Me.lblCommandLine.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label3.Location = New System.Drawing.Point(8, 8)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(136, 23)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "CommandLine:"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label10
        '
        Me.Label10.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label10.Location = New System.Drawing.Point(136, 128)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(120, 23)
        Me.Label10.TabIndex = 6
        Me.Label10.Text = "GetCommandLineArgs"
        '
        'nudExitCode
        '
        Me.nudExitCode.Location = New System.Drawing.Point(192, 8)
        Me.nudExitCode.Name = "nudExitCode"
        Me.nudExitCode.Size = New System.Drawing.Size(40, 20)
        Me.nudExitCode.TabIndex = 2
        '
        'Label4
        '
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(104, 8)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(88, 23)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "Exit &Code:"
        Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'pgeMethods
        '
        Me.pgeMethods.Controls.Add(Me.lstLogicalDrives)
        Me.pgeMethods.Controls.Add(Me.Label12)
        Me.pgeMethods.Controls.Add(Me.lstCommandLineArgs)
        Me.pgeMethods.Controls.Add(Me.Label10)
        Me.pgeMethods.Controls.Add(Me.grpMethods)
        Me.pgeMethods.Controls.Add(Me.nudExitCode)
        Me.pgeMethods.Controls.Add(Me.Label4)
        Me.pgeMethods.Controls.Add(Me.btnExit)
        Me.pgeMethods.Location = New System.Drawing.Point(4, 22)
        Me.pgeMethods.Name = "pgeMethods"
        Me.pgeMethods.Size = New System.Drawing.Size(546, 396)
        Me.pgeMethods.TabIndex = 3
        Me.pgeMethods.Text = "Methods"
        Me.pgeMethods.UseVisualStyleBackColor = False
        Me.pgeMethods.Visible = False
        '
        'lstLogicalDrives
        '
        Me.lstLogicalDrives.FormattingEnabled = True
        Me.lstLogicalDrives.Location = New System.Drawing.Point(16, 152)
        Me.lstLogicalDrives.Name = "lstLogicalDrives"
        Me.lstLogicalDrives.Size = New System.Drawing.Size(56, 95)
        Me.lstLogicalDrives.TabIndex = 5
        '
        'grpMethods
        '
        Me.grpMethods.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpMethods.Controls.Add(Me.btnExpand)
        Me.grpMethods.Controls.Add(Me.lblExpandResults)
        Me.grpMethods.Controls.Add(Me.Label8)
        Me.grpMethods.Controls.Add(Me.Label6)
        Me.grpMethods.Controls.Add(Me.txtExpand)
        Me.grpMethods.Location = New System.Drawing.Point(8, 40)
        Me.grpMethods.Name = "grpMethods"
        Me.grpMethods.Size = New System.Drawing.Size(876, 80)
        Me.grpMethods.TabIndex = 3
        Me.grpMethods.TabStop = False
        Me.grpMethods.Text = "ExpandEnvironmentVariables"
        '
        'btnExpand
        '
        Me.btnExpand.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnExpand.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnExpand.Location = New System.Drawing.Point(1086, 16)
        Me.btnExpand.Name = "btnExpand"
        Me.btnExpand.Size = New System.Drawing.Size(112, 23)
        Me.btnExpand.TabIndex = 2
        Me.btnExpand.Text = "&Expand"
        '
        'lblExpandResults
        '
        Me.lblExpandResults.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblExpandResults.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblExpandResults.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblExpandResults.Location = New System.Drawing.Point(104, 48)
        Me.lblExpandResults.Name = "lblExpandResults"
        Me.lblExpandResults.Size = New System.Drawing.Size(426, 24)
        Me.lblExpandResults.TabIndex = 4
        Me.lblExpandResults.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label8
        '
        Me.Label8.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label8.Location = New System.Drawing.Point(16, 48)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(88, 16)
        Me.Label8.TabIndex = 3
        Me.Label8.Text = "Results:"
        '
        'Label6
        '
        Me.Label6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label6.Location = New System.Drawing.Point(16, 24)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(88, 16)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "Input:"
        '
        'txtExpand
        '
        Me.txtExpand.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtExpand.Location = New System.Drawing.Point(104, 16)
        Me.txtExpand.Name = "txtExpand"
        Me.txtExpand.Size = New System.Drawing.Size(426, 20)
        Me.txtExpand.TabIndex = 1
        Me.txtExpand.Text = "windir = %windir%"
        '
        'lstEnvironmentVariables
        '
        Me.lstEnvironmentVariables.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.lstEnvironmentVariables.FormattingEnabled = True
        Me.lstEnvironmentVariables.Location = New System.Drawing.Point(8, 8)
        Me.lstEnvironmentVariables.Name = "lstEnvironmentVariables"
        Me.lstEnvironmentVariables.Size = New System.Drawing.Size(208, 537)
        Me.lstEnvironmentVariables.TabIndex = 0
        '
        'pgeSystemInformation
        '
        Me.pgeSystemInformation.Controls.Add(Me.lvwSystemInformation)
        Me.pgeSystemInformation.Location = New System.Drawing.Point(4, 22)
        Me.pgeSystemInformation.Name = "pgeSystemInformation"
        Me.pgeSystemInformation.Size = New System.Drawing.Size(546, 396)
        Me.pgeSystemInformation.TabIndex = 4
        Me.pgeSystemInformation.Text = "System Information"
        Me.pgeSystemInformation.UseVisualStyleBackColor = False
        Me.pgeSystemInformation.Visible = False
        '
        'lvwSystemInformation
        '
        Me.lvwSystemInformation.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.colProperty, Me.colValue})
        Me.lvwSystemInformation.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvwSystemInformation.Location = New System.Drawing.Point(0, 0)
        Me.lvwSystemInformation.Name = "lvwSystemInformation"
        Me.lvwSystemInformation.Size = New System.Drawing.Size(546, 396)
        Me.lvwSystemInformation.TabIndex = 0
        Me.lvwSystemInformation.View = System.Windows.Forms.View.Details
        '
        'colProperty
        '
        Me.colProperty.Name = "colProperty"
        Me.colProperty.Text = "Property"
        Me.colProperty.Width = 117
        '
        'colValue
        '
        Me.colValue.Name = "colValue"
        Me.colValue.Text = "Value"
        Me.colValue.Width = 341
        '
        'tabExplore
        '
        Me.tabExplore.Controls.Add(Me.pgeProperties)
        Me.tabExplore.Controls.Add(Me.pgeSpecialFolders)
        Me.tabExplore.Controls.Add(Me.pgeMethods)
        Me.tabExplore.Controls.Add(Me.pgeEnvironmentVariables)
        Me.tabExplore.Controls.Add(Me.pgeSystemInformation)
        Me.tabExplore.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.tabExplore.ItemSize = New System.Drawing.Size(59, 18)
        Me.tabExplore.Location = New System.Drawing.Point(0, 25)
        Me.tabExplore.Name = "tabExplore"
        Me.tabExplore.SelectedIndex = 0
        Me.tabExplore.Size = New System.Drawing.Size(554, 422)
        Me.tabExplore.TabIndex = 3
        '
        'pgeEnvironmentVariables
        '
        Me.pgeEnvironmentVariables.Controls.Add(Me.lblTEMP)
        Me.pgeEnvironmentVariables.Controls.Add(Me.btnGetEnvironmentVariable)
        Me.pgeEnvironmentVariables.Controls.Add(Me.Label2)
        Me.pgeEnvironmentVariables.Controls.Add(Me.lblEnvironmentVariable)
        Me.pgeEnvironmentVariables.Controls.Add(Me.lstEnvironmentVariables)
        Me.pgeEnvironmentVariables.Location = New System.Drawing.Point(4, 22)
        Me.pgeEnvironmentVariables.Name = "pgeEnvironmentVariables"
        Me.pgeEnvironmentVariables.Size = New System.Drawing.Size(546, 396)
        Me.pgeEnvironmentVariables.TabIndex = 1
        Me.pgeEnvironmentVariables.Text = "Environment Variables"
        Me.pgeEnvironmentVariables.UseVisualStyleBackColor = False
        Me.pgeEnvironmentVariables.Visible = False
        '
        'lblTEMP
        '
        Me.lblTEMP.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblTEMP.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblTEMP.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblTEMP.Location = New System.Drawing.Point(224, 168)
        Me.lblTEMP.Name = "lblTEMP"
        Me.lblTEMP.Size = New System.Drawing.Size(300, 23)
        Me.lblTEMP.TabIndex = 4
        Me.lblTEMP.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'btnGetEnvironmentVariable
        '
        Me.btnGetEnvironmentVariable.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnGetEnvironmentVariable.Location = New System.Drawing.Point(224, 144)
        Me.btnGetEnvironmentVariable.Name = "btnGetEnvironmentVariable"
        Me.btnGetEnvironmentVariable.Size = New System.Drawing.Size(152, 23)
        Me.btnGetEnvironmentVariable.TabIndex = 3
        Me.btnGetEnvironmentVariable.Text = "&Get TEMP Variable"
        '
        'Label2
        '
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(224, 8)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(200, 23)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Environment Variable Value:"
        '
        'lblEnvironmentVariable
        '
        Me.lblEnvironmentVariable.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblEnvironmentVariable.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblEnvironmentVariable.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblEnvironmentVariable.Location = New System.Drawing.Point(224, 32)
        Me.lblEnvironmentVariable.Name = "lblEnvironmentVariable"
        Me.lblEnvironmentVariable.Size = New System.Drawing.Size(300, 96)
        Me.lblEnvironmentVariable.TabIndex = 2
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileToolStripMenuItem1})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(554, 24)
        Me.MenuStrip1.TabIndex = 4
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'FileToolStripMenuItem1
        '
        Me.FileToolStripMenuItem1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.exitToolStripMenuItem})
        Me.FileToolStripMenuItem1.Name = "FileToolStripMenuItem1"
        Me.FileToolStripMenuItem1.Size = New System.Drawing.Size(40, 20)
        Me.FileToolStripMenuItem1.Text = "&File"
        '
        'exitToolStripMenuItem
        '
        Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"
        Me.exitToolStripMenuItem.Size = New System.Drawing.Size(109, 22)
        Me.exitToolStripMenuItem.Text = "E&xit"
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(554, 447)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.tabExplore)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "MainForm"
        Me.Text = "Windows Environment Sample"
        Me.pgeSpecialFolders.ResumeLayout(False)
        Me.pgeProperties.ResumeLayout(False)
        CType(Me.nudExitCode, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pgeMethods.ResumeLayout(False)
        Me.grpMethods.ResumeLayout(False)
        Me.grpMethods.PerformLayout()
        Me.pgeSystemInformation.ResumeLayout(False)
        Me.tabExplore.ResumeLayout(False)
        Me.pgeEnvironmentVariables.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblSystemFolder As System.Windows.Forms.Label
    Friend WithEvents pgeSpecialFolders As System.Windows.Forms.TabPage
    Friend WithEvents btnGetSystemFolder As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblSpecialFolder As System.Windows.Forms.Label
    Friend WithEvents lstFolders As System.Windows.Forms.ListBox
    Friend WithEvents btnRefreshTickCount As System.Windows.Forms.Button
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents lstCommandLineArgs As System.Windows.Forms.ListBox
    Friend WithEvents lblWorkingSet As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents btnExit As System.Windows.Forms.Button
    Friend WithEvents lblUserDomainName As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents pgeProperties As System.Windows.Forms.TabPage
    Friend WithEvents btnStackTrace As System.Windows.Forms.Button
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents lblVersion As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents lblUserName As System.Windows.Forms.Label
    Friend WithEvents lblOSVersion As System.Windows.Forms.Label
    Friend WithEvents lblMachineName As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents lblTickCount As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents lblSystemDirectory As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents lblCurrentDirectory As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents lblCommandLine As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents nudExitCode As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents pgeMethods As System.Windows.Forms.TabPage
    Friend WithEvents lstLogicalDrives As System.Windows.Forms.ListBox
    Friend WithEvents grpMethods As System.Windows.Forms.GroupBox
    Friend WithEvents btnExpand As System.Windows.Forms.Button
    Friend WithEvents lblExpandResults As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtExpand As System.Windows.Forms.TextBox
    Friend WithEvents lstEnvironmentVariables As System.Windows.Forms.ListBox
    Friend WithEvents pgeSystemInformation As System.Windows.Forms.TabPage
    Friend WithEvents lvwSystemInformation As System.Windows.Forms.ListView
    Friend WithEvents colProperty As System.Windows.Forms.ColumnHeader
    Friend WithEvents colValue As System.Windows.Forms.ColumnHeader
    Friend WithEvents tabExplore As System.Windows.Forms.TabControl
    Friend WithEvents pgeEnvironmentVariables As System.Windows.Forms.TabPage
    Friend WithEvents lblTEMP As System.Windows.Forms.Label
    Friend WithEvents btnGetEnvironmentVariable As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents lblEnvironmentVariable As System.Windows.Forms.Label
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents FileToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
