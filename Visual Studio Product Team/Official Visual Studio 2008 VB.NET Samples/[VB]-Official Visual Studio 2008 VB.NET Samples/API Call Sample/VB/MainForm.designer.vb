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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.MainTabControl = New System.Windows.Forms.TabControl
        Me.tpActiveProcesses = New System.Windows.Forms.TabPage
        Me.Label2 = New System.Windows.Forms.Label
        Me.btnRefreshActiveProcesses = New System.Windows.Forms.Button
        Me.lvwProcessList = New System.Windows.Forms.ListView
        Me.WindowsTitle = New System.Windows.Forms.ColumnHeader
        Me.ClassName = New System.Windows.Forms.ColumnHeader
        Me.WindowsHandle = New System.Windows.Forms.ColumnHeader
        Me.tpActiveWindows = New System.Windows.Forms.TabPage
        Me.btnRefreshActiveWindows = New System.Windows.Forms.Button
        Me.Label1 = New System.Windows.Forms.Label
        Me.lbActiveWindows = New System.Windows.Forms.ListBox
        Me.tpShowWindow = New System.Windows.Forms.TabPage
        Me.lblFunctionCalled = New System.Windows.Forms.Label
        Me.btnShow = New System.Windows.Forms.Button
        Me.Label6 = New System.Windows.Forms.Label
        Me.txtClassName = New System.Windows.Forms.TextBox
        Me.txtWindowCaption = New System.Windows.Forms.TextBox
        Me.Label5 = New System.Windows.Forms.Label
        Me.Label4 = New System.Windows.Forms.Label
        Me.Label3 = New System.Windows.Forms.Label
        Me.tpAPICalls = New System.Windows.Forms.TabPage
        Me.CallingVariationGroupBox = New System.Windows.Forms.GroupBox
        Me.btnBeep = New System.Windows.Forms.Button
        Me.rbAuto = New System.Windows.Forms.RadioButton
        Me.rbANSI = New System.Windows.Forms.RadioButton
        Me.rbUnicode = New System.Windows.Forms.RadioButton
        Me.rbDLLImport = New System.Windows.Forms.RadioButton
        Me.rbDeclare = New System.Windows.Forms.RadioButton
        Me.Label9 = New System.Windows.Forms.Label
        Me.DirectoryGroupBox = New System.Windows.Forms.GroupBox
        Me.txtDirectory = New System.Windows.Forms.TextBox
        Me.btnCreateDirectory = New System.Windows.Forms.Button
        Me.MouseGroupBox = New System.Windows.Forms.GroupBox
        Me.btnResetMouseButton = New System.Windows.Forms.Button
        Me.btnSwapMouseButton = New System.Windows.Forms.Button
        Me.DriveGroupBox = New System.Windows.Forms.GroupBox
        Me.Label8 = New System.Windows.Forms.Label
        Me.txtDriveLetter = New System.Windows.Forms.TextBox
        Me.btnGetDriveType = New System.Windows.Forms.Button
        Me.btnGetDiskFreeSpaceEx = New System.Windows.Forms.Button
        Me.btnGetFreeSpace = New System.Windows.Forms.Button
        Me.txtFunctionOutput = New System.Windows.Forms.TextBox
        Me.MiscGroupBox = New System.Windows.Forms.GroupBox
        Me.btnHibernate = New System.Windows.Forms.Button
        Me.btnGetOSVersion = New System.Windows.Forms.Button
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip
        Me.fileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.exitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem
        Me.MainTabControl.SuspendLayout()
        Me.tpActiveProcesses.SuspendLayout()
        Me.tpActiveWindows.SuspendLayout()
        Me.tpShowWindow.SuspendLayout()
        Me.tpAPICalls.SuspendLayout()
        Me.CallingVariationGroupBox.SuspendLayout()
        Me.DirectoryGroupBox.SuspendLayout()
        Me.MouseGroupBox.SuspendLayout()
        Me.DriveGroupBox.SuspendLayout()
        Me.MiscGroupBox.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MainTabControl
        '
        Me.MainTabControl.AccessibleDescription = "Main tab control"
        Me.MainTabControl.AccessibleName = "Main tab control"
        Me.MainTabControl.Controls.Add(Me.tpActiveProcesses)
        Me.MainTabControl.Controls.Add(Me.tpActiveWindows)
        Me.MainTabControl.Controls.Add(Me.tpShowWindow)
        Me.MainTabControl.Controls.Add(Me.tpAPICalls)
        Me.MainTabControl.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.MainTabControl.ItemSize = New System.Drawing.Size(94, 18)
        Me.MainTabControl.Location = New System.Drawing.Point(0, 39)
        Me.MainTabControl.Name = "MainTabControl"
        Me.MainTabControl.SelectedIndex = 0
        Me.MainTabControl.Size = New System.Drawing.Size(668, 401)
        Me.MainTabControl.TabIndex = 5
        '
        'tpActiveProcesses
        '
        Me.tpActiveProcesses.AccessibleDescription = "Active Process tab page"
        Me.tpActiveProcesses.AccessibleName = "Active Process tab page"
        Me.tpActiveProcesses.Controls.Add(Me.Label2)
        Me.tpActiveProcesses.Controls.Add(Me.btnRefreshActiveProcesses)
        Me.tpActiveProcesses.Controls.Add(Me.lvwProcessList)
        Me.tpActiveProcesses.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.tpActiveProcesses.Location = New System.Drawing.Point(4, 22)
        Me.tpActiveProcesses.Name = "tpActiveProcesses"
        Me.tpActiveProcesses.Size = New System.Drawing.Size(660, 375)
        Me.tpActiveProcesses.TabIndex = 0
        Me.tpActiveProcesses.Text = "Active Processes"
        Me.tpActiveProcesses.UseVisualStyleBackColor = False
        '
        'Label2
        '
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.Label2.ForeColor = System.Drawing.Color.Black
        Me.Label2.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label2.Location = New System.Drawing.Point(9, 261)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(637, 57)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "Press Refresh to fill in the list view with all active Windows processes.  The li" & _
            "st view is populated with a call to Win32API callback function EnumWindows."
        '
        'btnRefreshActiveProcesses
        '
        Me.btnRefreshActiveProcesses.AccessibleDescription = "Refresh button"
        Me.btnRefreshActiveProcesses.AccessibleName = "Refresh button"
        Me.btnRefreshActiveProcesses.BackColor = System.Drawing.SystemColors.Control
        Me.btnRefreshActiveProcesses.ForeColor = System.Drawing.SystemColors.WindowText
        Me.btnRefreshActiveProcesses.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnRefreshActiveProcesses.Location = New System.Drawing.Point(243, 327)
        Me.btnRefreshActiveProcesses.Name = "btnRefreshActiveProcesses"
        Me.btnRefreshActiveProcesses.Size = New System.Drawing.Size(103, 37)
        Me.btnRefreshActiveProcesses.TabIndex = 2
        Me.btnRefreshActiveProcesses.Text = "&Refresh"
        Me.btnRefreshActiveProcesses.UseVisualStyleBackColor = False
        '
        'lvwProcessList
        '
        Me.lvwProcessList.AccessibleDescription = "Process list view"
        Me.lvwProcessList.AccessibleName = "Process list view"
        Me.lvwProcessList.AllowColumnReorder = True
        Me.lvwProcessList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.WindowsTitle, Me.ClassName, Me.WindowsHandle})
        Me.lvwProcessList.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.lvwProcessList.FullRowSelect = True
        Me.lvwProcessList.Location = New System.Drawing.Point(9, 9)
        Me.lvwProcessList.Name = "lvwProcessList"
        Me.lvwProcessList.Size = New System.Drawing.Size(637, 244)
        Me.lvwProcessList.TabIndex = 1
        Me.lvwProcessList.View = System.Windows.Forms.View.Details
        '
        'WindowsTitle
        '
        Me.WindowsTitle.Name = "WindowsTitle"
        Me.WindowsTitle.Text = "Windows Caption"
        Me.WindowsTitle.Width = 250
        '
        'ClassName
        '
        Me.ClassName.Name = "ClassName"
        Me.ClassName.Text = "ClassName"
        Me.ClassName.Width = 150
        '
        'WindowsHandle
        '
        Me.WindowsHandle.Name = "WindowsHandle"
        Me.WindowsHandle.Text = "Windows Handle"
        Me.WindowsHandle.Width = 140
        '
        'tpActiveWindows
        '
        Me.tpActiveWindows.AccessibleDescription = "Active Window tab page"
        Me.tpActiveWindows.AccessibleName = "Active Window tab page"
        Me.tpActiveWindows.Controls.Add(Me.btnRefreshActiveWindows)
        Me.tpActiveWindows.Controls.Add(Me.Label1)
        Me.tpActiveWindows.Controls.Add(Me.lbActiveWindows)
        Me.tpActiveWindows.Location = New System.Drawing.Point(4, 22)
        Me.tpActiveWindows.Name = "tpActiveWindows"
        Me.tpActiveWindows.Size = New System.Drawing.Size(660, 375)
        Me.tpActiveWindows.TabIndex = 1
        Me.tpActiveWindows.Text = "Active Windows"
        Me.tpActiveWindows.UseVisualStyleBackColor = False
        '
        'btnRefreshActiveWindows
        '
        Me.btnRefreshActiveWindows.AccessibleDescription = "Refresh button"
        Me.btnRefreshActiveWindows.AccessibleName = "Refresh button"
        Me.btnRefreshActiveWindows.BackColor = System.Drawing.SystemColors.Control
        Me.btnRefreshActiveWindows.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.btnRefreshActiveWindows.ForeColor = System.Drawing.SystemColors.WindowText
        Me.btnRefreshActiveWindows.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnRefreshActiveWindows.Location = New System.Drawing.Point(243, 327)
        Me.btnRefreshActiveWindows.Name = "btnRefreshActiveWindows"
        Me.btnRefreshActiveWindows.Size = New System.Drawing.Size(103, 37)
        Me.btnRefreshActiveWindows.TabIndex = 6
        Me.btnRefreshActiveWindows.Text = "&Refresh"
        Me.btnRefreshActiveWindows.UseVisualStyleBackColor = False
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.Label1.ForeColor = System.Drawing.Color.Black
        Me.Label1.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label1.Location = New System.Drawing.Point(9, 261)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(637, 57)
        Me.Label1.TabIndex = 5
        Me.Label1.Text = resources.GetString("Label1.Text")
        '
        'lbActiveWindows
        '
        Me.lbActiveWindows.AccessibleDescription = "Active windows listbox"
        Me.lbActiveWindows.AccessibleName = "Active windows list box"
        Me.lbActiveWindows.FormattingEnabled = True
        Me.lbActiveWindows.Location = New System.Drawing.Point(9, 19)
        Me.lbActiveWindows.Name = "lbActiveWindows"
        Me.lbActiveWindows.Size = New System.Drawing.Size(637, 225)
        Me.lbActiveWindows.TabIndex = 0
        '
        'tpShowWindow
        '
        Me.tpShowWindow.AccessibleDescription = "Show window tab page"
        Me.tpShowWindow.AccessibleName = "Show window tab page"
        Me.tpShowWindow.Controls.Add(Me.lblFunctionCalled)
        Me.tpShowWindow.Controls.Add(Me.btnShow)
        Me.tpShowWindow.Controls.Add(Me.Label6)
        Me.tpShowWindow.Controls.Add(Me.txtClassName)
        Me.tpShowWindow.Controls.Add(Me.txtWindowCaption)
        Me.tpShowWindow.Controls.Add(Me.Label5)
        Me.tpShowWindow.Controls.Add(Me.Label4)
        Me.tpShowWindow.Controls.Add(Me.Label3)
        Me.tpShowWindow.Location = New System.Drawing.Point(4, 22)
        Me.tpShowWindow.Name = "tpShowWindow"
        Me.tpShowWindow.Size = New System.Drawing.Size(660, 375)
        Me.tpShowWindow.TabIndex = 2
        Me.tpShowWindow.Text = "Show Window"
        Me.tpShowWindow.UseVisualStyleBackColor = False
        '
        'lblFunctionCalled
        '
        Me.lblFunctionCalled.AccessibleDescription = "Function call description label"
        Me.lblFunctionCalled.AccessibleName = "Function call description label"
        Me.lblFunctionCalled.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.lblFunctionCalled.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.lblFunctionCalled.Location = New System.Drawing.Point(37, 271)
        Me.lblFunctionCalled.Name = "lblFunctionCalled"
        Me.lblFunctionCalled.Size = New System.Drawing.Size(580, 19)
        Me.lblFunctionCalled.TabIndex = 13
        Me.lblFunctionCalled.Text = "FindWindow (ClassName As String, WindowName As String) will be called."
        '
        'btnShow
        '
        Me.btnShow.AccessibleDescription = "Show button"
        Me.btnShow.AccessibleName = "Show button"
        Me.btnShow.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.btnShow.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnShow.Location = New System.Drawing.Point(243, 327)
        Me.btnShow.Name = "btnShow"
        Me.btnShow.Size = New System.Drawing.Size(103, 37)
        Me.btnShow.TabIndex = 12
        Me.btnShow.Text = "&Show"
        '
        'Label6
        '
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.Label6.ForeColor = System.Drawing.Color.Black
        Me.Label6.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label6.Location = New System.Drawing.Point(9, 94)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(627, 46)
        Me.Label6.TabIndex = 11
        Me.Label6.Text = "Note:  Open Calculator to use preset values, or enter values of your own for an a" & _
            "ctive window."
        '
        'txtClassName
        '
        Me.txtClassName.AccessibleDescription = "Class name text box"
        Me.txtClassName.AccessibleName = "Class name text box"
        Me.txtClassName.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.txtClassName.Location = New System.Drawing.Point(159, 196)
        Me.txtClassName.Name = "txtClassName"
        Me.txtClassName.Size = New System.Drawing.Size(458, 23)
        Me.txtClassName.TabIndex = 10
        Me.txtClassName.Text = "SciCalc"
        '
        'txtWindowCaption
        '
        Me.txtWindowCaption.AccessibleDescription = "Window caption text box"
        Me.txtWindowCaption.AccessibleName = "Window caption text box"
        Me.txtWindowCaption.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.txtWindowCaption.Location = New System.Drawing.Point(159, 169)
        Me.txtWindowCaption.Name = "txtWindowCaption"
        Me.txtWindowCaption.Size = New System.Drawing.Size(458, 23)
        Me.txtWindowCaption.TabIndex = 9
        Me.txtWindowCaption.Text = "Calculator"
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.Label5.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label5.Location = New System.Drawing.Point(56, 196)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(103, 28)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "Class Name:"
        '
        'Label4
        '
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.Label4.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label4.Location = New System.Drawing.Point(28, 169)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(131, 27)
        Me.Label4.TabIndex = 7
        Me.Label4.Text = "Window Caption: "
        '
        'Label3
        '
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.Label3.ForeColor = System.Drawing.Color.Black
        Me.Label3.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label3.Location = New System.Drawing.Point(9, 19)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(637, 56)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = resources.GetString("Label3.Text")
        '
        'tpAPICalls
        '
        Me.tpAPICalls.AccessibleDescription = "API calls tabpage"
        Me.tpAPICalls.AccessibleName = "API calls tabpage"
        Me.tpAPICalls.Controls.Add(Me.CallingVariationGroupBox)
        Me.tpAPICalls.Controls.Add(Me.Label9)
        Me.tpAPICalls.Controls.Add(Me.DirectoryGroupBox)
        Me.tpAPICalls.Controls.Add(Me.MouseGroupBox)
        Me.tpAPICalls.Controls.Add(Me.DriveGroupBox)
        Me.tpAPICalls.Controls.Add(Me.txtFunctionOutput)
        Me.tpAPICalls.Controls.Add(Me.MiscGroupBox)
        Me.tpAPICalls.Location = New System.Drawing.Point(4, 22)
        Me.tpAPICalls.Name = "tpAPICalls"
        Me.tpAPICalls.Size = New System.Drawing.Size(660, 375)
        Me.tpAPICalls.TabIndex = 4
        Me.tpAPICalls.Text = "API Calls"
        Me.tpAPICalls.UseVisualStyleBackColor = False
        '
        'CallingVariationGroupBox
        '
        Me.CallingVariationGroupBox.Controls.Add(Me.btnBeep)
        Me.CallingVariationGroupBox.Controls.Add(Me.rbAuto)
        Me.CallingVariationGroupBox.Controls.Add(Me.rbANSI)
        Me.CallingVariationGroupBox.Controls.Add(Me.rbUnicode)
        Me.CallingVariationGroupBox.Controls.Add(Me.rbDLLImport)
        Me.CallingVariationGroupBox.Controls.Add(Me.rbDeclare)
        Me.CallingVariationGroupBox.Location = New System.Drawing.Point(448, 104)
        Me.CallingVariationGroupBox.Name = "CallingVariationGroupBox"
        Me.CallingVariationGroupBox.Size = New System.Drawing.Size(168, 248)
        Me.CallingVariationGroupBox.TabIndex = 9
        Me.CallingVariationGroupBox.TabStop = False
        Me.CallingVariationGroupBox.Text = "Calling variations"
        '
        'btnBeep
        '
        Me.btnBeep.AccessibleDescription = "Beep button"
        Me.btnBeep.AccessibleName = "Beep button"
        Me.btnBeep.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnBeep.Location = New System.Drawing.Point(16, 176)
        Me.btnBeep.Name = "btnBeep"
        Me.btnBeep.Size = New System.Drawing.Size(136, 24)
        Me.btnBeep.TabIndex = 20
        Me.btnBeep.Text = "&Beep"
        '
        'rbAuto
        '
        Me.rbAuto.AccessibleDescription = "Auto Check"
        Me.rbAuto.AccessibleName = "Auto Check"
        Me.rbAuto.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbAuto.Location = New System.Drawing.Point(32, 136)
        Me.rbAuto.Name = "rbAuto"
        Me.rbAuto.Size = New System.Drawing.Size(128, 16)
        Me.rbAuto.TabIndex = 19
        Me.rbAuto.Text = "Auto"
        '
        'rbANSI
        '
        Me.rbANSI.AccessibleDescription = "ANSI Check"
        Me.rbANSI.AccessibleName = "ANSI Check"
        Me.rbANSI.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbANSI.Location = New System.Drawing.Point(32, 112)
        Me.rbANSI.Name = "rbANSI"
        Me.rbANSI.Size = New System.Drawing.Size(128, 16)
        Me.rbANSI.TabIndex = 18
        Me.rbANSI.Text = "ANSI"
        '
        'rbUnicode
        '
        Me.rbUnicode.AccessibleDescription = "Unicode check"
        Me.rbUnicode.AccessibleName = "Unicode check"
        Me.rbUnicode.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbUnicode.Location = New System.Drawing.Point(32, 88)
        Me.rbUnicode.Name = "rbUnicode"
        Me.rbUnicode.Size = New System.Drawing.Size(128, 16)
        Me.rbUnicode.TabIndex = 17
        Me.rbUnicode.Text = "Unicode"
        '
        'rbDLLImport
        '
        Me.rbDLLImport.AccessibleDescription = "DLLImport check"
        Me.rbDLLImport.AccessibleName = "DLLImport check"
        Me.rbDLLImport.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbDLLImport.Location = New System.Drawing.Point(32, 64)
        Me.rbDLLImport.Name = "rbDLLImport"
        Me.rbDLLImport.Size = New System.Drawing.Size(128, 16)
        Me.rbDLLImport.TabIndex = 16
        Me.rbDLLImport.Text = "DLLImport"
        '
        'rbDeclare
        '
        Me.rbDeclare.AccessibleDescription = "Declare chectk"
        Me.rbDeclare.AccessibleName = "Declare check"
        Me.rbDeclare.Checked = True
        Me.rbDeclare.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.rbDeclare.Location = New System.Drawing.Point(32, 40)
        Me.rbDeclare.Name = "rbDeclare"
        Me.rbDeclare.Size = New System.Drawing.Size(128, 16)
        Me.rbDeclare.TabIndex = 15
        Me.rbDeclare.Text = "Declare"
        '
        'Label9
        '
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!)
        Me.Label9.ForeColor = System.Drawing.Color.Black
        Me.Label9.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label9.Location = New System.Drawing.Point(24, 8)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(432, 16)
        Me.Label9.TabIndex = 8
        Me.Label9.Text = "This shows a variety of Win32 API calls:"
        '
        'DirectoryGroupBox
        '
        Me.DirectoryGroupBox.Controls.Add(Me.txtDirectory)
        Me.DirectoryGroupBox.Controls.Add(Me.btnCreateDirectory)
        Me.DirectoryGroupBox.Location = New System.Drawing.Point(24, 264)
        Me.DirectoryGroupBox.Name = "DirectoryGroupBox"
        Me.DirectoryGroupBox.Size = New System.Drawing.Size(168, 96)
        Me.DirectoryGroupBox.TabIndex = 6
        Me.DirectoryGroupBox.TabStop = False
        Me.DirectoryGroupBox.Text = "Directory"
        '
        'txtDirectory
        '
        Me.txtDirectory.AccessibleDescription = "Dierctory text"
        Me.txtDirectory.AccessibleName = "Dierctory text"
        Me.txtDirectory.Location = New System.Drawing.Point(16, 24)
        Me.txtDirectory.Name = "txtDirectory"
        Me.txtDirectory.Size = New System.Drawing.Size(128, 20)
        Me.txtDirectory.TabIndex = 4
        Me.txtDirectory.Text = "c:\myDirectory"
        '
        'btnCreateDirectory
        '
        Me.btnCreateDirectory.AccessibleDescription = "Create Directory button"
        Me.btnCreateDirectory.AccessibleName = "Create Directory button"
        Me.btnCreateDirectory.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnCreateDirectory.Location = New System.Drawing.Point(16, 56)
        Me.btnCreateDirectory.Name = "btnCreateDirectory"
        Me.btnCreateDirectory.Size = New System.Drawing.Size(128, 24)
        Me.btnCreateDirectory.TabIndex = 3
        Me.btnCreateDirectory.Text = "Create Directory"
        '
        'MouseGroupBox
        '
        Me.MouseGroupBox.Controls.Add(Me.btnResetMouseButton)
        Me.MouseGroupBox.Controls.Add(Me.btnSwapMouseButton)
        Me.MouseGroupBox.Location = New System.Drawing.Point(240, 104)
        Me.MouseGroupBox.Name = "MouseGroupBox"
        Me.MouseGroupBox.Size = New System.Drawing.Size(176, 120)
        Me.MouseGroupBox.TabIndex = 5
        Me.MouseGroupBox.TabStop = False
        Me.MouseGroupBox.Text = "Mouse"
        '
        'btnResetMouseButton
        '
        Me.btnResetMouseButton.AccessibleDescription = "Reset mouse button"
        Me.btnResetMouseButton.AccessibleName = "Reset mouse button"
        Me.btnResetMouseButton.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnResetMouseButton.Location = New System.Drawing.Point(16, 72)
        Me.btnResetMouseButton.Name = "btnResetMouseButton"
        Me.btnResetMouseButton.Size = New System.Drawing.Size(128, 24)
        Me.btnResetMouseButton.TabIndex = 1
        Me.btnResetMouseButton.Text = "Reset Mouse Buttons"
        '
        'btnSwapMouseButton
        '
        Me.btnSwapMouseButton.AccessibleDescription = "Swap Mouse button"
        Me.btnSwapMouseButton.AccessibleName = "Swap Mouse button"
        Me.btnSwapMouseButton.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnSwapMouseButton.Location = New System.Drawing.Point(16, 40)
        Me.btnSwapMouseButton.Name = "btnSwapMouseButton"
        Me.btnSwapMouseButton.Size = New System.Drawing.Size(128, 24)
        Me.btnSwapMouseButton.TabIndex = 0
        Me.btnSwapMouseButton.Text = "Swap Mouse Buttons"
        '
        'DriveGroupBox
        '
        Me.DriveGroupBox.Controls.Add(Me.Label8)
        Me.DriveGroupBox.Controls.Add(Me.txtDriveLetter)
        Me.DriveGroupBox.Controls.Add(Me.btnGetDriveType)
        Me.DriveGroupBox.Controls.Add(Me.btnGetDiskFreeSpaceEx)
        Me.DriveGroupBox.Controls.Add(Me.btnGetFreeSpace)
        Me.DriveGroupBox.Location = New System.Drawing.Point(24, 104)
        Me.DriveGroupBox.Name = "DriveGroupBox"
        Me.DriveGroupBox.Size = New System.Drawing.Size(168, 152)
        Me.DriveGroupBox.TabIndex = 4
        Me.DriveGroupBox.TabStop = False
        Me.DriveGroupBox.Text = "Drive"
        '
        'Label8
        '
        Me.Label8.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.Label8.Location = New System.Drawing.Point(80, 24)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(40, 16)
        Me.Label8.TabIndex = 6
        Me.Label8.Text = "Drive:"
        '
        'txtDriveLetter
        '
        Me.txtDriveLetter.AccessibleDescription = "Drive Letter text"
        Me.txtDriveLetter.AccessibleName = "Drive Letter text"
        Me.txtDriveLetter.Font = New System.Drawing.Font("Microsoft Sans Serif", 12.0!)
        Me.txtDriveLetter.Location = New System.Drawing.Point(120, 16)
        Me.txtDriveLetter.MaxLength = 1
        Me.txtDriveLetter.Name = "txtDriveLetter"
        Me.txtDriveLetter.Size = New System.Drawing.Size(24, 27)
        Me.txtDriveLetter.TabIndex = 5
        Me.txtDriveLetter.Text = "C"
        '
        'btnGetDriveType
        '
        Me.btnGetDriveType.AccessibleDescription = "Get Drive Type button"
        Me.btnGetDriveType.AccessibleName = "Get Drive Type button"
        Me.btnGetDriveType.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnGetDriveType.Location = New System.Drawing.Point(16, 112)
        Me.btnGetDriveType.Name = "btnGetDriveType"
        Me.btnGetDriveType.Size = New System.Drawing.Size(128, 24)
        Me.btnGetDriveType.TabIndex = 4
        Me.btnGetDriveType.Text = "GetDriveType"
        '
        'btnGetDiskFreeSpaceEx
        '
        Me.btnGetDiskFreeSpaceEx.AccessibleDescription = "Get Disk Free Space button"
        Me.btnGetDiskFreeSpaceEx.AccessibleName = "Get Disk Free Space button"
        Me.btnGetDiskFreeSpaceEx.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnGetDiskFreeSpaceEx.Location = New System.Drawing.Point(16, 80)
        Me.btnGetDiskFreeSpaceEx.Name = "btnGetDiskFreeSpaceEx"
        Me.btnGetDiskFreeSpaceEx.Size = New System.Drawing.Size(128, 24)
        Me.btnGetDiskFreeSpaceEx.TabIndex = 3
        Me.btnGetDiskFreeSpaceEx.Text = "GetDiskFreeSpaceEx"
        '
        'btnGetFreeSpace
        '
        Me.btnGetFreeSpace.AccessibleDescription = "Get Disk Free Space button"
        Me.btnGetFreeSpace.AccessibleName = "Get Disk Free Space button"
        Me.btnGetFreeSpace.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnGetFreeSpace.Location = New System.Drawing.Point(16, 48)
        Me.btnGetFreeSpace.Name = "btnGetFreeSpace"
        Me.btnGetFreeSpace.Size = New System.Drawing.Size(128, 24)
        Me.btnGetFreeSpace.TabIndex = 2
        Me.btnGetFreeSpace.Text = "GetDiskFreeSpace"
        '
        'txtFunctionOutput
        '
        Me.txtFunctionOutput.AccessibleDescription = "Function output text"
        Me.txtFunctionOutput.AccessibleName = "Function output text"
        Me.txtFunctionOutput.Location = New System.Drawing.Point(24, 40)
        Me.txtFunctionOutput.Multiline = True
        Me.txtFunctionOutput.Name = "txtFunctionOutput"
        Me.txtFunctionOutput.Size = New System.Drawing.Size(600, 56)
        Me.txtFunctionOutput.TabIndex = 0
        '
        'MiscGroupBox
        '
        Me.MiscGroupBox.Controls.Add(Me.btnHibernate)
        Me.MiscGroupBox.Controls.Add(Me.btnGetOSVersion)
        Me.MiscGroupBox.Location = New System.Drawing.Point(240, 240)
        Me.MiscGroupBox.Name = "MiscGroupBox"
        Me.MiscGroupBox.Size = New System.Drawing.Size(176, 112)
        Me.MiscGroupBox.TabIndex = 7
        Me.MiscGroupBox.TabStop = False
        Me.MiscGroupBox.Text = "Misc"
        '
        'btnHibernate
        '
        Me.btnHibernate.AccessibleDescription = "Hibernate button"
        Me.btnHibernate.AccessibleName = "Hibernate button"
        Me.btnHibernate.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnHibernate.Location = New System.Drawing.Point(16, 64)
        Me.btnHibernate.Name = "btnHibernate"
        Me.btnHibernate.Size = New System.Drawing.Size(128, 24)
        Me.btnHibernate.TabIndex = 3
        Me.btnHibernate.Text = "Hibernate"
        '
        'btnGetOSVersion
        '
        Me.btnGetOSVersion.AccessibleDescription = "Get OS version button"
        Me.btnGetOSVersion.AccessibleName = "Get OS version button"
        Me.btnGetOSVersion.ImeMode = System.Windows.Forms.ImeMode.NoControl
        Me.btnGetOSVersion.Location = New System.Drawing.Point(16, 32)
        Me.btnGetOSVersion.Name = "btnGetOSVersion"
        Me.btnGetOSVersion.Size = New System.Drawing.Size(128, 24)
        Me.btnGetOSVersion.TabIndex = 0
        Me.btnGetOSVersion.Text = "Get OS Version"
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.fileToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(668, 24)
        Me.MenuStrip1.TabIndex = 10
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'fileToolStripMenuItem
        '
        Me.fileToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.exitToolStripMenuItem})
        Me.fileToolStripMenuItem.Name = "fileToolStripMenuItem"
        Me.fileToolStripMenuItem.Size = New System.Drawing.Size(40, 20)
        Me.fileToolStripMenuItem.Text = "&File"
        '
        'exitToolStripMenuItem
        '
        Me.exitToolStripMenuItem.Name = "exitToolStripMenuItem"
        Me.exitToolStripMenuItem.Size = New System.Drawing.Size(109, 22)
        Me.exitToolStripMenuItem.Text = "E&xit"
        '
        'MainForm
        '
        Me.ClientSize = New System.Drawing.Size(668, 440)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.MainTabControl)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "MainForm"
        Me.Text = "Sample Application: Calling Windows API"
        Me.MainTabControl.ResumeLayout(False)
        Me.tpActiveProcesses.ResumeLayout(False)
        Me.tpActiveWindows.ResumeLayout(False)
        Me.tpShowWindow.ResumeLayout(False)
        Me.tpShowWindow.PerformLayout()
        Me.tpAPICalls.ResumeLayout(False)
        Me.tpAPICalls.PerformLayout()
        Me.CallingVariationGroupBox.ResumeLayout(False)
        Me.DirectoryGroupBox.ResumeLayout(False)
        Me.DirectoryGroupBox.PerformLayout()
        Me.MouseGroupBox.ResumeLayout(False)
        Me.DriveGroupBox.ResumeLayout(False)
        Me.DriveGroupBox.PerformLayout()
        Me.MiscGroupBox.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents MainTabControl As System.Windows.Forms.TabControl
    Friend WithEvents tpActiveProcesses As System.Windows.Forms.TabPage
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents btnRefreshActiveProcesses As System.Windows.Forms.Button
    Friend WithEvents lvwProcessList As System.Windows.Forms.ListView
    Friend WithEvents WindowsTitle As System.Windows.Forms.ColumnHeader
    Friend WithEvents ClassName As System.Windows.Forms.ColumnHeader
    Friend WithEvents WindowsHandle As System.Windows.Forms.ColumnHeader
    Friend WithEvents tpActiveWindows As System.Windows.Forms.TabPage
    Friend WithEvents btnRefreshActiveWindows As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lbActiveWindows As System.Windows.Forms.ListBox
    Friend WithEvents tpShowWindow As System.Windows.Forms.TabPage
    Friend WithEvents lblFunctionCalled As System.Windows.Forms.Label
    Friend WithEvents btnShow As System.Windows.Forms.Button
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents txtClassName As System.Windows.Forms.TextBox
    Friend WithEvents txtWindowCaption As System.Windows.Forms.TextBox
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents tpAPICalls As System.Windows.Forms.TabPage
    Friend WithEvents CallingVariationGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents btnBeep As System.Windows.Forms.Button
    Friend WithEvents rbAuto As System.Windows.Forms.RadioButton
    Friend WithEvents rbANSI As System.Windows.Forms.RadioButton
    Friend WithEvents rbUnicode As System.Windows.Forms.RadioButton
    Friend WithEvents rbDLLImport As System.Windows.Forms.RadioButton
    Friend WithEvents rbDeclare As System.Windows.Forms.RadioButton
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents DirectoryGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents txtDirectory As System.Windows.Forms.TextBox
    Friend WithEvents btnCreateDirectory As System.Windows.Forms.Button
    Friend WithEvents MouseGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents btnResetMouseButton As System.Windows.Forms.Button
    Friend WithEvents btnSwapMouseButton As System.Windows.Forms.Button
    Friend WithEvents DriveGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtDriveLetter As System.Windows.Forms.TextBox
    Friend WithEvents btnGetDriveType As System.Windows.Forms.Button
    Friend WithEvents btnGetDiskFreeSpaceEx As System.Windows.Forms.Button
    Friend WithEvents btnGetFreeSpace As System.Windows.Forms.Button
    Friend WithEvents txtFunctionOutput As System.Windows.Forms.TextBox
    Friend WithEvents MiscGroupBox As System.Windows.Forms.GroupBox
    Friend WithEvents btnHibernate As System.Windows.Forms.Button
    Friend WithEvents btnGetOSVersion As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents fileToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
